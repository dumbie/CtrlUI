using ArnoldVinkCode;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using static FpsOverlayer.AppVariables;

namespace FpsOverlayer.ToolsOverlay
{
    public partial class WindowTools : Window
    {
        //Load all notes to list
        private void LoadNotesList(string selectNoteName)
        {
            try
            {
                //Clear notes
                vNotesFiles.Clear();

                //Load notes
                List<string> noteFiles = AVFiles.GetFilesLevel("Notes", "*", 0);
                foreach (string fileName in noteFiles)
                {
                    string noteName = Path.GetFileNameWithoutExtension(fileName);
                    vNotesFiles.Add(noteName);
                }

                //Select note
                if (string.IsNullOrWhiteSpace(selectNoteName))
                {
                    combobox_Notes_Select.SelectedIndex = 0;
                }
                else
                {
                    combobox_Notes_Select.SelectedItem = selectNoteName;
                }
            }
            catch { }
        }
    }
}