using ArnoldVinkCode;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CtrlUI
{
    partial class WindowMain
    {
        //Filter the file name
        public string FileFilterName(string nameFile, bool removeExtension, bool removeSpaces, int takeWords)
        {
            try
            {
                //Remove file extension
                if (removeExtension)
                {
                    nameFile = Path.GetFileNameWithoutExtension(nameFile);
                }

                //Remove invalid characters
                nameFile = string.Join(string.Empty, nameFile.Split(Path.GetInvalidFileNameChars()));

                //Lowercase the rom name
                nameFile = nameFile.ToLower();

                //Remove symbols with text
                nameFile = Regex.Replace(nameFile, @"\((.*?)\)+", string.Empty);
                nameFile = Regex.Replace(nameFile, @"\{(.*?)\}+", string.Empty);
                nameFile = Regex.Replace(nameFile, @"\[(.*?)\]+", string.Empty);

                //Replace characters
                nameFile = nameFile.Replace("'", " ").Replace(".", " ").Replace(",", " ").Replace("-", " ").Replace("_", " ");

                //Replace double spaces
                nameFile = Regex.Replace(nameFile, @"\s+", " ");

                //Remove words
                string[] nameFilterRemoveContains = { "usa", "eur", "pal", "ntsc", "repack", "proper" };
                string[] nameRomSplit = nameFile.Split(' ').Where(x => !nameFilterRemoveContains.Any(x.Contains)).ToArray();

                //Take words
                if (takeWords <= 0)
                {
                    nameFile = string.Join(" ", nameRomSplit);
                }
                else
                {
                    nameFile = string.Join(" ", nameRomSplit.Take(takeWords));
                }

                //Remove spaces
                if (removeSpaces)
                {
                    nameFile = nameFile.Replace(" ", string.Empty);
                }
                else
                {
                    nameFile = AVFunctions.StringRemoveStart(nameFile, " ");
                    nameFile = AVFunctions.StringRemoveEnd(nameFile, " ");
                }
            }
            catch { }
            return nameFile;
        }
    }
}