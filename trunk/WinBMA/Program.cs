using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Configuration;

namespace WinBMA
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Settings.Load();
            }
            catch (Exception)
            {
                Settings.LoadDefaults();
            }

            Application.Run(new MainForm());

            Settings.Save();
        }


		#region "Properties"

		public static string AppPath {
			get { return System.IO.Path.GetDirectoryName(Program.ExePath); }
		}


		public static string ExePath {
			get { return System.Reflection.Assembly.GetEntryAssembly().Location; }
		}

		public static bool InIDE {
			get { return Debugger.IsAttached; }
		}

		#endregion

		#region "Internal Methods"

		static internal void KillSelf()
		{
			try {
				Process myProcess = Process.GetCurrentProcess();
				myProcess.Kill();
			} catch (Exception) {
			}
		}

		#endregion
    }
}