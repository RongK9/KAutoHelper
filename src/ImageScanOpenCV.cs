using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;

namespace KAutoHelper
{
    public class ImageScanOpenCV
    {
        /// <summary>
        /// Lấy image từ đường dẫn
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Bitmap GetImage(string path)
        {
            return new Bitmap(path);
        }

        /// <summary>
        /// Kiểm tra hình nhỏ có tồn tại trong hình lớn hay không (2 đường dẫn). theo tỷ lệ percent mặc định là 0.9
        /// </summary>
        /// <param name="main"></param>
        /// <param name="sub"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static Bitmap Find(string main, string sub, double percent = 0.9)
        {
            var mainImg = GetImage(main);
            var subImg = GetImage(sub);
            var res = Find(main, sub, percent);

            return res;
        }

        /// <summary>
        /// Kiểm tra hình nhỏ có tồn tại trong hình lớn hay không (2 image). theo tỷ lệ percent mặc định là 0.9
        /// </summary>
        /// <param name="main"></param>
        /// <param name="sub"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static Bitmap Find(Bitmap mainBitmap, Bitmap subBitmap, double percent = 0.9)
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    Image<Bgr, byte> source = new Image<Bgr, byte>(mainBitmap);
                    Image<Bgr, byte> template = new Image<Bgr, byte>(subBitmap);
                    Image<Bgr, byte> imageToShow = source.Copy();

                    using (Image<Gray, float> result = source.MatchTemplate(template, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed))
                    {
                        double[] minValues, maxValues;
                        System.Drawing.Point[] minLocations, maxLocations;
                        result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

                        if (maxValues[0] > percent)
                        {
                            Rectangle match = new Rectangle(maxLocations[0], template.Size);
                            imageToShow.Draw(match, new Bgr(System.Drawing.Color.Red), 2);
                        }
                        else
                        {
                            imageToShow = null;
                        }
                    }

