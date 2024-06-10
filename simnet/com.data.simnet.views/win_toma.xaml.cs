using simnet.com.data.simnet.models;
using simnet.com.data.simnet.services;
using System;
using System.Diagnostics;
using System.IO;
using System.Printing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Drawing;
using System.Windows.Documents;
using log4net.Repository.Hierarchy;
using log4net;

namespace simnet.com.data.simnet.views
{

    public class RawPrinterHelper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(RawPrinterHelper));

        // Structure and API declarions:
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)] public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)] public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)] public string pDataType;
        }
        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, Int32 level, [In, MarshalAs(UnmanagedType.LPStruct)] DOCINFOA di);

        [DllImport("winspool.Drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndDocPrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool StartPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool EndPagePrinter(IntPtr hPrinter);

        [DllImport("winspool.Drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, Int32 dwCount, out Int32 dwWritten);

        // SendBytesToPrinter()
        // When the function is given a printer name and an unmanaged array
        // of bytes, the function sends those bytes to the print queue.
        // Returns true on success, false on failure.
        public static bool SendBytesToPrinter(string szPrinterName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOA di = new DOCINFOA();
            bool bSuccess = false; // Assume failure unless you specifically succeed.

            di.pDocName = "My C#.NET RAW Document";
            di.pDataType = "RAW";

            // Open the printer.
            if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                // Start a document.
                if (StartDocPrinter(hPrinter, 1, di))
                {
                    // Start a page.
                    if (StartPagePrinter(hPrinter))
                    {
                        // Write your bytes.
                        bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
            // If you did not succeed, GetLastError may give more information
            // about why not.
            if (bSuccess == false)
            {
                dwError = Marshal.GetLastWin32Error();
            }
            return bSuccess;
        }

        public static bool SendFileToPrinter(string szPrinterName, string szFileName)
        {
            // Open the file.
            FileStream fs = new FileStream(szFileName, FileMode.Open);
            // Create a BinaryReader on the file.
            BinaryReader br = new BinaryReader(fs);
            // Dim an array of bytes big enough to hold the file's contents.
            Byte[] bytes = new Byte[fs.Length];
            bool bSuccess = false;
            // Your unmanaged pointer.
            IntPtr pUnmanagedBytes = new IntPtr(0);
            int nLength;

            nLength = Convert.ToInt32(fs.Length);
            // Read the contents of the file into the array.
            bytes = br.ReadBytes(nLength);
            // Allocate some unmanaged memory for those bytes.
            pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
            // Copy the managed byte array into the unmanaged array.
            Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
            // Send the unmanaged bytes to the printer.
            bSuccess = SendBytesToPrinter(szPrinterName, pUnmanagedBytes, nLength);
            // Free the unmanaged memory that you allocated earlier.
            Marshal.FreeCoTaskMem(pUnmanagedBytes);
            return bSuccess;
        }
        public static bool SendStringToPrinter(string szPrinterName, string szString)
        {
            IntPtr pBytes;
            Int32 dwCount;
            // How many characters are in the string?
            dwCount = szString.Length;
            // Assume that the printer is expecting ANSI text, and then convert
            // the string to ANSI text.
            pBytes = Marshal.StringToCoTaskMemAnsi(szString);
            // Send the converted ANSI string to the printer.
            SendBytesToPrinter(szPrinterName, pBytes, dwCount);
            Marshal.FreeCoTaskMem(pBytes);
            return true;
        }
    }
    /// <summary>
    /// Lógica de interacción para win_toma.xaml
    /// </summary>
    public partial class win_toma : Window
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SerialHealthMonitor));

        private PatientService patientService;
       // private DispatcherTimer timer;
        private Patient patient;
        private SerialHealthMonitor serialHealth;
        private const string CMD_TEMPERATURA = "aa19020f010100110100000000000000000000000000000000dbb46465";
        private const string CMD_SPO = "aa19020e0101001101000000000000000000000000000000002836d397";
        private const string CMD_NIBP = "aa19020d01010011010000000000000000000000000000000038701636";
        private const string hora_sop = " ";
        private string resnibp = " ";
        private string resnibp1 = " ";
        private double temperature = 0.0;
        string clasificacion_tem = String.Empty;
        private double spo_value = 0.0;
        private double pr_value = 0.0;
        private double sys_value = 0.0;
        private double dia_value = 0.0;
        private double mean_value = 0.0;
        private double prpre_value = 0.0;
        private double nibpPAS = 0.0;
        private double nibpPAD = 0.0;
        private double spo = 0.0;
        private double pre = 0.0;
        private PrintDialog impresora = new PrintDialog();
        public int txtem = 0;
        public int txspo = 0;
        public int txnibp = 0;

        public win_toma(Patient P)
        {
            this.patient = P; 

            InitializeComponent();
            //MessageBox.Show("PATIENT" + patient.Name);
            pbStatus.Visibility = Visibility.Hidden;
            patientService = new PatientService();
            grid_toma_signos.Visibility = Visibility.Visible;
            serialHealth = new SerialHealthMonitor();
            WindowState = WindowState.Maximized;
            bot_regresar.Visibility = Visibility.Hidden;
            boton_salir.Visibility = Visibility.Hidden;

        }

        //grid_toma_signos

        private void boton_Temperatita_Click(object sender, RoutedEventArgs e)
        {
            if (serialHealth.isConnected())
            {
                grid_toma_signos.Visibility = Visibility.Hidden;
                grid_toma_tempe.Visibility = Visibility.Visible;
                tex_temp2.Visibility = Visibility.Hidden;
                bot_siguiente_tem1.Visibility = Visibility.Hidden;
                boton_toma.Visibility = Visibility.Visible;
                bot_siguiente_tem2.Visibility = Visibility.Visible;
                txt_Signos_tem_resu3.Visibility = Visibility.Hidden;
                txt_resultado_temfin3.Visibility = Visibility.Hidden;
                bot_siguiente_tem3.Visibility = Visibility.Hidden;
                txt_resultado_hora.Visibility = Visibility.Hidden;
                tx_procentaje.Visibility = Visibility.Hidden;
                bot_regresar.Visibility = Visibility.Hidden;
                boton_salir.Visibility = Visibility.Hidden;

            }
            else
            {
                MessageBox.Show("Por favor encender el dispositivo del boton amarillo, Dispositivo Apagado");

            }


        }

        private void bot_siguiente_tem1_Click(object sender, RoutedEventArgs e)
        {
            tex_temp2.Visibility = Visibility.Visible;
            bot_siguiente_tem2.Visibility = Visibility.Visible;
            bot_siguiente_tem1.Visibility = Visibility.Hidden;
            boton_toma.Visibility = Visibility.Hidden;
        }

        
       
        private void UpdateProgressBar(int value)
        {
            // Update the ProgressBar value on the UI thread
            Dispatcher.Invoke(() =>
            {
                pbStatus.Value = value;
            });
        }
        private void DoWork()
        {
            for (int i = 0; i <= 100; i++)
            {
                System.Threading.Thread.Sleep(50); // Simulate work

                // Update the ProgressBar value on the UI thread
                Dispatcher.Invoke(() =>
                {
                    pbStatus.Value = i;
                });
            }
        }
        private void bot_siguiente_tem2_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                // Start a separate thread to simulate lengthy work
                Thread workerThread = new Thread(DoWork);
                workerThread.Start();
                txt_Signos_tem_resu3.Visibility = Visibility.Visible;
                txt_resultado_temfin3.Visibility = Visibility.Visible;
                bot_siguiente_tem3.Visibility = Visibility.Visible;
                txt_resultado_hora.Visibility = Visibility.Visible;
                txt_resultado_hora.Visibility = Visibility.Visible;
                pbStatus.Visibility = Visibility.Visible;
                tx_procentaje.Visibility = Visibility.Visible;
                bot_regresar.Visibility = Visibility.Hidden;
                boton_salir.Visibility = Visibility.Hidden;
                boton_toma.Visibility = Visibility.Hidden;


                using (SerialHealthMonitor srh = new SerialHealthMonitor())
                {
                   

                    srh.OpenPort();
                    Logger.Info($"Starting getting temperature command " + CMD_TEMPERATURA);

                    srh.SendHexData(CMD_TEMPERATURA);

                    // Wait for a moment to allow the device to respond (you can adjust this based on your use case).
                    System.Threading.Thread.Sleep(8000);

                    temperature = srh.HexToDecimal(srh.Temperature.ToString().Substring(0, 4), 10.0);
                    Logger.Info($"Object obtained  " + srh.Temperature.ToString().Substring(0, 4));

                    txt_resultado_temfin3.Text = "Temp " + temperature.ToString() + " °C";
                    Logger.Info($"Temperature obtained  " + temperature);
                    //MessageBox.Show("Temperature is " + temperature);




                    if (temperature >= 35.9 && temperature <= 37.4)

                    {
                        //MessageBox.Show("Temperatura normal.");
                        clasificacion_tem = "Temperatura normal.";
                        txt_resultado_hora.Foreground = Brushes.Green;
                    }
                    else if (temperature >= 34.5 && temperature <= 35.8)
                    {
                        //MessageBox.Show("Hipotermia leve.");
                        clasificacion_tem = "Hipotermia leve.";
                        txt_resultado_hora.Foreground = Brushes.Blue;
                    }
                    else if (temperature >= 33 && temperature <= 34.4)
                    {
                        //MessageBox.Show("Hipotermia leve.");
                        clasificacion_tem = "Hipotermia";
                        txt_resultado_hora.Foreground = Brushes.BlueViolet;
                    }
                    else if (temperature < 33)
                    {
                        txt_resultado_hora.Visibility = Visibility.Hidden;
                        bot_siguiente_tem3.Visibility = Visibility.Hidden;
                        txt_resultado_temfin3.Visibility = Visibility.Hidden;
                        MessageBox.Show("Por Favor Colocar Sensor de Temperatura");

                    }


                    else if (temperature >= 37.5 && temperature < 39)
                    {
                        //MessageBox.Show("Fiebre moderada.");
                        clasificacion_tem = "Fiebre moderada.";
                        txt_resultado_hora.Foreground = Brushes.Yellow;
                    }
                    else if (temperature >= 39 && temperature < 41)
                    {
                        //MessageBox.Show("Fiebre alta.");
                        clasificacion_tem = "Fiebre alta.";
                        txt_resultado_hora.Foreground = Brushes.Orange;
                    }
                    else if (temperature >= 41 && temperature < 42)
                    {
                        //MessageBox.Show("Hiperpirexia.");
                        clasificacion_tem = "Hiperpirexia.";
                        txt_resultado_hora.Foreground = Brushes.Red;
                    }

                    else if (temperature >= 43)
                    {
                        txt_resultado_hora.Visibility = Visibility.Hidden;
                        bot_siguiente_tem3.Visibility = Visibility.Hidden;
                        txt_resultado_temfin3.Visibility = Visibility.Hidden;
                        MessageBox.Show("Por Favor Repita el Procedimiento");

                    }


                    txt_resultado_hora.Text = clasificacion_tem;
                    //patientService.InsertTempData(new TempData(patient.PatientID, DateTime.Now, (float)Math.Round(temperature, 1), clasificacion_tem));

                    srh.ClosePort();
                }
               
               
            }
            catch (Exception ex)
            {

                Debug.WriteLine(ex.ToString());
            }
         
                
           

        }

        private void PrintText(string text)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                FlowDocument document = new FlowDocument(new Paragraph(new Run(text)));
                document.PagePadding = new Thickness(50);

                IDocumentPaginatorSource paginatorSource = document;
                printDialog.PrintDocument(paginatorSource.DocumentPaginator, "Print Example");
            }
        }


        private void bot_siguiente_tem3_Click_1(object sender, RoutedEventArgs e)
        {
           
 // revisar  no gurda temperatura 
            patientService.InsertTempData(new TempData(patient.PatientID, DateTime.Now, (float)Math.Round(temperature, 1), txt_resultado_hora.Text));

            grid_toma_signos.Visibility = Visibility.Visible;
            grid_toma_tempe.Visibility = Visibility.Hidden;
            boton_toma.Visibility = Visibility.Visible;

            txt_resultado_temperatura.Text = "Temp " + temperature.ToString() + " °C";
            txtem = 1;
        }

       /// <summary>
       ///  Toma de presión arterial event click 
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>

        private void boton_presion_Click(object sender, RoutedEventArgs e)
        {

            if (serialHealth.isConnected())
            {
                bot_regresar.Visibility = Visibility.Hidden;
                boton_salir.Visibility = Visibility.Hidden;
                //MessageBox.Show("Procesando");
                using (SerialHealthMonitor srh = new SerialHealthMonitor())
                {
                    serialHealth.OpenPort();
                Debug.WriteLine("Enviando");
                serialHealth.SendHexData(CMD_NIBP);
                // Wait for a moment to allow the device to respond (you can adjust this based on your use case).
                System.Threading.Thread.Sleep(8000);
                double nibpm = serialHealth.HexToDecimal(serialHealth.NIBP.ToString().Substring(0, 2), 1);
                double nibpd = serialHealth.HexToDecimal(serialHealth.NIBP.ToString().Substring(2, 2), 1);
                double nibph = serialHealth.HexToDecimal(serialHealth.NIBP.ToString().Substring(4, 2), 1);
                double nibpmi = serialHealth.HexToDecimal(serialHealth.NIBP.ToString().Substring(6, 2), 1);
                double nibps = serialHealth.HexToDecimal(serialHealth.NIBP.ToString().Substring(8, 2), 1);
                double nibpPAS = serialHealth.HexToDecimal(serialHealth.NIBP.ToString().Substring(12, 2), 1);
                double nibpPAD = serialHealth.HexToDecimal(serialHealth.NIBP.ToString().Substring(16, 2), 1);
                double nibpPAME = serialHealth.HexToDecimal(serialHealth.NIBP.ToString().Substring(20, 2), 1);
                double nibpPAM = serialHealth.HexToDecimal(serialHealth.NIBP.ToString().Substring(24, 2), 1);

                // txt_resultado_presion_hora.Text = nibpm.ToString() + "/" + nibpd.ToString() + " " + nibph.ToString() + ":" + nibpmi.ToString() + ":" + nibps.ToString();
                resnibp1 = nibpm.ToString() + "/" + nibpd.ToString() + " " + nibph.ToString() + ":" + nibpmi.ToString() + ":" + nibps.ToString();
                txt_resultado_presion.Text = nibpPAS.ToString();
                txt_resultado_presion_pre.Text = nibpPAD.ToString();
                //MessageBox.Show(" hora: " + resnibp1);

                serialHealth.ClosePort();

                grid_toma_signos.Visibility = Visibility.Hidden;
                grid_toma_tempe.Visibility = Visibility.Hidden;
                grid_toma_presion.Visibility = Visibility.Visible;

                txt_arterial_paso3.Visibility = Visibility.Hidden;

                txt_arterial_paso42.Visibility = Visibility.Hidden;
                bot_siguiente_presion2.Visibility = Visibility.Hidden;
                    bot_siguiente_presion1.Visibility = Visibility.Visible;
                    txt_Signos_presion_resl.Visibility = Visibility.Hidden;
                    boton_salir.Visibility = Visibility.Visible;
                    txt_resultado_presion.Visibility = Visibility.Hidden;
                bot_guardar_presion.Visibility = Visibility.Hidden;
                boton_gris.Visibility = Visibility.Hidden;

                txt_resultado_presion_pre.Visibility = Visibility.Hidden;
                txt_resultado_presion_hora.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                MessageBox.Show("Por favor encender el dispositivo del boton amarillo, Dispositivo Apagado");

            }


        }

        private void bot_siguiente_presion1_Click(object sender, RoutedEventArgs e)
        {
            txt_arterial_paso3.Visibility = Visibility.Visible;
            boton_gris.Visibility = Visibility.Visible;
            txt_arterial_paso42.Visibility = Visibility.Visible;
            bot_siguiente_presion2.Visibility = Visibility.Visible;
            txt_Signos_presion_resl.Visibility = Visibility.Hidden;
            txt_resultado_presion.Visibility = Visibility.Hidden;
            bot_guardar_presion.Visibility= Visibility.Hidden;
            bot_siguiente_presion1.Visibility = Visibility.Hidden;
            boton_salir.Visibility = Visibility.Hidden;
            bot_guardar_presion.Visibility = Visibility.Hidden;
            boton_salir_pre.Visibility = Visibility.Hidden;
        }

        private void bot_siguiente_presion2_Click(object sender, RoutedEventArgs e)
        {


            txt_resultado_presion_pre.Visibility = Visibility.Visible;
            txt_resultado_presion_hora.Visibility = Visibility.Visible;


            txt_Signos_presion_resl.Visibility = Visibility.Visible;
            txt_resultado_presion.Visibility = Visibility.Visible;
            bot_guardar_presion.Visibility = Visibility.Visible;


            using (SerialHealthMonitor srh = new SerialHealthMonitor())
            {
                serialHealth.OpenPort();
                Logger.Info($"Tomando Presion  Iniciando ");

                serialHealth.SendHexData(CMD_NIBP);

            // Wait for a moment to allow the device to respond (you can adjust this based on your use case).
            System.Threading.Thread.Sleep(8000);
            double nibpm = serialHealth.HexToDecimal(serialHealth.NIBP.ToString().Substring(0, 2), 1);
            double nibpd = serialHealth.HexToDecimal(serialHealth.NIBP.ToString().Substring(2, 2), 1);
            double nibph = serialHealth.HexToDecimal(serialHealth.NIBP.ToString().Substring(4, 2), 1);
            double nibpmi = serialHealth.HexToDecimal(serialHealth.NIBP.ToString().Substring(6, 2), 1);
            double nibps = serialHealth.HexToDecimal(serialHealth.NIBP.ToString().Substring(8, 2), 1);
            nibpPAS = serialHealth.HexToDecimal(serialHealth.NIBP.ToString().Substring(12, 2), 1);
            nibpPAD = serialHealth.HexToDecimal(serialHealth.NIBP.ToString().Substring(16, 2), 1);
            double nibpPAME = serialHealth.HexToDecimal(serialHealth.NIBP.ToString().Substring(20, 2), 1);
            double nibpPAM = serialHealth.HexToDecimal(serialHealth.NIBP.ToString().Substring(24, 2), 1);

            // txt_resultado_presion_hora.Text = nibpm.ToString() + "/" + nibpd.ToString() + " " + nibph.ToString() + ":" + nibpmi.ToString() + ":" + nibps.ToString();
            resnibp = nibpm.ToString() + "/" + nibpd.ToString() + " " + nibph.ToString() + ":" + nibpmi.ToString() + ":" + nibps.ToString();
                Logger.Info($"Presion tomada   " + resnibp);

                txt_resultado_presion.Text = "PAS mmHg  " + nibpPAS.ToString();
            txt_resultado_presion_pre.Text = "PAD mmHg  " + nibpPAD.ToString();
                Logger.Info($" PAS  " + nibpPAS.ToString()) ;
                Logger.Info($" PAD  " + nibpPAD.ToString());

                //MessageBox.Show(" hora: " + resnibp);

                string clasificacion_pre = "";
            int cmpVal = resnibp.CompareTo(resnibp1);

            if (cmpVal == 0)
            {

                txt_resultado_presion.Visibility = Visibility.Hidden;
                txt_resultado_presion_pre.Visibility = Visibility.Hidden;
                txt_resultado_hora.Visibility = Visibility.Hidden;
                bot_regresar.Visibility = Visibility.Hidden;
                boton_salir.Visibility = Visibility.Hidden;
                bot_guardar_presion.Visibility = Visibility.Hidden;
                txt_resultado_presion_hora.Visibility = Visibility.Hidden;

                MessageBox.Show("Por Favor Colocar Brazalete y repetir procedimiento.");

            }
            else if (cmpVal > 0)
            {
                //MessageBox.Show(" no iguales1  ");


                

                if (nibpPAS < 90 && nibpPAD < 60)
                {
                    //MessageBox.Show("La presión es baja.");
                    clasificacion_pre = "La presión es baja.";
                    txt_resultado_presion_hora.Foreground = Brushes.Blue;

                }
                else if ((nibpPAS >= 90 && nibpPAS <= 120) && (nibpPAD >= 60 && nibpPAD <= 80))
                {
                    //MessageBox.Show("La presión es normal.");
                    clasificacion_pre = "La presión es normal.";
                    txt_resultado_presion_hora.Foreground = Brushes.Green;
                }
                else if ((nibpPAS > 120 && nibpPAS <= 130) || (nibpPAD > 80 && nibpPAD <= 85))
                {
                    //MessageBox.Show("La presión es elevada (prehipeertension).");
                    clasificacion_pre = "La presión es elevada (Prehipertension).";
                    txt_resultado_presion_hora.Foreground = Brushes.Yellow;
                }
                else if ((nibpPAS > 130 && nibpPAS <= 180) || (nibpPAD > 85 && nibpPAD <= 120))
                {
                    //MessageBox.Show("La presión es alta (hipertensión).");
                    clasificacion_pre = "La presión es alta (Hipertensión).";
                    txt_resultado_presion_hora.Foreground = Brushes.Orange;
                }

                else if ((nibpPAS > 130 && nibpPAS <= 180) || (nibpPAD > 85 && nibpPAD <= 120))
                {
                    //MessageBox.Show("La presión es alta (hipertensión).");
                    clasificacion_pre = "La presión es alta (Hipertensión).";
                    txt_resultado_presion_hora.Foreground = Brushes.Orange;
                }
                else 
                {

                    //MessageBox.Show("La presión es muy alta. Consulta a un médico.");
                    clasificacion_pre = "Por favor Realizar nuevamente el procedimiento";
                    txt_resultado_presion_hora.Foreground = Brushes.Red;
                }


                sys_value = nibpPAS;
                dia_value = nibpPAD;
                mean_value = nibpPAME;
                prpre_value = nibpPAM;

                txt_resultado_presion_hora.Text = clasificacion_pre;

                serialHealth.ClosePort();
              }
            }

        } 

        private void bot_guardar_presion_Click(object sender, RoutedEventArgs e)
        {
            patientService.InsertNibpData(new NIBPData(patient.PatientID, DateTime.Now, (int)sys_value, (int)dia_value, (int)mean_value, (int)prpre_value, txt_resultado_presion_hora.Text));
            grid_toma_signos.Visibility = Visibility.Visible;
            grid_toma_presion.Visibility = Visibility.Hidden;
            boton_salir.Visibility = Visibility.Hidden;
            bot_guardar_presion.Visibility = Visibility.Hidden;
            txt_resultado_presion_toma.Text = "PAS " + nibpPAS.ToString() + " PAD " + nibpPAD.ToString();
            txnibp = 1;
        }

       



        private void boton_oximetria_Click(object sender, RoutedEventArgs e)
        {


            if (serialHealth.isConnected())
            {

                using (SerialHealthMonitor srh = new SerialHealthMonitor())
                {
                    Logger.Info($"Starting Obteniendo Oximetria, comando " +  CMD_SPO );

                    serialHealth.OpenPort();
                   
                    serialHealth.SendHexData(CMD_SPO);

                    // Wait for a moment to allow the device to respond (you can adjust this based on your use case).
                    System.Threading.Thread.Sleep(8000);
                    double spom = serialHealth.HexToDecimal(serialHealth.SOP.ToString().Substring(0, 2), 1);
                    double sopd = serialHealth.HexToDecimal(serialHealth.SOP.ToString().Substring(2, 2), 1);
                    double spoh = serialHealth.HexToDecimal(serialHealth.SOP.ToString().Substring(4, 2), 1);
                    double spomi = serialHealth.HexToDecimal(serialHealth.SOP.ToString().Substring(6, 2), 1);
                    double sops = serialHealth.HexToDecimal(serialHealth.SOP.ToString().Substring(8, 2), 1);
                    Logger.Info($"Obtained Object Serial Heal SOP" + serialHealth.SOP.ToString());
                    resnibp1 = spom.ToString() + "/" + sopd.ToString() + " " + spoh.ToString() + ":" + spomi.ToString() + ":" + sops.ToString();
                    Logger.Info($"Obtained Object" + resnibp1);

                    serialHealth.ClosePort();


                    grid_toma_signos.Visibility = Visibility.Hidden;
                    grid_toma_tempe.Visibility = Visibility.Hidden;
                    grid_toma_presion.Visibility = Visibility.Hidden;
                    grid_oximetria_resultado.Visibility = Visibility.Visible;

                    txt_oximetria_paso2.Visibility = Visibility.Hidden;

                    txt_oximetria_paso3.Visibility = Visibility.Hidden;
                    txt_oximetria_paso4.Visibility = Visibility.Hidden;
                    bot_siguiente_oximetria2.Visibility = Visibility.Hidden;

                    txt_resultado_oximetria.Visibility = Visibility.Hidden;
                    txt_Signos_oximetria_resul.Visibility = Visibility.Hidden;
                    bot_Toma_oximetria.Visibility = Visibility.Hidden;
                    txt_resultado_oximetria_pre.Visibility = Visibility.Hidden;
                    txt_resultado_oximetria_hora.Visibility = Visibility.Hidden;
                    txt_resultado_oximetria_hora_pre.Visibility = Visibility.Hidden;
                }
               }
            else
                {
                MessageBox.Show("Por favor encender el dispositivo del boton amarillo, Dispositivo Apagado");


            }

        }

        private void bot_siguiente_oximetria1_Click(object sender, RoutedEventArgs e)
        {
            txt_oximetria_paso2.Visibility = Visibility.Visible;
           
            txt_oximetria_paso3.Visibility = Visibility.Visible;
            txt_oximetria_paso4.Visibility = Visibility.Visible;
            bot_siguiente_oximetria2.Visibility = Visibility.Visible;
            bot_siguiente_oximetria1.Visibility = Visibility.Hidden;
            boton_salir_oxi.Visibility = Visibility.Hidden;
        }


        static void PrintToPrinter(string text)
        {
            LocalPrintServer printServer = new LocalPrintServer();
            PrintQueue defaultPrintQueue = printServer.DefaultPrintQueue;

            using (PrintSystemJobInfo printJob = defaultPrintQueue.AddJob())
            {
                using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(printJob.JobStream, System.Text.Encoding.Unicode))
                {
                    streamWriter.Write(text);
                }
            }
        }



        private void bot_siguiente_oximetria2_Click_1(object sender, RoutedEventArgs e)
        {
           
            txt_resultado_oximetria.Visibility = Visibility.Visible;
            txt_Signos_oximetria_resul.Visibility = Visibility.Visible;
            bot_Toma_oximetria.Visibility = Visibility.Visible;
            txt_resultado_oximetria_pre.Visibility = Visibility.Visible;
            bot_siguiente_oximetria2.Visibility = Visibility.Hidden;
            txt_resultado_oximetria_hora.Visibility = Visibility.Visible;
            txt_resultado_oximetria_hora_pre.Visibility = Visibility.Visible;
            // boton_salir_oxi.Visibility = Visibility.Hidden;

            using (SerialHealthMonitor srh = new SerialHealthMonitor())
            {

                serialHealth.OpenPort();
            Debug.WriteLine("Enviando");
            serialHealth.SendHexData(CMD_SPO);

            // Wait for a moment to allow the device to respond (you can adjust this based on your use case).
            System.Threading.Thread.Sleep(8000);
            double spom = serialHealth.HexToDecimal(serialHealth.SOP.ToString().Substring(0, 2), 1);
            double sopd = serialHealth.HexToDecimal(serialHealth.SOP.ToString().Substring(2, 2), 1);
            double spoh = serialHealth.HexToDecimal(serialHealth.SOP.ToString().Substring(4, 2), 1);
            double spomi = serialHealth.HexToDecimal(serialHealth.SOP.ToString().Substring(6, 2), 1);
            double sops = serialHealth.HexToDecimal(serialHealth.SOP.ToString().Substring(8, 2), 1);
            spo = serialHealth.HexToDecimal(serialHealth.SOP.ToString().Substring(12, 2),1);
            pre = serialHealth.HexToDecimal(serialHealth.SOP.ToString().Substring(16, 2),1);
            resnibp = spom.ToString() + "/" + sopd.ToString() + " " + spoh.ToString() + ":" + spomi.ToString() + ":" + sops.ToString();
            txt_resultado_oximetria_pre.Text = "FP bpm " + pre.ToString();
            txt_resultado_oximetria.Text = "SpO2 % " + spo.ToString();
            // MessageBox.Show("Temperature is " + temperature);

            //txt_resultado_oximetria_hora.Text = spom.ToString() + "/" + sopd.ToString() + " " + spoh.ToString() + ":" + spomi.ToString() + ":" + sops.ToString();
            string hora_sop = spom.ToString() + "/" + sopd.ToString() + " " + spoh.ToString() + ":" + spomi.ToString() + ":" + sops.ToString();
            string clasificacion_spo = " ";
            string clasificacion_pres = " ";


            int cmpVal = resnibp.CompareTo(resnibp1);

            if (cmpVal == 0)
            {

                txt_resultado_oximetria.Visibility = Visibility.Hidden;
                txt_resultado_oximetria_pre.Visibility = Visibility.Hidden;
                txt_resultado_oximetria_hora.Visibility = Visibility.Hidden;
                txt_resultado_oximetria_hora_pre.Visibility = Visibility.Hidden;
                bot_Toma_oximetria.Visibility = Visibility.Hidden;
                bot_siguiente_oximetria2.Visibility = Visibility.Visible;

                MessageBox.Show("Por Favor Colocar Pinza y repetir procedimiento.");

            }
            else if (cmpVal > 0)
            {
                //MessageBox.Show(" no iguales1  ");



                if (spo >= 96)
                {
                    //MessageBox.Show("Saturación de oxígeno y ratio de pulso normales.");
                    clasificacion_spo = "Saturación de oxígeno normales.";
                    txt_resultado_oximetria_hora.Foreground = Brushes.Blue;
                }
                else if (spo >= 94 && spo <= 95)
                {
                    //MessageBox.Show("Saturación de oxígeno aceptable y ratio de pulso aceptable.");
                    clasificacion_spo = "Saturación de oxígeno aceptable.";
                    txt_resultado_oximetria_hora.Foreground = Brushes.Green;

                }
                else if (spo >= 90 && spo <= 93)
                {
                    //MessageBox.Show("Saturación de oxígeno baja. Consulte a su médico.");
                    clasificacion_spo = "Saturación de oxígeno baja.";
                    txt_resultado_oximetria_hora.Foreground = Brushes.Orange;

                }
                else if (spo > 74 && spo <= 89)
                {

                    // MessageBox.Show("Saturación de oxígeno muy baja. Busque ayuda médica urgente.");
                    clasificacion_spo = "Saturación de oxígeno muy baja.";
                    txt_resultado_oximetria_hora.Foreground = Brushes.Red;
                }
                else if (spo <= 74)
                {

                    // MessageBox.Show("Saturación de oxígeno muy baja. Busque ayuda médica urgente.");
                    //clasificacion_spo = "ja.";
                    //txt_resultado_oximetria_hora.Foreground = Brushes.Red;
                    MessageBox.Show("Por Favor Colocar Brazalete y repetir procedimiento.");

                }



                if (pre <= 65)
                {
                    //MessageBox.Show("Saturación de oxígeno y ratio de pulso normales.");
                    // clasificacion_pres = "Pulso normale.";
                    // txt_resultado_oximetria_hora_pre.Foreground = Brushes.Blue;
                    MessageBox.Show("Por Favor Colocar Brazalete y repetir procedimiento.");
                }

                else if (pre <= 100)
                {
                    //MessageBox.Show("Saturación de oxígeno y ratio de pulso normales.");
                    clasificacion_pres = "Pulso normale.";
                    txt_resultado_oximetria_hora_pre.Foreground = Brushes.Blue;
                }
                else if (pre >= 101 && pre <= 109)
                {
                    //MessageBox.Show("Saturación de oxígeno aceptable y ratio de pulso aceptable.");
                    clasificacion_pres = "Pulso aceptable.";
                    txt_resultado_oximetria_hora_pre.Foreground = Brushes.Green;

                }
                else if (pre >= 110 && pre <= 130)
                {
                    //MessageBox.Show("Saturación de oxígeno baja. Consulte a su médico.");
                    clasificacion_pres = "Consulte a su médico.";
                    txt_resultado_oximetria_hora_pre.Foreground = Brushes.Orange;

                }
                else if (pre >= 131 && pre <= 160)
                {

                    // MessageBox.Show("Saturación de oxígeno muy baja. Busque ayuda médica urgente.");
                    clasificacion_pres = "Busque ayuda médica urgente.";
                    txt_resultado_oximetria_hora_pre.Foreground = Brushes.Red;
                }
                 else if (pre >= 161) 
                {

                    // MessageBox.Show("Saturación de oxígeno muy baja. Busque ayuda médica urgente.");
                 //   clasificacion_pres = "Busque ayuda médica urgente.";
                   // txt_resultado_oximetria_hora_pre.Foreground = Brushes.Red;
                    MessageBox.Show("Por Favor Colocar Brazalete y repetir procedimiento.");
                }
            
            }

            spo_value = spo;
            pr_value = pre;
            txt_resultado_oximetria_hora.Text = clasificacion_spo;
            txt_resultado_oximetria_hora_pre.Text = clasificacion_pres;
            serialHealth.ClosePort();
               }


        }

        private void bot_Toma_oximetria_Click(object sender, RoutedEventArgs e)
        {
            patientService.InsertOxyData(new SPO2Data(patient.PatientID,DateTime.Now,(int)spo_value, (int) pr_value, txt_resultado_oximetria_hora.Text, txt_resultado_oximetria_hora_pre.Text));
        
           // patientService.InsertOxyData(new SPO2Data(patient.PatientID, DateTime.Now, spo, pre, txt_resultado_hora.Text));

            grid_toma_signos.Visibility = Visibility.Visible;
            grid_oximetria_resultado.Visibility = Visibility.Hidden;
            txt_resultado_sop_toma.Text = "SpO2 % " + spo.ToString() + " FP " + pre.ToString();
            txspo = 1;
        }

        private void Button_Click_Salir(object sender, RoutedEventArgs e)
        {
            UserView udv = new UserView();
            this.Hide();
            udv.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UserDiagnosticView udv = new UserDiagnosticView(patient);
            this.Hide();
            udv.Show();
        }

       
        private void Button_Click_Salir_toma(object sender, RoutedEventArgs e)
        {
            UserView udv = new UserView();
            this.Hide();
            udv.Show();

                      


        }




        private void boton_toma_Click(object sender, RoutedEventArgs e)
        {
            grid_toma_presion.Visibility = Visibility.Hidden;
            grid_toma_signos.Visibility = Visibility.Visible;
            grid_toma_tempe.Visibility = Visibility.Hidden;
            grid_oximetria_resultado.Visibility = Visibility.Hidden;
        }

        private void boton_imprimir_Click(object sender, RoutedEventArgs e)
        {
           
            if (txspo >= 1 && txnibp >= 1 && txtem >= 1)
            {
                string concat = "   SIM_NET   " + "\n" +
                "   RESULTADOS:   " + "\n" +
                "" + txt_resultado_sop_toma.Text + "\n" +
                "" + txt_resultado_presion_toma.Text + "\n" +
                "" + txt_resultado_temperatura.Text + "\n" +
                ".      " + "\n" +
                " .     " + "\n" +
                "  .    " + "\n" +
                "   .   " + "\n" +
                "    .  " + "\n" +
                "     . " + "\n" +
                "    .  " + "\n";
                PrintText(concat);
                //  txt_resultado_presion_toma.Text
                //    txt_resultado_temperatura.Text
            }
            else
            {
                MessageBox.Show("Por Favor Tomar sus Signos Vitales");
            }
                
        }
    }
}
