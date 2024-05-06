using ArnoldVinkCode;
using ScreenCaptureImport;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using static ArnoldVinkCode.AVProcess;
using static ArnoldVinkCode.AVSettings;
using static ScreenCapture.AppVariables;

namespace ScreenCapture
{
    public partial class CaptureScreen
    {
        public static async Task CaptureVideoProcess(int captureDelay)
        {
            try
            {
                await Task.Delay(captureDelay);
                //Check if capture process is running
                List<ProcessMulti> screenCaptureProcess = Get_ProcessesMultiByName("ScreenCaptureTool", true);
                bool processRunningVideo = screenCaptureProcess.Any(x => x.Argument.ToLower() == "-video");
                if (processRunningVideo)
                {
                    Debug.WriteLine("Video capture process is running, stopping video capture.");
                    await ArnoldVinkPipes.PipeClientSendString("ScreenCaptureTool", 1000, "-videostop");
                }
                else
                {
                    Debug.WriteLine("Starting video capture process.");
                    Process startProcess = new Process();
                    startProcess.StartInfo.FileName = "ScreenCaptureTool.exe";
                    startProcess.StartInfo.Arguments = "-video";
                    startProcess.Start();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Screen video capture process start failed: " + ex.Message);
            }
        }

        public static void StopCaptureVideoToFile()
        {
            try
            {
                //Check video capture
                if (CaptureImport.CaptureVideoIsRecording())
                {
                    Debug.WriteLine("Stopping active video capture...");

                    //Update the tray notify icon
                    AppTrayMenuNow.TrayNotifyIcon.Text = AVFunctions.StringCut("Processing " + vCaptureFileName, 59, "...");
                    AppTrayMenuNow.TrayNotifyIcon.Icon = new Icon(AVEmbedded.EmbeddedResourceToStream(null, "ScreenCaptureTool.Assets.AppIconProcessing.ico"));
                    AVActions.DispatcherInvoke(delegate
                    {
                        vWindowOverlay.ellipse_Status.Fill = (SolidColorBrush)Application.Current.Resources["ApplicationIgnoredBrush"];
                    });

                    //Request to stop video capture
                    bool captureResultStop = CaptureImport.CaptureVideoStop();
                    Debug.WriteLine("Stopped video capturing: " + captureResultStop);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed stopping video capture: " + ex.Message);
            }
        }

        public static async void CaptureEventVideoCaptureStopped()
        {
            try
            {
                Debug.WriteLine("Video capture stopped event triggered, closing application.");
                await AppClose.Application_Exit();
            }
            catch { }
        }

        public static async void CaptureEventDeviceChangeDetected()
        {
            try
            {
                Debug.WriteLine("Device change event triggered, stopping capture and closing application.");
                await AppClose.Application_Exit();
            }
            catch { }
        }

        public static async Task<bool> StartCaptureVideoToFile()
        {
            try
            {
                //Check video capture
                if (CaptureImport.CaptureVideoIsRecording())
                {
                    return false;
                }

                //Register capture events
                CaptureImport.CaptureEventVideoCaptureStopped(CaptureEventVideoCaptureStopped);
                CaptureImport.CaptureEventDeviceChangeDetected(CaptureEventDeviceChangeDetected);

                //Capture tool settings
                VideoFormats VideoSaveFormat = (VideoFormats)SettingLoad(vConfiguration, "VideoSaveFormat", typeof(int));
                VideoRateControls VideoRateControl = (VideoRateControls)SettingLoad(vConfiguration, "VideoRateControl", typeof(int));
                int VideoBitRate = SettingLoad(vConfiguration, "VideoBitRate", typeof(int));
                int VideoFrameRate = SettingLoad(vConfiguration, "VideoFrameRate", typeof(int));
                int VideoMaxPixelDimension = SettingLoad(vConfiguration, "VideoMaxPixelDimension", typeof(int));

                AudioFormats AudioSaveFormat = (AudioFormats)SettingLoad(vConfiguration, "AudioSaveFormat", typeof(int));
                int AudioChannels = SettingLoad(vConfiguration, "AudioChannels", typeof(int));
                int AudioBitRate = SettingLoad(vConfiguration, "AudioBitRate", typeof(int));
                int AudioBitDepth = SettingLoad(vConfiguration, "AudioBitDepth", typeof(int));
                int AudioSampleRate = SettingLoad(vConfiguration, "AudioSampleRate", typeof(int));

                int CaptureMonitorId = SettingLoad(vConfiguration, "CaptureMonitorId", typeof(int)) - 1;
                bool CaptureSoundEffect = SettingLoad(vConfiguration, "CaptureSoundEffect", typeof(bool));
                bool CaptureDrawBorder = SettingLoad(vConfiguration, "CaptureDrawBorder", typeof(bool));
                bool CaptureDrawMouseCursor = SettingLoad(vConfiguration, "CaptureDrawMouseCursor", typeof(bool));

                //Screen capture settings
                CaptureSettings captureSettings = new CaptureSettings();
                captureSettings.HDRtoSDR = true;
                captureSettings.MaxPixelDimension = VideoMaxPixelDimension;
                captureSettings.MonitorId = CaptureMonitorId;
                captureSettings.SoundEffect = CaptureSoundEffect;
                captureSettings.DrawBorder = CaptureDrawBorder;
                captureSettings.DrawMouseCursor = CaptureDrawMouseCursor;

                //Media recording settings
                MediaSettings mediaSettings = new MediaSettings();
                mediaSettings.VideoFormat = VideoSaveFormat;
                mediaSettings.VideoFrameRate = VideoFrameRate;
                mediaSettings.VideoRateControl = VideoRateControl;
                mediaSettings.VideoBitRate = VideoBitRate;
                mediaSettings.AudioFormat = AudioSaveFormat;
                mediaSettings.AudioChannels = AudioChannels;
                mediaSettings.AudioBitRate = AudioBitRate;
                mediaSettings.AudioBitDepth = AudioBitDepth;
                mediaSettings.AudioSampleRate = AudioSampleRate;

                //Initialize screen capture
                if (!CaptureImport.CaptureInitialize(captureSettings, out CaptureDetails vCaptureDetails, false))
                {
                    Debug.WriteLine("Failed to initialize screen capture.");
                    return false;
                }
                else
                {
                    //Wait for capturer to have initialized
                    await Task.Delay(500);
                }

                //Set screenshot name
                string fileSaveName = "(" + DateTime.Now.ToShortDateString() + ") " + DateTime.Now.ToString("HH.mm.ss.ffff");
                if (vCaptureDetails.HDREnabled)
                {
                    if (vCaptureDetails.HDRtoSDR)
                    {
                        fileSaveName += " (HDRtoSDR)";
                    }
                    else
                    {
                        fileSaveName += " (HDR)";
                    }
                }
                else
                {
                    fileSaveName += " (SDR)";
                }
                fileSaveName = "Video " + AVFiles.FileNameReplaceInvalidChars(fileSaveName, "-");
                vCaptureFileName = fileSaveName;

                //Check capture location
                string fileSaveFolder = SettingLoad(vConfiguration, "CaptureLocation", typeof(string));
                if (string.IsNullOrWhiteSpace(fileSaveFolder) || !Directory.Exists(fileSaveFolder))
                {
                    //Check captures folder in app directory
                    if (!Directory.Exists("Captures"))
                    {
                        Directory.CreateDirectory("Captures");
                    }

                    //Set save folder to captures in app directory
                    fileSaveFolder = "Captures";
                }

                //Combine save path
                string fileSavePath = fileSaveFolder + "\\" + fileSaveName + ".mp4";

                //Start video capture
                bool captureStarted = CaptureImport.CaptureVideoStart(fileSavePath, mediaSettings);
                if (captureStarted)
                {
                    //Return result
                    Debug.WriteLine("Successfully started video screen capture.");
                    return true;
                }
                else
                {
                    //Return result
                    Debug.WriteLine("Failed starting video screen capture.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Screen video capture failed: " + ex.Message);
                return false;
            }
        }
    }
}