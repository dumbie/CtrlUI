using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace LibraryShared
{
    public partial class ImageFunctions
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

        //Begin bitmap image
        private static BitmapImage BeginBitmapImage(int pixelWidth)
        {
            try
            {
                BitmapImage imageToBitmapImage = new BitmapImage();
                imageToBitmapImage.BeginInit();
                imageToBitmapImage.CreateOptions = BitmapCreateOptions.None;
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

        //End bitmap image
        private static BitmapImage EndBitmapImage(BitmapImage imageToBitmapImage, ref MemoryStream imageMemoryStream)
        {
            try
            {
                imageToBitmapImage.EndInit();
                imageToBitmapImage.Freeze();

                //Clear memory stream
                if (imageMemoryStream != null)
                {
                    imageMemoryStream.Close();
                    imageMemoryStream.Dispose();
                }

                return imageToBitmapImage;
            }
            catch { }
            return null;
        }

        //Convert bytes to a BitmapImage
        public static BitmapImage BytesToBitmapImage(byte[] byteArray, int pixelWidth)
        {
            try
            {
                //Prepare application bitmap image
                BitmapImage imageToBitmapImage = BeginBitmapImage(pixelWidth);
                MemoryStream imageMemoryStream = new MemoryStream(byteArray);

                //Set the stream source
                imageToBitmapImage.StreamSource = imageMemoryStream;

                //Return application bitmap image
                return EndBitmapImage(imageToBitmapImage, ref imageMemoryStream);
            }
            catch { }
            return null;
        }

        //Convert file to a BitmapImage
        public static BitmapImage FileToBitmapImage(string[] imageSource, IntPtr windowHandle, int pixelWidth, int iconIndex)
        {
            try
            {
                //Prepare application bitmap image
                BitmapImage imageToBitmapImage = BeginBitmapImage(pixelWidth);
                MemoryStream imageMemoryStream = new MemoryStream();

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
                            imageToBitmapImage.UriSource = new Uri(loadFileLower, UriKind.RelativeOrAbsolute);
                        }
                        else if (File.Exists("Assets\\Apps\\" + loadFileSafe + ".png"))
                        {
                            imageToBitmapImage.UriSource = new Uri("Assets\\Apps\\" + loadFileSafe + ".png", UriKind.RelativeOrAbsolute);
                        }
                        else if (File.Exists("Assets\\Roms\\" + loadFileSafe + ".png"))
                        {
                            imageToBitmapImage.UriSource = new Uri("Assets\\Roms\\" + loadFileSafe + ".png", UriKind.RelativeOrAbsolute);
                        }
                        else if (File.Exists(loadFileLower) && !loadFileLower.EndsWith(".exe") && !loadFileLower.EndsWith(".dll") && !loadFileLower.EndsWith(".bin") && !loadFileLower.EndsWith(".tmp"))
                        {
                            imageToBitmapImage.UriSource = new Uri(loadFileLower, UriKind.RelativeOrAbsolute);
                        }
                        else if (File.Exists(loadFileLower) && (loadFileLower.EndsWith(".exe") || loadFileLower.EndsWith(".dll") || loadFileLower.EndsWith(".bin") || loadFileLower.EndsWith(".tmp")))
                        {
                            Bitmap executableImage = ExtractImage.GetBitmapFromExecutable(loadFileLower, iconIndex);
                            if (executableImage != null)
                            {
                                executableImage.Save(imageMemoryStream, ImageFormat.Png);
                                imageMemoryStream.Seek(0, SeekOrigin.Begin);
                                imageToBitmapImage.StreamSource = imageMemoryStream;
                                executableImage.Dispose();
                            }
                        }

                        //Return application bitmap image
                        if (imageToBitmapImage.UriSource != null || imageToBitmapImage.StreamSource != null)
                        {
                            return EndBitmapImage(imageToBitmapImage, ref imageMemoryStream);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Failed loading image: " + loadFile + "/" + ex.Message);
                    }
                }

                //Image source not found, loading default or window icon
                if (windowHandle == IntPtr.Zero)
                {
                    imageToBitmapImage.UriSource = new Uri("Assets\\Apps\\Unknown.png", UriKind.RelativeOrAbsolute);
                }
                else
                {
                    BitmapSource windowImage = ExtractImage.GetBitmapSourceFromWinow(windowHandle);
                    if (windowImage != null)
                    {
                        PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
                        pngEncoder.Frames.Add(BitmapFrame.Create(windowImage));
                        pngEncoder.Save(imageMemoryStream);
                        imageMemoryStream.Seek(0, SeekOrigin.Begin);
                        imageToBitmapImage.StreamSource = imageMemoryStream;
                    }
                    else
                    {
                        imageToBitmapImage.UriSource = new Uri("Assets\\Apps\\Unknown.png", UriKind.RelativeOrAbsolute);
                    }
                }

                //Return application bitmap image
                return EndBitmapImage(imageToBitmapImage, ref imageMemoryStream);
            }
            catch { }
            return null;
        }
    }
}