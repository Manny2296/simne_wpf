using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace simnet.com.data.simnet.models
{
    internal class SerialHealthMonitor : IDisposable
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SerialHealthMonitor));
        private SerialPort comPort;
        public string Temperature { get; set; }
        public string SOP { get; set; }
        public string NIBP { get; set; }
        public SerialHealthMonitor()
        {
            comPort = new SerialPort(ConfigurationManager.AppSettings["SerialPort"].ToString());
            int baudRate = 9600;
            comPort.BaudRate = baudRate;
            comPort.DataReceived += SerialPort_DataReceived;
            Logger.Info($"SerialHealthMonitor initialized with port {comPort.PortName} and baud rate {baudRate}");

        }

        public bool isConnected()
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
                Logger.Info($"Port {comPort.PortName} is available.");
            }
            catch (System.UnauthorizedAccessException)
            {
                // The port is in use by another application
              //  MessageBox.Show("El Dispositivo está siendo utilizado por otra aplicación");
                isPortBeingUsed = true;
                Logger.Warn($"Port {comPort.PortName} is in use by another application.");
            }
            catch (System.IO.IOException)
            {
                // The port does not exist or is disconnected
             //   MessageBox.Show("El puerto no existe o está desconectado");
            }

            if (isPortAvailable || isPortBeingUsed)
            {
                // The port is available (device connected)
             //   MessageBox.Show("Dispositivo conectado");
              Logger.Error($"Port {comPort.PortName} does not exist or is disconnected.");
                return true;
            }
            else
            {
              //  // The port is in use by another application or the device is disconnected
              //  MessageBox.Show("Dispositivo desconectado");
                return false;
            }






        }




        public void OpenPort()
        {
            try
            {
                comPort.Open();
                Logger.Info($"Port {comPort.PortName} opened successfully.");   
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error opening the serial port: " + ex.Message);
                Logger.Error($"Error opening the serial port {comPort.PortName}.", ex);
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
        /// <summary>
        /// convert string hexa to decimal value 
        /// </summary>
        /// <param name="hexValue"></param>
        /// <returns></returns>
        public double HexToDecimal(string hexValue, double divisor)
        {
            long decimalValue = Convert.ToInt64(hexValue, 16);
            double result = (double)decimalValue / divisor ;
            return result;
        }

        public void SendHexData(string hexData)
        {
            if (comPort.IsOpen)
            {
                try
                {
                    byte[] bytesToSend = StringToByteArray(hexData);
                    comPort.Write(bytesToSend, 0, bytesToSend.Length);
                  //  Debug.WriteLine("Data sent successfully.");
                    Logger.Info($"Hex data sent successfully: {hexData}");
                }
                catch (Exception ex)
                {
                 //   Debug.WriteLine("Error sending data: " + ex.Message);
                    Logger.Error($"Error sending hex data: {hexData}", ex);
                }
            }
            else
            {
               // Debug.WriteLine("Serial port is not open. Call OpenPort() first.");
                Logger.Warn("Serial port is not open. Call OpenPort() first.");
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
                    Logger.Info($"Data sent successfully: {data}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error sending data: " + ex.Message);
                    Logger.Error($"Error sending data: {data}", ex);
                }
            }
            else
            {
                Debug.WriteLine("Serial port is not open. Call OpenPort() first.");
                Logger.Warn("Serial port is not open. Call OpenPort() first.");
            }
        }

        public void ClosePort()
        {
            if (comPort.IsOpen)
            {
                comPort.Close();
                Debug.WriteLine("## Serial port closed.");
                Logger.Info("Serial port closed.");
            }
            else
            {
                Debug.WriteLine("## Serial port is not open.");
                Logger.Warn("Serial port is not open.");
            }
        }
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                SerialPort sp = (SerialPort)sender;
                string receivedData = sp.ReadExisting();
               // Debug.WriteLine("## Received data: " + receivedData);
                Logger.Info($"Received data: {receivedData}");
                byte[] bytes = StringToBytes(receivedData);
                string hexavalue = BytesToHex(bytes);
              //  Debug.WriteLine("## Hexa Value" + hexavalue);
                Logger.Info($"Hex value: {hexavalue}");
                string temperatura_hexa = hexavalue.Substring(hexavalue.Length - 14);
                Logger.Info($"Temperature: {temperatura_hexa}");
               // Debug.WriteLine("## Temperatura: " + temperatura_hexa);
                string SPO_hexa = hexavalue.Substring(hexavalue.Length - 28);
                Logger.Info($"SOP: {SPO_hexa}");
               // Debug.WriteLine("## SOP: " + SPO_hexa);
                string NIPB_hexa = hexavalue.Substring(hexavalue.Length - 36);
               // Debug.WriteLine("## NIPB: " + NIPB_hexa);
                Logger.Info($"NIBP: {NIPB_hexa}");
                // MessageBox.Show("REc" + receivedData);
                Temperature = temperatura_hexa;
                SOP = SPO_hexa;
                NIBP = NIPB_hexa;
       
            }
            catch (Exception ex)
            {

                Debug.WriteLine("Exception : " + ex.ToString());
                Logger.Error("Exception in SerialPort_DataReceived.", ex);

            }

        }

        public static byte[] StringToBytes(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            return bytes;
        }

        
        public void Dispose()
        {
            comPort.Dispose();
            Logger.Info("SerialHealthMonitor disposed.");
        }
    }
}
