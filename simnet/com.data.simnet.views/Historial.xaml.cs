using simnet.com.data.simnet.models;
using simnet.com.data.simnet.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace simnet.com.data.simnet.views
{
    /// <summary>
    /// Lógica de interacción para Historial.xaml
    /// </summary>
    public partial class Historial : Window
    {
        private Patient patiente;
        private List<TempData> TempDataLis = new List<TempData>();
        private List<SPO2Data> OxyDataLis = new List<SPO2Data>();
        private List<NIBPData> NibpDataLis = new List<NIBPData>();


        private PatientService patientService;

        public Historial()
        {

        }

        public Historial(Patient P)
        {
            this.patiente = P;
            
            patientService = new PatientService();
            DataContext = this;
            InitializeComponent();
            txt_nombre.Text = P.Name;
            txt_documento.Text = P.IdentityCode;
            // Historico de temperaturas
            this.TempDataLis = patientService.GetTempDatasbyPatientID(P.PatientID);
            this.OxyDataLis = patientService.GetOxybyPatientID(P.PatientID);
            this.NibpDataLis = patientService.GetNibpbyPatientID(P.PatientID);


            // lbl_name.Content = P.Name;
            dta_grid_temp.ItemsSource = TempDataLis;
            dta_grid_oxy.ItemsSource = OxyDataLis;
            dta_grid_presion.ItemsSource = NibpDataLis;

            WindowState = WindowState.Maximized;

        }

      //  internal List<TempData> TempDataList1 { get => TempDataList; set => TempDataList = value; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UserDiagnosticView udv = new UserDiagnosticView(patiente);
            this.Hide();
            udv.Show();
        }

        private void Button_Click_Salir(object sender, RoutedEventArgs e)
        {
            UserView udv = new UserView();
            this.Hide();
            udv.Show();
        }

       
    }
}
