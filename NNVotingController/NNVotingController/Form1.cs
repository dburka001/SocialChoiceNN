using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NNVotingController
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            /*
            int[,] a = SupportFunction.GetPermutations(3);
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    Console.Write(a[i,j].ToString() + "\t");
                }
                Console.Write("\n");
            }
            */
            

            string mainPath = GetParentDirectory(Application.StartupPath, 4) + "\\";
            StreamReader sr = new StreamReader(mainPath + "pythonpath.config", Encoding.Default);
            string resultPath = sr.ReadLine();
            sr.Close();
            Properties.Settings.Default.MainPath = mainPath;
            Properties.Settings.Default.PythonPath = resultPath;
            Properties.Settings.Default.Save();
            CreateButtons();
        }

        public string GetParentDirectory(string path, int parentCount)
        {
            if (string.IsNullOrEmpty(path) || parentCount < 1)
                return path;

            string parent = System.IO.Path.GetDirectoryName(path);

            if (--parentCount > 0)
                return GetParentDirectory(parent, parentCount);

            return parent;
        }        

        private void NNTrainClick(object sender, EventArgs e)
        {            
            setUCposition(new _01TrainNeuralNetwork());
        }

        private void NNUseClick(object sender, EventArgs e)
        {
            setUCposition(new _02UseNeuralNetwork());
        }

        private void MainInputClick(object sender, EventArgs e)
        {
            setUCposition(new _03GenerateMainInput());
        }

        private void UnanimityClick(object sender, EventArgs e)
        {
            setUCposition(new _04GenerateUnanimityInput());
        }

        private void ParetoClick(object sender, EventArgs e)
        {
            setUCposition(new _05GenerateParetoInput());
        }

        private void ResultsXLSClick(object sender, EventArgs e)
        {
            setUCposition(new _06ResultsXLS());
        }

        private void setUCposition(UserControl uc)
        {
            mainPanel.Controls.Clear();
            mainPanel.Controls.Add(uc);
            uc.Dock = DockStyle.Fill;            
        }
    }
}
