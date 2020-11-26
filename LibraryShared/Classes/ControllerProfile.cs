using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LibraryShared
{
    public partial class Classes
    {
        public class ControllerProfile : INotifyPropertyChanged
        {
            //Controller Information
            private string PrivProductName;
            public string ProductName
            {
                get { return this.PrivProductName; }
                set
                {
                    if (this.PrivProductName != value)
                    {
                        this.PrivProductName = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private string PrivProductID;
            public string ProductID
            {
                get { return this.PrivProductID; }
                set
                {
                    if (this.PrivProductID != value)
                    {
                        this.PrivProductID = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private string PrivVendorName;
            public string VendorName
            {
                get { return this.PrivVendorName; }
                set
                {
                    if (this.PrivVendorName != value)
                    {
                        this.PrivVendorName = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private string PrivVendorID;
            public string VendorID
            {
                get { return this.PrivVendorID; }
                set
                {
                    if (this.PrivVendorID != value)
                    {
                        this.PrivVendorID = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            //Controller Mapping Thumb Stick
            private int? PrivButtonThumbLeft;
            public int? ButtonThumbLeft
            {
                get { return this.PrivButtonThumbLeft; }
                set
                {
                    if (this.PrivButtonThumbLeft != value)
                    {
                        this.PrivButtonThumbLeft = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private int? PrivButtonThumbRight;
            public int? ButtonThumbRight
            {
                get { return this.PrivButtonThumbRight; }
                set
                {
                    if (this.PrivButtonThumbRight != value)
                    {
                        this.PrivButtonThumbRight = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            //Controller Mapping D-Pad
            private int? PrivDPadLeft;
            public int? DPadLeft
            {
                get { return this.PrivDPadLeft; }
                set
                {
                    if (this.PrivDPadLeft != value)
                    {
                        this.PrivDPadLeft = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private int? PrivDPadUp;
            public int? DPadUp
            {
                get { return this.PrivDPadUp; }
                set
                {
                    if (this.PrivDPadUp != value)
                    {
                        this.PrivDPadUp = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private int? PrivDPadRight;
            public int? DPadRight
            {
                get { return this.PrivDPadRight; }
                set
                {
                    if (this.PrivDPadRight != value)
                    {
                        this.PrivDPadRight = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private int? PrivDPadDown;
            public int? DPadDown
            {
                get { return this.PrivDPadDown; }
                set
                {
                    if (this.PrivDPadDown != value)
                    {
                        this.PrivDPadDown = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            //Controller Mapping Buttons
            private int? PrivButtonA;
            public int? ButtonA
            {
                get { return this.PrivButtonA; }
                set
                {
                    if (this.PrivButtonA != value)
                    {
                        this.PrivButtonA = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private int? PrivButtonB;
            public int? ButtonB
            {
                get { return this.PrivButtonB; }
                set
                {
                    if (this.PrivButtonB != value)
                    {
                        this.PrivButtonB = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private int? PrivButtonX;
            public int? ButtonX
            {
                get { return this.PrivButtonX; }
                set
                {
                    if (this.PrivButtonX != value)
                    {
                        this.PrivButtonX = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private int? PrivButtonY;
            public int? ButtonY
            {
                get { return this.PrivButtonY; }
                set
                {
                    if (this.PrivButtonY != value)
                    {
                        this.PrivButtonY = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private int? PrivButtonBack;
            public int? ButtonBack
            {
                get { return this.PrivButtonBack; }
                set
                {
                    if (this.PrivButtonBack != value)
                    {
                        this.PrivButtonBack = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private int? PrivButtonStart;
            public int? ButtonStart
            {
                get { return this.PrivButtonStart; }
                set
                {
                    if (this.PrivButtonStart != value)
                    {
                        this.PrivButtonStart = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private int? PrivButtonGuide;
            public int? ButtonGuide
            {
                get { return this.PrivButtonGuide; }
                set
                {
                    if (this.PrivButtonGuide != value)
                    {
                        this.PrivButtonGuide = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private int? PrivButtonTouchpad;
            public int? ButtonTouchpad
            {
                get { return this.PrivButtonTouchpad; }
                set
                {
                    if (this.PrivButtonTouchpad != value)
                    {
                        this.PrivButtonTouchpad = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private int? PrivButtonMedia;
            public int? ButtonMedia
            {
                get { return this.PrivButtonMedia; }
                set
                {
                    if (this.PrivButtonMedia != value)
                    {
                        this.PrivButtonMedia = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private int? PrivButtonShoulderLeft;
            public int? ButtonShoulderLeft
            {
                get { return this.PrivButtonShoulderLeft; }
                set
                {
                    if (this.PrivButtonShoulderLeft != value)
                    {
                        this.PrivButtonShoulderLeft = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private int? PrivButtonShoulderRight;
            public int? ButtonShoulderRight
            {
                get { return this.PrivButtonShoulderRight; }
                set
                {
                    if (this.PrivButtonShoulderRight != value)
                    {
                        this.PrivButtonShoulderRight = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private int? PrivButtonTriggerLeft;
            public int? ButtonTriggerLeft
            {
                get { return this.PrivButtonTriggerLeft; }
                set
                {
                    if (this.PrivButtonTriggerLeft != value)
                    {
                        this.PrivButtonTriggerLeft = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private int? PrivButtonTriggerRight;
            public int? ButtonTriggerRight
            {
                get { return this.PrivButtonTriggerRight; }
                set
                {
                    if (this.PrivButtonTriggerRight != value)
                    {
                        this.PrivButtonTriggerRight = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            //Controller Button
            private bool PrivFakeGuideButton;
            public bool FakeGuideButton
            {
                get { return this.PrivFakeGuideButton; }
                set
                {
                    if (this.PrivFakeGuideButton != value)
                    {
                        this.PrivFakeGuideButton = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            //Controller Trigger
            private bool PrivUseButtonTriggers;
            public bool UseButtonTriggers
            {
                get { return this.PrivUseButtonTriggers; }
                set
                {
                    if (this.PrivUseButtonTriggers != value)
                    {
                        this.PrivUseButtonTriggers = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private int PrivDeadzoneTriggerLeft = 0;
            public int DeadzoneTriggerLeft
            {
                get { return this.PrivDeadzoneTriggerLeft; }
                set
                {
                    if (this.PrivDeadzoneTriggerLeft != value)
                    {
                        this.PrivDeadzoneTriggerLeft = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private int PrivDeadzoneTriggerRight = 0;
            public int DeadzoneTriggerRight
            {
                get { return this.PrivDeadzoneTriggerRight; }
                set
                {
                    if (this.PrivDeadzoneTriggerRight != value)
                    {
                        this.PrivDeadzoneTriggerRight = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private double PrivSensitivityTrigger = 1;
            public double SensitivityTrigger
            {
                get { return this.PrivSensitivityTrigger; }
                set
                {
                    if (this.PrivSensitivityTrigger != value)
                    {
                        this.PrivSensitivityTrigger = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            //Controller D-Pad
            private bool PrivDPadFourWayMovement;
            public bool DPadFourWayMovement
            {
                get { return this.PrivDPadFourWayMovement; }
                set
                {
                    if (this.PrivDPadFourWayMovement != value)
                    {
                        this.PrivDPadFourWayMovement = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            //Controller Thumb Stick
            private bool PrivThumbFlipMovement;
            public bool ThumbFlipMovement
            {
                get { return this.PrivThumbFlipMovement; }
                set
                {
                    if (this.PrivThumbFlipMovement != value)
                    {
                        this.PrivThumbFlipMovement = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private bool PrivThumbFlipAxesLeft;
            public bool ThumbFlipAxesLeft
            {
                get { return this.PrivThumbFlipAxesLeft; }
                set
                {
                    if (this.PrivThumbFlipAxesLeft != value)
                    {
                        this.PrivThumbFlipAxesLeft = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private bool PrivThumbFlipAxesRight;
            public bool ThumbFlipAxesRight
            {
                get { return this.PrivThumbFlipAxesRight; }
                set
                {
                    if (this.PrivThumbFlipAxesRight != value)
                    {
                        this.PrivThumbFlipAxesRight = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private bool PrivThumbReverseAxesLeft;
            public bool ThumbReverseAxesLeft
            {
                get { return this.PrivThumbReverseAxesLeft; }
                set
                {
                    if (this.PrivThumbReverseAxesLeft != value)
                    {
                        this.PrivThumbReverseAxesLeft = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private bool PrivThumbReverseAxesRight;
            public bool ThumbReverseAxesRight
            {
                get { return this.PrivThumbReverseAxesRight; }
                set
                {
                    if (this.PrivThumbReverseAxesRight != value)
                    {
                        this.PrivThumbReverseAxesRight = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private int PrivDeadzoneThumbLeft = 0;
            public int DeadzoneThumbLeft
            {
                get { return this.PrivDeadzoneThumbLeft; }
                set
                {
                    if (this.PrivDeadzoneThumbLeft != value)
                    {
                        this.PrivDeadzoneThumbLeft = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private int PrivDeadzoneThumbRight = 0;
            public int DeadzoneThumbRight
            {
                get { return this.PrivDeadzoneThumbRight; }
                set
                {
                    if (this.PrivDeadzoneThumbRight != value)
                    {
                        this.PrivDeadzoneThumbRight = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private double PrivSensitivityThumb = 1;
            public double SensitivityThumb
            {
                get { return this.PrivSensitivityThumb; }
                set
                {
                    if (this.PrivSensitivityThumb != value)
                    {
                        this.PrivSensitivityThumb = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            //Trigger Rumble
            private int PrivTriggerRumbleStrength = 10;
            public int TriggerRumbleStrength
            {
                get { return this.PrivTriggerRumbleStrength; }
                set
                {
                    if (this.PrivTriggerRumbleStrength != value)
                    {
                        this.PrivTriggerRumbleStrength = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            //Controller Rumble
            private int PrivControllerRumbleStrength = 80;
            public int ControllerRumbleStrength
            {
                get { return this.PrivControllerRumbleStrength; }
                set
                {
                    if (this.PrivControllerRumbleStrength != value)
                    {
                        this.PrivControllerRumbleStrength = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            //Controller Led
            private int PrivLedBrightness = 25;
            public int LedBrightness
            {
                get { return this.PrivLedBrightness; }
                set
                {
                    if (this.PrivLedBrightness != value)
                    {
                        this.PrivLedBrightness = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            //Controller Ignore
            private bool PrivIgnore = false;
            public bool Ignore
            {
                get { return this.PrivIgnore; }
                set
                {
                    if (this.PrivIgnore != value)
                    {
                        this.PrivIgnore = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}