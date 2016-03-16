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

namespace NNVotingController
{
    public partial class _02UseNeuralNetwork : UserControl
    {
        private string resultPath = Properties.Settings.Default.MainPath + Properties.Settings.Default.ResultPath;
        List<string> inputs;
        List<string> networks;

        public _02UseNeuralNetwork()
        {
            InitializeComponent();
            setPanelPositions();
            inputs = Directory.GetFiles(resultPath, "*.noncond", SearchOption.TopDirectoryOnly).ToList<string>();
            networks = Directory.GetFiles(resultPath, "*.network", SearchOption.TopDirectoryOnly).ToList<string>();
            for (int i = 0; i < inputs.Count; i++)
            {
                inputs[i] = Path.GetFileNameWithoutExtension(inputs[i]);    
            }
            for (int i = 0; i < networks.Count; i++)
            {
                networks[i] = Path.GetFileNameWithoutExtension(networks[i]);
            }
            listInputs.DataSource = inputs;
            listNetworks.DataSource = networks;
        }

        private void setPanelPositions()
        {
            panelInput.Top = 0;
            panelInput.Height = (int)Math.Round((this.Height - btnStart.Height) * 0.45, 0);
            panelInput.Left = 0;
            panelInput.Width = this.Width;
            panelNetwork.Top = panelInput.Height;
            panelNetwork.Height = (int)Math.Round((this.Height - btnStart.Height) * 0.45, 0);
            panelNetwork.Width = this.Width;
            panelNetwork.Left = 0;
        }

        private void _02UseNeuralNetwork_Resize(object sender, EventArgs e)
        {
            setPanelPositions();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            foreach (int inputID in listInputs.SelectedIndices)
            {
                foreach (int networkID in listNetworks.SelectedIndices)
                {
                    string args = "\"" + resultPath.Replace('\\', '/') + "/\"";
                    args += " " + inputs[inputID];
                    args += " " + networks[networkID];
                    Form1.runPythonCode("\"" + Properties.Settings.Default.MainPath + @"Neural Network - Evaluate\nn_evaluate.py" + "\"", args);
                }    
            }
            MessageBox.Show("Done");
        }
    }
}
