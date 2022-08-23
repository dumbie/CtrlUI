using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using static DirectXInput.AppVariables;
using static LibraryShared.Settings;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Update drivers buttons
        async void btn_Settings_InstallDrivers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await Message_UpdateDrivers();
            }
            catch { }
        }

        //Change screenshot location
        private void Btn_Settings_ScreenshotLocationChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
                {
                    folderBrowserDialog.ShowDialog();
                    if (!string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                    {
                        Debug.WriteLine("Screenshot location selected: " + folderBrowserDialog.SelectedPath);
                        Setting_Save(vConfigurationDirectXInput, "ScreenshotLocation", folderBrowserDialog.SelectedPath);
                        textblock_Settings_ScreenshotLocation.Text = textblock_Settings_ScreenshotLocation.Tag + folderBrowserDialog.SelectedPath;
                    }
                }
            }
            catch { }
        }

        //Open screenshot location
        private void Btn_Settings_ScreenshotLocationOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Check screenshot location
                string screenshotSaveFolder = Setting_Load(vConfigurationDirectXInput, "ScreenshotLocation").ToString();
                if (!Directory.Exists(screenshotSaveFolder))
                {
                    //Check screenshots folder in app directory
                    if (!Directory.Exists("Screenshots"))
                    {
                        Directory.CreateDirectory("Screenshots");
                    }
                    Process.Start("Screenshots");
                }
                else
                {
                    Process.Start(screenshotSaveFolder);
                }
            }
            catch { }
        }
    }
}