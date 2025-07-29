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
        void MessageBoxSetResult()
        {
            try
            {
                vMessageBoxResult = listView_MessageBox.SelectedItem as DataBindString;
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
                if (e.Key == VirtualKey.Space)
                {
                    MessageBoxSetResult();
                }
            }
            catch { }
        }

        //Handle messagebox mouse/touch tapped
        void ListView_MessageBox_MousePressUp(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                //Check if an actual ListViewItem is clicked
                if (!AVInterface.CheckClickedListViewItem(e))
                {
                    return;
                }

                //Check which mouse button is pressed
                if (vMousePressDownLeft)
                {
                    MessageBoxSetResult();
                }
            }
            catch { }
        }
    }
}