                    return imageToShow == null ? null : imageToShow.ToBitmap();
                }
                catch { }
            }
            return null;
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        /// <summary>
        /// Đưa ra tọa độ của hình nhỏ trong hình lớn theo tỷ lệ chính xác đưa vào là percent mặc định là 0.9
        /// </summary>
        /// <param name="main"></param>
        /// <param name="sub"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static System.Drawing.Point? FindOutPoint(Bitmap mainBitmap, Bitmap subBitmap, double percent = 0.9)
        {
            if (subBitmap == null || mainBitmap == null)
                return null;

            if (subBitmap.Width > mainBitmap.Width || subBitmap.Height > mainBitmap.Height)
                return null;
            System.Drawing.Point? resPoint = null;
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    Image<Bgr, byte> source = new Image<Bgr, byte>(mainBitmap);
                    Image<Bgr, byte> template = new Image<Bgr, byte>(subBitmap);

                    using (Image<Gray, float> result = source.MatchTemplate(template, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed))
                    {
                        double[] minValues, maxValues;
                        System.Drawing.Point[] minLocations, maxLocations;
                        result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

                        if (maxValues[0] > percent)
                        {
                            resPoint = maxLocations[0];
                        }
                    }
                    break;
                }
                catch (Exception ex)
                {

                }
            }
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            return resPoint;
        }

        public static List<Point> FindOutPoints(Bitmap mainBitmap, Bitmap subBitmap, double percent = 0.9)
        {
            List<Point> resPoint = new List<Point>();
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    Image<Bgr, byte> source = new Image<Bgr, byte>(mainBitmap);
                    Image<Bgr, byte> template = new Image<Bgr, byte>(subBitmap);

                    while (true)
                    {
                        using (Image<Gray, float> result = source.MatchTemplate(template, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed))
                        {
                            double[] minValues, maxValues;
                            System.Drawing.Point[] minLocations, maxLocations;
                            result.MinMax(out minValues, out maxValues, out minLocations, out maxLocations);

                            if (maxValues[0] > percent)
                            {
                                Rectangle match = new Rectangle(maxLocations[0], template.Size);
                                source.Draw(match, new Bgr(Color.Blue), -1);
                                resPoint.Add(maxLocations[0]);
                            }
                            else
                            {
                                break;
                            }

                        }
                    }
                    break;
                }
                catch { }
            }

            return resPoint;
        }

        /// <summary>
        /// Đưa ra danh sách tọa độ các pixcel thỏa mãn trong hình.
        /// Nếu không tìm thấy thì count = 0
        /// </summary>
        /// <param name="mainBitmap"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static List<Point> FindColor(Bitmap mainBitmap, Color color)
        {
            int searchValue = color.ToArgb();
            List<Point> result = new List<Point>();
            using (Bitmap bmp = mainBitmap)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        if (searchValue.Equals(bmp.GetPixel(x, y).ToArgb()))
                            result.Add(new Point(x, y));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Đưa ra danh sách tọa độ các pixcel thỏa mãn trong hình.
        /// Nếu không tìm thấy thì count = 0
        /// Đưa vào là mã màu Hex của color ví dụ:
        /// #FFFFFFFF
        /// </summary>
        /// <param name="mainBitmap"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public static List<Point> FindColor(Bitmap mainBitmap, string color)
        {
            Color Color = (Color)System.Windows.Media.ColorConverter.ConvertFromString(color);
            return FindColor(mainBitmap, Color);
        }

        public static void TestDilate(Bitmap bmp)
        {
            while (true)
            {

                Image<Gray, byte> imgOld = new Image<Gray, byte>(bmp);
                imgOld.Save("old.png");
                Image<Gray, byte> img2 = (new Image<Gray, byte>(imgOld.Width, imgOld.Height, new Gray(255))).Sub(imgOld);
                img2.Save("img23.png");
                Image<Gray, byte> eroded = new Image<Gray, byte>(img2.Size);
                Image<Gray, byte> temp = new Image<Gray, byte>(img2.Size);
                Image<Gray, byte> skel = new Image<Gray, byte>(img2.Size);
                skel.SetValue(0);
                CvInvoke.Threshold(img2, img2, 127, 255, 0);
                var element = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(2, 2), new Point(-1, -1));
                bool done = false;

                //while (!done)
                {
                    CvInvoke.Dilate(imgOld, eroded, element, new Point(-1, -1), 1, BorderType.Reflect, default(MCvScalar));
                    //CvInvoke.Erode(img2, eroded, element, new Point(-1, -1), 1, BorderType.Reflect, default(MCvScalar));
                    //CvInvoke.Dilate(eroded, temp, element, new Point(-1, -1), 1, BorderType.Reflect, default(MCvScalar));
                    //CvInvoke.Subtract(img2, temp, temp);
                    //CvInvoke.BitwiseOr(skel, temp, skel);
                    eroded.CopyTo(img2);
                    //if (CvInvoke.CountNonZero(img2) == 0) done = true;
                }
                //return skel.Bitmap;

                skel.Bitmap.Save("ele.png");
                img2.Save("img2.png");
                eroded.Save("eroded.png");
                temp.Save("temp.png");
              }
        }
        

        /// <summary>
        /// Đọc chữ từ hình
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string RecolizeText(Bitmap img)
        {
            string text = "";

            text = Get_Text_From_Image.Get_Text(img);

            return text;
        }

        public static void SplitImageInFolder(string folderPath)
        {
            DirectoryInfo dir = new DirectoryInfo(folderPath);
            foreach (var item in dir.GetFiles())
            {
                Bitmap Bm_image_sour = new Bitmap(item.FullName);
                Bitmap image_new = Get_Text_From_Image.make_new_image(new Image<Gray, byte>(Bm_image_sour).ToBitmap());
                Bm_image_sour.Dispose();

                int cout_picture = Get_Text_From_Image.split_image(image_new, Path.GetFileNameWithoutExtension(item.Name));
            }
        }

        public static Bitmap ThreshHoldBinary(Bitmap bmp, byte threshold = 190)
        {
            Image<Gray, Byte> img = new Image<Gray, Byte>(bmp);

            var bmp1 = img.ThresholdBinary(new Gray(threshold), new Gray(255));

            //bmp1.Save("adasdsad.png");

            return bmp1.ToBitmap();
        }

        public static Bitmap NotWhiteToTransparentPixelReplacement(Bitmap bmp)
        {
            bmp = CreateNonIndexedImage(bmp);
            for (var x = 0; x < bmp.Width; x++)
                for (var y = 0; y < bmp.Height; y++)
                {
                    var pixel = bmp.GetPixel(x, y);
                    if (pixel.R > 200 && pixel.G > 200 && pixel.B > 200)
                        bmp.SetPixel(x, y, Color.Transparent);
                }

            return bmp;
        }

        public static Bitmap WhiteToBlackPixelReplacement(Bitmap bmp)
        {
            bmp = CreateNonIndexedImage(bmp);
            for (var x = 0; x < bmp.Width; x++)
                for (var y = 0; y < bmp.Height; y++)
                {
                    var pixel = bmp.GetPixel(x, y);
                    if (pixel.R > 20 && pixel.G > 230 && pixel.B > 230)
                        bmp.SetPixel(x, y, Color.Black);
                }

            return bmp;
        }

        public static Bitmap TransparentToWhitePixelReplacement(Bitmap bmp)
        {
            bmp = CreateNonIndexedImage(bmp);
            for (var x = 0; x < bmp.Width; x++)
                for (var y = 0; y < bmp.Height; y++)
                {
                    var pixel = bmp.GetPixel(x, y);
                    if (pixel.A >= 1)
                        bmp.SetPixel(x, y, Color.White);
                }

            return bmp;
        }

        public static Bitmap CreateNonIndexedImage(Image src)
        {
            Bitmap newBmp = new Bitmap(src.Width, src.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            using (Graphics gfx = Graphics.FromImage(newBmp))
            {
                gfx.DrawImage(src, 0, 0);
            }

            return newBmp;
        }

        public static Bitmap ScanShape(Bitmap img)
        {

            if (img == null)
            {
                return null;
            }

            try
            {
                Image<Bgr, byte> imgInput = new Image<Bgr, byte>(img);

                var temp = imgInput.SmoothGaussian(5).Convert<Gray, byte>().ThresholdBinaryInv(new Gray(10), new Gray(255));
                //temp.Save("bbb.png");
                VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
                Mat m = new Mat();

                CvInvoke.FindContours(temp, contours, m, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);

                for (int i = 0; i < contours.Size; i++)
                {
                    double perimeter = CvInvoke.ArcLength(contours[i], true);
                    VectorOfPoint approx = new VectorOfPoint();
                    CvInvoke.ApproxPolyDP(contours[i], approx, 0.04 * perimeter, true);

                    CvInvoke.DrawContours(imgInput, contours, i, new MCvScalar(0, 0, 255), 2);

                    //moments  center of the shape

                    var moments = CvInvoke.Moments(contours[i]);
                    int x = (int)(moments.M10 / moments.M00);
                    int y = (int)(moments.M01 / moments.M00);

                    if (approx.Size == 3)
                    {
                        CvInvoke.PutText(imgInput, "Triangle", new Point(x, y),
                            Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
                    }

                    if (approx.Size == 4)
                    {
                        Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);

                        double ar = (double)rect.Width / rect.Height;

                        if (ar >= 0.95 && ar <= 1.05)
                        {
                            CvInvoke.PutText(imgInput, "Square", new Point(x, y),
                            Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
                        }
                        else
                        {
                            CvInvoke.PutText(imgInput, "Rectangle", new Point(x, y),
                            Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
                        }

                    }

                    if (approx.Size == 6)
                    {
                        CvInvoke.PutText(imgInput, "Hexagon", new Point(x, y),
                            Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
                    }


                    if (approx.Size > 6)
                    {
                        CvInvoke.PutText(imgInput, "Circle", new Point(x, y),
                            Emgu.CV.CvEnum.FontFace.HersheySimplex, 0.5, new MCvScalar(0, 0, 255), 2);
                    }

                }
                return imgInput.Bitmap;
            }
            catch (Exception ex)
            {
                return null;
                //MessageBox.Show(ex.Message);
            }
        }
    }


    public class Get_Text_From_Image
    {
        private static int saisot = 5;
        private static int red = 217;
        private static int collor_Byte_Start = 160;
        private static string path_langue = @"C:\";
        static string TempFolder = "image_temp";
        static string StandarFolder = "image_standand";


        private static List<Color> TemplateColors = new List<Color>()
        {
            Color.FromArgb(255,0,0,0)
        };

        public static void information(string Path_Langue)
        {
            path_langue = Path_Langue;
        }

        public static string Get_Text(Bitmap Bm_image_sour)
        {
            string text = "";

            Bitmap image_new = (Bitmap)Bm_image_sour.Clone();// make_new_image(new Image<Gray, byte>(Bm_image_sour).ToBitmap());
            //image_new.Save("aaaaa.png");
            Bm_image_sour.Dispose();

            int cout_picture = split_image(image_new);

            text = Get_Text(cout_picture);

            return text;
        }


        #region Sub Function

        public static Bitmap make_new_image(Bitmap Bm_image_sour)
        {
            //infor image and make a new bitmap same size
            int _width = Bm_image_sour.Width;
            int _height = Bm_image_sour.Height;

            Bitmap Bm_image = new Bitmap(_width, _height);

            // set color cho điểm pixel của mà các chỉ số màu A,R,B,G thỏa mãn trong phạm vi sai sót cho phép
            int collor_Byte_Stop = 230;
            for (int i = collor_Byte_Start; i < collor_Byte_Stop; i++)
            {
                red = i;
                Get_List_Point();
            }
            return Bm_image;

            #region ham con
            void Get_List_Point()
            {
                //  get point 
                for (int i = 0; i < _width; i++)
                {
                    for (int j = 0; j < _height; j++)
                    {
                        Color color = Bm_image_sour.GetPixel(i, j);

                       if (Check_sailenh_Color(color, TemplateColors, saisot))
                        {
                            try
                            {
                                Bm_image.SetPixel(i, j, Color.Black);
                            }
                            catch (Exception) { }
                        }

                    }
                }
            }

            bool Check_sailenh_Color(Color indexColor, List<Color> templateColor, int sailech)
            {
                bool result = false;

                foreach (var item in templateColor)
                {
                    if ((indexColor.R + sailech >= item.R && indexColor.R - sailech <= item.R) 
                        && (indexColor.G + sailech >= item.G && indexColor.G - sailech <= item.G) 
                        && (indexColor.B + sailech >= item.B && indexColor.B - sailech <= item.B))
                    {
                        result = true;
                        break;
                    }
                }

                return result;
            }
            #endregion
        }

        public static int split_image(Bitmap image, string name = "")
        {
            //sự dụng bitmap image  để tách image
            image.Save("aaa.png");
            int cout_picture = 0;
            bool is_start = false;
            int width_start = 0;
            int width_stop = 0;
            int _height_top = 200;
            int _height_bottom = 0;

            int _width = image.Width;
            int _height = image.Height;

            for (int i = 0; i < _width; i++)
            {
                int cout_Black = 0;


                for (int j = 0; j < _height; j++)
                {
                    Color color = image.GetPixel(i, j);

                    if (color.Name != "ff000000")
                    {
                        cout_Black++;

                        if (_height_top > j) _height_top = j;
                        if (_height_bottom < j) _height_bottom = j;

                    }
                    else
                    {

                    }
                }
                if (cout_Black > 1 && is_start == false)
                {
                    width_start = i - 1; is_start = true;
                }
                if (cout_Black < 1 && is_start == true)
                {
                    width_stop = i + 1; is_start = false;
                    save_image_splip();
                    cout_picture++;
                    _height_top = 200;
                    _height_bottom = 0;
                }
            }

            void save_image_splip()
            {
                int _width_image_slip = width_stop - width_start;
                int _height_image_split = _height_bottom - _height_top;
                Bitmap image_split = new Bitmap(_width_image_slip, _height_image_split);
                for (int i = 0; i < _width_image_slip; i++)
                {
                    for (int j = 0; j < _height_image_split; j++)
                    {
                        try
                        {
                            Color color_image_split = image.GetPixel(width_start + i, _height_top + j);
                            image_split.SetPixel(i, j, color_image_split);
                        }
                        catch { }

                    }
                }

                string path_folder = TempFolder;
                check_folder_exists(path_folder);
                string output = path_folder + @"\" + name + cout_picture + ".jpg";

                //=>kiem tra thu muc ton tai

                image_split.Save(output);
                image_split.Dispose();
            }

            
            return cout_picture;
        }


        protected static string Get_Text(int cout_picture)
        {
            string text = "";
            List<string> character = new List<string>() { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"};
            for (int i = 0; i < cout_picture; i++)
            {
                List<double> ketqua = new List<double>();
                for (int j = 0; j < character.Count; j++)
                {
                    try
                    {
                        string name_text = character[j];
                        double max = 0;
                        double currentMax = 0;
                        string folderPath = StandarFolder + @"\" + name_text;
                        DirectoryInfo dir = new DirectoryInfo(folderPath);
                        foreach (var item in dir.GetFiles())
                        {
                            string path_image_standate = item.FullName;
                            Bitmap standand = new Bitmap(path_image_standate);

                            string path_image = TempFolder +@"\" + i + ".jpg";
                            Bitmap main = new Bitmap(path_image);
                            currentMax = Image_Equal(main, standand);
                            standand.Dispose();
                            main.Dispose();

                            if (currentMax > max)
                            {
                                max = currentMax;
                            }
                        }

                        ketqua.Add(max);
                    }
                    catch { }
                }

                int index_max_trung = 0;
                double _ketqua = 0;
                for (int j = 0; j < character.Count; j++)
                {
                    if (_ketqua < ketqua[j])
                    {
                        _ketqua = ketqua[j];
                        index_max_trung = j;
                    }
                }

                text+= character[index_max_trung];
            }
            return text;
        }

        public static double Image_Equal(Bitmap main, Bitmap standand)
        {
            double count = 0;
            double trung = 0;

            //int _max_width = standand.Width;
            //int _max_height = standand.Height;
            //if (main.Width > standand.Width) _max_width = main.Width;
            //if (main.Height > standand.Height) _max_height = main.Height;

            Bitmap sub = new Bitmap(main, new System.Drawing.Size(standand.Width, standand.Height));

            for (int i = 0; i < standand.Width; i++)
            {
                for (int j = 0; j < standand.Height; j++)
                {
                    count++;
                    if (sub.GetPixel(i, j).Equals(standand.GetPixel(i, j)))
                    {
                        trung += 1;
                    }
                }
            }

            return trung / count;
        }

        protected static void check_folder_exists(string path)
        {
            bool is_exists = Directory.Exists(path);
            if (!is_exists)
            {
                Directory.CreateDirectory(path);
            }
        }
        #endregion
    }
}
