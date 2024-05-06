using System.Collections.Generic;
using static ArnoldVinkCode.AVClasses;

namespace ScreenCapture
{
    partial class WindowMain
    {
        void Load_ComboBox_Items()
        {
            try
            {
                List<ComboBoxItemValue> itemsScreenshotSaveFormat = new List<ComboBoxItemValue>
                {
                    new ComboBoxItemValue()
                    {
                        Text = "JXR (HDR and SDR)",
                    },
                    new ComboBoxItemValue()
                    {
                        Text = "JPG (SDR only)",
                    },
                    new ComboBoxItemValue()
                    {
                        Text = "PNG (SDR only)",
                    },
                    new ComboBoxItemValue()
                    {
                        Text = "BMP (SDR only)",
                    },
                    new ComboBoxItemValue()
                    {
                        Text = "TIF (SDR only)",
                    },
                    new ComboBoxItemValue()
                    {
                        Text = "HEIF (SDR only)",
                    }
                };
                combobox_ScreenshotSaveFormat.Items.Clear();
                combobox_ScreenshotSaveFormat.ItemsSource = itemsScreenshotSaveFormat;
                combobox_ScreenshotSaveFormat.DisplayMemberPath = "Text";

                List<ComboBoxItemValue> itemsVideoSaveFormat = new List<ComboBoxItemValue>
                {
                    new ComboBoxItemValue()
                    {
                        Text = "H264 (SDR only)",
                    },
                    new ComboBoxItemValue()
                    {
                        Text = "HEVC",
                    }
                };
                combobox_VideoSaveFormat.Items.Clear();
                combobox_VideoSaveFormat.ItemsSource = itemsVideoSaveFormat;
                combobox_VideoSaveFormat.DisplayMemberPath = "Text";

                List<ComboBoxItemValue> itemsVideoFrameRate = new List<ComboBoxItemValue>
                {
                    new ComboBoxItemValue()
                    {
                        Text = "30 FPS",
                        Value = "30"
                    },
                    new ComboBoxItemValue()
                    {
                        Text = "60 FPS",
                        Value = "60"
                    },
                    new ComboBoxItemValue()
                    {
                        Text = "90 FPS",
                        Value = "90"
                    },
                    new ComboBoxItemValue()
                    {
                        Text = "120 FPS",
                        Value = "120"
                    }
                };
                combobox_VideoFrameRate.Items.Clear();
                combobox_VideoFrameRate.ItemsSource = itemsVideoFrameRate;
                combobox_VideoFrameRate.DisplayMemberPath = "Text";

                List<ComboBoxItemValue> itemsAudioSaveFormat = new List<ComboBoxItemValue>
                {
                    new ComboBoxItemValue()
                    {
                        Text = "MP3",
                        Value = "MP3"
                    },
                    new ComboBoxItemValue()
                    {
                        Text = "AAC",
                        Value = "AAC"
                    },
                    new ComboBoxItemValue()
                    {
                        Text = "FLAC",
                        Value = "FLAC"
                    }
                };
                combobox_AudioSaveFormat.Items.Clear();
                combobox_AudioSaveFormat.ItemsSource = itemsAudioSaveFormat;
                combobox_AudioSaveFormat.DisplayMemberPath = "Text";

                List<ComboBoxItemValue> itemsAudioChannels = new List<ComboBoxItemValue>
                {
                    new ComboBoxItemValue()
                    {
                        Text = "2 channels",
                        Value = "2"
                    },
                    new ComboBoxItemValue()
                    {
                        Text = "5 channels",
                        Value = "5"
                    },
                    new ComboBoxItemValue()
                    {
                        Text = "7 channels",
                        Value = "7"
                    }
                };
                combobox_AudioChannels.Items.Clear();
                combobox_AudioChannels.ItemsSource = itemsAudioChannels;
                combobox_AudioChannels.DisplayMemberPath = "Text";

                List<ComboBoxItemValue> itemsAudioBitDepth = new List<ComboBoxItemValue>
                {
                    new ComboBoxItemValue()
                    {
                        Text = "16 bit",
                        Value = "16"
                    },
                    new ComboBoxItemValue()
                    {
                        Text = "24 bit",
                        Value = "24"
                    },
                    new ComboBoxItemValue()
                    {
                        Text = "32 bit",
                        Value = "32"
                    }
                };
                combobox_AudioBitDepth.Items.Clear();
                combobox_AudioBitDepth.ItemsSource = itemsAudioBitDepth;
                combobox_AudioBitDepth.DisplayMemberPath = "Text";

                List<ComboBoxItemValue> itemsAudioSampleRate = new List<ComboBoxItemValue>
                {
                    new ComboBoxItemValue()
                    {
                        Text = "44.100 Hz",
                        Value = "44100"
                    },
                    new ComboBoxItemValue()
                    {
                        Text = "48.000 Hz",
                        Value = "48000"
                    },
                    new ComboBoxItemValue()
                    {
                        Text = "96.000 Hz",
                        Value = "96000"
                    },
                    new ComboBoxItemValue()
                    {
                        Text = "192.000 Hz",
                        Value = "192000"
                    }
                };
                combobox_AudioSampleRate.Items.Clear();
                combobox_AudioSampleRate.ItemsSource = itemsAudioSampleRate;
                combobox_AudioSampleRate.DisplayMemberPath = "Text";

                List<ComboBoxItemValue> itemsOverlayPosition = new List<ComboBoxItemValue>
                {
                    new ComboBoxItemValue()
                    {
                        Text = "Top Left",
                        Value = "TopLeft"
                    },
                    new ComboBoxItemValue()
                    {
                        Text = "Top Center",
                        Value = "TopCenter"
                    },
                    new ComboBoxItemValue()
                    {
                        Text = "Top Right",
                        Value = "TopRight"
                    },
                    new ComboBoxItemValue()
                    {
                        Text = "Right Center",
                        Value = "RightCenter"
                    },
                    new ComboBoxItemValue()
                    {
                        Text = "Bottom Right",
                        Value = "BottomRight"
                    },
                    new ComboBoxItemValue()
                    {
                        Text = "Bottom Center",
                        Value = "BottomCenter"
                    },
                    new ComboBoxItemValue()
                    {
                        Text = "Bottom Left",
                        Value = "BottomLeft"
                    },
                    new ComboBoxItemValue()
                    {
                        Text = "Left Center",
                        Value = "LeftCenter"
                    }
                };
                combobox_OverlayPosition.Items.Clear();
                combobox_OverlayPosition.ItemsSource = itemsOverlayPosition;
                combobox_OverlayPosition.DisplayMemberPath = "Text";

                List<ComboBoxItemValue> itemsVideoRateControl = new List<ComboBoxItemValue>
                {
                    new ComboBoxItemValue()
                    {
                        Text = "Variable Rate Control",
                    },
                    new ComboBoxItemValue()
                    {
                        Text = "Constant Rate Control",
                    }
                };
                combobox_VideoRateControl.Items.Clear();
                combobox_VideoRateControl.ItemsSource = itemsVideoRateControl;
                combobox_VideoRateControl.DisplayMemberPath = "Text";
            }
            catch { }
        }
    }
}