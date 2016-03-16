using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NNVotingController
{
    partial class Form1
    {
        public static void runPythonCode(string cmd, string args)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.FileName = Properties.Settings.Default.PythonPath;
            start.Arguments = string.Format("{0} {1}", cmd, args);
            start.UseShellExecute = false;
            start.RedirectStandardOutput = true;
            try
            {
                using (Process process = Process.Start(start))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        Console.Write(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error with Python start!\n" + 
                    Properties.Settings.Default.PythonPath + "\n" +
                    Properties.Settings.Default.MainPath + "\n" +
                    ex.Message);
            }
        }
    }
}
