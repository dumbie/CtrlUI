using ArnoldVinkStyles;
using System;
using System.Diagnostics;
using Windows.System;
using Windows.UI.Xaml.Input;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Set the messagebox result when clicked on listbox
        void MessageBoxSetResult(DataBindString dataBindString)
        {
            try
            {
                vMessageBoxResult = dataBindString;
                Debug.WriteLine("Set messagebox result to: " + vMessageBoxResult.Name);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MessageBox result error: " + ex.Message);
            }
        }

        //Handle messagebox keyboard/controller tapped
        void ListView_MessageBox_KeyPressUp(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                //Check which key is pressed
                if (e.Key == VirtualKey.Space)
                {
                    //Get clicked item
                    DataBindString clickedObject = AVListView.GetRoutedListViewItemObject<DataBindString>(e);

                    MessageBoxSetResult(clickedObject);
                }
            }
            catch { }
        }

        //Handle messagebox mouse/touch tapped
        void ListView_MessageBox_MousePressUp(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                //Check which mouse button is pressed
                if (vMousePressDownLeft)
                {
                    //Get clicked item
                    DataBindString clickedObject = AVListView.GetRoutedListViewItemObject<DataBindString>(e);

                    MessageBoxSetResult(clickedObject);
                }
            }
            catch { }
        }
    }
}