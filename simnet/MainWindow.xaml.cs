using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace simnet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
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

        public string PatienFullName {
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

