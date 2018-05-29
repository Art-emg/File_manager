﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;

namespace FileManager
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static public ObservableCollection<ItemModel> Items { get; set; } = new ObservableCollection<ItemModel>();
        public ItemModel SelectedItemList { get; set; }
        public SqlConnection sqlConnection;
        public bool PreviewCheck = false;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            try
            {
                Items.Clear();
                RightSearchDirText.Text = "";
                foreach (string drive in Directory.GetLogicalDrives())
                {
                    String stringPath = "Images/drive(dark).png";
                    Uri imageUri = new Uri(stringPath, UriKind.Relative);
                    BitmapImage imageBitmap = new BitmapImage(imageUri);
                    System.Windows.Controls.Image myImage = new System.Windows.Controls.Image();
                    myImage.Source = imageBitmap;
                    ImageSource imageSource = imageBitmap;

                    ItemModel im = new ItemModel(drive, imageSource);
                    Items.Add(im);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        } // выводит список дисков

        public MainWindow(string pathList)
        {
            InitializeComponent();
            DataContext = this;
            try
            {
                var dirs = Directory.GetDirectories(pathList);
                Items.Clear();
                ItemModel backdot = new ItemModel("..", null);
                Items.Add(backdot);

                foreach (var dir in dirs)
                {
                    String stringPath = "Images/folder.png";
                    Uri imageUri = new Uri(stringPath, UriKind.Relative);
                    BitmapImage imageBitmap = new BitmapImage(imageUri);
                    System.Windows.Controls.Image myImage = new System.Windows.Controls.Image();
                    myImage.Source = imageBitmap;
                    ImageSource imageSource = imageBitmap;

                    FileInfo fileInfo = new FileInfo(dir);
                    ItemModel im = new ItemModel(fileInfo.Name, imageSource);
                    Items.Add(im);
                }

                var files = Directory.GetFiles(pathList);
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
            } catch (Exception ex) { MessageBox.Show(ex.Message); }
        } // выводит список папок и файлов

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                TreeViewLeft.mw = this;
                TreeViewLeft.StartCreateTree(FolderView);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }


        #region TreeFiew дерево каталогов   

        private void ReloadTreeView_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FolderView.Items.Clear();
                TreeViewLeft.StartCreateTree(FolderView);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        #endregion

        #region Вывод файлов и папок в ListBox справа  

        private void RightSearchDirButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RightSearchDirText.Text != null && RightSearchDirText.Text != "")
                    ListFiles.OutDirAndFiles(RightListFile, RightSearchDirText.Text);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // двойной щелчек по ListBox элементу (отрытие папок, запуск файлов)
        private void RightListFile_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ListFiles.DB_ClickInList(RightListFile, RightSearchDirText, SelectedItemList.Name);
            }
            catch (System.NullReferenceException) { }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        //Вывод всех дисков в правый лист
        private void RightDrivesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RightSearchDirText.Text = "";
                ListFiles.OutputDrives(RightListFile, RightSearchDirText);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }

        //Возвращаемся назад в папку
        private void UpPathButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ListFiles.UpInPath(RightListFile, RightSearchDirText, ListFiles.varListPath);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void RightSearchDirText_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    ListFiles.SearchDir(RightListFile, RightSearchDirText);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        #endregion

        #region операции с папками файлами 

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RightListFile.SelectedItem != null && SelectedItemList.Name != "..")
                {
                    if (File.Exists(Path.Combine(ListFiles.varListPath, SelectedItemList.Name)))
                        OperationsWithFiles.CopyFile(RightListFile, "Copy", SelectedItemList.Name);

                    else if (Directory.Exists(Path.Combine(ListFiles.varListPath, SelectedItemList.Name)))
                        OperationsWithDirectories.CopyDir(RightListFile, SelectedItemList.Name);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void CutButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RightListFile.SelectedItem != null && SelectedItemList.Name != "..")
                {
                    if (File.Exists(Path.Combine(ListFiles.varListPath, SelectedItemList.Name)))
                        OperationsWithFiles.CopyFile(RightListFile, "Cut", SelectedItemList.Name);

                    else if (Directory.Exists(Path.Combine(ListFiles.varListPath, SelectedItemList.Name)))
                        OperationsWithDirectories.CutDir(RightListFile, SelectedItemList.Name);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void PasteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(ListFiles.thisPath))
                {
                    if (ListFiles.CutOrCopy == "Copy")
                    {
                        OperationsWithFiles.PasteAfterCopyFile(RightListFile);
                    }
                    else if (ListFiles.CutOrCopy == "Cut")
                    {
                        OperationsWithFiles.PasteAfterCutFile(RightListFile);
                    }
                }
                else if (Directory.Exists(ListFiles.thisPath))
                {
                    OperationsWithDirectories.PasteDir(RightListFile, ListFiles.CutOrCopy);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (RightListFile.SelectedItem != null && SelectedItemList.Name != "..")
                {
                    string valid = Path.Combine(ListFiles.varListPath, SelectedItemList.Name);
                    if (valid != Directory.GetDirectoryRoot(valid))
                    {
                        if (File.Exists(Path.Combine(ListFiles.varListPath, SelectedItemList.Name)))
                        {
                            OperationsWithFiles.DeleteFile(RightListFile, SelectedItemList.Name);
                        }
                        else if (Directory.Exists(Path.Combine(ListFiles.varListPath, SelectedItemList.Name)))
                        {
                            OperationsWithDirectories.DeleteDir(RightListFile, SelectedItemList.Name);
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }

        private void RenameButton_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (RightListFile.SelectedItem != null && SelectedItemList.Name != "..")
                {
                    string textForNewWin = "Переименовать \"" + SelectedItemList.Name + "\"";
                    InputWin renameWin = new InputWin(RightListFile, ListFiles.varListPath, textForNewWin);
                    renameWin.Owner = this;
                    renameWin.Show();
                    renameWin.Enter += Rename;
                    renameWin.Output += ListFiles.OutDirAndFiles;
                }
        } catch (Exception ex) { MessageBox.Show(ex.Message); }
}

        public void Rename(string str)
        {
            try
            {
                if (Directory.Exists(ListFiles.varListPath + "\\" + SelectedItemList.Name))
                {
                    OperationsWithDirectories.RenameDir(RightListFile, str, SelectedItemList.Name);
                }
                else if (File.Exists(ListFiles.varListPath + "\\" + SelectedItemList.Name))
                {
                    OperationsWithFiles.RenameFile(RightListFile, str, SelectedItemList.Name);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }

        private void addDirButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (Directory.Exists(ListFiles.varListPath))
                {
                    InputWin addDirWin = new InputWin(RightListFile, ListFiles.varListPath, "Создать папку с именем: ");
                    addDirWin.Owner = this;
                    addDirWin.Show();
                    addDirWin.Enter += OperationsWithDirectories.CreateDir;
                    addDirWin.Output += ListFiles.OutDirAndFiles;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }

        private void addFileButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (Directory.Exists(ListFiles.varListPath))
                {
                    InputWin addDirWin = new InputWin(RightListFile, ListFiles.varListPath, "Создать файл с именем (и расширением): ");
                    addDirWin.Owner = this;
                    addDirWin.Show();
                    addDirWin.Enter += OperationsWithFiles.CreateFile;
                    addDirWin.Output += ListFiles.OutDirAndFiles;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }

        #endregion

        #region Комбинации клавиатуры для копирования, создания и тд
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (RightListFile.SelectedItem != null && SelectedItemList.Name != "..")
                {
                    string valid = Path.Combine(ListFiles.varListPath, SelectedItemList.Name);
                    if (valid != Directory.GetDirectoryRoot(valid))
                    {
                        if (File.Exists(Path.Combine(ListFiles.varListPath, SelectedItemList.Name)))
                        {
                            OperationsWithFiles.DeleteFile(RightListFile, SelectedItemList.Name);
                        }
                        else if (Directory.Exists(Path.Combine(ListFiles.varListPath, SelectedItemList.Name)))
                        {
                            OperationsWithDirectories.DeleteDir(RightListFile, SelectedItemList.Name);
                        }
                    }
                }
            }//Del
            if (e.Key == Key.Back)
            {
                ListFiles.UpInPath(RightListFile, RightSearchDirText, ListFiles.varListPath);
            } //Back
            if (e.Key == Key.Enter)
            {
                if (RightListFile.SelectedItem != null)
                {
                    ListFiles.DB_ClickInList(RightListFile, RightSearchDirText, SelectedItemList.Name);
                }
            }// Enter

            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.C)// Ctrl + C
            {
                if (RightListFile.SelectedItem != null && SelectedItemList.Name != "..")
                {
                    if (File.Exists(Path.Combine(ListFiles.varListPath, SelectedItemList.Name)))
                        OperationsWithFiles.CopyFile(RightListFile, "Copy", SelectedItemList.Name);

                    else if (Directory.Exists(Path.Combine(ListFiles.varListPath, RightListFile.SelectedItem.ToString())))
                        OperationsWithDirectories.CopyDir(RightListFile, SelectedItemList.Name);
                }
            }// Ctrl + C
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.X) // Ctrl + X
            {
                if (RightListFile.SelectedValue != null && SelectedItemList.Name != "..")
                {
                    if (File.Exists(Path.Combine(ListFiles.varListPath, RightListFile.SelectedItem.ToString())))
                        OperationsWithFiles.CopyFile(RightListFile, "Cut", SelectedItemList.Name);

                    else if (Directory.Exists(Path.Combine(ListFiles.varListPath, RightListFile.SelectedItem.ToString())))
                        OperationsWithDirectories.CutDir(RightListFile, SelectedItemList.Name);
                }
            }// Ctrl + X
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.V) // Ctrl + V
            {
                try
                {
                    if (File.Exists(ListFiles.thisPath))
                    {
                        if (ListFiles.CutOrCopy == "Copy")
                        {
                            OperationsWithFiles.PasteAfterCopyFile(RightListFile);
                        }
                        else if (ListFiles.CutOrCopy == "Cut")
                        {
                            OperationsWithFiles.PasteAfterCutFile(RightListFile);
                        }
                    }
                    else if (Directory.Exists(ListFiles.thisPath))
                    {
                        OperationsWithDirectories.PasteDir(RightListFile, ListFiles.CutOrCopy);
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
            }// Ctrl + V
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.R)
            {
                try
                {
                    if (RightListFile.SelectedItem != null && SelectedItemList.Name != "..")
                    {
                        string textForNewWin = "Переименовать \"" + SelectedItemList.Name + "\"";
                        InputWin renameWin = new InputWin(RightListFile, ListFiles.varListPath, textForNewWin);
                        renameWin.Owner = this;
                        renameWin.Show();
                        renameWin.Enter += Rename;
                        renameWin.Output += ListFiles.OutDirAndFiles;
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }// Ctrl + R
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.D)
            {
                if (Directory.Exists(ListFiles.varListPath))
                {
                    InputWin addDirWin = new InputWin(RightListFile, ListFiles.varListPath, "Создать папку с именем: ");
                    addDirWin.Owner = this;
                    addDirWin.Show();
                    addDirWin.Enter += OperationsWithDirectories.CreateDir;
                    addDirWin.Output += ListFiles.OutDirAndFiles;
                }
            }// Ctrl + D
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.F)
            {
                if (Directory.Exists(ListFiles.varListPath))
                {
                    InputWin addDirWin = new InputWin(RightListFile, ListFiles.varListPath, "Создать файл с именем (и расширением): ");
                    addDirWin.Owner = this;
                    addDirWin.Show();
                    addDirWin.Enter += OperationsWithFiles.CreateFile;
                    addDirWin.Output += ListFiles.OutDirAndFiles;
                }
            }// Ctrl + F
        }
        #endregion

        private void zipButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (SelectedItemList != null)
                {
                    string startPath = Path.Combine(ListFiles.varListPath, SelectedItemList.Name);
                    if (Directory.Exists(Path.Combine(ListFiles.varListPath, SelectedItemList.Name)) && Directory.GetParent(SelectedItemList.Name) !=null)
                    {
                        string zipPath = Path.Combine(ListFiles.varListPath, SelectedItemList.Name + @".zip");
                        ZipFile.CreateFromDirectory(startPath, zipPath);
                        ListFiles.OutDirAndFiles(RightListFile, ListFiles.varListPath);
                    }
                    else { MessageBox.Show("Архивировать можно только папки"); };
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }

        private void DeCompressButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (SelectedItemList != null)
                {
                    string startPath = Path.Combine(ListFiles.varListPath, SelectedItemList.Name);
                    if (Path.GetExtension(startPath).ToUpper() == ".ZIP")
                    {
                        try
                        {
                            string decompPath = Path.Combine(ListFiles.varListPath, Path.GetFileNameWithoutExtension(SelectedItemList.Name));
                            //Directory.CreateDirectory(decompPath);
                            ZipFile.ExtractToDirectory(startPath, decompPath);
                            ListFiles.OutDirAndFiles(RightListFile, ListFiles.varListPath);
                        }
                        catch (System.IO.IOException)
                        {
                            MessageBoxResult res = MessageBox.Show("Файл уже существует. Заменить файл?", "Предупреждение", MessageBoxButton.YesNo);
                            if (res == MessageBoxResult.Yes)
                            {
                                string decompPath = Path.Combine(ListFiles.varListPath, Path.GetFileNameWithoutExtension(SelectedItemList.Name));
                                Directory.Delete(decompPath, true);
                                ZipFile.ExtractToDirectory(startPath, decompPath);
                            }
                            else { return; }
                        }

                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SearchTextBox.Text != "" && SearchTextBox.Text != null)
                {
                    if (ListFiles.varListPath == "")
                    {
                        MessageBox.Show($"Зайдите на диск, в котором нужно искать \"{SearchTextBox.Text}\"");
                        return;
                    }
                    Items.Clear();
                    
                    GetFileList(SearchTextBox.Text, ListFiles.varListPath);
                    
                    ListFiles.varListPath = "";
                    //RightSearchDirText.Text = ListFiles.varListPath;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }

        private void GetFileList(string fileSearchPattern, string rootFolderPath)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(rootFolderPath);
                FileInfo[] files = di.GetFiles(fileSearchPattern, SearchOption.TopDirectoryOnly);
                DirectoryInfo[] dirs = di.GetDirectories(fileSearchPattern, SearchOption.TopDirectoryOnly);
                
                foreach (var dir in dirs)
                {
                    #region перевод изображение в иконку
                    String stringPath = "Images/folder.png";
                    Uri imageUri = new Uri(stringPath, UriKind.Relative);
                    BitmapImage imageBitmap = new BitmapImage(imageUri);
                    System.Windows.Controls.Image myImage = new System.Windows.Controls.Image();
                    myImage.Source = imageBitmap;
                    ImageSource imageSource = imageBitmap;
                    #endregion

                    FileInfo fileInfo = new FileInfo(dir.FullName);
                    ItemModel im = new ItemModel(fileInfo.FullName, imageSource);
                    Items.Add(im);
                }
                foreach (var file in files)
                {
                    ImageSource imageSource = null;

                    FileInfo fileInfo = new FileInfo(file.FullName);
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
                    
                    Items.Add(new ItemModel(fileInfo.FullName, imageSource));
                    
                }

                DirectoryInfo[] diArr = di.GetDirectories();

                foreach (DirectoryInfo info in diArr)
                {
                    GetFileList(fileSearchPattern, info.FullName);
                }
            }catch(System.UnauthorizedAccessException){}
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }



         private void RightListFile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (PreviewCheck == true)
                {
                    if (RightListFile.SelectedIndex != -1)
                    {
                        string rashir = Path.GetExtension(SelectedItemList.Name);
                        if (rashir.ToUpper() == ".PNG" || rashir.ToUpper() == ".JPG" || rashir.ToUpper() == ".JPEG")
                        {
                            string stringPath = Path.Combine(ListFiles.varListPath, SelectedItemList.Name);
                            previewImg.Source = BitmapFrame.Create(new Uri(stringPath));

                        }
                        else
                        {
                            previewImg.Source = null;
                        }
                    }
                }
                if (SelectedItemList != null)
                    InfoString.Text = Path.Combine(ListFiles.varListPath, SelectedItemList.Name);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (HidZona.Visibility == Visibility.Hidden)
                {
                    HidZona.Visibility = Visibility.Visible;
                    VisGrid.Width = new GridLength(200);
                    PreviewCheck = true;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (HidZona.Visibility == Visibility.Visible)
                {
                    HidZona.Visibility = Visibility.Hidden;
                    VisGrid.Width = new GridLength(0);
                    PreviewCheck = false;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }

        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Window1 a = new Window1();
                a.Show();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка"); }
        }

    }
}
