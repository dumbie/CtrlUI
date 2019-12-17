using System.Threading.Tasks;
using System.Windows;
using static CtrlUI.AppVariables;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Edit application profile
        async Task Popup_Show_ProfileManager()
        {
            try
            {
                //Select application category
                vFilePickerStrings = new string[][] { new[] { "Ignored shortcuts", "IgnoredShortcuts" }, new[] { "Shortcut locations", "ShortcutLocations" }, new[] { "File locations", "FileLocations" }, new[] { "Blocked processes", "BlockedProcesses" }, new[] { "Blocked shortcuts", "BlockedShortcuts" }, new[] { "Blocked shortcut uri's", "BlockedShortcutUris" } };
                vFilePickerFilterIn = new string[] { };
                vFilePickerFilterOut = new string[] { };
                vFilePickerTitle = "Profile Category";
                vFilePickerDescription = "Please select the profile to manage:";
                vFilePickerShowNoFile = false;
                vFilePickerShowRoms = false;
                vFilePickerShowFiles = false;
                vFilePickerShowDirectories = false;
                grid_Popup_FilePicker_stackpanel_Description.Visibility = Visibility.Collapsed;
                await Popup_Show_FilePicker("String", -1, false, null);

                while (vFilePickerResult == null && !vFilePickerCancelled && !vFilePickerCompleted) { await Task.Delay(500); }
                if (vFilePickerCancelled) { return; }
            }
            catch { }
        }
    }
}