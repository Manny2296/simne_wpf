using simnet.com.data.simnet.services;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Threading;

namespace simnet
{   
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //DBConnection dBConnection = new DBConnection();
        private SerialPort comPort;
        //private DispatcherTimer timer;
        private PatientService patientService;
        public MainWindow()
        {
               DataContext = new MainWindowViewModel();
        patientService = new PatientService();
        // Initialize the COM port and subscribe to the event when the port state changes
        comPort = new SerialPort(ConfigurationManager.AppSettings["SerialPort"].ToString());
        // Obtener paciente por numero de documento; 
        com.data.simnet.models.Patient patient = patientService.GetPatientById("93389830");
        MessageBox.Show("PATIENT NAME CONSULTADO : " + patient.Name);
            Debug.WriteLine("###### APP STARTED See ya ######");
            string applicationPath = ConfigurationManager.AppSettings["ApplicationPath"];
        comPort.DataReceived += SerialPort_DataReceived;
            //MessageBox.Show($"Application Path: {applicationPath}");
            //execAppGetData(applicationPath);
           // string portName = "COM3";
            int baudRate = 9600;


        comPort.BaudRate = baudRate;
            InitializeComponent();
         
         

            //  System.Data.SQLite.SQLiteDataReader sQLiteDataReader = dBConnection.getPatients();
        }


        public void OpenPort()
        {
            try
            {
                comPort.Open();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error opening the serial port: " + ex.Message);
            }
        }
        /// <summary>
        ///  Metodo para convertir de string a hexadecimal
        /// </summary>
        /// <param name="input"> string to be parsed</param>
        /// <returns></returns>
        public static string StringToHex(string input)
        {
            char[] charArray = input.ToCharArray();
            StringBuilder hexBuilder = new StringBuilder();

            foreach (char c in charArray)
            {
                hexBuilder.Append(((int)c).ToString("X2"));
            }

            return hexBuilder.ToString();
        }

        /// <summary>
        /// Método para convertir de bytes to hexadecimal 
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string BytesToHex(byte[] bytes)
        {
            StringBuilder hexBuilder = new StringBuilder(bytes.Length * 2);

            foreach (byte b in bytes)
            {
                hexBuilder.Append(b.ToString("X2"));
            }

            return hexBuilder.ToString();
        }

        public void SendHexData(string hexData)
        {
            if (comPort.IsOpen)
            {
                try
                {
                    byte[] bytesToSend = StringToByteArray(hexData);
                    comPort.Write(bytesToSend, 0, bytesToSend.Length);
                    Debug.WriteLine("Data sent successfully.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error sending data: " + ex.Message);
                }
            }
            else
            {
                Debug.WriteLine("Serial port is not open. Call OpenPort() first.");
            }
        }

