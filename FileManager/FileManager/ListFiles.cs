using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Drawing;


namespace FileManager
{
    public class ListFiles : MainWindow
    {
        public static MainWindow mw = null;
        public static ListFiles rlf = new ListFiles(); // создаем статический экз данного класса для работы с нестатичискими методами
        public static string varListPath = "";// путь , с которым работаем

        public static string thisPath = null; // полный путь копирумого объекта (с названием файла)
        public static string pathPaste = null; //полный путь вставляемого объекта (с названием файла)
        public static string thisName = null; // имя объекта с которым работаем (Select)
        public static string CutOrCopy = null; //"флаг" (да, с типом string) для метода, котрый определяет копир. или вырезать

        public ObservableCollection<ItemModel> Items { get; set; } = new ObservableCollection<ItemModel>();

        public static void OutputDrives(ListBox listFiles, TextBox pathTextBox) // вывод всех дисков в ListBox
        {
            try
            {
                listFiles.Items.Clear();
                pathTextBox.Text = "";
                foreach (string drive in Directory.GetLogicalDrives())
                    listFiles.Items.Add(drive);
            }
            catch (Exception ex) { MessageBox.Show("Произошла ошибка: " + ex.Message); }
        }

        public static void SearchDir(ListBox listFiles, TextBox pathTextBox)// Поиск пути в текстБоксе
        {
            try
            {   if(pathTextBox.Text !="")
                OutDirAndFiles(listFiles, pathTextBox.Text);
            }
            catch (Exception ex) { MessageBox.Show("Произошла ошибка: " + ex.Message); }
        }

        public static void InTreeClick(ListBox listFiles,TextBox pathTextBox, string FPath)
        {
            try
            {
                OutDirAndFiles(listFiles, FPath);
                pathTextBox.Text = FPath;
                ListFiles.varListPath = FPath;
               

            }
            catch ( System.UnauthorizedAccessException) {MessageBox.Show("Отказ в доступе", "Ошибка"); }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public static void DB_ClickInList(ListBox listItems, TextBox pathTextBox)
        {
            try
            {
                if (listItems.SelectedItem.ToString() == "..")
                {
                    UpInPath(listItems, pathTextBox, ListFiles.varListPath);
                }

                else if (Directory.Exists(Path.Combine(ListFiles.varListPath, listItems.SelectedItem.ToString())))
                {
                    ListFiles.varListPath = Path.Combine(ListFiles.varListPath, listItems.SelectedItem.ToString());
                    pathTextBox.Text = ListFiles.varListPath;
                    OutDirAndFiles(listItems, varListPath);

                }

                else if (File.Exists(Path.Combine(ListFiles.varListPath, listItems.SelectedItem.ToString())))
                {
                    Process.Start(Path.Combine(ListFiles.varListPath, listItems.SelectedItem.ToString()));
                }
            }
            catch (System.UnauthorizedAccessException) { MessageBox.Show("Отказ в доступе"); }
            catch (System.NullReferenceException) { }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public static void UpInPath(ListBox listFiles, TextBox pathTextBox)
        {
            try
            {
                int lastIndex;
                if (ListFiles.varListPath.LastIndexOf('\\') >= ListFiles.varListPath.LastIndexOf('/'))
                    lastIndex = ListFiles.varListPath.LastIndexOf('\\');

                else lastIndex = ListFiles.varListPath.LastIndexOf('/');               
                if (lastIndex == -1)
                {
                    ListFiles.varListPath = "";
                    OutputDrives(listFiles, pathTextBox);
                }
                else
                {   // DirectoryInfo DirInf = new DirectoryInfo (varListPath);
                    // DirInf = varListPath;
                    // имя после последнего бэкслэша
                    ListFiles.varListPath = ListFiles.varListPath.Remove(lastIndex); //Path.GetFullPath (DirInf.Parent.ToString()); 

                    pathTextBox.Text = ListFiles.varListPath;

                    ////подсчет всех вхождений "\" в строку
                    //int countIndex = 0;
                    //for(int i=0; i < rlf.varListPath.Length; i++)
                    //{
                    //    if (rlf.varListPath[i] == '\\')
                    //        countIndex++;
                    //}
                    //MessageBox.Show(countIndex.ToString());

                    DirectoryInfo dir = new DirectoryInfo(ListFiles.varListPath);
                    DirectoryInfo[] dirs = dir.GetDirectories();

                    listFiles.Items.Clear();
                    listFiles.Items.Add("..");
                    foreach (DirectoryInfo ListDir in dirs)
                        listFiles.Items.Add(ListDir);

                    FileInfo[] files = dir.GetFiles();

                    foreach (FileInfo ListFiles in files)
                        listFiles.Items.Add(ListFiles);
                }
            }
            catch (Exception ex) { MessageBox.Show("Произошла ошибка: " + ex.Message); }
        } // не используется, пережиток времени

        public static void UpInPath(ListBox listFiles, TextBox pathTextBox,  string pathText)
        {
            try
            {
                if (varListPath != "")
                {
                    if (Directory.GetParent(varListPath) == null)
                    {
                        ListFiles.varListPath = "";
                        OutputDrives(listFiles, pathTextBox);
                    }
                    else
                    {
                        ListFiles.varListPath = Directory.GetParent(varListPath).ToString(); //ListFiles.varListPath.Remove(lastIndex); 
                        pathTextBox.Text = ListFiles.varListPath;
                        OutDirAndFiles(listFiles, varListPath);
                    }
                }
        }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
}

        public static void OutDirAndFiles(ListBox listFiles, string pathDir)
        {
            try
            {
                listFiles.Items.Clear();
                ListFiles.varListPath = pathDir;
                DirectoryInfo dir = new DirectoryInfo(pathDir);
                DirectoryInfo[] dirs = dir.GetDirectories();

                listFiles.Items.Add("..");
                foreach (DirectoryInfo ListDir in dirs)
                    listFiles.Items.Add(ListDir);

                //FileInfo[] files = dir.GetFiles();

                //foreach (FileInfo ListFiles in files)
                //    listFiles.Items.Add(ListFiles);
                ///////////////////////////////
                var files = Directory.GetFiles(@"C:\Windows");
                foreach (var file in files)
                {
                    ImageSource imageSource = null;

                    FileInfo fileInfo = new FileInfo(file);

                    Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(fileInfo.FullName);

                    if (icon != null)
                    {
                        using (var bmp = icon.ToBitmap())
                        {
                            var stream = new MemoryStream();
                            bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                            imageSource = BitmapFrame.Create(stream);
                        }
                    }

                    Items.Add(new ItemModel(fileInfo.Name, imageSource));
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

       
    }
}
