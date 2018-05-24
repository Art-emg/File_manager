using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FileManager
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            TreeViewLeft.mw = this;
            TreeViewLeft.StartCreateTree(FolderView);
            ListFiles.OutputDrives(RightListFile, RightSearchDirText);
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
            if(RightSearchDirText.Text != null|| RightSearchDirText.Text != "")
                ListFiles.SearchDir(RightListFile, RightSearchDirText);
        }

        // двойной щелчек по ListBox элементу (отрытие папок, запуск файлов)
        private void RightListFile_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListFiles.DB_ClickInList(RightListFile, RightSearchDirText);
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
            if (RightListFile.SelectedValue != null)
            {
                if (File.Exists(Path.Combine(ListFiles.varListPath, RightListFile.SelectedItem.ToString())))
                    OperationsWithFiles.CopyFile(RightListFile, "Copy");

                else if (Directory.Exists(Path.Combine(ListFiles.varListPath, RightListFile.SelectedItem.ToString())))
                    OperationsWithDirectories.CopyDir(RightListFile);
            }
        }

        private void CutButton_Click(object sender, RoutedEventArgs e)
        {
            if (RightListFile.SelectedValue != null)
            {
                if (File.Exists(Path.Combine(ListFiles.varListPath, RightListFile.SelectedItem.ToString())))
                    OperationsWithFiles.CopyFile(RightListFile, "Cut");

                else if (Directory.Exists(Path.Combine(ListFiles.varListPath, RightListFile.SelectedItem.ToString())))
                    OperationsWithDirectories.CutDir(RightListFile);
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
            if (RightListFile.SelectedValue != null)
            {
                string valid = Path.Combine(ListFiles.varListPath, RightListFile.SelectedValue.ToString());
                if (valid != Directory.GetDirectoryRoot(valid))
                {
                    if (File.Exists(Path.Combine(ListFiles.varListPath, RightListFile.SelectedValue.ToString())))
                    {
                        OperationsWithFiles.DeleteFile(RightListFile);
                    }
                    else if (Directory.Exists(Path.Combine(ListFiles.varListPath, RightListFile.SelectedValue.ToString())))
                    {
                        OperationsWithDirectories.DeleteDir(RightListFile);
                    }
                }
            }
        }

        private void RenameButton_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (RightListFile.SelectedValue != null)
                {
                    string textForNewWin = "Переименовать \"" + RightListFile.SelectedValue.ToString() + "\"";
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
            if (Directory.Exists(ListFiles.varListPath + "\\" + RightListFile.SelectedValue.ToString()))
            {
                OperationsWithDirectories.RenameDir(RightListFile, str);
            }
            else if (File.Exists(ListFiles.varListPath + "\\" + RightListFile.SelectedValue.ToString()))
            {
                OperationsWithFiles.RenameFile(RightListFile, str);
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
                if (RightListFile.SelectedValue != null)
                {
                    string valid = Path.Combine(ListFiles.varListPath, RightListFile.SelectedValue.ToString());
                    if (valid != Directory.GetDirectoryRoot(valid))
                    {
                        if (File.Exists(Path.Combine(ListFiles.varListPath, RightListFile.SelectedValue.ToString())))
                        {
                            OperationsWithFiles.DeleteFile(RightListFile);
                        }
                        else if (Directory.Exists(Path.Combine(ListFiles.varListPath, RightListFile.SelectedValue.ToString())))
                        {
                            OperationsWithDirectories.DeleteDir(RightListFile);
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
                if (RightListFile.SelectedValue != null)
                {
                    ListFiles.DB_ClickInList(RightListFile, RightSearchDirText);

                }
            }// Enter

            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.C)// Ctrl + C
            {
                if (RightListFile.SelectedValue != null)
                {
                    if (File.Exists(Path.Combine(ListFiles.varListPath, RightListFile.SelectedItem.ToString())))
                        OperationsWithFiles.CopyFile(RightListFile, "Copy");

                    else if (Directory.Exists(Path.Combine(ListFiles.varListPath, RightListFile.SelectedItem.ToString())))
                        OperationsWithDirectories.CopyDir(RightListFile);
                }
            }// Ctrl + C
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && e.Key == Key.X) // Ctrl + X
            {
                if (RightListFile.SelectedValue != null)
                {
                    if (File.Exists(Path.Combine(ListFiles.varListPath, RightListFile.SelectedItem.ToString())))
                        OperationsWithFiles.CopyFile(RightListFile, "Cut");

                    else if (Directory.Exists(Path.Combine(ListFiles.varListPath, RightListFile.SelectedItem.ToString())))
                        OperationsWithDirectories.CutDir(RightListFile);
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
                    if (RightListFile.SelectedValue != null)
                    {
                        string textForNewWin = "Переименовать \"" + RightListFile.SelectedValue.ToString() + "\"";
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
           // ListFiles.varListPath
        }
    }
}
