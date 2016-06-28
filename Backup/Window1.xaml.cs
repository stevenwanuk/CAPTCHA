using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace ImageBindingDemo
{
    public partial class Window1 : Window
    {
        private List<Byte> _picData;
        public List<Byte> MyPhotoData
        {
            get { return _picData; }
            set { _picData = value; }
        }

        public Window1()
        {
            MyPhotoData = new List<Byte>();
            String filepath = AppDomain.CurrentDomain.BaseDirectory + "image\\user.ico";
            MyPhotoData.AddRange(File.ReadAllBytes(filepath));
            InitializeComponent();

            //2. loading from resources
            imgCodeBehind.BeginInit();
            BitmapImage bmp = new BitmapImage(new Uri("Image/Weather.jpg", UriKind.Relative));
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            imgCodeBehind.Source = bmp;
            imgCodeBehind.EndInit();

            //3. alternative way to load from resource
            bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.StreamSource = Application.GetResourceStream(new Uri("Image/folder.ico", UriKind.RelativeOrAbsolute)).Stream;
            bmp.EndInit();
            imgCodeBehind2.Source = bmp;
            
            //4. loading from external file
            byte[] bytes = File.ReadAllBytes("Image/User.ico");
            MemoryStream strm = new MemoryStream(bytes);
            bmp = new BitmapImage();
            //setting the caching option to OnLoad ensures no file locking
            bmp.CacheOption = BitmapCacheOption.OnLoad;
            bmp.BeginInit();
            bmp.StreamSource = strm;
            bmp.EndInit();
            strm.Close();
            imgFileBinding.Source = bmp;

            //data coming from DB will also usually be as byte[] (blog in database). The same logic as above can be used to bind to Image
            //see the CustomImageControl to see how it this can be written as a converter and directly used in binding
        }

        //5. Binding via property
        public string ImagePath
        {
            get { return @"C:\ImageBindingDemo\ImageBindingDemo\image\weather.jpg"; }
        }
    }
}
