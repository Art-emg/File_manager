using System;
using System.Collections.Generic;
using System.Data.Entity;
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
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        changeDatabaseEntities db;
        public Window1()
        {
            
            InitializeComponent();
            
            
                
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            db = new changeDatabaseEntities();
            db.Change.Load();
            chan.ItemsSource = db.Change.Local.ToBindingList();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
