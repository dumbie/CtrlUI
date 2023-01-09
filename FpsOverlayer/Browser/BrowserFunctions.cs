using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
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
                //Dispose webviewer
                if (vWebViewer != null)
                {
                    vWebViewer.Dispose();
                }

                //Create webviewer
                vWebViewer = new WebView2();

                //Add webviewer to grid
                grid_Browser.Children.Clear();
                grid_Browser.Children.Add(vWebViewer);

                //Set software render mode
                string userDataFolder = "BrowserCache";
                CoreWebView2EnvironmentOptions environmentOptions = new CoreWebView2EnvironmentOptions { AdditionalBrowserArguments = "--disable-gpu" };
                CoreWebView2Environment environment = await CoreWebView2Environment.CreateAsync(null, userDataFolder, environmentOptions);

                //Register webviewer events
                await vWebViewer.EnsureCoreWebView2Async(environment);
                vWebViewer.SourceChanged += WebView2_SourceChanged;
                vWebViewer.CoreWebView2.NewWindowRequested += WebView2_NewWindowRequested;

                //Set default link source
                string defaultLink = Convert.ToString(Setting_Load(vConfigurationFpsOverlayer, "BrowserDefaultLink"));
                vWebViewer.Source = new Uri(defaultLink);

                Debug.WriteLine("Added browser to grid.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to add browser: " + ex.Message);
            }
        }

        //Switch clickthrough mode
        public async Task Browser_Switch_Clickthrough()
        {
            try
            {
                if (vBrowserClickThrough)
                {
                    //Hide bottom bar
                    grid_Bottom.Visibility = Visibility.Visible;

                    //Update the window style
                    await UpdateWindowStyleVisible(false);

                    vBrowserClickThrough = false;
                }
                else
                {
                    //Show bottom bar
                    grid_Bottom.Visibility = Visibility.Collapsed;

                    //Update the window style
                    await UpdateWindowStyleVisible(true);

                    vBrowserClickThrough = true;
                }
            }
            catch { }
        }

        //Switch browser visibility
        public async Task SwitchBrowserVisibility()
        {
            try
            {
                if (vWindowVisible && vBrowserClickThrough)
                {
                    await Browser_Switch_Clickthrough();
                }
                else if (vWindowVisible)
                {
                    await Browser_Close();
                }
                else
                {
                    await Show();
                }
            }
            catch { }
        }

        //Close the browser
        public async Task Browser_Close()
        {
            try
            {
                //Fix Setting unload
                //Unload loaded website
                //vWebViewer.Source = new Uri("about:blank");
                vWebViewer.Dispose();

                //Close the browser overlay
                await Hide();
            }
            catch { }
        }
    }
}