using System.Windows;
using System.Windows.Controls;
using static LibraryShared.Enums;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Handle genre menu keyboard/controller tapped
        async void Button_GenreMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button senderFramework = (Button)sender;
                if (senderFramework.Name == "button_GenreMenu_Games") { await ChangeGenreListBox(AppCategory.Game); }
                else if (senderFramework.Name == "button_GenreMenu_Apps") { await ChangeGenreListBox(AppCategory.App); }
                else if (senderFramework.Name == "button_GenreMenu_Emulators") { await ChangeGenreListBox(AppCategory.Emulator); }
                else if (senderFramework.Name == "button_GenreMenu_Launchers") { await ChangeGenreListBox(AppCategory.Launcher); }
                else if (senderFramework.Name == "button_GenreMenu_Shortcuts") { await ChangeGenreListBox(AppCategory.Shortcut); }
                else if (senderFramework.Name == "button_GenreMenu_Processes") { await ChangeGenreListBox(AppCategory.Process); }
                else if (senderFramework.Name == "button_GenreMenu_Search") { await ChangeGenreListBox(AppCategory.Search); }
            }
            catch { }
        }
    }
}