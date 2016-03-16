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
    public partial class _04GenerateUnanimityInput : UserControl
    {
        private int inputSeed, nbrOfVoters, nbrOfNominees, nbrOfSamples, nbrOfExtraRows;
        private int[,] data, result, binaryData;
        private int nbrOfRankingColumns = 0;
        private int currentRow = 0;
        private bool allowDiffVoterOrder = true;
        private bool isBinary = true;

        Random rnd;

        public _04GenerateUnanimityInput()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!getInputData()) return;
            rnd = new Random(inputSeed);
            createDataMatrix();
            fillColumns();
            if (rdbBoth.Checked)
            {
                saveFiles(nbrOfNominees.ToString());
                saveFiles("noncond");
            }
            else if (rdbTraining.Checked) saveFiles(nbrOfNominees.ToString());
            else if (rdbInput.Checked) saveFiles("noncond");
            lblDone.Visible = true;
        }
        private void InputValueChanged(object sender, EventArgs e)
        {
            lblDone.Visible = false;
            nudRepetition.Enabled = chkDiffVoterOrder.Checked;
        }
        private bool getInputData()
        {
            try
            {
                inputSeed = Convert.ToInt32(nudSeed.Value);
                nbrOfVoters = Convert.ToInt32(nudVoters.Value);
                nbrOfNominees = Convert.ToInt32(nudNominees.Value);
                nbrOfSamples = Convert.ToInt32(nudSamples.Value);
                nbrOfExtraRows = Convert.ToInt32(nudRepetition.Value);
                allowDiffVoterOrder = chkDiffVoterOrder.Checked;
                if (rdbBinary.Checked || rdbSum.Checked) isBinary = true; else isBinary = false;
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("Error in input data");
                return false;
            }
        }

        #region Generate data
        private void createDataMatrix()
        {
            data = new int[nbrOfSamples * (nbrOfExtraRows + 1), nbrOfVoters * nbrOfNominees];
            result = new int[data.GetLength(0), nbrOfNominees];
            if (isBinary)
            {
                nbrOfRankingColumns = nbrOfNominees * (nbrOfNominees - 1) / 2;
                binaryData = new int[nbrOfSamples * (nbrOfExtraRows + 1), nbrOfVoters * nbrOfRankingColumns];
            }
        }
        private void fillColumns()
        {
            currentRow = 0;

            while (currentRow < nbrOfSamples)
            {
                int[][] orderArray = null;
                if (!allowDiffVoterOrder)
                {
                    int[,] availOrders = SupportFunction.GetPermutations(nbrOfNominees - 1);
                    int[] voterPreferenceDistributon = SupportFunction.CreatePreferenceDistribution(availOrders.GetLength(0), nbrOfVoters, rnd);
                    orderArray = SupportFunction.CreateOrderArray(availOrders, voterPreferenceDistributon, RandomOrder(nbrOfVoters));
                }

                // Set row winner
                int currentWinner = rnd.Next(0, nbrOfNominees);
                // Fill current row
                int[] order = null;
                for (int currentVoter = 0; currentVoter < nbrOfVoters; currentVoter++)
                {
                    if (!allowDiffVoterOrder) order = orderArray[currentVoter];
                    fillRankingColumns(nbrOfNominees * currentVoter, currentWinner, order);
                }
                recodeRow(currentRow);
                currentRow++;

                // Add last row again with differenet sorting of voters
                if (allowDiffVoterOrder) AddExtraRows();
            }
        }
        private void fillRankingColumns(int startCol, int currentWinner, int[] order)
        {            
            if (allowDiffVoterOrder)
            {
                order = RandomOrder(nbrOfNominees - 1);
            }

            // Data
            for (int counter = 0; counter < nbrOfNominees; counter++)
            {
                if (counter == currentWinner) data[currentRow, startCol + counter] = nbrOfNominees;
                else
                {
                    if (counter < currentWinner) 
                        data[currentRow, startCol + counter] = order[counter] + 1;
                    else
                        data[currentRow, startCol + counter] = order[counter - 1] + 1;
                }
            }
            // Result
            for (int col = 0; col < result.GetLength(1); col++)
            {
                if (col == currentWinner)
                    result[currentRow, col] = 1;
                else
                    result[currentRow, col] = 0;
            }
        }
        private int[] RandomOrder(int rndLength)
        {
            int[] order = new int[rndLength];
            for (int i = 0; i < order.Length; i++) { order[i] = i; }

            //Random order
            int x = 0, y = 0;
            for (int i = 0; i < order.Length; i++)
            {
                y = rnd.Next(i, order.Length);
                x = order[i];
                order[i] = order[y];
                order[y] = x;
            }

            return order;
        }
        private void AddExtraRows()
        {
            int originalRow = currentRow - 1;

            for (int i = 0; i < nbrOfExtraRows; i++)
            {
                int[] order = RandomOrder(nbrOfVoters);

                for (int voterID = 0; voterID < order.Length; voterID++)
                {
                    for (int columnID = 0; columnID < nbrOfNominees; columnID++)
                    {
                        data[currentRow, (voterID * nbrOfNominees) + columnID] =
                            data[originalRow, (order[voterID] * nbrOfNominees) + columnID];
                    }
                }
                for (int col = 0; col < result.GetLength(1); col++)
                {
                    result[currentRow, col] = result[originalRow, col];
                }
                currentRow++;
                nbrOfSamples++;
            }
        }
        private void recodeRow(int rowID)
        {
            if (rdbBorda.Checked) return;
            for (int col = 0; col < data.GetLength(1); col++)
            {
                if (rdbNormal.Checked) data[rowID, col] =
                    nbrOfNominees + 1 - data[rowID, col];
                if (rdbAttila.Checked) data[rowID, col] =
                    Convert.ToInt32(Math.Ceiling((double)nbrOfNominees / 2) + Math.Pow(-1, nbrOfNominees - data[rowID, col] + 1) * Math.Ceiling((double)(nbrOfNominees - data[rowID, col]) / 2));
            }
            if (isBinary) binaryRecode(rowID);
        }
        private void binaryRecode(int rowID)
        {
            //A>B, A>C, ... A>Y, B>C, B>D...., X>Y
            for (int currentVoter = 0; currentVoter < nbrOfVoters; currentVoter++)
            {
                int startCol = nbrOfRankingColumns * currentVoter;
                int[] order = new int[nbrOfNominees];
                for (int currentNominee = 0; currentNominee < nbrOfNominees; currentNominee++)
                {
                    order[currentNominee] = data[rowID, nbrOfNominees * currentVoter + currentNominee] - 1;
                }
                //Order matrix
                int[,] orderMatrix = new int[nbrOfNominees, nbrOfNominees];
                for (int row = 0; row < orderMatrix.GetLength(0); row++)
                {
                    for (int col = 0; col < orderMatrix.GetLength(1); col++)
                    {
                        if (order[row] > order[col])
                            orderMatrix[row, col] = 1;
                        else
                            orderMatrix[row, col] = 0;
                    }
                }
                //Set Ranking Columns
                int counter = 0;
                for (int row = 0; row < orderMatrix.GetLength(0); row++)
                {
                    for (int col = row + 1; col < orderMatrix.GetLength(1); col++)
                    {
                        binaryData[rowID, startCol + counter] = orderMatrix[row, col];
                        counter++;
                    }
                }
            }
        }
        #endregion

        #region Save files
        private void saveFiles(string extension)
        {
            StreamWriter currentWriter;

            try
            {
                string filename = Properties.Settings.Default.MainPath + Properties.Settings.Default.ResultPath;
                filename += timeStamp()
                    + nbrOfNominees.ToString().PadLeft(2, '0') + "jelolt_"
                    + nbrOfVoters.ToString().PadLeft(2, '0') + "szavazo_"
                    + nbrOfSamples.ToString() + "minta_"
                    + inputSeed.ToString() + "seed"
                    + txtDescription.Text;

                currentWriter = new StreamWriter(filename + "." + extension, false, Encoding.Default);                

                for (int row = 0; row < data.GetLength(0); row++)
                {
                    int currentNbrOfColumns;
                    if (!isBinary)
                        currentNbrOfColumns = data.GetLength(1);
                    else
                        currentNbrOfColumns = binaryData.GetLength(1);
                    for (int col = 0; col < currentNbrOfColumns; col++)
                    {
                        if (col != 0) currentWriter.Write(";");
                        if (!isBinary)
                            currentWriter.Write(data[row, col].ToString());
                        else
                            currentWriter.Write(binaryData[row, col].ToString());
                    }
                    if (extension != "noncond")
                    for (int col = 0; col < result.GetLength(1); col++)
                    {
                        currentWriter.Write(";" + result[row, col].ToString());
                    }                    
                    currentWriter.Write("\n");
                }

                currentWriter.Close();
                if (rdbSum.Checked) { SupportFunction.ConvertToSum(filename, "." + extension, nbrOfNominees, nbrOfVoters); }
            }
            catch (Exception)
            {
                MessageBox.Show("Error with saving the files!");
            }
        }
        private string timeStamp()
        {
            string timeStamp = "";
            if (chkTimeStamp.Checked)
            {
                timeStamp += DateTime.Now.ToShortDateString().Replace(".", "");
                timeStamp += DateTime.Now.ToLongTimeString().Replace(":", "").PadLeft(6, '0');
                timeStamp += "_";
            }
            return timeStamp;
        }
        #endregion
    }
}
