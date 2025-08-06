using ArnoldVinkStyles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using static ArnoldVinkCode.AVClassConverters;
using static ArnoldVinkCode.AVClasses;
using static ArnoldVinkStyles.AVSortObservableCollection;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Handle sorting mouse/touch tapped
        private async void ListView_Sorting_MousePressUp(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                //Check which mouse button is pressed
                if (vMousePressDownLeft)
                {
                    //Get clicked item
                    ProfileShared clickedObject = AVListView.GetRoutedListViewItemObject<ProfileShared>(e);

                    await ListView_Sorting_Click(clickedObject);
                }
            }
            catch { }
        }

        //Handle sorting keyboard/controller tapped
        private async void ListView_Sorting_KeyPressUp(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                //Check which key is pressed
                if (e.Key == VirtualKey.Space)
                {
                    //Get clicked item
                    ProfileShared clickedObject = AVListView.GetRoutedListViewItemObject<ProfileShared>(e);

                    await ListView_Sorting_Click(clickedObject);
                }
            }
            catch { }
        }

        //Handle sorting click
        private async Task ListView_Sorting_Click(ProfileShared profileShared)
        {
            try
            {
                //Check clicked object
                if (profileShared == null)
                {
                    Debug.WriteLine("Clicked ListView object is null.");
                    return;
                }

                //Sort functions
                dynamic sortListView = profileShared.Object1;
                dynamic sortOrderBy = profileShared.Object2;
                dynamic sortWhere = profileShared.Object3;
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
                SortObservableCollection(sortListView, sortOrderBy, sortWhere);

                //Close sorting popup
                await Popup_Close_Sorting();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ListView sort handle failed: " + ex.Message);
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