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

namespace FileManager
{
    /// <summary>
    /// Логика взаимодействия для hashWin.xaml
    /// </summary>
    public partial class hashWin : Window
    {
        public hashWin()
        {
            InitializeComponent();
        }

        public hashWin(string file, string hash)
        {
           InitializeComponent();

            TitleWin.Text = "MD5 хеш для файла " + file;
            inputText.Text = hash;

            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            //inputText.Focus();
            

        }

    }
}
