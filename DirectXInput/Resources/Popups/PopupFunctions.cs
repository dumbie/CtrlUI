using System.Threading.Tasks;
using System.Windows;
using static DirectXInput.AppVariables;

namespace DirectXInput
{
    partial class WindowMain
    {
        //Show and close Messagebox Popup
        async Task<int> MessageBoxPopup(string Question, string Description, string Answer1, string Answer2, string Answer3, string Answer4)
        {
            try
            {
                //Set messagebox question content
                grid_MessageBox_Text.Text = Question;
                if (!string.IsNullOrWhiteSpace(Description))
                {
                    grid_MessageBox_Description.Text = Description;
                    grid_MessageBox_Description.Visibility = Visibility.Visible;
                }
                else
                {
                    grid_MessageBox_Description.Text = string.Empty;
                    grid_MessageBox_Description.Visibility = Visibility.Collapsed;
                }
                if (!string.IsNullOrWhiteSpace(Answer1))
                {
                    grid_MessageBox_Btn1.Content = Answer1;
                    grid_MessageBox_Btn1.Visibility = Visibility.Visible;
                }
                else
                {
                    grid_MessageBox_Btn1.Content = string.Empty;
                    grid_MessageBox_Btn1.Visibility = Visibility.Collapsed;
                }
                if (!string.IsNullOrWhiteSpace(Answer2))
                {
                    grid_MessageBox_Btn2.Content = Answer2;
                    grid_MessageBox_Btn2.Visibility = Visibility.Visible;
                }
                else
                {
                    grid_MessageBox_Btn2.Content = string.Empty;
                    grid_MessageBox_Btn2.Visibility = Visibility.Collapsed;
                }
                if (!string.IsNullOrWhiteSpace(Answer3))
                {
                    grid_MessageBox_Btn3.Content = Answer3;
                    grid_MessageBox_Btn3.Visibility = Visibility.Visible;
                }
                else
                {
                    grid_MessageBox_Btn3.Content = string.Empty;
                    grid_MessageBox_Btn3.Visibility = Visibility.Collapsed;
                }
                if (!string.IsNullOrWhiteSpace(Answer4))
                {
                    grid_MessageBox_Btn4.Content = Answer4;
                    grid_MessageBox_Btn4.Visibility = Visibility.Visible;
                }
                else
                {
                    grid_MessageBox_Btn4.Content = string.Empty;
                    grid_MessageBox_Btn4.Visibility = Visibility.Collapsed;
                }

                //Reset messagebox variables
                vMessageBoxPopupResult = 0;
                vMessageBoxPopupCancelled = false;
                vMessageBoxOpen = true;

                //Show the popup
                UpdateElementVisibility(grid_Message_MessageBox, true);
                UpdateElementEnabled(grid_Message_MessageBox, true);

                //Hide the background
                UpdateElementOpacity(grid_Main, 0.08);
                UpdateElementEnabled(grid_Main, false);

                //Wait for user messagebox input
                while (vMessageBoxPopupResult == 0 && !vMessageBoxPopupCancelled) { await Task.Delay(500); }
                if (vMessageBoxPopupCancelled) { return 0; }

                //Close and reset messageboxpopup
                CloseMessageBoxPopup();
            }
            catch { }
            return vMessageBoxPopupResult;
        }

        //Close and reset messageboxpopup
        void CloseMessageBoxPopup()
        {
            try
            {
                //Reset messagebox variables
                vMessageBoxPopupCancelled = true;
                vMessageBoxOpen = false;

                //Hide the popup
                UpdateElementVisibility(grid_Message_MessageBox, false);
                UpdateElementEnabled(grid_Message_MessageBox, false);

                //Show the background
                UpdateElementOpacity(grid_Main, 1);
                UpdateElementEnabled(grid_Main, true);
            }
            catch { }
        }
    }
}