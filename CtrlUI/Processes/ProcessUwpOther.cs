using System.Threading.Tasks;
using static ArnoldVinkCode.AVUwpAppx;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Update uwp application
        async Task UwpListUpdateApplication(DataBindFile selectedItem)
        {
            try
            {
                await Notification_Send_Status("Refresh", "Updating " + selectedItem.Name);

                //Update application from list
                UwpUpdateApplicationByAppUserModelId(selectedItem.PathFile);
            }
            catch { }
        }

        //Remove uwp application
        async Task UwpListRemoveApplication(DataBindFile selectedItem)
        {
            try
            {
                await Notification_Send_Status("RemoveCross", "Removing " + selectedItem.Name);

                //Remove application from pc
                bool uwpRemoved = UwpRemoveApplicationByPackageFullName(selectedItem.PathFull);

                //Remove application from list
                if (uwpRemoved)
                {
                    await ListBoxRemoveItem(lb_FilePicker, List_FilePicker, selectedItem, true);
                }
            }
            catch { }
        }
    }
}