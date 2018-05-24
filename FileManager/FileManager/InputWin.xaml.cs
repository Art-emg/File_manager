using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для RenameWindow.xaml
    /// </summary>
    public partial class InputWin : Window
    {
        public event Action<string> Enter;
        public event Action<ListBox, string> Output;
        public InputWin()
        {
            InitializeComponent();
        }
        string textTitle = "";
        ListBox lb;
        string path;

        public InputWin(string str) : this()
        {
            textTitle = str;

        }
        public InputWin(ListBox _lb, string _path, string title) : this()
        {
            textTitle = title;
            path = _path;
            lb = _lb;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(inputText.Text))
                Enter(inputText.Text);
                Output(lb, path);
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Focus();
            inputText.Focus();
            TitleWin.Text = textTitle.ToString();

        }

        private void inputText_KeyPress(object sender, KeyEventArgs e)
        {

            Regex pat = new Regex(@"\\");
            bool b = pat.IsMatch(e.Key.ToString());
            if (b != false)
            {
                e.Handled = true;
            }
            if (e.Key == Key.Enter)
            {
                if (!String.IsNullOrEmpty(inputText.Text))
                    Enter(inputText.Text);
                Output(lb, path);
                this.Close();
            }
        }
    }
}
