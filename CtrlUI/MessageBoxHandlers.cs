using ArnoldVinkStyles;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
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
                vMessageBoxResult = lb_MessageBox.SelectedItem as DataBindString;
                Debug.WriteLine("Set messagebox result to: " + vMessageBoxResult.Name);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MessageBox result error: " + ex.Message);
            }
        }

        //Handle messagebox keyboard/controller tapped
        void ListBox_MessageBox_KeyPressUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    MessageBoxSetResult();
                }
            }
            catch { }
        }

        //Handle messagebox mouse/touch tapped
        void ListBox_MessageBox_MousePressUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //Check if an actual ListBoxItem is clicked
                if (!AVInterface.ListBoxItemClickCheck((DependencyObject)e.OriginalSource)) { return; }

                //Check which mouse button is pressed
                if (e.ClickCount == 1)
                {
                    if (vMousePressDownLeft)
                    {
                        MessageBoxSetResult();
                    }
                }
            }
            catch { }
        }
    }
}