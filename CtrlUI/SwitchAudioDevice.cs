using IMMDevice;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using static LibraryShared.AppImport;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        public class AudioDeviceSummary
        {
            public string Id = string.Empty;
            public string Name = string.Empty;
        }

        async Task SwitchAudioDevice()
        {
            try
            {
                Debug.WriteLine("Listing all the active audio playback devices.");

                //Get the current active audio device
                AudioDeviceSummary deviceCurrent = GetDefaultDevice();

                //Get all the playback audio devices
                List<DataBindString> Answers = new List<DataBindString>();
                List<AudioDeviceSummary> deviceListSummary = new List<AudioDeviceSummary>();
                IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
                IMMDeviceCollection deviceCollection = deviceEnumerator.EnumAudioEndpoints(EDataFlow.eRender, DeviceState.ACTIVE);

                uint deviceCount = deviceCollection.GetCount();
                for (uint deviceIndex = 0; deviceIndex < deviceCount; deviceIndex++)
                {
                    IMMDevice.IMMDevice deviceItem = deviceCollection.Item(deviceIndex);

                    //Get the audio device id
                    string deviceId = deviceItem.GetId();

                    //Get the audio device name
                    PropertyVariant propertyVariant = new PropertyVariant();
                    IPropertyStore propertyStore = deviceItem.OpenPropertyStore(STGM.STGM_READ);
                    propertyStore.GetValue(ref PKEY_Device_FriendlyName, out propertyVariant);
                    string deviceName = Marshal.PtrToStringUni(propertyVariant.pwszVal);

                    //Add device to summary list
                    deviceListSummary.Add(new AudioDeviceSummary() { Id = deviceId, Name = deviceName });

                    //Add device to answers list
                    DataBindString Answer1 = new DataBindString();
                    Answer1.ImageBitmap = FileToBitmapImage(new string[] { "pack://application:,,,/Assets/Icons/VolumeUp.png" }, IntPtr.Zero, -1);
                    Answer1.Name = deviceName;
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
                    AudioDeviceSummary ChangeDevice = deviceListSummary.Where(x => x.Name.ToLower() == ResultMultiple.Name.ToLower()).FirstOrDefault();
                    if (ChangeDevice != null)
                    {
                        SetDefaultDevice(ChangeDevice.Id);
                    }
                }
            }
            catch
            {
                Debug.WriteLine("Failed to load the audio devices");
                Popup_Show_Status("VolumeUp", "No audio available");
            }
        }

        void SetDefaultDevice(string deviceId)
        {
            try
            {
                PolicyConfigClient autoPolicyConfigClient = new PolicyConfigClient();
                autoPolicyConfigClient.SetDefaultEndpoint(deviceId, ERole.eMultimedia);
                Debug.WriteLine("Changed default audio device: " + deviceId);
                Popup_Show_Status("VolumeUp", "Switched audio device");
            }
            catch
            {
                Debug.WriteLine("Failed to set new default device: " + deviceId);
                Popup_Show_Status("VolumeUp", "Switching audio failed");
            }
        }

        AudioDeviceSummary GetDefaultDevice()
        {
            try
            {
                IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)new MMDeviceEnumerator();
                IMMDevice.IMMDevice deviceItem = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);

                //Get the audio device id
                string deviceId = deviceItem.GetId();

                //Get the audio device name
                PropertyVariant propertyVariant = new PropertyVariant();
                IPropertyStore propertyStore = deviceItem.OpenPropertyStore(STGM.STGM_READ);
                propertyStore.GetValue(ref PKEY_Device_FriendlyName, out propertyVariant);
                string deviceName = Marshal.PtrToStringUni(propertyVariant.pwszVal);

                return new AudioDeviceSummary() { Id = deviceId, Name = deviceName };
            }
            catch
            {
                Debug.WriteLine("Failed to get the default device.");
                return null;
            }
        }
    }
}