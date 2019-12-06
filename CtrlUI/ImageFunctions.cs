using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using static ArnoldVinkCode.AVInteropDll;

namespace CtrlUI
{
    partial class ImageFunctions
    {
        //Convert file to a string
        public static string FileToString(string[] StringSource)
        {
            try
            {
                //Load application bitmap image
                foreach (string ImageFile in StringSource)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(ImageFile)) { continue; }
                        string ImageFileLower = ImageFile.ToLower();
                        ImageFileLower = AVFunctions.StringRemoveStart(ImageFileLower, " ");
                        ImageFileLower = AVFunctions.StringRemoveEnd(ImageFileLower, " ");
                        //Debug.WriteLine("Loading string: " + ImageFileLower);

                        if (File.Exists(ImageFileLower))
                        {
                            return File.ReadAllText(ImageFileLower);
                        }
                    }
                    catch { }
                }
            }
            catch { }
            return string.Empty;
        }

        //Prepare application bitmap image
        private static BitmapImage PrepareNewBitmapImage(int pixelWidth)
        {
            try
            {
                BitmapImage imageToBitmapImage = new BitmapImage();
                imageToBitmapImage.BeginInit();
                imageToBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                if (pixelWidth > 0)
                {
                    imageToBitmapImage.DecodePixelWidth = pixelWidth;
                }
                return imageToBitmapImage;
            }
            catch { }
            return null;
        }

        //Convert file to a BitmapImage
        public static BitmapImage FileToBitmapImage(string[] imageSource, IntPtr mainWindowHandle, int pixelWidth)
        {
            try
            {
                //Prepare application bitmap image
                BitmapImage imageToBitmapImage = PrepareNewBitmapImage(pixelWidth);

                //Load application bitmap image
                foreach (string imageFile in imageSource)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(imageFile)) { continue; }
                        string imageFileLower = imageFile.ToLower();
                        imageFileLower = AVFunctions.StringRemoveStart(imageFileLower, " ");
                        imageFileLower = AVFunctions.StringRemoveEnd(imageFileLower, " ");
                        string imageFileSafe = string.Join("", imageFileLower.Split(Path.GetInvalidFileNameChars()));
                        Debug.WriteLine("Loading image: " + imageFileLower);

                        if (imageFileLower.StartsWith("pack://application:,,,"))
                        {
                            imageToBitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            imageToBitmapImage.UriSource = new Uri(imageFileLower, UriKind.RelativeOrAbsolute);
                        }
                        else if (File.Exists("Assets\\Apps\\" + imageFileSafe + ".png"))
                        {
                            imageToBitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            imageToBitmapImage.UriSource = new Uri("Assets\\Apps\\" + imageFileSafe + ".png", UriKind.RelativeOrAbsolute);
                        }
                        else if (File.Exists("Assets\\Roms\\" + imageFileSafe + ".png"))
                        {
                            imageToBitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            imageToBitmapImage.UriSource = new Uri("Assets\\Roms\\" + imageFileSafe + ".png", UriKind.RelativeOrAbsolute);
                        }
                        else if (File.Exists(imageFileLower) && !imageFileLower.EndsWith(".exe") && !imageFileLower.EndsWith(".dll") && !imageFileLower.EndsWith(".bin") && !imageFileLower.EndsWith(".tmp"))
                        {
                            imageToBitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            imageToBitmapImage.UriSource = new Uri(imageFileLower, UriKind.RelativeOrAbsolute);
                        }
                        else if (File.Exists(imageFileLower) && (imageFileLower.EndsWith(".exe") || imageFileLower.EndsWith(".dll") || imageFileLower.EndsWith(".bin") || imageFileLower.EndsWith(".tmp")))
                        {
                            imageToBitmapImage.CreateOptions = BitmapCreateOptions.None;
                            System.Drawing.Bitmap iconImage = ExtractIco.ExtractIco.GetBitmapFromExePath(imageFileLower);
                            if (iconImage != null)
                            {
                                MemoryStream iconMemoryStream = new MemoryStream();
                                iconImage.Save(iconMemoryStream, ImageFormat.Png);
                                iconMemoryStream.Seek(0, SeekOrigin.Begin);
                                imageToBitmapImage.StreamSource = iconMemoryStream;
                                iconImage.Dispose();
                            }
                        }

                        //Return application bitmap image
                        if (imageToBitmapImage.UriSource != null || imageToBitmapImage.StreamSource != null)
                        {
                            imageToBitmapImage.EndInit();
                            imageToBitmapImage.Freeze();
                            return imageToBitmapImage;
                        }
                    }
                    catch (Exception ex)
                    {
                        //Prepare application bitmap image
                        imageToBitmapImage = PrepareNewBitmapImage(pixelWidth);
                        Debug.WriteLine("Failed loading image: " + ex.Message);
                    }
                }

                //Image source not found, loading default or window icon
                if (mainWindowHandle == IntPtr.Zero)
                {
                    imageToBitmapImage.UriSource = new Uri("Assets\\Apps\\Unknown.png", UriKind.RelativeOrAbsolute);
                }
                else
                {
                    BitmapImage windowIcon = WindowIconToBitmapImage(mainWindowHandle);
                    if (windowIcon != null)
                    {
                        return windowIcon;
                    }
                    else
                    {
                        imageToBitmapImage.UriSource = new Uri("Assets\\Apps\\Unknown.png", UriKind.RelativeOrAbsolute);
                    }
                }

                //Return application bitmap image
                imageToBitmapImage.EndInit();
                imageToBitmapImage.Freeze();
                return imageToBitmapImage;
            }
            catch { }
            return null;
        }

        //Get the process window icon
        public static BitmapImage WindowIconToBitmapImage(IntPtr MainWindowHandle)
        {
            try
            {
                int GCL_HICON = -14;
                int GCL_HICONSM = -34;
                int ICON_SMALL = 0;
                int ICON_BIG = 1;
                int ICON_SMALL2 = 2;

                //Prepare application bitmap image
                BitmapImage ImageToBitmapImage = new BitmapImage();
                ImageToBitmapImage.BeginInit();
                ImageToBitmapImage.CacheOption = BitmapCacheOption.OnLoad;

                IntPtr iconHandle = SendMessage(MainWindowHandle, (int)WindowMessages.WM_GETICON, ICON_BIG, 0);
                if (iconHandle == IntPtr.Zero)
                {
                    iconHandle = SendMessage(MainWindowHandle, (int)WindowMessages.WM_GETICON, ICON_SMALL, 0);
                }
                if (iconHandle == IntPtr.Zero)
                {
                    iconHandle = SendMessage(MainWindowHandle, (int)WindowMessages.WM_GETICON, ICON_SMALL2, 0);
                }
                if (iconHandle == IntPtr.Zero)
                {
                    iconHandle = GetClassLongAuto(MainWindowHandle, GCL_HICON);
                }
                if (iconHandle == IntPtr.Zero)
                {
                    iconHandle = GetClassLongAuto(MainWindowHandle, GCL_HICONSM);
                }
                if (iconHandle == IntPtr.Zero)
                {
                    return null;
                }

                BitmapSource iconSource = Imaging.CreateBitmapSourceFromHIcon(iconHandle, new Int32Rect(), BitmapSizeOptions.FromEmptyOptions());

                PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    pngEncoder.Frames.Add(BitmapFrame.Create(iconSource));
                    pngEncoder.Save(memoryStream);

                    ImageToBitmapImage.StreamSource = memoryStream;
                    ImageToBitmapImage.EndInit();
                    ImageToBitmapImage.Freeze();
                }

                return ImageToBitmapImage;
            }
            catch { return null; }
        }
    }
}