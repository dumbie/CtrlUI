using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using static ArnoldVinkStyles.AVSortObservableCollection;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Show the application move popup
        async Task Popup_Show_AppMove(ListView listBox, DataBindApp dataBindApp)
        {
            try
            {
                //Set move application variables
                vMoveAppListView = listBox;
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
                //Sort list by number
                SortFunction<DataBindApp> sortFunction = new SortFunction<DataBindApp>();
                sortFunction.Function = x => x.Number;
                SortObservableCollection(vMoveAppListView, sortFunction, null);

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
                SortObservableCollection(vMoveAppListView, sortFunction, null);

                //Show moving notification
                await Notification_Show_Status("Sorting", "Moving app left");

                //Save json applist
                JsonSaveList_Applications();
            }
            catch { }
        }

        async Task MoveApplicationList_Right()
        {
            try
            {
                //Sort list by number
                SortFunction<DataBindApp> sortFunction = new SortFunction<DataBindApp>();
                sortFunction.Function = x => x.Number;
                SortObservableCollection(vMoveAppListView, sortFunction, null);

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
                SortObservableCollection(vMoveAppListView, sortFunction, null);

                //Show moving notification
                await Notification_Show_Status("Sorting", "Moving app right");

                //Save json applist
                JsonSaveList_Applications();
            }
            catch { }
        }
    }
}