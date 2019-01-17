using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Beacon
{
    class TextOutput
    {
        static string Path = Properties.Settings.Default.Path;

        static internal void NewText(Data d)
        {
            string textToWrite = $"From:{d.Sender} To:{d.Receiver} At: {d.Submission} | {d.Message}";
            string path = Path + $"\\MessageID{d.Stamp}.txt";
            System.IO.File.WriteAllText(path, textToWrite);
        }
    }
}
