using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace FileManager
{
    //преобразует из строки в изображение
    [ValueConversion(typeof(string), typeof(BitmapImage))]

    public class HeaderToImageConverter : IValueConverter
    {
        public static HeaderToImageConverter Instance = new HeaderToImageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //получает полный путь
            string path = (string)value;

            //если путь пустой, игнорируем
            if (path == null)
                return null;
            
            //по дефолту эта иконка
            var image = "Images/file.png";

            //получаем имя для папки/файла обрезая полный путь
            string name = TreeViewLeft.GetFileFolderName(path);

            // проверяем какая иконка должна быть (диск, папка, мб потом файл)
            if (string.IsNullOrEmpty(name))
                image = "Images/drive(dark).png";
            else if (new FileInfo(path).Attributes.HasFlag(FileAttributes.Directory))
                image = "Images/folder.png";

            return new BitmapImage(new Uri($"pack://application:,,,/{image}"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

