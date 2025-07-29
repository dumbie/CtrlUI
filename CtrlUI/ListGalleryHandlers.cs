using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using static ArnoldVinkStyles.AVImage;
using static CtrlUI.AppVariables;
using static LibraryShared.Classes;

namespace CtrlUI
{
    partial class WindowMain
    {
        async Task RightClickGallery(ListView listboxSender, int listboxSelectedIndex, DataBindApp dataBindApp)
        {
            try
            {
                Debug.WriteLine("Right clicked gallery: " + dataBindApp.Name + " from: " + listboxSender.Name);

                List<DataBindString> Answers = new List<DataBindString>();

                DataBindString AnswerRemove = new DataBindString();
                AnswerRemove.ImageBitmap = await FileToBitmapImage(new AVImageFile()
                {
                    FilePaths = ["Assets/Default/Icons/Remove.png"],
                    BackupPath = vImageBackupSource,
                    Dispatcher = this.Dispatcher
                });
                AnswerRemove.Name = "Remove the media file";
                Answers.Add(AnswerRemove);

                //Get media information
                string mediaInformation = dataBindApp.PathGallery;

                DataBindString messageResult = await Popup_Show_MessageBox("What would you like to do with " + dataBindApp.Name + "?", mediaInformation, "", Answers);
                if (messageResult != null)
                {
                    if (messageResult == AnswerRemove)
                    {
                        await List_FileRemove_Prompt(listboxSender, listboxSelectedIndex, dataBindApp);
                    }
                }
            }
            catch { }
        }
    }
}