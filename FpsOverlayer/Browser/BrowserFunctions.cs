using Microsoft.Web.WebView2.Wpf;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVFunctions;
using static ArnoldVinkCode.AVWindowFunctions;
using static FpsOverlayer.AppVariables;
using static LibraryShared.Settings;

namespace FpsOverlayer.OverlayCode
{
    public partial class WindowBrowser : Window
    {
        //Add browser to grid
        private async Task Browser_Add_Grid()
        {
            try
            {
                //Check webviewer
                if (vBrowserWebView != null)
                {
                    return;
                }

                //Update the window style (UIAccess render workaround)
                this.Topmost = false;
                await Task.Delay(500);

                //Set creation properties 
                CoreWebView2CreationProperties creationProperties = new CoreWebView2CreationProperties();
                creationProperties.UserDataFolder = "BrowserCache";
                creationProperties.AdditionalBrowserArguments = "--disable-gpu";

                //Create webviewer
                vBrowserWebView = new WebView2();
                vBrowserWebView.CreationProperties = creationProperties;

                //Add webviewer to grid
                grid_Browser.Children.Clear();
                grid_Browser.Children.Add(vBrowserWebView);

                //Hide link hint grid
                grid_Link_Hint.Visibility = Visibility.Collapsed;

                //Register webviewer events
                await vBrowserWebView.EnsureCoreWebView2Async();
                vBrowserWebView.CoreWebView2.SourceChanged += WebView2_SourceChanged;
                vBrowserWebView.CoreWebView2.NewWindowRequested += WebView2_NewWindowRequested;
                vBrowserWebView.CoreWebView2.DownloadStarting += WebView2_DownloadStarting;
                vBrowserWebView.CoreWebView2.NavigationStarting += WebView2_NavigationStarting;
                vBrowserWebView.CoreWebView2.NavigationCompleted += WebView2_NavigationCompleted;

                //Update the window style (UIAccess render workaround)
                await Task.Delay(500);
                this.Topmost = true;

                Debug.WriteLine("Added browser to grid.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to add browser: " + ex.Message);
            }
        }

        //Remove browser from grid
        private void Browser_Remove_Grid()
        {
            try
            {
                //Check unload setting
                if (!Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "BrowserUnload")))
                {
                    return;
                }

                //Dispose webviewer
                if (vBrowserWebView != null)
                {
                    vBrowserWebView.Dispose();
                    vBrowserWebView = null;
                }

                //Reset browser interface
                Browser_Reset_Interface();

                Debug.WriteLine("Removed browser from grid.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to remove browser: " + ex.Message);
            }
        }

        //Switch clickthrough mode
        public void Browser_Switch_Clickthrough(bool forceVisible)
        {
            try
            {
                if (forceVisible || vBrowserWindowClickThrough)
                {
                    //Show menu bar
                    grid_Menu.Visibility = Visibility.Visible;

                    //Update the window style
                    vBrowserWindowClickThrough = false;
                    WindowUpdateStyleVisible(vInteropWindowHandle, true, true, vBrowserWindowClickThrough);
                }
                else
                {
                    //Hide menu bar
                    grid_Menu.Visibility = Visibility.Collapsed;
                    grid_Link.Visibility = Visibility.Collapsed;

                    //Update the window style
                    vBrowserWindowClickThrough = true;
                    WindowUpdateStyleVisible(vInteropWindowHandle, true, true, vBrowserWindowClickThrough);
                }
            }
            catch { }
        }

        //Switch browser visibility
        public void Browser_Switch_Visibility()
        {
            try
            {
                if (vWindowVisible && vBrowserWindowClickThrough)
                {
                    Browser_Switch_Clickthrough(false);
                }
                else if (vWindowVisible)
                {
                    Hide();
                }
                else
                {
                    Show();
                }
            }
            catch { }
        }

        //Open link in browser
        private async void Browser_Open_Link(string linkString, bool closeLinkMenu)
        {
            try
            {
                //Add browser to grid
                await Browser_Add_Grid();

                //Check current link
                string currentLink = vBrowserWebView == null ? string.Empty : vBrowserWebView.Source.ToString();
                if (currentLink == linkString)
                {
                    Debug.WriteLine("Same link, reloading page.");
                    vBrowserWebView.Reload();
                }
                else
                {
                    linkString = StringLinkFixup(linkString);
                    vBrowserWebView.CoreWebView2.Navigate(linkString);
                }

                if (closeLinkMenu)
                {
                    grid_Link.Visibility = Visibility.Collapsed;
                }
            }
            catch { }
        }

        //Reset browser interface
        public void Browser_Reset_Interface()
        {
            try
            {
                //Clear browser grid
                grid_Browser.Children.Clear();

                //Show link hint grid
                grid_Link_Hint.Visibility = Visibility.Visible;

                //Reset current link
                textblock_Link.Text = "Current website link";
            }
            catch { }
        }
    }
}