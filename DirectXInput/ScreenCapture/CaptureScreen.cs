using ScreenCapture;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using static DirectXInput.AppVariables;
using static LibraryShared.SoundPlayer;

namespace DirectXInput
{
    public class CaptureScreen
    {
        public static async Task CaptureScreenToFile()
        {
            //Prepare screen capture
            IntPtr bitmapIntPtr = IntPtr.Zero;
            try
            {
                //Screen capture settings
                CaptureSettings vCaptureSettings = new CaptureSettings();

                //Initialize screen capture
                if (!CaptureImport.CaptureInitialize(vCaptureSettings, out CaptureDetails vCaptureDetails))
                {
                    Debug.WriteLine("Failed to initialize screen capture.");

                    //Play capture sound
                    PlayInterfaceSound(vConfigurationCtrlUI, "ScreenshotFail", true, true);

                    return;
                }
                else
                {
                    //Wait for capturer to have initialized
                    await Task.Delay(100);
                }

                //Capture screenshot
                try
                {
                    bitmapIntPtr = CaptureImport.CaptureScreenshot();
                }
                catch { }

                //Check screenshot
                if (bitmapIntPtr == IntPtr.Zero)
                {
                    Debug.WriteLine("Screenshot capture is corrupted.");

                    //Play capture sound
                    PlayInterfaceSound(vConfigurationCtrlUI, "ScreenshotFail", true, true);

                    return;
                }

                //Set file name
                string imageSaveName = DateTime.Now.ToString("HH.mm.ss.ffff") + " (" + DateTime.Now.ToShortDateString() + ")";
                if (vCaptureDetails.HDREnabled)
                {
                    if (vCaptureSettings.HDRtoSDR)
                    {
                        imageSaveName += " (HDRtoSDR)";
                    }
                    else
                    {
                        imageSaveName += " (HDR)";
                    }
                }
                else
                {
                    imageSaveName += " (SDR)";
                }
                imageSaveName = "\\Screenshot " + CaptureFunctions.FileNameReplaceInvalidChars(imageSaveName);

                //Create screenshots folder in app directory
                if (!Directory.Exists("Screenshots"))
                {
                    Directory.CreateDirectory("Screenshots");
                }
                string imageSaveFolder = "Screenshots";

                //Save screenshot to file
                bool screenshotExport = CaptureImport.CaptureSaveFilePng(bitmapIntPtr, imageSaveFolder + imageSaveName + ".png");
                Debug.WriteLine("Screenshot png export succeeded: " + screenshotExport);

                //Play capture sound
                if (screenshotExport)
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "Screenshot", true, true);
                }
                else
                {
                    PlayInterfaceSound(vConfigurationCtrlUI, "ScreenshotFail", true, true);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Screen capture failed: " + ex.Message);
            }
            finally
            {
                //Clear screen capture resources
                if (bitmapIntPtr != IntPtr.Zero)
                {
                    CaptureImport.CaptureFreeMemory(bitmapIntPtr);
                }

                //Reset screen capture resources
                CaptureImport.CaptureReset();
            }
        }
    }
}