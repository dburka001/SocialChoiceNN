using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace NNVotingController
{
    public partial class _01TrainNeuralNetwork : UserControl
    {
        private string resultPath = Properties.Settings.Default.MainPath + Properties.Settings.Default.ResultPath;
        List<NeuralNetworkInput> nninput = new List<NeuralNetworkInput>();
        List<string> inputs = new List<string>();

        public _01TrainNeuralNetwork()
        {
            InitializeComponent();
            List<string> tempInputs = Directory.GetFiles(resultPath, "*", SearchOption.TopDirectoryOnly).ToList<string>();
            foreach (string fileName in tempInputs)
            {
                Regex rex = new Regex(@"\.[1-9][0-9]*$");
                if (rex.IsMatch(fileName)) inputs.Add(fileName);
            }
            for (int i = 0; i < inputs.Count; i++)
            {
                inputs[i] = Path.GetFileName(inputs[i]);
            }
            listTraining.DataSource = inputs;
        }

        private struct NeuralNetworkInput
        {
            public string Path;
            public string FileBase;
            public int NbrOfNominees;
            public int Seed;

            public NeuralNetworkInput(string Path, string FileBase, int NbrOfNominees, int Seed)
	        {
                this.Path = Path;
                this.FileBase = FileBase;
                this.NbrOfNominees = NbrOfNominees;
                this.Seed = Seed;
	        }
        }

        private void createInputList(string fileName)
        {
            int seedFrom = (int)nudSeedFrom.Value;
            int seedTo = (int)nudSeedTo.Value;
            int nbrOfNominees = Convert.ToInt32(Path.GetExtension(fileName).TrimStart('.'));         
            string fileBase = Path.GetFileNameWithoutExtension(fileName);
            string path = resultPath.Replace('\\', '/');
            for (int i = seedFrom; i <= seedTo; i++)
            {
                nninput.Add(new NeuralNetworkInput(path, fileBase, nbrOfNominees, i));
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            nninput.Clear();
            foreach (string fileName in listTraining.SelectedItems)
            {
                createInputList(fileName);    
            }
            foreach (NeuralNetworkInput nni in nninput)
            {
                string args = "\"" + nni.Path + "\"";
                args += " " + nni.FileBase;
                args += " " + nni.NbrOfNominees.ToString();
                args += " " + nni.Seed.ToString();
                Console.WriteLine(args);
                Form1.runPythonCode("\"" + Properties.Settings.Default.MainPath + @"Neural Network - Training\nn_training.py" + "\"", args);
            }
            MessageBox.Show("Done");
        }
    }
}
