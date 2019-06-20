using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.IO.Compression;
using System.Windows;

namespace FileManager
{
    class Crypto
    {
        public static string EncryptFile(string sInputFilename, string sOutputDir, string sKey, bool isDir)
        {
                   
            string sOutputFilename = Path.Combine(sOutputDir, Translit.TranslitFileName(Path.GetFileNameWithoutExtension(sInputFilename)) + ".des");
            FileStream fsInput = new FileStream(sInputFilename, FileMode.Open, FileAccess.Read);
            FileStream fsEncrypted = new FileStream(sOutputFilename, FileMode.Create, FileAccess.Write);

            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();

            DES.Key = Get8BytesHashCode(sKey);
            DES.IV = Get8BytesHashCode(sKey);
            ICryptoTransform desencrypt = DES.CreateEncryptor();

            CryptoStream cryptostream = new CryptoStream(fsEncrypted, desencrypt, CryptoStreamMode.Write);

            string CryptFileName = Translit.TranslitFileName(Path.GetFileName(sInputFilename));
            if (isDir)
            {
                CryptFileName = "/" + CryptFileName;
            }
            byte[] byteArray = Encoding.ASCII.GetBytes(CryptFileName + "\n");
            MemoryStream stream = new MemoryStream(byteArray);

            byte[] bytearrayinputFileName = new byte[stream.Length];
            stream.Read(bytearrayinputFileName, 0, bytearrayinputFileName.Length);
            fsEncrypted.Write(bytearrayinputFileName, 0, bytearrayinputFileName.Length);

            byte[] bytearrayinput = new byte[fsInput.Length];
            fsInput.Read(bytearrayinput, 0, bytearrayinput.Length);
            cryptostream.Write(bytearrayinput, 0, bytearrayinput.Length);

            cryptostream.Close();
            fsEncrypted.Close();
            fsInput.Close();
       
            return sOutputFilename;
        }

        public static string DecryptFile(string sInputFilename, string sOutputDir, string sKey)
        {
            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();

            DES.Key = Get8BytesHashCode(sKey);
            DES.IV = Get8BytesHashCode(sKey);
            ICryptoTransform desdecrypt = DES.CreateDecryptor();

            // считывается первая строка файла 
            string DecryptFileName;
            using (StreamReader reader = new StreamReader(sInputFilename))
            {
                DecryptFileName = reader.ReadLine();
            }           

            //поток читающий зашифрованный файл
            FileStream fsread = new FileStream(sInputFilename, FileMode.Open, FileAccess.Read);

            fsread.Seek((DecryptFileName.Length + 1), SeekOrigin.Begin);

            CryptoStream cryptostreamDecr = new CryptoStream(fsread, desdecrypt, CryptoStreamMode.Read);
            DecryptFileName = Path.GetFileName(DecryptFileName);
            FileStream fsDecrypted = new FileStream(Path.Combine(sOutputDir, Path.GetFileName(DecryptFileName)), FileMode.Create);
            byte[] bytearrayinput = new byte[1024];
            int length;
            try
            {
                do
                {
                    length = cryptostreamDecr.Read(bytearrayinput, 0, 1024);
                    fsDecrypted.Write(bytearrayinput, 0, length);
                } while (length == 1024);
                fsDecrypted.Flush();
                fsDecrypted.Close();

                fsread.Flush();
                fsread.Close();
            }
            catch (CryptographicException)
            {
                MessageBox.Show("Плохие данные");
                if (File.Exists(Path.Combine(sOutputDir, Path.GetFileName(DecryptFileName))))
                {
                    fsDecrypted.Flush();
                    fsDecrypted.Close();
                    fsread.Flush();
                    fsread.Close();
                    File.Delete(Path.Combine(sOutputDir, Path.GetFileName(DecryptFileName)));
                  
                }
            }


            return Path.Combine(sOutputDir, DecryptFileName);
        }

        static byte[] Get8BytesHashCode(string strText)
        {

            byte[] hashCode = null;
            if (!string.IsNullOrEmpty(strText))
            {
                byte[] byteContents = Encoding.Unicode.GetBytes(strText);
                System.Security.Cryptography.SHA256 hash = new System.Security.Cryptography.SHA256CryptoServiceProvider();
                byte[] hashText = hash.ComputeHash(byteContents);

                Int64 hashCodeStart = BitConverter.ToInt64(hashText, 0);
                Int64 hashCodeMedium = BitConverter.ToInt64(hashText, 8);
                Int64 hashCodeEnd = BitConverter.ToInt64(hashText, 24);
                hashCode = BitConverter.GetBytes(hashCodeStart ^ hashCodeMedium ^ hashCodeEnd);
            }
            return (hashCode);
        }

        public static void EncryptDirectory(string sInputDirname, string sOutputDirname, string sKey)
        {
            string tempZipFile = sInputDirname + ".zip";
            ZipFile.CreateFromDirectory(sInputDirname, tempZipFile);
            EncryptFile(tempZipFile, sOutputDirname, sKey, true);
            File.Delete(tempZipFile);          
        }

        public static void DecryptDirectory(string sInputDirname, string sOutputDir, string sKey)
        {
            string tempZipFile = DecryptFile(sInputDirname, sOutputDir, sKey);
            if (File.Exists(tempZipFile))
            {
                ZipFile.ExtractToDirectory(tempZipFile, Path.Combine(sOutputDir, Path.GetFileNameWithoutExtension(tempZipFile)));
                File.Delete(tempZipFile);
            }

        }

        public static string ComputeMD5Checksum(string path)
        {
            using (FileStream fs = System.IO.File.OpenRead(path))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] fileData = new byte[fs.Length];
                fs.Read(fileData, 0, (int)fs.Length);
                byte[] checkSum = md5.ComputeHash(fileData);
                string result = BitConverter.ToString(checkSum).Replace("-", String.Empty);
                return result;
                
            }
        }
    }
}