        private static byte[] StringToByteArray(string hex)
        {
            int length = hex.Length;
            byte[] bytes = new byte[length / 2];

            for (int i = 0; i < length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes;
        }

        public void SendData(string data)
        {
            if (comPort.IsOpen)
            {
                try
                {
                    comPort.Write(data);
                    Debug.WriteLine("Data sent successfully: " + data);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error sending data: " + ex.Message);
                }
            }
            else
            {
                Debug.WriteLine("Serial port is not open. Call OpenPort() first.");
            }
        }

        public void ClosePort()
        {
            if (comPort.IsOpen)
            {
                comPort.Close();
                Debug.WriteLine("## Serial port closed.");
            }
            else
            {
                Debug.WriteLine("## Serial port is not open.");
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort sp = (SerialPort)sender;
                string receivedData = sp.ReadExisting();
                Debug.WriteLine("## Received data: " + receivedData);
                byte[] bytes = StringToBytes(receivedData);
                string hexavalue =  BytesToHex(bytes);
                Debug.WriteLine("## Hexa Value" + hexavalue);
                string temperatura_hexa = hexavalue.Substring(hexavalue.Length - 14);
                Debug.WriteLine("## Temperatura: " + temperatura_hexa);
                string SPO_hexa = hexavalue.Substring(hexavalue.Length - 14);
                Debug.WriteLine("## SOP: " + SPO_hexa);
                string NIPB_hexa = hexavalue.Substring(hexavalue.Length - 36);
                Debug.WriteLine("## NIPB: " + NIPB_hexa);
                string oxi_hexa = hexavalue.Substring(hexavalue.Length - 36);
                Debug.WriteLine("## oxi: " + oxi_hexa);

                // MessageBox.Show("REc" + receivedData);
            }
            catch (Exception ex)
            {

                Debug.WriteLine("Exception : " + ex.ToString());
            }
            
        }

        public static byte[] StringToBytes(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            return bytes;
        }

        public void execAppGetData(string appliationdata)
        {

            string applicationPath = appliationdata;

            try
            {
                // Start the application
                Process process = Process.Start(applicationPath);

                // Wait for the application's main window to become available
                //process.WaitForInputIdle();

                // Find the main window using UI Automation
                AutomationElement mainWindow = AutomationElement.FromHandle(process.MainWindowHandle);

                // Find the "importar" button by its name
                System.Windows.Automation.Condition condition = new PropertyCondition(AutomationElement.NameProperty, ConfigurationManager.AppSettings["Buttoname"].ToString());
                AutomationElementCollection button = mainWindow.FindAll(TreeScope.Descendants, condition);

                if (button != null)
                {
                    // Simulate a click action on the button
                    MessageBox.Show("Ya LO ENNCONTRE click");
                    InvokePattern invokePattern = button[0].GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                    MessageBox.Show("Ya INVOCARE ");
                    invokePattern?.Invoke();
                    MessageBox.Show("Ya hize click");
                }
                else
                {
                    Debug.WriteLine("text");
                    MessageBox.Show("Button not found. No hize nada");
                }

                // Optionally, close the application after clicking the button
                process.CloseMainWindow();

            }
            catch (Exception e)
            {

                MessageBox.Show("Aplicacion no instalada en esta maquina, no hay conexión" + e.Message);
            }


        }

        private void ComPort_PinChanged(object sender, SerialPinChangedEventArgs e)
        {
            isConnected();
        }

        private void isConnected()
        {
            // Open the COM port (this will throw an exception if the port is not available)
            // Check if the port is available (not in use)
            bool isPortAvailable = false;
            bool isPortBeingUsed = false;
            try
            {
                comPort.Open();
                isPortAvailable = true;
                comPort.Close(); // Close the port after checking its availability
            }
            catch (System.UnauthorizedAccessException)
            {
                // The port is in use by another application
                MessageBox.Show("El Dispositivo está siendo utilizado por otra aplicación");
                isPortBeingUsed = true;
            }
            catch (System.IO.IOException)
            {
                // The port does not exist or is disconnected
                MessageBox.Show("El puerto no existe o está desconectado");
            }

            if (isPortAvailable || isPortBeingUsed)
            {
                // The port is available (device connected)
                MessageBox.Show("Dispositivo conectado");
            }
            else
            {
                // The port is in use by another application or the device is disconnected
                MessageBox.Show("Dispositivo desconectado");
            }






        }



        private void btn_is_open_Click(object sender, RoutedEventArgs e)
        {
            isConnected();
        }

       

        private void Button_Click_temperature(object sender, RoutedEventArgs e)
        {
            string hexDataToSend = "aa19020f010100110100000000000000000000000000000000dbb46465";
            comPort.Open();
            Console.WriteLine("Enviando");
            SendHexData(hexDataToSend);

            // Wait for a moment to allow the device to respond (you can adjust this based on your use case).
            System.Threading.Thread.Sleep(8000);

            comPort.Close();
        }

       

        private void Button_Click_SPO(object sender, RoutedEventArgs e)
        {
            string hexDataToSend = "aa19020e0101001101000000000000000000000000000000002836d397";
            comPort.Open();
            Console.WriteLine("Enviando");
            SendHexData(hexDataToSend);

            // Wait for a moment to allow the device to respond (you can adjust this based on your use case).
            System.Threading.Thread.Sleep(8000);

            comPort.Close();
        }

        private void Button_Click_NIBP(object sender, RoutedEventArgs e)
        {
            string hexDataToSend = "aa19020d01010011010000000000000000000000000000000038701636";
            comPort.Open();
            Console.WriteLine("Enviando");
            SendHexData(hexDataToSend);

            // Wait for a moment to allow the device to respond (you can adjust this based on your use case).
            System.Threading.Thread.Sleep(8000);

            comPort.Close();
        }
    }

    /// <summary>
    ///  ViewModel para info del paciente.
    ///  PatientName1 : Nombre del paciente
    ///  PatientName2:  Nombre 2 del paciente
    ///  PatientLastName1 : Apellido 1 del paciente 
    ///  PatientLastName2: Apellido 2 del paciente
    ///  TypeDocument : Tipo de Documento
    ///  Age: Edad
    ///  Gender : Sexo
    ///  Mail : Correo Electronico
    ///  Tel : Telefono
    /// </summary>
    public class MainWindowViewModel
    {
        public MainWindowViewModel()
        {
            PatientName1 = "Manuel";
            PatientName2 = "Felipe";
            PatienLastName1 = "Sanchez";
            PatienLastName2 = "Riaño";
            TypeDocument = "CC";
            DocumentNumber = "1018484513";
            Age = "27";
            Gender = "Masculino";
            Mail = "manuelfelip-96@gmail.com";
            Tel = "3183771756";
            VitalSigns = new ObservableCollection<VitalSign>
            {
                new VitalSign {SignName = "Ritmo Cardiaco", SignValue = "75 BPM"},
                new VitalSign {SignName = "SpO2", SignValue = "90%"},
                new VitalSign {SignName = "Temperatura", SignValue = "98.6 C"}
            };



        }

        public string PatientName1 { get; set; }
        public string PatientName2 { get; set; }

        public string PatienLastName1 { get; set; }
        public string PatienLastName2 { get; set; }

        public string TypeDocument { get; set; }

        public string DocumentNumber { get; set; }
        public string Age { get; set; }
        public string Gender { get; set; }
        public string Mail { get; set; }
        public string Tel { get; set; }
        public ObservableCollection<VitalSign> VitalSigns { get; set; }

        public string PatienFullName
        {
            get
            {
                return PatientName1 + " " + PatientName2 + " " + PatienLastName1 + " " + PatienLastName2;
            }
        }


    }

    public class VitalSign
    {
        public string SignName { get; set; }

        public string SignValue { get; set; }
    }
}

