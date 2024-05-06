using ArnoldVinkCode;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVProcess;
using static ArnoldVinkCode.AVSettings;
using static ScreenCapture.AppCheck;
using static ScreenCapture.AppClasses;
using static ScreenCapture.AppVariables;

namespace ScreenCapture
{
    public partial class AppStartup
    {
        //Startup application
        public static async Task Application_Startup(string[] launchArgs)
        {
            try
            {
                //Application startup checks
                Startup_Check();

                //Application settings checks
                vWindowMain.Settings_Check();

                //Check if capture process is running
                List<ProcessMulti> screenCaptureProcess = Get_ProcessesMultiByName("ScreenCaptureTool", true);
                bool processRunningTool = screenCaptureProcess.Count(x => string.IsNullOrWhiteSpace(x.Argument)) > 1;
                bool processRunningVideo = screenCaptureProcess.Count(x => x.Argument.ToLower() == "-video") > 1;

                //Check launch arguments
                CaptureTypes launchAction = CaptureTypes.None;
                if (launchArgs != null && launchArgs.Any())
                {
                    foreach (string launchArgument in launchArgs)
                    {
                        try
                        {
                            //Convert launch argument to lower
                            string launchArgumentLower = launchArgument.ToLower();
                            if (launchArgumentLower.StartsWith("-image"))
                            {
                                launchAction = CaptureTypes.Image;
                            }
                            else if (launchArgumentLower.StartsWith("-video"))
                            {
                                launchAction = CaptureTypes.Video;
                            }
                        }
                        catch { }
                    }
                }

                //Check launch action
                if (launchAction == CaptureTypes.Image)
                {
                    Debug.WriteLine("Screen Capture Tool image.");

                    //Capture image to file
                    bool captureResult = await CaptureScreen.CaptureImageToFile();

                    //Show capture overlay window
                    if (SettingLoad(vConfiguration, "OverlayShowScreenshot", typeof(bool)))
                    {
                        vWindowOverlay.ShowOverlay(captureResult ? CaptureTypes.Image : CaptureTypes.Failed);
                        await Task.Delay(2500);
                        vWindowOverlay.Hide();
                    }

                    //Close application
                    await AppClose.Application_Exit();
                }
                else if (launchAction == CaptureTypes.Video)
                {
                    //Check if application is already running
                    if (processRunningVideo)
                    {
                        Debug.WriteLine("Application is already running, closing the process");
                        await AppClose.Application_Exit();
                        return;
                    }

                    Debug.WriteLine("Screen Capture Tool video.");

                    //Capture video to file
                    bool captureResult = await CaptureScreen.StartCaptureVideoToFile();

                    //Check capture result
                    if (captureResult)
                    {
                        //Show capture overlay window
                        if (SettingLoad(vConfiguration, "OverlayShowRecording", typeof(bool)))
                        {
                            vWindowOverlay.ShowOverlay(CaptureTypes.Video);
                        }

                        //Create application tray menu
                        AppTrayMenuNow.Application_CreateTrayMenu();

                        //Start pipes server
                        SocketServer.EnablePipesServer();
                    }
                    else
                    {
                        //Show capture overlay window
                        if (SettingLoad(vConfiguration, "OverlayShowRecording", typeof(bool)))
                        {
                            vWindowOverlay.ShowOverlay(CaptureTypes.Failed);
                            await Task.Delay(2500);
                            vWindowOverlay.Hide();
                        }

                        //Close application
                        await AppClose.Application_Exit();
                    }
                }
                else
                {
                    //Check if application is already running
                    if (processRunningTool)
                    {
                        Debug.WriteLine("Application is already running, closing the process");
                        await AppClose.Application_Exit();
                        return;
                    }

                    Debug.WriteLine("Screen Capture Tool launch.");

                    //Initialize Settings
                    vWindowMain.Settings_Load();
                    vWindowMain.Settings_Save();

                    //Create application tray menu
                    AppTrayMenuTool.Application_CreateTrayMenu();

                    //Check settings if window needs to be shown
                    if (SettingLoad(vConfiguration, "AppFirstLaunch", typeof(bool)))
                    {
                        Debug.WriteLine("First launch showing the window.");
                        vWindowMain.Application_ShowWindow();
                    }

                    //Start keyboard hotkeys
                    AVInputOutputHotkey.Start();
                    AVInputOutputHotkey.EventHotkeyPressed += AppHotkeys.EventHotkeyPressed;

                    //Enable socket server
                    await SocketServer.EnableSocketServer();
                }
            }
            catch { }
        }
    }
}