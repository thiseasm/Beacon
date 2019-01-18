using System.IO;
using System;

namespace Beacon
{
    class TextOutput
    {
        static string Path = Properties.Settings.Default.Path;

        static internal void NewText(Data d)
        {
            string textToWrite = $"From:{d.Sender} To:{d.Receiver} At: {d.Submission} | {d.Message}";
            string path = Path + $"\\MessageID{d.Stamp}.txt";
            File.WriteAllText(path, textToWrite);
        }

        static internal void EditText(Data d)
        {
            string textToOverwrite = $"From:{d.Sender} To:{d.Receiver} At: {d.Submission} | {d.Message} *MESSAGE EDITED AT {DateTime.UtcNow}*";
            string path = Path + $"\\MessageID{d.Stamp}.txt";
            File.WriteAllText(path, textToOverwrite);
        }
    }
}
