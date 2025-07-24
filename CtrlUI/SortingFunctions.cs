using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using static ArnoldVinkCode.AVClasses;
using static ArnoldVinkStyles.AVFocus;
using static ArnoldVinkStyles.AVSortObservableCollection;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;
using static LibraryShared.SoundPlayer;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Show and close Sorting popup
        public async Task Popup_Show_Sorting()
        {
            try
            {
                //Check if the popup is already open
                if (!vSortingOpen)
                {
                    //Play the opening sound
                    PlayInterfaceSound(vConfigurationCtrlUI, "PromptOpen", false, false);

                    //Save the previous focus element
                    AVFocusDetailsSave(vSortingElementFocus, null);
                }
                else
                {
                    Debug.WriteLine("Sorting popup is already open.");
                    return;
                }

                //Get focused listbox
                ListBox sortListBox = GetFocusedListBox();

                //Check focused listbox
                if (sortListBox == null)
                {
                    Debug.WriteLine("No focused listbox, returning.");
                    Notification_Show_Status("SortFilter", "Nothing to sort");
                    return;
                }

                //Check sort listbox
                if (!vSelectNearCharacterLists.Contains(sortListBox.Name))
                {
                    Debug.WriteLine("No sorting listbox, returning.");
                    Notification_Show_Status("SortFilter", "Nothing to sort");
                    return;
                }

                //Check file picker path
                if (sortListBox == lb_FilePicker)
                {
                    if (vFilePickerCurrentPath == "PC" || vFilePickerCurrentPath == "UWP")
                    {
                        Debug.WriteLine("Invalid sorting path, returning.");
                        Notification_Show_Status("SortFilter", "Nothing to sort");
                        return;
                    }
                }

                //Clear current sorting items
                lb_Sorting.Items.Clear();

                //Reset popup variables
                vSortingOpen = true;
                checkbox_Sorting_Direction.IsChecked = false;

                //Show the popup
                Popup_Show_Element(grid_Popup_Sorting);

                //Add sorting listbox items
                if (sortListBox == lb_FilePicker)
                {
                    //Function
                    SortFunction<DataBindFile> sortFuncFileType = new SortFunction<DataBindFile>()
                    {
                        Function = x => x.FileType,
                        Direction = SortDirection.Default
                    };
                    SortFunction<DataBindFile> sortFuncName = new SortFunction<DataBindFile>()
                    {
                        Function = x => x.Name,
                    };
                    SortFunction<DataBindFile> sortFuncDateModified = new SortFunction<DataBindFile>()
                    {
                        Function = x => x.DateModified
                    };
                    SortFunction<DataBindFile> sortFuncDateCreated = new SortFunction<DataBindFile>()
                    {
                        Function = x => x.DateCreated
                    };
                    SortFunction<DataBindFile> sortFuncFileExtension = new SortFunction<DataBindFile>()
                    {
                        Function = x => x.Extension
                    };
                    SortFunction<DataBindFile> sortFuncFileSize = new SortFunction<DataBindFile>()
                    {
                        Function = x => x.Size
                    };

                    //Item
                    lb_Sorting.Items.Add(new ProfileShared()
                    {
                        String1 = "Sort by name",
                        Object1 = sortListBox,
                        Object2 = (List<SortFunction<DataBindFile>>)[sortFuncFileType, sortFuncName]
                    });

                    lb_Sorting.Items.Add(new ProfileShared()
                    {
                        String1 = "Sort by date modified",
                        Object1 = sortListBox,
                        Object2 = (List<SortFunction<DataBindFile>>)[sortFuncFileType, sortFuncDateModified]
                    });

                    lb_Sorting.Items.Add(new ProfileShared()
                    {
                        String1 = "Sort by date created",
                        Object1 = sortListBox,
                        Object2 = (List<SortFunction<DataBindFile>>)[sortFuncFileType, sortFuncDateCreated]
                    });

                    lb_Sorting.Items.Add(new ProfileShared()
                    {
                        String1 = "Sort by file type",
                        Object1 = sortListBox,
                        Object2 = (List<SortFunction<DataBindFile>>)[sortFuncFileType, sortFuncFileExtension]
                    });

                    lb_Sorting.Items.Add(new ProfileShared()
                    {
                        String1 = "Sort by file size",
                        Object1 = sortListBox,
                        Object2 = (List<SortFunction<DataBindFile>>)[sortFuncFileType, sortFuncFileSize]
                    });
                }
                else
                {
                    //Function
                    SortFunction<DataBindApp> sortFuncName = new SortFunction<DataBindApp>()
                    {
                        Function = x => x.Name
                    };

                    //Check name title
                    string sortNameTitle = "Sort by name";
                    if (sortListBox == lb_Emulators)
                    {
                        sortNameTitle = "Sort by platform name";
                    }

                    //Item
                    lb_Sorting.Items.Add(new ProfileShared()
                    {
                        String1 = sortNameTitle,
                        Object1 = sortListBox,
                        Object2 = sortFuncName
                    });
                }

                if (sortListBox == lb_Apps || sortListBox == lb_Games || sortListBox == lb_Emulators)
                {
                    //Function
                    SortFunction<DataBindApp> sortFuncNumber = new SortFunction<DataBindApp>()
                    {
                        Function = x => x.Number
                    };
                    SortFunction<DataBindApp> sortFuncLastLaunch = new SortFunction<DataBindApp>()
                    {
                        Function = x => x.LastLaunch
                    };
                    SortFunction<DataBindApp> sortFuncRunTime = new SortFunction<DataBindApp>()
                    {
                        Function = x => x.RunningTime
                    };

                    //Item
                    lb_Sorting.Items.Add(new ProfileShared()
                    {
                        String1 = "Sort by position",
                        Object1 = sortListBox,
                        Object2 = sortFuncNumber
                    });

                    lb_Sorting.Items.Add(new ProfileShared()
                    {
                        String1 = "Sort by last launch time",
                        Object1 = sortListBox,
                        Object2 = sortFuncLastLaunch
                    });

                    lb_Sorting.Items.Add(new ProfileShared()
                    {
                        String1 = "Sort by total running time",
                        Object1 = sortListBox,
                        Object2 = sortFuncRunTime
                    });
                }

                if (sortListBox == lb_Emulators)
                {
                    //Function
                    SortFunction<DataBindApp> sortFuncName = new SortFunction<DataBindApp>()
                    {
                        Function = x => x.EmulatorName
                    };

                    SortFunction<DataBindApp> sortFuncCategory = new SortFunction<DataBindApp>()
                    {
                        Function = x => x.EmulatorCategory
                    };

                    //Item
                    lb_Sorting.Items.Add(new ProfileShared()
                    {
                        String1 = "Sort by emulator name",
                        Object1 = sortListBox,
                        Object2 = sortFuncName
                    });

                    lb_Sorting.Items.Add(new ProfileShared()
                    {
                        String1 = "Sort by emulator category",
                        Object1 = sortListBox,
                        Object2 = sortFuncCategory
                    });
                }
                else if (sortListBox == lb_Launchers)
                {
                    //Function
                    SortFunction<DataBindApp> sortFuncLauncher = new SortFunction<DataBindApp>()
                    {
                        Function = x => x.Launcher
                    };

                    //Item
                    lb_Sorting.Items.Add(new ProfileShared()
                    {
                        String1 = "Sort by launcher",
                        Object1 = sortListBox,
                        Object2 = sortFuncLauncher
                    });
                }
                else if (sortListBox == lb_Shortcuts)
                {
                    //Function
                    SortFunction<DataBindApp> sortFuncDate = new SortFunction<DataBindApp>()
                    {
                        Function = x => x.DateModified
                    };

                    //Item
                    lb_Sorting.Items.Add(new ProfileShared()
                    {
                        String1 = "Sort by date",
                        Object1 = sortListBox,
                        Object2 = sortFuncDate
                    });
                }
                else if (sortListBox == lb_Processes)
                {
                    //Function
                    SortFunction<DataBindApp> sortFuncRunTime = new SortFunction<DataBindApp>()
                    {
                        Function = x => x.StatusProcessRunningTime
                    };

                    //Item
                    lb_Sorting.Items.Add(new ProfileShared()
                    {
                        String1 = "Sort by running time",
                        Object1 = sortListBox,
                        Object2 = sortFuncRunTime
                    });
                }
                else if (sortListBox == lb_Gallery)
                {
                    //Function
                    SortFunction<DataBindApp> sortFuncDate = new SortFunction<DataBindApp>()
                    {
                        Function = x => x.DateModified
                    };

                    //Item
                    lb_Sorting.Items.Add(new ProfileShared()
                    {
                        String1 = "Sort by date",
                        Object1 = sortListBox,
                        Object2 = sortFuncDate
                    });
                }
                else if (sortListBox == lb_Search)
                {
                    //Function
                    SortFunction<DataBindApp> sortFuncCategory = new SortFunction<DataBindApp>()
                    {
                        Function = x => x.Category
                    };

                    //Item
                    lb_Sorting.Items.Add(new ProfileShared()
                    {
                        String1 = "Sort by category",
                        Object1 = sortListBox,
                        Object2 = sortFuncCategory
                    });
                }

                //Focus on first listbox answer
                await ListBoxFocusIndex(lb_Sorting, false, 0, vProcessCurrent.WindowHandleMain);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Sorting show error: " + ex.Message);
            }
        }

        //Close and reset Sorting popup
        async Task Popup_Close_Sorting()
        {
            try
            {
                //Play the closing sound
                PlayInterfaceSound(vConfigurationCtrlUI, "PromptClose", false, false);

                //Reset the popup variables
                vSortingOpen = false;

                //Hide the popup
                Popup_Hide_Element(grid_Popup_Sorting);

                //Focus on the previous focus element
                await AVFocusDetailsFocus(vSortingElementFocus, vProcessCurrent.WindowHandleMain);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Sorting close error: " + ex.Message);
            }
        }

        //Switch sorting direction
        public void SortingSwitchDirection()
        {
            try
            {
                checkbox_Sorting_Direction.IsChecked = !(bool)checkbox_Sorting_Direction.IsChecked;
            }
            catch { }
        }
    }
}