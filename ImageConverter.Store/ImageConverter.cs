using System.Drawing;
using System.IO;

namespace ImageConverter.Store
{
    public class ImageConverter
    {
        public void GetImageInformation()
        {
            var image = new Bitmap("C:/Users/a.kerina/Desktop/picture2.png");
            for (var i = 0; i < image.Height; i++)
            {
                var color = image.GetPixel(0, i);
                var a = color.R;
            }
            
        }
    }
}
