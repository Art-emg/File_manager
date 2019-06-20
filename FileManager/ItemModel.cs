using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FileManager
{
    public class ItemModel
    {
        public ItemModel(string name, ImageSource image)
        {
            Name = name;
            Image = image;
        }

        public string Name { get; set; }
        public ImageSource Image { get; set; }
    }
}
