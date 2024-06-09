using simnet.com.data.simnet.models;
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
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TextBox txt_input;
        public MainWindow(TextBox textBox)
        {
            this.txt_input = textBox;
            InitializeComponent();
            TypedText.Text = txt_input.Text;
            letras_minus.Visibility = Visibility.Visible;
            letras_mayuscu.Visibility = Visibility.Hidden;
            numeros.Visibility = Visibility.Hidden;
            correo.Visibility = Visibility.Hidden;
        }
        private void KeyButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string buttonText = button.Content.ToString();

                if (buttonText == "Borrar")
                {
                    if (TypedText.Text.Length > 0)
                    {
                        TypedText.Text = TypedText.Text.Substring(0, TypedText.Text.Length - 1);
                        txt_input.Text= txt_input.Text.Substring(0, txt_input.Text.Length - 1);

                    }
                }
                else
                {
                    try
                    {
                        TypedText.Text += buttonText;
                        txt_input.Text += buttonText;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Wrong caracter not allowed");
                        TypedText.Text = TypedText.Text.Substring(0, TypedText.Text.Length - 1);
                        txt_input.Text = txt_input.Text.Substring(0, txt_input.Text.Length - 1);

                    }
                  

                }
            }
        
        
        }

        public void tecla_mayus ()
        {
            letras_minus.Visibility = Visibility.Hidden;
            letras_mayuscu.Visibility = Visibility.Visible;
            numeros.Visibility = Visibility.Hidden;
            correo.Visibility = Visibility.Hidden;

        }

        private void Button_Click_mayus(object sender, RoutedEventArgs e)
        {
            letras_minus.Visibility = Visibility.Hidden;
            letras_mayuscu.Visibility = Visibility.Visible;
            numeros.Visibility = Visibility.Hidden;
            correo.Visibility = Visibility.Hidden;
        }

        private void Button_Click_minus(object sender, RoutedEventArgs e)
        {
            letras_minus.Visibility = Visibility.Visible;
            letras_mayuscu.Visibility = Visibility.Hidden;
            numeros.Visibility = Visibility.Hidden;
            correo.Visibility = Visibility.Hidden;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Usuario encontrado: " );
        }

        private void Button_Click_123(object sender, RoutedEventArgs e)
        {
            numeros.Visibility = Visibility.Visible;
            letras_minus.Visibility = Visibility.Hidden;
            letras_mayuscu.Visibility = Visibility.Hidden;
            correo.Visibility = Visibility.Hidden;

        }

        private void Button_Click_correo(object sender, RoutedEventArgs e)
        {
            numeros.Visibility = Visibility.Hidden;
            letras_minus.Visibility = Visibility.Hidden;
            letras_mayuscu.Visibility = Visibility.Hidden;
            correo.Visibility = Visibility.Visible;
        }

    }
}
