using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using static ArnoldVinkCode.AVSortObservableCollection;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Show the application move popup
        async Task Popup_Show_AppMove(ListBox listBox, DataBindApp dataBindApp)
        {
            try
            {
                //Set move application variables
                vMoveAppListBox = listBox;
                vMoveAppDataBind = dataBindApp;

                //Set the application image
                image_MoveAppIcon.Source = dataBindApp.ImageBitmap;

                //Show the move popup
                await Popup_Show(grid_Popup_MoveApplication, btn_MoveAppLeft, 0.80);
            }
            catch { }
        }

        void MoveApplicationList_Left()
        {
            try
            {
                //Sort list by number
                SortFunction<DataBindApp> sortFunction = new SortFunction<DataBindApp>();
                sortFunction.Function = x => x.Number;
                SortObservableCollection(vMoveAppListBox, sortFunction, null);

                //Get the target application
                IEnumerable<DataBindApp> combinedApps = CombineAppLists(true, true, true, false, false, false, false).Where(x => x.Category == vMoveAppDataBind.Category);
                DataBindApp targetAppDataBind = combinedApps.OrderByDescending(x => x.Number).FirstOrDefault(x => x.Number < vMoveAppDataBind.Number);
                int selectedNumber = vMoveAppDataBind.Number;
                int targetNumber = targetAppDataBind.Number;
                Debug.WriteLine("Current number: " + selectedNumber + " / New number: " + targetNumber);

                //Update the application number
                vMoveAppDataBind.Number = targetNumber;
                targetAppDataBind.Number = selectedNumber;

                //Sort list by number
                SortObservableCollection(vMoveAppListBox, sortFunction, null);

                //Show moving notification
                Notification_Show_Status("Sorting", "Moving app left");

                //Save json applist
                JsonSaveList_Applications();
            }
            catch { }
        }

        void MoveApplicationList_Right()
        {
            try
            {
                //Sort list by number
                SortFunction<DataBindApp> sortFunction = new SortFunction<DataBindApp>();
                sortFunction.Function = x => x.Number;
                SortObservableCollection(vMoveAppListBox, sortFunction, null);

                //Get the target application
                IEnumerable<DataBindApp> combinedApps = CombineAppLists(true, true, true, false, false, false, false).Where(x => x.Category == vMoveAppDataBind.Category);
                DataBindApp targetAppDataBind = combinedApps.OrderBy(x => x.Number).FirstOrDefault(x => x.Number > vMoveAppDataBind.Number);
                int selectedNumber = vMoveAppDataBind.Number;
                int targetNumber = targetAppDataBind.Number;
                Debug.WriteLine("Current number: " + selectedNumber + " / New number: " + targetNumber);

                //Update the application number
                vMoveAppDataBind.Number = targetNumber;
                targetAppDataBind.Number = selectedNumber;

                //Sort list by number
                SortObservableCollection(vMoveAppListBox, sortFunction, null);

                //Show moving notification
                Notification_Show_Status("Sorting", "Moving app right");

                //Save json applist
                JsonSaveList_Applications();
            }
            catch { }
        }
    }
}