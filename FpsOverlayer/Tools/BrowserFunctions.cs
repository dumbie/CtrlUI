using Microsoft.Web.WebView2.Wpf;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using static ArnoldVinkCode.AVFunctions;
using static ArnoldVinkCode.AVSettings;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer.ToolsOverlay
{
    public partial class WindowTools : Window
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

                //Update window style (UIAccess render workaround)
                this.Topmost = false;
                await Task.Delay(500);

                //Set creation properties 
                CoreWebView2CreationProperties creationProperties = new CoreWebView2CreationProperties();
                creationProperties.UserDataFolder = "BrowserCache";
                creationProperties.AdditionalBrowserArguments = "--disable-gpu";

                //Create webviewer
                vBrowserWebView = new WebView2();
                vBrowserWebView.CreationProperties = creationProperties;
                vBrowserWebView.DefaultBackgroundColor = Color.Transparent;

                //Add webviewer to grid
                grid_Browser.Children.Clear();
                grid_Browser.Children.Add(vBrowserWebView);

                //Hide link hint grid
                grid_Browser_LinkHint.Visibility = Visibility.Collapsed;

                //Register webviewer events
                await vBrowserWebView.EnsureCoreWebView2Async();
                vBrowserWebView.CoreWebView2.SourceChanged += WebView2_SourceChanged;
                vBrowserWebView.CoreWebView2.NewWindowRequested += WebView2_NewWindowRequested;
                vBrowserWebView.CoreWebView2.DownloadStarting += WebView2_DownloadStarting;
                vBrowserWebView.CoreWebView2.NavigationStarting += WebView2_NavigationStarting;
                vBrowserWebView.CoreWebView2.NavigationCompleted += WebView2_NavigationCompleted;

                //Update window style (UIAccess render workaround)
                await Task.Delay(500);
                this.Topmost = true;

                Debug.WriteLine("Added browser to grid.");
            }
            catch (Exception ex)
            {
                //Remove browser from grid
                Browser_Remove_Grid("Error: " + ex.Message);
                Debug.WriteLine("Failed to add browser: " + ex.Message);
            }
        }

        //Remove browser from grid
        private void Browser_Remove_Grid(string errorMessage)
        {
            try
            {
                //Check unload setting
                if (!SettingLoad(vConfigurationFpsOverlayer, "BrowserUnload", typeof(bool)))
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
                Browser_Reset_Interface(errorMessage);

                Debug.WriteLine("Removed browser from grid.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to remove browser: " + ex.Message);
            }
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
                    linkString = StringLinkCleanup(linkString);
                    vBrowserWebView.CoreWebView2.Navigate(linkString);
                }

                if (closeLinkMenu)
                {
                    grid_Browser_Manage.Visibility = Visibility.Collapsed;
                }
            }
            catch { }
        }

        //Reset browser interface
        public void Browser_Reset_Interface(string browserError)
        {
            try
            {
                //Clear browser grid
                grid_Browser.Children.Clear();

                //Show link hint grid
                grid_Browser_LinkHint.Visibility = Visibility.Visible;

                //Reset current link
                textbox_Browser_Link.Text = string.Empty;

                //Set error text
                textblock_Browser_Error.Text = browserError;
            }
            catch { }
        }

        //Update browser opacity
        public async Task Browser_Update_Opacity()
        {
            try
            {
                //Get current background color
                string currentBackground = (await vBrowserWebView.CoreWebView2.ExecuteScriptAsync("window.getComputedStyle(document.body).backgroundColor")).Replace(" ", string.Empty).Trim();
                //Debug.WriteLine("Original background: " + currentBackground);

                //Convert background
                string colorRed = "0";
                string colorGreen = "0";
                string colorBlue = "0";
                string colorAlpha = "0";
                try
                {
                    int valueStart = currentBackground.IndexOf('(');
                    int valueEnd = currentBackground.IndexOf(')');
                    string[] values = currentBackground.Substring(valueStart + 1, valueEnd - valueStart - 1).Split(',');
                    colorRed = values[0];
                    colorGreen = values[1];
                    colorBlue = values[2];
                    colorAlpha = values[3];
                }
                catch { }

                //Check default background
                if (colorRed == "0" && colorGreen == "0" && colorBlue == "0" && colorAlpha == "0")
                {
                    colorRed = "255";
                    colorGreen = "255";
                    colorBlue = "255";
                    colorAlpha = "1.00";
                }

                //Update browser opacity
                string targetOpacity = SettingLoad(vConfigurationFpsOverlayer, "BrowserOpacity", typeof(string)).Replace(",", ".");
                string rgbaOpacity = "rgba(" + colorRed + "," + colorGreen + "," + colorBlue + "," + targetOpacity + ")";
                await vBrowserWebView.CoreWebView2.ExecuteScriptAsync("document.documentElement.style.setProperty('opacity', '" + targetOpacity + "', 'important');");
                await vBrowserWebView.CoreWebView2.ExecuteScriptAsync("document.documentElement.style.setProperty('background', '" + rgbaOpacity + "', 'important');");
                Debug.WriteLine("Changed browser background: " + rgbaOpacity);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to update browser opacity: " + ex.Message);
            }
        }
    }
}