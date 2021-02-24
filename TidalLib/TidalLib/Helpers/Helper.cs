using System;
using System.IO;
using System.Text;

namespace TidalLib.Helpers
{
    public static class Helper
    {
        /// <summary>
        /// Reads all text from a file without requesting exclusive access from the OS. Used instead of File.ReadAllText() to be able to read a file, which another program is currently interacting with.
        /// </summary>
        public static ReadOnlySpan<char> ReadFile(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs, Encoding.Default))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
