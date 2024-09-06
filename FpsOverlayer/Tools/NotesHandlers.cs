using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer.ToolsOverlay
{
    public partial class WindowTools : Window
    {
        //Handle starting point
        private void button_Notes_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    vWindowMousePoint = Mouse.GetPosition(null);
                    vWindowMargin = border_Notes.Margin;
                    vWindowWidth = border_Notes.ActualWidth;
                    vWindowHeight = border_Notes.ActualHeight;
                }
            }
            catch { }
        }

        //Handle resize window
        private void button_NotesResize_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Point mouseOffset = Mouse.GetPosition(null);
                    double differenceX = mouseOffset.X - vWindowMousePoint.X;
                    double differenceY = mouseOffset.Y - vWindowMousePoint.Y;

                    double newWidth = vWindowWidth + differenceX;
                    double newHeight = vWindowHeight + differenceY;
                    if (newWidth > border_Notes.MinWidth)
                    {
                        border_Notes.Width = newWidth;
                    }
                    if (newHeight > border_Notes.MinHeight)
                    {
                        border_Notes.Height = newHeight;
                    }
                }
            }
            catch { }
        }

        //Handle move window
        private void button_NotesMove_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Point mouseOffset = Mouse.GetPosition(null);
                    double differenceX = mouseOffset.X - vWindowMousePoint.X;
                    double differenceY = mouseOffset.Y - vWindowMousePoint.Y;

                    Thickness newMargin = vWindowMargin;
                    newMargin.Left += differenceX;
                    newMargin.Top += differenceY;
                    border_Notes.Margin = newMargin;
                }
            }
            catch { }
        }

        //Show or hide manage menu
        private void button_Notes_Manage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (grid_Notes_Manage.Visibility == Visibility.Visible)
                {
                    grid_Notes_Manage.Visibility = Visibility.Collapsed;
                }
                else
                {
                    grid_Notes_Manage.Visibility = Visibility.Visible;
                }
            }
            catch { }
        }

        //Remove note
        private void button_Notes_Remove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string fileName = textbox_Notes_Name.Text;
                string fileNameFilter = fileName.ToLower().Replace(" ", "");
                string filePath = "Notes\\" + fileName + ".txt";

                //Check file name
                if (fileNameFilter == "default")
                {
                    Debug.WriteLine("Default note cannot be deleted.");
                    textbox_Notes_Name.BorderBrush = (SolidColorBrush)Application.Current.Resources["ApplicationInvalidBrush"];
                    return;
                }

                //Delete file
                File.Delete(filePath);

                //Reload combobox
                LoadNotesList(string.Empty);

                //Update status
                textbox_Notes_Name.BorderBrush = (SolidColorBrush)Application.Current.Resources["ApplicationValidBrush"];
                Debug.WriteLine("Removed note: " + filePath);
            }
            catch (Exception ex)
            {
                textbox_Notes_Name.BorderBrush = (SolidColorBrush)Application.Current.Resources["ApplicationInvalidBrush"];
                Debug.WriteLine("Removing note failed: " + ex.Message);
            }
        }

        //Add note
        private void button_Notes_Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string fileName = textbox_Notes_Name.Text;
                string filePath = "Notes\\" + fileName + ".txt";

                //Check if note exists
                if (File.Exists(filePath))
                {
                    Debug.WriteLine("Note already exists: " + fileName);
                    textbox_Notes_Name.BorderBrush = (SolidColorBrush)Application.Current.Resources["ApplicationInvalidBrush"];
                    return;
                }

                //Create note file
                File.WriteAllText(filePath, string.Empty);

                //Reload combobox
                LoadNotesList(fileName);

                //Update status
                textbox_Notes_Name.BorderBrush = (SolidColorBrush)Application.Current.Resources["ApplicationValidBrush"];
                Debug.WriteLine("Added note: " + filePath);
            }
            catch (Exception ex)
            {
                textbox_Notes_Name.BorderBrush = (SolidColorBrush)Application.Current.Resources["ApplicationInvalidBrush"];
                Debug.WriteLine("Adding note failed: " + ex.Message);
            }
        }

        //Save note
        private void textbox_Notes_Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string fileName = textbox_Notes_Name.Text;
                string filePath = "Notes\\" + fileName + ".txt";

                //Save text to file
                File.WriteAllText(filePath, textbox_Notes_Text.Text);

                //Update status
                textbox_Notes_Name.BorderBrush = (SolidColorBrush)Application.Current.Resources["ApplicationValidBrush"];
                Debug.WriteLine("Saved note: " + filePath);
            }
            catch (Exception ex)
            {
                try { textbox_Notes_Name.BorderBrush = (SolidColorBrush)Application.Current.Resources["ApplicationInvalidBrush"]; } catch { }
                Debug.WriteLine("Saving note failed: " + ex.Message);
            }
        }

        //Rename note
        private void button_Notes_Rename_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string fileNameOld = combobox_Notes_Select.SelectedItem.ToString();
                string fileNameNew = textbox_Notes_Name.Text;
                string fileNameFilter = fileNameOld.ToLower().Replace(" ", "");

                //Check file name
                if (fileNameFilter == "default")
                {
                    Debug.WriteLine("Cannot rename default note.");
                    textbox_Notes_Name.BorderBrush = (SolidColorBrush)Application.Current.Resources["ApplicationInvalidBrush"];
                    return;
                }
                else if (fileNameOld == fileNameNew)
                {
                    Debug.WriteLine("Note name has not changed.");
                    textbox_Notes_Name.BorderBrush = (SolidColorBrush)Application.Current.Resources["ApplicationInvalidBrush"];
                    return;
                }

                //Rename file name
                string filePathOld = "Notes\\" + fileNameOld + ".txt";
                string filePathNew = "Notes\\" + fileNameNew + ".txt";
                File.Move(filePathOld, filePathNew);

                //Reload combobox
                LoadNotesList(fileNameNew);

                //Update status
                textbox_Notes_Name.BorderBrush = (SolidColorBrush)Application.Current.Resources["ApplicationValidBrush"];
                Debug.WriteLine("Renamed note: " + filePathOld + " to " + filePathNew);
            }
            catch (Exception ex)
            {
                textbox_Notes_Name.BorderBrush = (SolidColorBrush)Application.Current.Resources["ApplicationInvalidBrush"];
                Debug.WriteLine("Renaming note failed: " + ex.Message);
            }
        }

        //Load note
        private void combobox_Notes_Select_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string noteName = combobox_Notes_Select.SelectedItem.ToString();
                string filePath = "Notes\\" + noteName + ".txt";
                string noteString = File.ReadAllText(filePath);

                //Update textbox text
                textbox_Notes_Name.Text = noteName;
                textbox_Notes_Text.Text = noteString;

                //Update status
                textbox_Notes_Name.BorderBrush = (SolidColorBrush)Application.Current.Resources["ApplicationValidBrush"];
                Debug.WriteLine("Loaded note: " + filePath);
            }
            catch (Exception ex)
            {
                textbox_Notes_Name.BorderBrush = (SolidColorBrush)Application.Current.Resources["ApplicationInvalidBrush"];
                Debug.WriteLine("Loading note failed: " + ex.Message);
            }
        }
    }
}