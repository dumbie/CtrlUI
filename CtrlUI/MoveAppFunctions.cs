using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Show the application move popup
        async Task Popup_Show_AppMove(DataBindApp dataBindApp)
        {
            try
            {
                //Set the move application variable
                vMoveAppDataBind = dataBindApp;

                //Set the application image
                image_MoveAppIcon.Source = dataBindApp.ImageBitmap;

                //Show the move popup
                await Popup_Show(grid_Popup_MoveApplication, btn_MoveAppLeft, 0.80);
            }
            catch { }
        }

        async Task MoveApplicationList_Left()
        {
            try
            {
                //Sort the lists by number
                await SortAppListsByNumber(true);

                //Get the target application
                IEnumerable<DataBindApp> combinedApps = CombineAppLists(true, true, true, false, false, false).Where(x => x.Category == vMoveAppDataBind.Category);
                DataBindApp targetAppDataBind = combinedApps.OrderByDescending(x => x.Number).Where(x => x.Number < vMoveAppDataBind.Number).FirstOrDefault();
                int selectedNumber = vMoveAppDataBind.Number;
                int targetNumber = targetAppDataBind.Number;
                Debug.WriteLine("Current number: " + selectedNumber + " / New number: " + targetNumber);

                //Update the application number
                vMoveAppDataBind.Number = targetNumber;
                targetAppDataBind.Number = selectedNumber;

                //Sort the lists by number
                await SortAppListsByNumber(true);
                await Notification_Send_Status("Sorting", "Moving app left");

                //Save json applist
                JsonSaveApplications();
            }
            catch { }
        }

        async Task MoveApplicationList_Right()
        {
            try
            {
                //Sort the lists by number
                await SortAppListsByNumber(true);

                //Get the target application
                IEnumerable<DataBindApp> combinedApps = CombineAppLists(true, true, true, false, false, false).Where(x => x.Category == vMoveAppDataBind.Category);
                DataBindApp targetAppDataBind = combinedApps.OrderBy(x => x.Number).Where(x => x.Number > vMoveAppDataBind.Number).FirstOrDefault();
                int selectedNumber = vMoveAppDataBind.Number;
                int targetNumber = targetAppDataBind.Number;
                Debug.WriteLine("Current number: " + selectedNumber + " / New number: " + targetNumber);

                //Update the application number
                vMoveAppDataBind.Number = targetNumber;
                targetAppDataBind.Number = selectedNumber;

                //Sort the lists by number
                await SortAppListsByNumber(true);
                await Notification_Send_Status("Sorting", "Moving app right");

                //Save json applist
                JsonSaveApplications();
            }
            catch { }
        }
    }
}