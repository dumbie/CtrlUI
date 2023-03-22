using ArnoldVinkCode;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static ArnoldVinkCode.AVFocus;
using static CtrlUI.AppVariables;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Handle category menu keyboard/controller tapped
        public async void Button_Category_Menu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button senderFramework = (Button)sender;
                if (senderFramework.Name == "button_Category_Menu_Apps") { await CategoryListChange(ListCategory.App); }
                else if (senderFramework.Name == "button_Category_Menu_Games") { await CategoryListChange(ListCategory.Game); }
                else if (senderFramework.Name == "button_Category_Menu_Emulators") { await CategoryListChange(ListCategory.Emulator); }
                else if (senderFramework.Name == "button_Category_Menu_Launchers") { await CategoryListChange(ListCategory.Launcher); }
                else if (senderFramework.Name == "button_Category_Menu_Shortcuts") { await CategoryListChange(ListCategory.Shortcut); }
                else if (senderFramework.Name == "button_Category_Menu_Processes") { await CategoryListChange(ListCategory.Process); }
                else if (senderFramework.Name == "button_Category_Menu_Search") { await CategoryListChange(ListCategory.Search); }
            }
            catch { }
        }

        //Get category list count
        public int CategoryListCount(ListCategory listCategory)
        {
            try
            {
                if (listCategory == ListCategory.App) { return List_Apps.Count; }
                else if (listCategory == ListCategory.Game) { return List_Games.Count; }
                else if (listCategory == ListCategory.Emulator) { return List_Emulators.Count; }
                else if (listCategory == ListCategory.Launcher) { return List_Launchers.Count; }
                else if (listCategory == ListCategory.Shortcut) { return List_Shortcuts.Count; }
                else if (listCategory == ListCategory.Process) { return List_Processes.Count; }
                else if (listCategory == ListCategory.Search) { return List_Search.Count; }
            }
            catch
            {
                Debug.WriteLine("Failed to get category count.");
            }
            return -1;
        }

        //Check active category list
        async Task CategoryListCheckActive()
        {
            try
            {
                if (CategoryListCount(vCurrentListCategory) <= 0 && vCurrentListCategory != ListCategory.Search)
                {
                    await AVActions.DispatcherInvoke(async delegate
                    {
                        ListCategory? listCategorySwitch = CategoryListPreviousWithItems(vCurrentListCategory, false);
                        if (listCategorySwitch == null)
                        {
                            listCategorySwitch = CategoryListNextWithItems(vCurrentListCategory, false);
                        }
                        await CategoryListChange((ListCategory)listCategorySwitch);
                    });
                }
            }
            catch { }
        }

        //Get first category with items
        public ListCategory? CategoryListFirstWithItems()
        {
            try
            {
                if (List_Apps.Count > 0) { return ListCategory.App; }
                else if (List_Games.Count > 0) { return ListCategory.Game; }
                else if (List_Emulators.Count > 0) { return ListCategory.Emulator; }
                else if (List_Launchers.Count > 0) { return ListCategory.Launcher; }
                else if (List_Shortcuts.Count > 0) { return ListCategory.Shortcut; }
                else if (List_Processes.Count > 0) { return ListCategory.Process; }
                else if (List_Search.Count > 0) { return ListCategory.Search; }
            }
            catch
            {
                Debug.WriteLine("Failed to get first category with items.");
            }
            return null;
        }

        //Get next category with items
        public ListCategory? CategoryListNextWithItems(ListCategory listCategory, bool loopCategory)
        {
            try
            {
                int enumCount = Enum.GetNames(typeof(ListCategory)).Length - 1;
                int switchCategory = Convert.ToInt32(listCategory) + 1;
                for (int i = switchCategory; i <= enumCount; i++)
                {
                    ListCategory switchListCategory = (ListCategory)i;
                    if (switchListCategory == ListCategory.Search || CategoryListCount(switchListCategory) > 0) { return switchListCategory; }
                }

                if (loopCategory)
                {
                    return CategoryListFirstWithItems();
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                Debug.WriteLine("Failed to get next category with items.");
                return null;
            }
        }

        //Get previous category with items
        public ListCategory? CategoryListPreviousWithItems(ListCategory listCategory, bool loopCategory)
        {
            try
            {
                int switchCategory = Convert.ToInt32(listCategory) - 1;
                for (int i = switchCategory; i >= 0; i--)
                {
                    ListCategory switchListCategory = (ListCategory)i;
                    if (switchListCategory == ListCategory.Search || CategoryListCount(switchListCategory) > 0) { return switchListCategory; }
                }

                if (loopCategory)
                {
                    int enumCount = Enum.GetNames(typeof(ListCategory)).Length - 1;
                    return (ListCategory)enumCount;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                Debug.WriteLine("Failed to get previous category with items.");
                return null;
            }
        }

        //Update listbox category items count
        void CategoryListUpdateCount()
        {
            try
            {
                //Check current list category
                int listCount = CategoryListCount(vCurrentListCategory);
                string listCountString = listCount.ToString();

                //Check the list count
                if (listCount <= 0)
                {
                    listCountString = string.Empty;
                }

                AVActions.DispatcherInvoke(delegate
                {
                    textblock_Category_Count.Text = listCountString;
                });

                //Debug.WriteLine("Updating the lists count to: " + listCountString);
            }
            catch { }
        }

        //Change listbox category visibility
        async Task CategoryListChange(ListCategory listCategory)
        {
            try
            {
                //Check if category has items
                if (listCategory != ListCategory.Search && CategoryListCount(listCategory) <= 0)
                {
                    //await Notification_Send_Status("Close", "Selected category has no items.");
                    Debug.WriteLine("Category " + listCategory + " has no items, falling back to first with items.");
                    listCategory = (ListCategory)CategoryListFirstWithItems();
                }

                //Set target listbox and textblock
                ListBox targetListbox = null;
                TextBlock targetTextblock = null;
                if (listCategory == ListCategory.App)
                {
                    targetListbox = lb_Apps;
                    targetTextblock = textblock_Category_Menu_Apps;
                }
                else if (listCategory == ListCategory.Game)
                {
                    targetListbox = lb_Games;
                    targetTextblock = textblock_Category_Menu_Games;
                }
                else if (listCategory == ListCategory.Emulator)
                {
                    targetListbox = lb_Emulators;
                    targetTextblock = textblock_Category_Menu_Emulators;
                }
                else if (listCategory == ListCategory.Launcher)
                {
                    targetListbox = lb_Launchers;
                    targetTextblock = textblock_Category_Menu_Launchers;
                }
                else if (listCategory == ListCategory.Shortcut)
                {
                    targetListbox = lb_Shortcuts;
                    targetTextblock = textblock_Category_Menu_Shortcuts;
                }
                else if (listCategory == ListCategory.Process)
                {
                    targetListbox = lb_Processes;
                    targetTextblock = textblock_Category_Menu_Processes;
                }
                else if (listCategory == ListCategory.Search)
                {
                    targetListbox = lb_Search;
                    targetTextblock = textblock_Category_Menu_Search;
                }

                //Show target listbox
                lb_Apps.Visibility = Visibility.Collapsed;
                lb_Games.Visibility = Visibility.Collapsed;
                lb_Emulators.Visibility = Visibility.Collapsed;
                lb_Launchers.Visibility = Visibility.Collapsed;
                lb_Shortcuts.Visibility = Visibility.Collapsed;
                lb_Processes.Visibility = Visibility.Collapsed;
                lb_Search.Visibility = Visibility.Collapsed;
                targetListbox.Visibility = Visibility.Visible;

                //Update button foreground
                textblock_Category_Menu_Apps.Style = (Style)Application.Current.Resources["TextBlockGrayLight"];
                textblock_Category_Menu_Games.Style = (Style)Application.Current.Resources["TextBlockGrayLight"];
                textblock_Category_Menu_Emulators.Style = (Style)Application.Current.Resources["TextBlockGrayLight"];
                textblock_Category_Menu_Launchers.Style = (Style)Application.Current.Resources["TextBlockGrayLight"];
                textblock_Category_Menu_Shortcuts.Style = (Style)Application.Current.Resources["TextBlockGrayLight"];
                textblock_Category_Menu_Processes.Style = (Style)Application.Current.Resources["TextBlockGrayLight"];
                textblock_Category_Menu_Search.Style = (Style)Application.Current.Resources["TextBlockGrayLight"];
                targetTextblock.Style = (Style)Application.Current.Resources["TextBlockWhiteLight"];

                //Update list category variable
                vCurrentListCategory = listCategory;

                //Update category list count
                CategoryListUpdateCount();

                //Show or hide search interface
                if (listCategory == ListCategory.Search)
                {
                    //Show search interface
                    stackpanel_Search_Interface.Visibility = Visibility.Visible;

                    //Focus on the interface
                    if (lb_Search.Items.Count > 0)
                    {
                        await ListBoxFocusIndex(lb_Search, false, 0, vProcessCurrent.WindowHandleMain);
                    }
                    else
                    {
                        await FocusElement(grid_Search_textbox, vProcessCurrent.WindowHandleMain);
                    }
                }
                else
                {
                    //Hide search interface
                    stackpanel_Search_Interface.Visibility = Visibility.Collapsed;

                    //Focus on the listbox
                    await ListBoxFocusIndex(targetListbox, false, 0, vProcessCurrent.WindowHandleMain);
                }
            }
            catch { }
        }
    }
}