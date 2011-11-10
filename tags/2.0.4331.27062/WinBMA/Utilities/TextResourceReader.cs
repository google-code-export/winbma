using System;
using System.IO;
using System.Windows.Resources;

namespace WinBMA.Utilities
{
    public static class TextResourceReader
    {
        public static string GetFromResources(string path)
        {
            return GetFromResources(new Uri(path, UriKind.Relative));
        }

        public static string GetFromResources(Uri path)
        {
            StreamResourceInfo stream = App.GetResourceStream(path);

            using (StreamReader reader = new StreamReader(stream.Stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}