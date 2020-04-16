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
        public static string FileToString(string[] stringSource)
        {
            try
            {
                //Load application bitmap image
                foreach (string loadFile in stringSource)
                {
                    try
                    {
                        //Validate the load path
                        if (string.IsNullOrWhiteSpace(loadFile)) { continue; }

                        //Adjust the load path
                        string loadFileLower = loadFile.ToLower();
                        loadFileLower = AVFunctions.StringRemoveStart(loadFileLower, " ");
                        loadFileLower = AVFunctions.StringRemoveEnd(loadFileLower, " ");
                        //Debug.WriteLine("Loading text: " + loadFileLower);

                        //Read the text file
                        if (File.Exists(loadFileLower))
                        {
                            return File.ReadAllText(loadFileLower);
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
        public static BitmapImage FileToBitmapImage(string[] imageSource, IntPtr mainWindowHandle, int pixelWidth, int iconIndex)
        {
            try
            {
                //Prepare application bitmap image
                BitmapImage imageToBitmapImage = PrepareNewBitmapImage(pixelWidth);

                //Load application bitmap image
                foreach (string loadFile in imageSource)
                {
                    try
                    {
                        //Validate the load path
                        if (string.IsNullOrWhiteSpace(loadFile)) { continue; }

                        //Adjust the load path
                        string loadFileLower = loadFile.ToLower();
                        loadFileLower = AVFunctions.StringRemoveStart(loadFileLower, " ");
                        loadFileLower = AVFunctions.StringRemoveEnd(loadFileLower, " ");
                        string loadFileSafe = string.Join(string.Empty, loadFileLower.Split(Path.GetInvalidFileNameChars()));
                        //Debug.WriteLine("Loading image: " + loadFileLower + "/" + loadFileSafe);

                        if (loadFileLower.StartsWith("pack://application:,,,"))
                        {
                            imageToBitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            imageToBitmapImage.UriSource = new Uri(loadFileLower, UriKind.RelativeOrAbsolute);
                        }
                        else if (File.Exists("Assets\\Apps\\" + loadFileSafe + ".png"))
                        {
                            imageToBitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            imageToBitmapImage.UriSource = new Uri("Assets\\Apps\\" + loadFileSafe + ".png", UriKind.RelativeOrAbsolute);
                        }
                        else if (File.Exists("Assets\\Roms\\" + loadFileSafe + ".png"))
                        {
                            imageToBitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            imageToBitmapImage.UriSource = new Uri("Assets\\Roms\\" + loadFileSafe + ".png", UriKind.RelativeOrAbsolute);
                        }
                        else if (File.Exists(loadFileLower) && !loadFileLower.EndsWith(".exe") && !loadFileLower.EndsWith(".dll") && !loadFileLower.EndsWith(".bin") && !loadFileLower.EndsWith(".tmp"))
                        {
                            imageToBitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            imageToBitmapImage.UriSource = new Uri(loadFileLower, UriKind.RelativeOrAbsolute);
                        }
                        else if (File.Exists(loadFileLower) && (loadFileLower.EndsWith(".exe") || loadFileLower.EndsWith(".dll") || loadFileLower.EndsWith(".bin") || loadFileLower.EndsWith(".tmp")))
                        {
                            imageToBitmapImage.CreateOptions = BitmapCreateOptions.None;
                            System.Drawing.Bitmap iconImage = ExtractIco.ExtractIco.GetBitmapFromExePath(loadFileLower, iconIndex);
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