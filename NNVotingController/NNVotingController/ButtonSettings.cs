using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NNVotingController
{
    partial class Form1
    {
        private int nbrOfButtons = 6;
        public Button[] controlButtons;

        private void setButtonPositions()
        {
            for (int currentButton = 0; currentButton < controlButtons.Length; currentButton++)
            {
                controlButtons[currentButton].Width = buttonPanel.Width;
                controlButtons[currentButton].Height = (int)Math.Floor((double)(buttonPanel.Height / nbrOfButtons));
                controlButtons[currentButton].Top = controlButtons[currentButton].Height * currentButton;
            }
        }

        private void buttonPanel_Resize(object sender, EventArgs e)
        {
            setButtonPositions();
        }

        private void CreateButtons()
        {
            controlButtons = new Button[nbrOfButtons];
            for (int currentButton = 0; currentButton < controlButtons.Length; currentButton++)
            {
                controlButtons[currentButton] = new Button();
                buttonPanel.Controls.Add(controlButtons[currentButton]);
            }
            setButtonPositions();
            controlButtons[0].Text = "Train Neural Network";
            controlButtons[0].Click += NNTrainClick;
            controlButtons[1].Text = "Use Neural Network";
            controlButtons[1].Click += NNUseClick;
            controlButtons[2].Text = "Generate Main Input";
            controlButtons[2].Click += MainInputClick;
            controlButtons[3].Text = "Generate Unanimity Input";
            controlButtons[3].Click += UnanimityClick;      
            controlButtons[4].Text = "Generate Pareto Input";
            controlButtons[4].Click += ParetoClick;
            controlButtons[5].Text = "Create XLS files from results";
            controlButtons[5].Click += ResultsXLSClick;                  
        }
    }
}
