using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVAudioDevice;
using static ArnoldVinkStyles.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task SwitchAudioDevice()
        {
            try
            {
                Debug.WriteLine("Listing all the active audio playback devices.");

                //Get the current active audio device
                AudioDeviceSummary deviceCurrent = GetDefaultDevice();

                //Get all the available audio devices
                List<AudioDeviceSummary> devicesList = ListAudioDevices();

                //Add all devices to answers list
                List<DataBindString> Answers = new List<DataBindString>();

                foreach (AudioDeviceSummary audioDevice in devicesList)
                {
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "Assets/Default/Icons/VolumeUp.png" }, null, vImageBackupSource, -1, -1, IntPtr.Zero, 0);
                    Answer1.Name = audioDevice.Name;
                    Answers.Add(Answer1);
                }

                //Show the messagebox prompt
                DataBindString messageResult = await Popup_Show_MessageBox("Switch audio playback device", "", "Please select the audio playback device you want to start listening from, your current audio device is: " + deviceCurrent.Name, Answers);
                if (messageResult != null)
                {
                    //Change the default device
                    AudioDeviceSummary changeDevice = devicesList.FirstOrDefault(x => x.Name.ToLower() == messageResult.Name.ToLower());
                    if (changeDevice != null)
                    {
                        if (SetDefaultDevice(changeDevice.Identifier))
                        {
                            Notification_Show_Status("VolumeUp", "Switched audio device");
                        }
                        else
                        {
                            Notification_Show_Status("VolumeUp", "Switching audio failed");
                        }
                    }
                }
            }
            catch
            {
                Debug.WriteLine("Failed to load the audio devices");
                Notification_Show_Status("VolumeUp", "No audio available");
            }
        }
    }
}