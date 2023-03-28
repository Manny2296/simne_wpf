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
    /// Lógica de interacción para UserDiagnosticView.xaml
    /// </summary>
    public partial class UserDiagnosticView : Window
    {
        //  private PatientService pastientService;
        private Patient patiente;

        public UserDiagnosticView(Patient p)
        {
            this.patiente = p;

            InitializeComponent();
            lbl_name.Content = p.Name;
            WindowState = WindowState.Maximized;
        }

        private void boton_actualizacion_Click(object sender, RoutedEventArgs e)
        {
           
            UserView udv = new UserView(patiente);
            this.Hide();
            udv.Show(); 
        }

        private void boton_toma_nuestra_Click(object sender, RoutedEventArgs e)
        {

            
            //grid_toma_signos.Visibility = Visibility.Visible
            win_toma udv = new win_toma(patiente);
            this.Hide();
            udv.Show();
            
        }

       
        private void boton_historial_Click(object sender, RoutedEventArgs e)
        {
            Historial udv = new Historial(patiente);
            this.Hide(); udv.Show();
        }

        private void Button_Click_Salir(object sender, RoutedEventArgs e)
        {
            UserView udv = new UserView();
            this.Hide();
            udv.Show();
        }
    }
}
