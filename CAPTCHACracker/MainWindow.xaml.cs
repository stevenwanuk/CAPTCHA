using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using Tesseract;

namespace CAPTCHACracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public Bitmap ABitmap;
        public Bitmap OriginBitmap;

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            LoadTarget();
        }

        public void LoadTarget()
        {

            var path = System.IO.Path.Combine(Environment.CurrentDirectory, "sample", "misc.png");
            var image = new BitmapImage(new Uri(path));
            //image.DecodePixelWidth = 130;
            //image.DecodePixelHeight = 80;
            this.OriginImg.Source = image;

            OriginBitmap = BitmapImage2Bitmap(image);
        }


        public List<Bitmap> PreHandle2(Bitmap original)
        {
            var preOcr = new FiltersSequence(
                Grayscale.CommonAlgorithms.RMY,
                new BradleyLocalThresholding());

            var grayscale = preOcr.Apply(original);
            GrayImg.Source = Bitmap2BitmapImageSource(grayscale);

            var filter = new BlobsFiltering
            {
                CoupledSizeFiltering = true,
                MinHeight = 6,
                MinWidth = 6
            };
            var filterImage = filter.Apply(new Invert().Apply(grayscale));
            ThresholdImg.Source = Bitmap2BitmapImageSource(filterImage);

            var invertImg = new Invert().Apply(filterImage);
            DoneImg.Source = Bitmap2BitmapImageSource(invertImg);

            var labels = new ConnectedComponentsLabeling();
            labels.BlobCounter.ObjectsOrder = ObjectsOrder.XY;
            labels.Apply(filterImage);

            var bitmaps = new List<Bitmap>();
            for (int i = 0; i < labels.ObjectCount; i++)
            {
                
                var candidate = labels.BlobCounter.GetObjectsInformation()[i];
                var edgePoint = labels.BlobCounter.GetBlobsEdgePoints(candidate);
                labels.BlobCounter.ExtractBlobsImage(filterImage, candidate, false);
                if (candidate.Image != null)
                {

                    var charcter = new Invert().Apply(candidate.Image.ToManagedImage());
                    var resizer = new ResizeBilinear(10, 10);
                    var finalImage = resizer.Apply(charcter);
                    bitmaps.Add(finalImage);

                    TesseractEngine a = new TesseractEngine(@"./tessdata", "eng", EngineMode.TesseractAndCube);
                    Page p = a.Process(finalImage);
                    TBlock.Text += p.GetText();
                }
                
            }
            return bitmaps;
        }

        public Bitmap PreHandle(Bitmap b)
        {

            var bnew = new Bitmap(b.Width, b.Height, PixelFormat.Format24bppRgb);
            

            var g = Graphics.FromImage(bnew);
            g.DrawImage(b, 0, 0);
            g.Dispose();
            BppImg.Source = Bitmap2BitmapImageSource(b);
            //Gray
            //b = new Grayscale(0.2125, 0.7154, 0.0721).Apply(b);
            b = new Grayscale(0.11, 0.59, 0.3).Apply(b);
            GrayImg.Source = Bitmap2BitmapImageSource(b);

            b = new Threshold(179).Apply(b);
            ThresholdImg.Source = Bitmap2BitmapImageSource(b);

            b = new BlobsFiltering(1, 1, b.Width, b.Height).Apply(b);

            return b;
        }

        public List<Bitmap> SplitCharacter(Bitmap bitmap)
        {

            var list = new List<Bitmap>();
            int[] cols = new int[bitmap.Width];
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    var pixel = bitmap.GetPixel(x, y);
                    if (pixel.R == 0)
                    {
                        //black
                        cols[x] = 1;
                        break;
                    }
                }
            }

            int left = 0, right = 0;
            for (int i = 0; i < cols.Length; i++)
            {

                if (cols[i] > 0 || (i + 1 < cols.Length && cols[i + 1] > 0))
                {
                    if (left == 0)
                    {
                        left = i;
                    }
                    else
                    {
                        right = i;
                    }
                }
                else
                {

                    if (left > 0 || right > 0)
                    {
                        var crop = new Crop(new System.Drawing.Rectangle(left, 0, right - left + 1, bitmap.Height));
                        var small = crop.Apply(bitmap);
                        list.Add(small);
                    }
                    left = right = 0;
                }
            }
            return list;
        }



        private void PreHandle_OnClick(object sender, RoutedEventArgs e)
        {

            //ABitmap = PreHandle(OriginBitmap);
            //DoneImg.Source = Bitmap2BitmapImageSource(ABitmap);

            
            ListAllBitmap(PreHandle2(OriginBitmap));
        }


        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private BitmapSource Bitmap2BitmapImageSource(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            BitmapSource retval;

            try
            {
                retval = Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
            }

            return retval;
        }

        public static Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (var outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
        }

        private void ButtonSplit_OnClick(object sender, RoutedEventArgs e)
        {
            var imageList = SplitCharacter(ABitmap);
            ListAllBitmap(imageList);
        }

        public void ListAllBitmap(List<Bitmap> imageList)
        {
            if (imageList.Count > 0)
            {
                Split1.Source = Bitmap2BitmapImageSource(imageList[0]);
            }
            if (imageList.Count > 1)
            {
                Split2.Source = Bitmap2BitmapImageSource(imageList[1]);
            }
            if (imageList.Count > 2)
            {
                Split3.Source = Bitmap2BitmapImageSource(imageList[2]);
            }
            if (imageList.Count > 3)
            {
                Split4.Source = Bitmap2BitmapImageSource(imageList[3]);
            }
            if (imageList.Count > 4)
            {
                Split5.Source = Bitmap2BitmapImageSource(imageList[4]);
            }
            if (imageList.Count > 5)
            {
                Split6.Source = Bitmap2BitmapImageSource(imageList[5]);
            }
        }

        public List<Bitmap> ToResizeAndCenterIt(List<Bitmap> list, int w = 20, int h = 20)
        {
            List<Bitmap> resizeList = new List<Bitmap>();

            for (int i = 0; i < list.Count; i++)
            {
                list[i] = new Invert().Apply(list[i]);
                int sw = list[i].Width;
                int sh = list[i].Height;
                Crop corpFilter = new Crop(new System.Drawing.Rectangle(0, 0, w, h));
                list[i] = corpFilter.Apply(list[i]);
                list[i] = new Invert().Apply(list[i]);
                int centerX = (w - sw) / 2;
                int centerY = (h - sh) / 2;
                list[i] = new CanvasMove(new IntPoint(centerX, centerY), Color.White).Apply(list[i]);
                resizeList.Add(list[i]);
            }
            return resizeList;
        }
    }

}
