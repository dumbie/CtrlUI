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

        //Convert file to a BitmapImage
        public static BitmapImage FileToBitmapImage(string[] ImageSource, IntPtr MainWindowHandle, int PixelWidth)
        {
            try
            {
                //Prepare application bitmap image
                BitmapImage ImageToBitmapImage = new BitmapImage();
                ImageToBitmapImage.BeginInit();
                ImageToBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                if (PixelWidth > 0) { ImageToBitmapImage.DecodePixelWidth = PixelWidth; }

                //Load application bitmap image
                foreach (string ImageFile in ImageSource)
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(ImageFile)) { continue; }
                        string ImageFileLower = ImageFile.ToLower();
                        ImageFileLower = AVFunctions.StringRemoveStart(ImageFileLower, " ");
                        ImageFileLower = AVFunctions.StringRemoveEnd(ImageFileLower, " ");
                        string ImageFileSafe = string.Join("", ImageFileLower.Split(Path.GetInvalidFileNameChars()));
                        //Debug.WriteLine("Loading image: " + ImageFileLower);

                        if (ImageFileLower.StartsWith("pack://application:,,,"))
                        {
                            ImageToBitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            ImageToBitmapImage.UriSource = new Uri(ImageFileLower, UriKind.RelativeOrAbsolute);
                        }
                        else if (File.Exists("Assets\\Apps\\" + ImageFileSafe + ".png"))
                        {
                            ImageToBitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            ImageToBitmapImage.UriSource = new Uri("Assets\\Apps\\" + ImageFileSafe + ".png", UriKind.RelativeOrAbsolute);
                        }
                        else if (File.Exists("Assets\\Roms\\" + ImageFileSafe + ".png"))
                        {
                            ImageToBitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            ImageToBitmapImage.UriSource = new Uri("Assets\\Roms\\" + ImageFileSafe + ".png", UriKind.RelativeOrAbsolute);
                        }
                        else if (File.Exists(ImageFileLower) && !ImageFileLower.EndsWith(".exe") && !ImageFileLower.EndsWith(".dll") && !ImageFileLower.EndsWith(".tmp"))
                        {
                            ImageToBitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            ImageToBitmapImage.UriSource = new Uri(ImageFileLower, UriKind.RelativeOrAbsolute);
                        }
                        else if (File.Exists(ImageFileLower) && (ImageFileLower.EndsWith(".exe") || ImageFileLower.EndsWith(".dll") || ImageFileLower.EndsWith(".tmp")))
                        {
                            ImageToBitmapImage.CreateOptions = BitmapCreateOptions.None;
                            System.Drawing.Bitmap IconImage = ExtractIco.ExtractIco.GetBitmapFromExePath(ImageFileLower);
                            if (IconImage != null)
                            {
                                MemoryStream IconMemoryStream = new MemoryStream();
                                IconImage.Save(IconMemoryStream, ImageFormat.Png);
                                IconMemoryStream.Seek(0, SeekOrigin.Begin);
                                ImageToBitmapImage.StreamSource = IconMemoryStream;
                                IconImage.Dispose();
                            }
                        }

                        //Check if the image has been loaded
                        if (ImageToBitmapImage.UriSource != null || ImageToBitmapImage.StreamSource != null) { break; }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Failed loading image: " + ex.Message);
                    }
                }

                //Check if an image has been loaded
                if (ImageToBitmapImage.UriSource == null && ImageToBitmapImage.StreamSource == null)
                {
                    if (MainWindowHandle == IntPtr.Zero)
                    {
                        ImageToBitmapImage.UriSource = new Uri("Assets\\Apps\\Unknown.png", UriKind.RelativeOrAbsolute);
                    }
                    else
                    {
                        BitmapImage WindowIcon = WindowIconToBitmapImage(MainWindowHandle);
                        if (WindowIcon != null)
                        {
                            ImageToBitmapImage = null;
                            return WindowIcon;
                        }
                        else
                        {
                            ImageToBitmapImage.UriSource = new Uri("Assets\\Apps\\Unknown.png", UriKind.RelativeOrAbsolute);
                        }
                    }
                }

                //Return application bitmap image
                ImageToBitmapImage.EndInit();
                ImageToBitmapImage.Freeze();
                return ImageToBitmapImage;
            }
            catch { return null; }
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