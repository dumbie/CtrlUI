using ArnoldVinkStyles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using static ArnoldVinkCode.AVClassConverters;
using static ArnoldVinkCode.AVClasses;
using static ArnoldVinkStyles.AVSortObservableCollection;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Handle sorting mouse/touch tapped
        private async void ListBox_Sorting_MousePressUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //Check if an actual ListBoxItem is clicked
                if (!AVInterface.ListBoxItemClickCheck((DependencyObject)e.OriginalSource)) { return; }

                //Check which mouse button is pressed
                if (e.ClickCount == 1)
                {
                    await ListBox_Sorting_Handle();
                }
            }
            catch { }
        }

        //Handle sorting keyboard/controller tapped
        private async void ListBox_Sorting_KeyPressUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Space)
                {
                    await ListBox_Sorting_Handle();
                }
            }
            catch { }
        }

        private async Task ListBox_Sorting_Handle()
        {
            try
            {
                //Sort functions
                ProfileShared selectedItem = lb_Sorting.SelectedItem as ProfileShared;
                dynamic sortListBox = selectedItem.Object1;
                dynamic sortOrderBy = selectedItem.Object2;
                dynamic sortWhere = selectedItem.Object3;
                Type orderType = GetDynamicType(sortOrderBy);

                //Get sorting direction
                SortDirection sortDirection = (bool)checkbox_Sorting_Direction.IsChecked ? SortDirection.Descending : SortDirection.Ascending;

                //Set sorting direction
                if (orderType == typeof(List<SortFunction<DataBindFile>>))
                {
                    var sortFunctions = (List<SortFunction<DataBindFile>>)sortOrderBy;
                    foreach (var sortFunc in sortFunctions)
                    {
                        if (sortFunc.Direction != SortDirection.Default)
                        {
                            sortFunc.Direction = sortDirection;
                        }
                    }
                }
                else if (orderType == typeof(SortFunction<DataBindApp>))
                {
                    var sortFunc = (SortFunction<DataBindApp>)sortOrderBy;
                    sortFunc.Direction = sortDirection;
                }

                //Sort observable list
                SortObservableCollection(sortListBox, sortOrderBy, sortWhere);

                //Close sorting popup
                await Popup_Close_Sorting();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ListBox sort handle failed: " + ex.Message);
            }
        }

        private void Grid_Popup_Sorting_button_Direction_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SortingSwitchDirection();
            }
            catch { }
        }
    }
}