using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace FileManager
{
    public class OperationsWithFiles:ListFiles
    {
        public static void CopyFile(ListBox listItems, string flag, string selItem) // запись пути копируемого файла в "буфер"
        {
            try
            {   if (selItem != "..")
                {
                    ListFiles.thisName = selItem;
                    ListFiles.thisPath = Path.Combine(ListFiles.varListPath, ListFiles.thisName);
                    // MessageBox.Show(pathCopy);
                    ListFiles.CutOrCopy = flag;
                }
            }
            catch (System.NullReferenceException) { }
            catch (Exception ex) { MessageBox.Show("Произошла ошибка: " + ex.Message); }
        }

        public static void PasteAfterCopyFile(ListBox listItems)// вставка файла, с возможностью замены
        {
            try
            {
                if (ListFiles.thisPath != null)
                {
                    pathPaste = Path.Combine(varListPath + "\\" + ListFiles.thisName);
                    if (thisPath != pathPaste)
                    {
                        File.Copy(thisPath, pathPaste, false);
                    }
                    OutDirAndFiles(listItems, varListPath);

                    Change copyDB = new Change(DateTime.Now, "Copy File", thisPath, pathPaste);
                    changeDatabaseEntities db = new changeDatabaseEntities();
                    db.Change.Add(copyDB);
                    db.SaveChanges();

                }

            }
            catch (System.IO.IOException) // если файл уже существует, выводит сообщ. о замене файла
            {
                MessageBoxResult res = MessageBox.Show("Файл уже существует. Заменить файл?", "Предупреждение", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    File.Copy(thisPath, pathPaste, true);
                }
                else { return; }
            }
            catch (Exception ex) { MessageBox.Show("Произошла ошибка: " + ex.Message); }
        }

        public static void PasteAfterCutFile(ListBox listItems)//вырезание и вставка файла
        {
            try
            {
                pathPaste = Path.Combine(varListPath + "\\" + ListFiles.thisName);
                File.Move(thisPath, pathPaste);
                OutDirAndFiles(listItems, varListPath);
            }
            catch (System.IO.IOException)
            {
                PasteAfterCopyFile(listItems);
                File.Delete(thisPath);
            }
            catch (Exception ex) { MessageBox.Show("Произошла ошибка: " + ex.Message); }
}

        public static void DeleteFile(ListBox listItems, string selItem)
        {
            try
            {

                    MessageBoxResult res = MessageBox.Show("Удалить " + selItem + " ?", "Предупреждение", MessageBoxButton.YesNo);
                      if (res == MessageBoxResult.Yes)
                      {
                        ListFiles.thisName = selItem;
                        thisPath = Path.Combine(ListFiles.varListPath, ListFiles.thisName);
                        File.Delete(thisPath);
                        OutDirAndFiles(listItems, varListPath);
                      }
                      else return;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        public static void RenameFile(ListBox listItems, string renameText, string selItem)
        {
            thisName = Path.Combine(varListPath, selItem);
            string thisRename = varListPath + "\\" + renameText;
            File.Move(thisName, thisRename);
            OutDirAndFiles(listItems, varListPath);
        }

        public static void CreateFile(string nameFile)
        {
            try
            {
                File.Create(varListPath + "\\" + nameFile);
            }
            //catch (System.NullReferenceException) { }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
