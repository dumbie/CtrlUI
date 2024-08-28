﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        //Remove note
        private void button_Notes_Remove_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            }
            catch { }
        }

        //Add note
        private void button_Notes_Add_Click(object sender, RoutedEventArgs e)
        {
            try
            {
            }
            catch { }
        }

        //Save note
        private void textbox_Notes_Text_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
            }
            catch { }
        }

        //Rename note
        private void textbox_Notes_Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
            }
            catch { }
        }

        private void combobox_Notes_Select_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
            }
            catch { }
        }
    }
}