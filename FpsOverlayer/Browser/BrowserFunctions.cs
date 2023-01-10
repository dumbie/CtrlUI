using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVWindowFunctions;
using static FpsOverlayer.AppVariables;
using static LibraryShared.Settings;

namespace FpsOverlayer.OverlayCode
{
    public partial class WindowBrowser : Window
    {
        //Set default browser values
        private async Task Browser_Setup()
        {
            try
            {
                //Register webviewer events
                if (!vBrowserInitialized)
                {
                    await webview_Browser.EnsureCoreWebView2Async();
                    webview_Browser.CoreWebView2.SourceChanged += WebView2_SourceChanged;
                    webview_Browser.CoreWebView2.NewWindowRequested += WebView2_NewWindowRequested;
                    webview_Browser.CoreWebView2.DownloadStarting += WebView2_DownloadStarting;
                    webview_Browser.CoreWebView2.NavigationStarting += WebView2_NavigationStarting;
                    webview_Browser.CoreWebView2.NavigationCompleted += WebView2_NavigationCompleted;
                    vBrowserInitialized = true;
                }

                //Set default link source
                string currentLink = webview_Browser.Source.ToString();
                if (currentLink == "about:blank")
                {
                    string defaultLink = Convert.ToString(Setting_Load(vConfigurationFpsOverlayer, "BrowserDefaultLink"));
                    //Browser_Open_Link(defaultLink);
                }

                Debug.WriteLine("Set default browser values.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to default browser: " + ex.Message);
            }
        }

        //Reset browser default values
        private void Browser_Unload()
        {
            try
            {
                //Check unload setting
                if (!Convert.ToBoolean(Setting_Load(vConfigurationFpsOverlayer, "BrowserUnload")))
                {
                    return;
                }

                //Set blank page
                Browser_Open_Link("about:blank");

                Debug.WriteLine("Reset browser default values.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to reset browser: " + ex.Message);
            }
        }

        //Switch clickthrough mode
        public async Task Browser_Switch_Clickthrough(bool forceVisible)
        {
            try
            {
                if (forceVisible || vBrowserWindowClickThrough)
                {
                    //Show menu bar
                    grid_Menu.Visibility = Visibility.Visible;

                    //Update the window style
                    vBrowserWindowClickThrough = false;
                    await UpdateWindowStyleVisible(vInteropWindowHandle, true, vBrowserWindowNoActivate, vBrowserWindowClickThrough);
                }
                else
                {
                    //Hide menu bar
                    grid_Menu.Visibility = Visibility.Collapsed;
                    grid_Link.Visibility = Visibility.Collapsed;

                    //Update the window style
                    vBrowserWindowClickThrough = true;
                    await UpdateWindowStyleVisible(vInteropWindowHandle, true, vBrowserWindowNoActivate, vBrowserWindowClickThrough);
                }
            }
            catch { }
        }

        //Switch browser visibility
        public async Task SwitchBrowserVisibility()
        {
            try
            {
                if (vWindowVisible && vBrowserWindowClickThrough)
                {
                    await Browser_Switch_Clickthrough(false);
                }
                else if (vWindowVisible)
                {
                    await Hide();
                }
                else
                {
                    await Show();
                }
            }
            catch { }
        }

        //Open link in browser
        private void Browser_Open_Link(string linkString)
        {
            try
            {
                string currentLink = webview_Browser.Source.ToString();
                if (currentLink == linkString)
                {
                    Debug.WriteLine("Same link, reloading page.");
                    webview_Browser.Reload();
                }
                else
                {
                    if (!linkString.Contains(":"))
                    {
                        linkString = "https://" + linkString;
                    }
                    linkString = linkString.Replace(",", ".");
                    webview_Browser.CoreWebView2.Navigate(linkString);
                }
            }
            catch { }
        }
    }
}