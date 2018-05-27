﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FileManager
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static public ObservableCollection<ItemModel> Items { get; set; } = new ObservableCollection<ItemModel>();
        public ItemModel SelectedItemList { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

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
        } // выводит список дисков

        public MainWindow(string pathList)
        {
            InitializeComponent();
            DataContext = this;
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
        } // выводит список папок и файлов

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TreeViewLeft.mw = this;
            TreeViewLeft.StartCreateTree(FolderView);
            //ListFiles.OutputDrives(RightListFile, RightSearchDirText);
        }
         
        #region TreeFiew дерево каталогов   

        private void ReloadTreeView_Click(object sender, RoutedEventArgs e)
        {
            FolderView.Items.Clear();
            TreeViewLeft.StartCreateTree(FolderView);
        }

        #endregion
        
        #region Вывод файлов и папок в ListBox справа  

        private void RightSearchDirButton_Click(object sender, RoutedEventArgs e)
        {
            if (RightSearchDirText.Text != null && RightSearchDirText.Text != "")
                ListFiles.OutDirAndFiles(RightListFile, RightSearchDirText.Text);

        }

        // двойной щелчек по ListBox элементу (отрытие папок, запуск файлов)
        private void RightListFile_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           // MessageBox.Show(SelectedItemList.Name);

            ListFiles.DB_ClickInList(RightListFile, RightSearchDirText, SelectedItemList.Name);
        }

        //Вывод всех дисков в правый лист
        private void RightDrivesButton_Click(object sender, RoutedEventArgs e)
        {
            RightSearchDirText.Text = "";
            ListFiles.OutputDrives(RightListFile, RightSearchDirText);

        }

        //Возвращаемся назад в папку
        private void UpPathButton_Click(object sender, RoutedEventArgs e)
        {
            ListFiles.UpInPath(RightListFile, RightSearchDirText, ListFiles.varListPath);
        }

        private void RightSearchDirText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ListFiles.SearchDir(RightListFile, RightSearchDirText);
            }
        }

        

        #endregion

        #region операции с папками файлами 

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItemList.Name != null)
            {
                if (File.Exists(Path.Combine(ListFiles.varListPath, SelectedItemList.Name)))
                    OperationsWithFiles.CopyFile(RightListFile, "Copy", SelectedItemList.Name);

                else if (Directory.Exists(Path.Combine(ListFiles.varListPath, SelectedItemList.Name)))
                    OperationsWithDirectories.CopyDir(RightListFile, SelectedItemList.Name);
            }
        }

        private void CutButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItemList.Name != null)
            {
                if (File.Exists(Path.Combine(ListFiles.varListPath, SelectedItemList.Name)))
                    OperationsWithFiles.CopyFile(RightListFile, "Cut", SelectedItemList.Name);

                else if (Directory.Exists(Path.Combine(ListFiles.varListPath, SelectedItemList.Name)))
                    OperationsWithDirectories.CutDir(RightListFile, SelectedItemList.Name);
            }
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
            if (SelectedItemList.Name != null)
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

        private void RenameButton_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (SelectedItemList.Name != null)
                {
                    string textForNewWin = "Переименовать \"" + SelectedItemList.Name + "\"";
                    InputWin renameWin = new InputWin(RightListFile, ListFiles.varListPath, textForNewWin);
                    renameWin.Owner = this;
                    renameWin.Show();
                    renameWin.Enter += Rename;
                    renameWin.Output += ListFiles.OutDirAndFiles;
                }
            }catch(Exception ex) { MessageBox.Show(ex.Message); }
        }

        public void Rename(string str)
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

        private void addDirButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

        private void addFileButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

        #endregion

        #region Комбинации клавиатуры для копирования, создания и тд
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (SelectedItemList.Name != null)
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
            if(e.Key == Key.Back)
            {
                ListFiles.UpInPath(RightListFile, RightSearchDirText, ListFiles.varListPath);
            } //Back
            if (e.Key == Key.Enter)
            {
                try
                {
                    if (SelectedItemList.Name != null)
                    {
                        ListFiles.DB_ClickInList(RightListFile, RightSearchDirText, SelectedItemList.Name);
                    }
                }
                catch (System.NullReferenceException) { }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }// Enter

            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.C)// Ctrl + C
            {
                if (SelectedItemList.Name != null)
                {
                    if (File.Exists(Path.Combine(ListFiles.varListPath, SelectedItemList.Name)))
                        OperationsWithFiles.CopyFile(RightListFile, "Copy", SelectedItemList.Name);

                    else if (Directory.Exists(Path.Combine(ListFiles.varListPath, RightListFile.SelectedItem.ToString())))
                        OperationsWithDirectories.CopyDir(RightListFile, SelectedItemList.Name);
                }
            }// Ctrl + C
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.X) // Ctrl + X
            {
                if (RightListFile.SelectedValue != null)
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
                    if (SelectedItemList.Name != null)
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

        }
    }
}
