using simnet.com.data.simnet.models;
using simnet.com.data.simnet.services;
using simnet.com.data.simnet.views;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace simnet
{
    /// <summary>
    /// Lógica de interacción para UserVIew.xaml
    /// </summary>
    public partial class UserView : Window
    {
        private PatientService patientService;
        private Patient patient;
        private const string DEVICE_ID = "000C8C9921010000077C950C";
        private int flag_insert = 0;

        private simnet.com.data.simnet.views.MainWindow keyboard;
        private simnet.com.data.simnet.views.Teclado_numero keyboard_numero;
        private simnet.com.data.simnet.views.teclado_letras keyboard_letras;



        public UserView()

        {
            
            InitializeComponent();
            patientService = new PatientService();
            //start form state
            functionVisibility(1);
          

        }

        public UserView(Patient p)
        {
            InitializeComponent();
            this.patient = p;
            patientService = new PatientService();
            PopulateFormwithPatient(patient);

            

        }
        private void functionVisibility(int opt)
        {
            switch (opt)
            {
                case 1:
                    {
                        //state 1 : usuario encontrado > form visible
                        Grid_user_data.Visibility = Visibility.Hidden;
                        btn_update.Visibility = Visibility.Hidden;
                        txt_datos_actua.Visibility = Visibility.Hidden;
                        break;
                    }
                case 2:
                    {
                        //state 2 : usuario encontrado > form visible
                        Grid_user_data.Visibility = Visibility.Visible;
                        btn_update.Visibility = Visibility.Visible;
                        txt_datos_actua.Visibility = Visibility.Visible;

                        break;
                    }
                default:
                    break;
            }
        }

        private void PopulateFormwithPatient(Patient patient)
        {
            // MessageBox.Show("Usuario encontrado: " + patient.Name);
            txt_documento.IsEnabled = false;
            txt_nombre.Text = patient.Name;
            //txt_nombre.IsEnabled = false;
            txt_estatura.Text = patient.Height.ToString();
            // txt_estatura.IsEnabled = false;
            //txt_edad.IsEnabled = false;
            if (patient.Gender == 1)
            {
                SetGender(1);

            }
            if (patient.Gender == 0)
            {
                SetGender(0);
            }

            //cmbx_genero.IsEnabled = false;
            txt_peso.Text = patient.Weight.ToString();
            //txt_peso.IsEnabled = false;
            txt_email.Text = patient.Address;
            //txt_email.IsEnabled = false;
            txt_edad.Text = patient.Age.ToString();
            txt_mobile.Text = patient.Mobile.ToString();
            //txt_mobile.IsEnabled = false;
            

            functionVisibility(2);
      
            

            btn_buscar.Visibility = Visibility.Hidden;
            txt_Documento.Visibility = Visibility.Hidden;
         
        }      


        private void Button_Click(object sender, RoutedEventArgs e)
        {
           
            if(!String.IsNullOrEmpty(txt_documento.Text))
            {
                try
                {



                    patient = patientService.GetPatientById(txt_documento.Text);
                    if (patient != null)
                    {
                        PopulateFormwithPatient(patient);
                        patient.ReadFlag = "99";
                        flag_insert = 0;
                       

                    }
                    else
                    {



                        MessageBox.Show("Usuario No Registrado por favor Registrarse.");
                        patient = new Patient();
                        flag_insert = 1;
                      





                        functionVisibility(2);

                        btn_buscar.Visibility = Visibility.Hidden;
                        txt_Documento.Visibility = Visibility.Hidden;


                    }
                    keyboard_numero.Close();


                }
                catch (System.Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }

            }
            else
            {
                MessageBox.Show("Documento es NULL o Esta Vacío");
                keyboard_numero.Close();

            }


        }

        public static string GenerateUuid()
        {
            Guid uuid = Guid.NewGuid();
            return uuid.ToString();
        }

        private void SetGender(int genderValue)
        {
            foreach (ComboBoxItem item in cmbx_genero.Items)
            {
                if (item.Tag.ToString() == genderValue.ToString())
                {
                    cmbx_genero.SelectedItem = item;
                    break;
                }
            }
        }

        private void txt_genero_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // MessageBox.Show("Econtrado : ");
            if (cmbx_genero.SelectedItem != null)
            {
                ComboBoxItem selectedGenderItem = (ComboBoxItem)cmbx_genero.SelectedItem;
                string selectedGender = selectedGenderItem.Content.ToString();
                int genderValue = int.Parse(selectedGenderItem.Tag.ToString());

              // lblSelectedGender.Content = $"Selected Gender: {selectedGender} ({genderValue})";
            }
        }


        private void btn_update_Click(object sender, RoutedEventArgs e)
        {
            //insert user 
            if (patient != null && flag_insert ==1)
            {
               
                patient.PatientID = GenerateUuid();
                patient.DeviceSN = DEVICE_ID;
                patient.Doctor = "Simnet²";
                patient.IdentityCode = txt_documento.Text;
                patient.ReadFlag = "99";
                patient.SocietyCode = "0";
                //  MessageBox.Show("Usuario no registrado, porfavor llene el formulario");
                // patient = new Patient("{" + txt_documento.Text + "}", DEVICE_ID, 0, txt_nombre.Text, Int32.Parse(cmbx_genero.Tag.ToString()),float.Parse(txt_estatura.Text),float.Parse(txt_peso.Text),txt_mobile.Text,"Dr. ECO²",0,txt_documento.Text,"0",0,Int32.Parse(txt_edad.Text),txt_email.Text,"0",DateTime.Now) ;
                patientService.InsertPatient(patient);
                txt_documento.Text = "";
                txt_documento.IsEnabled = true;
                Grid_user_data.Visibility = Visibility.Hidden;
                MessageBox.Show("Usuario registrado correctamente");
                UserDiagnosticView udv = new UserDiagnosticView(patient);
                flag_insert = 0;
                this.Hide();
                udv.Show();

            }
            //update new user
            else if (patient != null && flag_insert ==0)
            {
                patient.IdentityCode = txt_documento.Text;
                patient.ReadFlag = "99";
                patient.SocietyCode = "0";
                patientService.UpdatePatient(patient);
                txt_documento.Text = "";
                txt_documento.IsEnabled = true;
                Grid_user_data.Visibility = Visibility.Hidden;
                //MessageBox.Show("Usuario Actualizado correctamente");
                // MessageBox.Show("Actualizado correctamente");
                UserDiagnosticView udv = new UserDiagnosticView(patient);
                this.Hide();
                udv.Show();
            }

        }

       


        private void txt_nombre_TextChanged(object sender, TextChangedEventArgs e)
        {
          if(patient == null)
            {
                patient = new Patient();
            }
         
           
            
            if (!String.IsNullOrEmpty(txt_nombre.Text))
            {
                patient.Name = txt_nombre.Text;

            }
        }

        private void txt_estatura_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (patient == null)
            {
                patient = new Patient();
            }

            if (!String.IsNullOrEmpty(txt_estatura.Text))
            {
               
                    patient.Height = float.Parse(txt_estatura.Text);

                

            }
           
        }

        private void txt_peso_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (patient == null)
            {
                patient = new Patient();
            }

            if (!String.IsNullOrEmpty(txt_peso.Text))
            {
                patient.Weight = float.Parse(txt_peso.Text);
            }


            
        }

        private void txt_edad_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (patient == null)
            {
                patient = new Patient();
            }


            if (!String.IsNullOrEmpty(txt_edad.Text))
            {
                patient.Age = Int32.Parse(txt_edad.Text);

            }
            
        }

        private void txt_email_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (patient == null)
            {
                patient = new Patient();
            }
            if (!String.IsNullOrEmpty(txt_email.Text))
            {
                patient.Address = txt_email.Text;

            }

            
        }
        private void txt_nombre_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
      
            // Check if the input is numeric
            if (!IsNumericInput(e.Text))
            {
                e.Handled = true; // Suppress the input if it's not numeric
            }
        }

        private bool IsNumericInput(string input)
        {
            foreach (char c in input)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }

        private void txt_mobile_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (patient == null)
            {
                patient = new Patient();
            }
            if (!String.IsNullOrEmpty(txt_mobile.Text))
            {
                patient.Mobile = txt_mobile.Text;

            }


            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //if (keyboard != null)
            //{
            //    if (keyboard.IsActive)
            //    {
            //        keyboard.Close();

            //    }
            //    keyboard = new simnet.com.data.simnet.views.MainWindow(txt_nombre);

            //    keyboard.Show();
            //}
            //else
            //{
            //    keyboard = new simnet.com.data.simnet.views.MainWindow(txt_nombre);

            //    keyboard.Show();
            //}

            //simnet.com.data.simnet.views.Teclado_numero udv = new simnet.com.data.simnet.views.Teclado_numero();
            
            //udv.Show();

        }


        private void txt_peso_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (keyboard_numero != null)
            {
                if (keyboard_numero.IsActive || keyboard_numero.IsInitialized)
                {
                    keyboard_numero.Close();

                }
                keyboard_numero = new simnet.com.data.simnet.views.Teclado_numero(txt_peso);

                keyboard_numero.Show();
            }
            else
            {
                keyboard_numero = new simnet.com.data.simnet.views.Teclado_numero(txt_peso);

                keyboard_numero.Show();
            }
        }

        private void txt_edad_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (keyboard_numero != null)
            {
                if (keyboard_numero.IsActive)
                {
                    keyboard_numero.Close();

                }
                keyboard_numero = new simnet.com.data.simnet.views.Teclado_numero(txt_edad);

                keyboard_numero.Show();
            }
            else
            {
                keyboard_numero = new simnet.com.data.simnet.views.Teclado_numero(txt_edad);

                keyboard_numero.Show();
            }
        }


        private void txt_documento_TouchDown(object sender, System.Windows.Input.TouchEventArgs e)
        {

        }


        private void txt_nombre_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (keyboard_letras != null)
            {
                if (keyboard_letras.IsActive)
                {
                    keyboard_letras.Close();

                }
                keyboard_letras = new simnet.com.data.simnet.views.teclado_letras(txt_nombre);

                keyboard_letras.Show();
            }
            else
            {
                keyboard_letras = new simnet.com.data.simnet.views.teclado_letras(txt_nombre);

                keyboard_letras.Show();
            }
        }

        private void txt_estatura_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (keyboard_numero != null)
            {
                if (keyboard_numero.IsActive)
                {
                    keyboard_numero.Close();

                }
                keyboard_numero = new simnet.com.data.simnet.views.Teclado_numero(txt_estatura);

                keyboard_numero.Show();
            }
            else
            {
                keyboard_numero = new simnet.com.data.simnet.views.Teclado_numero(txt_estatura);

                keyboard_numero.Show();
            }
        }

        private void txt_documento_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            if (keyboard_numero != null)
            {
                if (keyboard_numero.IsActive || keyboard_numero.IsInitialized)
                {
                    keyboard_numero.Close();

                }
                keyboard_numero = new simnet.com.data.simnet.views.Teclado_numero(txt_documento);

                keyboard_numero.Show();
            }
            else
            {
                keyboard_numero = new simnet.com.data.simnet.views.Teclado_numero(txt_documento);

                keyboard_numero.Show();
            }
         
            

        }

        private void txt_correo_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (keyboard != null)
            {
                if (keyboard.IsActive)
                {
                    keyboard.Close();

                }
                keyboard = new simnet.com.data.simnet.views.MainWindow(txt_email);

                keyboard.Show();
            }
            else
            {
                keyboard = new simnet.com.data.simnet.views.MainWindow(txt_email);

                keyboard.Show();
            }
        }

        private void tex_celular(object sender, MouseButtonEventArgs e)
        {
            if (keyboard_numero != null)
            {
                if (keyboard_numero.IsActive)
                {
                    keyboard_numero.Close();

                }
                keyboard_numero = new simnet.com.data.simnet.views.Teclado_numero(txt_mobile);

                keyboard_numero.Show();
            }
            else
            {
                keyboard_numero = new simnet.com.data.simnet.views.Teclado_numero(txt_mobile);

                keyboard_numero.Show();
            }
        }
    }
}
