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
using System.ComponentModel;
using System.IO;

namespace ImageBindingDemo
{
    /// <summary>
    /// Interaction logic for CustomImageControl.xaml
    /// </summary>
    public partial class CustomImageControl : UserControl
    {
        const short _thumbnailSize = 100;
        public CustomImageControl()
        {
            //string filepath = AppDomain.CurrentDomain.BaseDirectory + "image\\user.ico";
            //ImageData = new List<Byte>();
            //ImageData.AddRange(File.ReadAllBytes(filepath));
            InitializeComponent();
            //trying to load the image in design mode causes issues since the path is incorrectly identified
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                userImage.Width = 10;
                userImage.Height = 10;
                return;
            }
        }

        public static readonly DependencyProperty ImageDataProperty = DependencyProperty.Register(
    "ImageData", typeof(List<Byte>), typeof(CustomImageControl), new FrameworkPropertyMetadata(null,
                                        FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        public List<Byte> ImageData
        {
            get { return (List<Byte>)GetValue(ImageDataProperty); }
            set { SetValue(ImageDataProperty, value); }
        }


        private void imgParentPanel_Drop(object sender, DragEventArgs e)
        {
            string[] fileNames = e.Data.GetData(System.Windows.DataFormats.FileDrop, true) as string[];

            if (fileNames.Length > 0)
            {
                ImageData = new List<Byte>();
                ImageData.AddRange(File.ReadAllBytes(fileNames[0]));
            }
        }

        /// <summary>
        /// This is handled to reset the size of the image control according to the size of the Bitmap
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void userImage_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            BitmapImage image = userImage.Source as BitmapImage;
            if (image != null)
            {
                if ((image.Width > _thumbnailSize) || (image.Height > _thumbnailSize))
                {
                    userImage.Width = _thumbnailSize;
                    userImage.Height = _thumbnailSize;
                    userImage.Stretch = Stretch.Uniform;
                }
                else
                {
                    userImage.Stretch = Stretch.None;
                    userImage.Width = image.Width;
                    userImage.Height = image.Height;
                }
            }
        }

        //i would expect this event to get raised when i update the ImageData, since it is the source, but for some reason, it works
        //the other way and i get notified when the target is updated.. not sure what i am missing here
        private void userImage_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            BitmapImage image = userImage.Source as BitmapImage;
        }

    }
}
