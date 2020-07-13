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

            //Controller Thumbs
            private string PrivThumbLeftX;
            public string ThumbLeftX
            {
                get { return this.PrivThumbLeftX; }
                set
                {
                    if (this.PrivThumbLeftX != value)
                    {
                        this.PrivThumbLeftX = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private string PrivThumbLeftY;
            public string ThumbLeftY
            {
                get { return this.PrivThumbLeftY; }
                set
                {
                    if (this.PrivThumbLeftY != value)
                    {
                        this.PrivThumbLeftY = value;
                        NotifyPropertyChanged();
                    }
                }
            }

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

            private string PrivThumbRightX;
            public string ThumbRightX
            {
                get { return this.PrivThumbRightX; }
                set
                {
                    if (this.PrivThumbRightX != value)
                    {
                        this.PrivThumbRightX = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            private string PrivThumbRightY;
            public string ThumbRightY
            {
                get { return this.PrivThumbRightY; }
                set
                {
                    if (this.PrivThumbRightY != value)
                    {
                        this.PrivThumbRightY = value;
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

            //Controller D-Pad
            private string PrivDPadLeft;
            public string DPadLeft
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

            private string PrivDPadUp;
            public string DPadUp
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

            private string PrivDPadRight;
            public string DPadRight
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

            private string PrivDPadDown;
            public string DPadDown
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

            //Controller Buttons
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

            private string PrivTriggerLeftAnalog;
            public string TriggerLeftAnalog
            {
                get { return this.PrivTriggerLeftAnalog; }
                set
                {
                    if (this.PrivTriggerLeftAnalog != value)
                    {
                        this.PrivTriggerLeftAnalog = value;
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

            private string PrivTriggerRightAnalog;
            public string TriggerRightAnalog
            {
                get { return this.PrivTriggerRightAnalog; }
                set
                {
                    if (this.PrivTriggerRightAnalog != value)
                    {
                        this.PrivTriggerRightAnalog = value;
                        NotifyPropertyChanged();
                    }
                }
            }

            //Controller Settings
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

            //Controller Rumble
            private int PrivRumbleStrength = 75;
            public int RumbleStrength
            {
                get { return this.PrivRumbleStrength; }
                set
                {
                    if (this.PrivRumbleStrength != value)
                    {
                        this.PrivRumbleStrength = value;
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

            //Controller Input
            private bool PrivControllerIgnore = false;
            public bool ControllerIgnore
            {
                get { return this.PrivControllerIgnore; }
                set
                {
                    if (this.PrivControllerIgnore != value)
                    {
                        this.PrivControllerIgnore = value;
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