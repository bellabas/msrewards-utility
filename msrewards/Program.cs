using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace msrewards
{
    class Program
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        static void Main(string[] args)
        {
            string url = "https://www.bing.com/search?q=";
            string[] words = new string[]
            {
                "reddit", "facebook", "vscode", "instagram", "amd", "twitter", "google", "youtube", "office", "microsoft",
                "gmail", "xbox", "playstation", "nvidia", "cat", "twitch", "amazon", "netflix", "batman", "ebay",
                "steam", "android", "samsung", "github", "outlook", "gog", "epic", "messenger", "japan", "msi", "intel", "ibm", "dotnet"
            };


            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.txt");
            if (files.Length != 0)
            {
                words = File.ReadAllText(files[0]).Split(' ');
            }

            // checks if we have an opened msedge process if not set to an instance
            Process proc = Process.GetProcessesByName("msedge").FirstOrDefault();
            if (proc == null)
            {
                proc = Process.Start("msedge");
                Thread.Sleep(3000);
            }

            int i = 0;
            while (!proc.HasExited && i < words.Length)
            {
                Process.Start(url + words[i]);
                Thread.Sleep(3000);
                if (!proc.HasExited)
                {
                    foreach (var p in Process.GetProcessesByName("msedge"))
                    {
                        // set to topmost window before sending keys
                        SetForegroundWindow(p.MainWindowHandle);
                        // topmost - nosize - nomove
                        SetWindowPos(p.MainWindowHandle, new IntPtr(-1), 0, 0, 0, 0, 0x0002 | 0x0001);
                    }
                    SendKeys.SendWait("^(w)");
                }
                i++;
            }

            // set msedge to top but no topmost (back to default mode)
            if (!proc.HasExited)
            {
                foreach (var p in Process.GetProcessesByName("msedge"))
                {
                    // nontopmost
                    SetWindowPos(p.MainWindowHandle, new IntPtr(-2), 0, 0, 0, 0, 0x0002 | 0x0001);
                }
            }
        }
    }
}
