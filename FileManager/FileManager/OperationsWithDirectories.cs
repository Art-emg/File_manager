﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace FileManager
{
    class OperationsWithDirectories : ListFiles
    {
        static string fullEndPathCopy = ""; // полный путь к папке вместе с ее именем
        static string dirNameCopy = ""; // имя папки которую копируем
        public static MainWindow mw;

        public static void CopyDir(ListBox listItems)
        {
            try
            {
                dirNameCopy = listItems.SelectedValue.ToString();
                thisPath = Path.Combine(varListPath, dirNameCopy);
                CutOrCopy = "Copy";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public static void CutDir(ListBox listItems)
        {
            try
            {
                dirNameCopy = listItems.SelectedValue.ToString();
                thisPath = Path.Combine(varListPath, dirNameCopy);
                CutOrCopy = "Cut";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public static void PasteDir(ListBox listItems, string flag)
        {
            try
            {
                fullEndPathCopy = Path.Combine(varListPath, dirNameCopy);
                if (thisPath != fullEndPathCopy)
                {
                    if (flag == "Cut")
                    {
                        Directory.Move(thisPath, fullEndPathCopy);
                    }
                    else if (flag == "Copy")
                    {
                        Directory.CreateDirectory(fullEndPathCopy);
                        copyDirAlgorithm(thisPath, fullEndPathCopy);
                    }
                    OutDirAndFiles(listItems, varListPath);
                }
            }catch (Exception ex) { MessageBox.Show(ex.Message); }

        }

        static void copyDirAlgorithm(string begin_dir, string end_dir)// проход через рекурсию всех подпапок и файлов в папке
        {
            try
            {
                //Берём нашу исходную папку
                DirectoryInfo dir_inf = new DirectoryInfo(begin_dir);
                //Перебираем все внутренние папки
                foreach (DirectoryInfo dir in dir_inf.GetDirectories())
                {
                    //Проверяем - если директории не существует, то создаём;
                    if (Directory.Exists(end_dir + "\\" + dir.Name) != true)
                    {
                        Directory.CreateDirectory(end_dir + "\\" + dir.Name);
                    }

                    //Рекурсия (перебираем вложенные папки и делаем для них то-же самое).
                    copyDirAlgorithm(dir.FullName, end_dir + "\\" + dir.Name);
                }

                //Перебираем файлики в папке источнике.
                foreach (string file in Directory.GetFiles(begin_dir))
                {
                    //Определяем (отделяем) имя файла с расширением - без пути (но с слешем "\").
                    string filik = file.Substring(file.LastIndexOf('\\'), file.Length - file.LastIndexOf('\\'));
                    //Копируем файлик с перезаписью из источника в приёмник.
                    string fullEndDir = end_dir + "\\" + filik;

                    File.Copy(file, fullEndDir, true);
                }
            }catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public static void DeleteDir(ListBox listItems)
        {
            try
            {   if (listItems.SelectedItem.ToString() != null)
                {
                    thisPath = Path.Combine(varListPath, listItems.SelectedValue.ToString());
                    if (Directory.GetDirectories(thisPath).Length + Directory.GetFiles(thisPath).Length > 0)
                    {
                        MessageBoxResult res = MessageBox.Show("Папка не пуста. Удалить папку?", "Предупреждение", MessageBoxButton.YesNo);
                        if (res == MessageBoxResult.Yes)
                        {
                            Directory.Delete(thisPath, true);
                            OutDirAndFiles(listItems, varListPath);
                        }
                        else { return; }
                    }
                    else
                    {
                        Directory.Delete(thisPath);
                        OutDirAndFiles(listItems, varListPath);
                    }
                }
            }catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public static void RenameDir(ListBox listItems, string renameText)
        {
            try
            {
                thisName = Path.Combine(varListPath, listItems.SelectedValue.ToString());
                string thisRename = varListPath + "\\" + renameText;
                Directory.Move(thisName, thisRename);
                OutDirAndFiles(listItems, varListPath);
            }catch(Exception ex) { MessageBox.Show(ex.Message); }
        }

        public static void CreateDir(string nameDir)
        {
            try
            {
                Directory.CreateDirectory(varListPath + "\\" + nameDir);
            }
            //catch (System.NullReferenceException) { }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

    }

}

