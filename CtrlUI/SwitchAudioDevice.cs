using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static ArnoldVinkCode.AVAudioDevice;
using static CtrlUI.ImageFunctions;
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
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/VolumeUp.png" }, IntPtr.Zero, -1);
                    Answer1.Name = audioDevice.Name;
                    Answers.Add(Answer1);
                }

                DataBindString cancelString = new DataBindString();
                cancelString.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/Close.png" }, IntPtr.Zero, -1);
                cancelString.Name = "Cancel";
                Answers.Add(cancelString);

                //Show the messagebox prompt
                DataBindString ResultMultiple = await Popup_Show_MessageBox("Switch audio playback device", "", "Please select the audio playback device you want to start listening from, your current audio device is: " + deviceCurrent.Name, Answers);
                if (ResultMultiple != null)
                {
                    //Change the default device
                    AudioDeviceSummary ChangeDevice = devicesList.Where(x => x.Name.ToLower() == ResultMultiple.Name.ToLower()).FirstOrDefault();
                    if (ChangeDevice != null)
                    {
                        if (SetDefaultDevice(ChangeDevice.Identifier))
                        {
                            Popup_Show_Status("VolumeUp", "Switched audio device");
                        }
                        else
                        {
                            Popup_Show_Status("VolumeUp", "Switching audio failed");
                        }
                    }
                }
            }
            catch
            {
                Debug.WriteLine("Failed to load the audio devices");
                Popup_Show_Status("VolumeUp", "No audio available");
            }
        }
    }
}