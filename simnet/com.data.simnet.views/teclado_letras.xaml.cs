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
    /// Lógica de interacción para teclado_letras.xaml
    /// </summary>
    public partial class teclado_letras : Window
    {
        TextBox txt_input;

        public teclado_letras(TextBox textBox)
        {

            this.txt_input = textBox;
            InitializeComponent();
            TypedText.Text = txt_input.Text;
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
                        txt_input.Text = txt_input.Text.Substring(0, txt_input.Text.Length - 1);

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



    }
}