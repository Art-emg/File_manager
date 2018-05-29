using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FileManager
{
    public class TreeViewLeft
    {
        
        public static MainWindow mw = null;
        #region начало создания дерева, создает корневые элементы, и вызывает метода для дальнейшей работы

        public static void StartCreateTree(TreeView treeView)
        {
            foreach (string drive in Directory.GetLogicalDrives())
            {
                // Создает объект TreeViewItem
                TreeViewItem item = new TreeViewItem()
                {
                    // Устанавливает зоголовок
                    Header = drive,
                    //Устанавливает полны путь
                    Tag = drive
                };

                // Добавляет пустой подэлемент
                item.Items.Add(null);

                // Listen out for item being expended
                item.Expanded += Folder_Expanded;

                // Добавляет в корень дерева
                treeView.Items.Add(item);
                item.MouseLeftButtonUp += treeItem_MouseLeftButtonUp;

            }

        }
        #endregion

        #region Добавляет подкаталоги и тд
        public static void Folder_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = (TreeViewItem)sender;

            //если элемент не содержит данных, или только нулевые данные
            if (item.Items.Count != 1 || item.Items[0] != null)
                return;

            //очищает нулевые подкаталоги
            item.Items.Clear();

            //получаем полный путь
            string fullPath = (string)item.Tag;

                //создаем список для каталогов
                List<string> directories = new List<string>();

            try
            {
                if (Directory.GetDirectories(fullPath).Length > 100)
                 return;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
            // получаем каталоги из папки, обходя ошибки
            try
            {
                string[] dirs = Directory.GetDirectories(fullPath);
                if (dirs.Length > 0)

                    directories.AddRange(dirs);

            }
            catch { }


            // для каждого каталога 
            directories.ForEach(directoryPath =>
            {
                //Создаем элемент каталога
                TreeViewItem subItem = new TreeViewItem()
                {
                    //Устанавливает Header как имя папки
                    Header = GetFileFolderName(directoryPath),
                    //и Tag как полный путь
                    Tag = directoryPath
                };

                //Добавляем пустой элемент чтобы мы могли развернуть дерево папки
                subItem.Items.Add(null);

                //midle expanding
                subItem.Expanded += Folder_Expanded;

                item.Items.Add(subItem);

            });
        }

        
        #endregion

        #region Находит имя папки или файла из полного пути 
        public static string GetFileFolderName(string path)
        {
            // если нет пути, возвращяет пустое 
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            //Находит последний бэкслэш в пути
            int lastIndex = path.LastIndexOf('\\');

            //если не находит, возвращает весь путь 
            if (lastIndex <= 0)
                return path;

            // возвращает имя после последнего бэкслэша
            return path.Substring(lastIndex + 1);
        }
        #endregion

        private static void treeItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
                {
                    TreeViewItem item = (TreeViewItem)mw.FolderView.SelectedItem;

                    //MessageBox.Show(item.Tag.ToString());
                    ListFiles.InTreeClick(mw.RightListFile, mw.RightSearchDirText, item.Tag.ToString());
                }
    }
}
