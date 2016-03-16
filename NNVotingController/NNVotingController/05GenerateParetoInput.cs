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
    public partial class _05GenerateParetoInput : UserControl
    { 
        private int inputSeed, nbrOfVoters, nbrOfNominees, nbrOfSamples, nbrOfExtraRows;
        private int[,] data, result, binaryData;
        private int nbrOfRankingColumns = 0;
        private int currentRow = 0;
        private bool allowDiffVoterOrder = true;
        private bool isBinary = true;

        Random rnd;

        public _05GenerateParetoInput()
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
            int currentDominated = -1;

            while (currentRow < nbrOfSamples)
            {
                do
                {
                    // Set row winner
                    currentDominated = rnd.Next(0, nbrOfNominees);
                    int currentDominant = rnd.Next(0, nbrOfNominees - 1);
                    if (currentDominant >= currentDominated) currentDominant++;
                    // Fill current row
                    for (int currentVoter = 0; currentVoter < nbrOfVoters; currentVoter++)
                    {
                        fillRankingColumns(nbrOfNominees * currentVoter, currentDominated, currentDominant);
                    }                                  
                    int oldDominated = currentDominated;
                    do
                    {
                        oldDominated = currentDominated;
                        currentDominated = getDominated(currentDominated, currentDominant);
                    } while (oldDominated != currentDominated && currentDominated != -1);                    
                    // Result                    
                    for (int col = 0; col < result.GetLength(1); col++)
                    {
                        if (col == currentDominated)
                            result[currentRow, col] = 1;
                        else
                            result[currentRow, col] = 0;
                    }
                } while (currentDominated < 0 || !(allowDiffVoterOrder || orderIsOkay()));                                        
                
                recodeRow(currentRow);
                currentRow++;

                // Add last row again with differenet sorting of voters
                if (allowDiffVoterOrder) AddExtraRows();
            }
        }
        private void fillRankingColumns(int startCol, int currentDominated, int currentDominant)
        {
            int[] order = RandomOrder(nbrOfNominees - 2);           

            // Set Values
            int currentDominatedValue = rnd.Next(1, nbrOfNominees);
            int currentDominantValue = rnd.Next(currentDominatedValue + 1, nbrOfNominees + 1);
            for (int i = 0; i < order.Length; i++)
            {
                order[i]++;
                if (order[i] >= currentDominatedValue) order[i]++;
                if (order[i] >= currentDominantValue) order[i]++;
            }
            // Data
            for (int counter = 0; counter < nbrOfNominees; counter++)
            {
                if (counter == currentDominated) data[currentRow, startCol + counter] = currentDominatedValue;
                else
                {
                    if (counter == currentDominant) data[currentRow, startCol + counter] = currentDominantValue;
                    else
                    {
                        if (counter < currentDominated && counter < currentDominant)
                            data[currentRow, startCol + counter] = order[counter];
                        if ((counter < currentDominated && counter >= currentDominant) || (counter >= currentDominated && counter < currentDominant))
                            data[currentRow, startCol + counter] = order[counter - 1];
                        if (counter >= currentDominated && counter >= currentDominant)
                            data[currentRow, startCol + counter] = order[counter - 2];
                    }
                }
            }            
        }
        private int getDominated(int currentDominated, int currentDominant)
        {
            int resultDominated = currentDominated;
            // Returns -1 if no clear dominated exists
            for (int i = 0; i < nbrOfNominees; i++)
            {
                for (int j = 0; j < nbrOfNominees; j++)
                {
                    if (i != j && checkDominance(i, j))
                    {
                        if (currentDominated != j)
                        {
                            if (!checkDominance(j, currentDominated))                                
                            {
                                if (checkDominance(currentDominated, j)) 
                                    resultDominated = j;
                                else
                                    resultDominated = -1;
                            }
                        }
                    }
                }    
            }
            return resultDominated;
        }
        private bool checkDominance(int a, int b)
        {
            // returns true if a dominates b
            bool aIsDominant = true;
            for (int currentVoter = 0; currentVoter < nbrOfVoters; currentVoter++)
            {
                int currenStartCol = currentVoter * nbrOfNominees;
                if (data[currentRow, currenStartCol + a] < data[currentRow, currenStartCol + b])
                    aIsDominant = false;
            }
            return aIsDominant;
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
        private bool orderIsOkay()
        {
            int[,] availOrders = SupportFunction.GetPermutations(nbrOfNominees);
            int[] voterPreferenceDistributon = new int[availOrders.GetLength(0)];
            for (int prefID = 0; prefID < voterPreferenceDistributon.Length; prefID++)
            {
                voterPreferenceDistributon[prefID] = 0;
            }                        
            // Loop through current row, and count the different preference orders
            int[][] currentOrderArray = new int[nbrOfVoters][];
            for (int voterID = 0; voterID < nbrOfVoters; voterID++)
            {
                // Create Order Array from current Row
                currentOrderArray[voterID] = new int[nbrOfNominees];
                for (int nomineeID = 0; nomineeID < nbrOfNominees; nomineeID++)
                {
                    currentOrderArray[voterID][nomineeID] = data[currentRow, nbrOfNominees * voterID + nomineeID];    
                }
                // Find the ID of the matching order                
                for (int orderID = 0; orderID < availOrders.GetLength(0); orderID++)
                {                    
                    bool isMatch = true;
                    for (int nomineeID = 0; nomineeID < nbrOfNominees; nomineeID++)
                    {
                        if (currentOrderArray[voterID][nomineeID] != availOrders[orderID, nomineeID])
                        {
                            isMatch = false;
                            break;
                        }
                    }
                    if (isMatch)
                    {
                        voterPreferenceDistributon[orderID]++;
                        break;
                    }
                }
            }
            // Get the allowed order array
            int[][] allowedOrderArray = SupportFunction.CreateOrderArray(availOrders, voterPreferenceDistributon, RandomOrder(nbrOfVoters));
            // Check whether the two order array match
            bool isOkay = true;
            for (int orderID = 0; orderID < allowedOrderArray.Length; orderID++)
            {
                for (int nomineeID = 0; nomineeID < allowedOrderArray[orderID].Length; nomineeID++)
                {
                    if (allowedOrderArray[orderID][nomineeID] != currentOrderArray[orderID][nomineeID] - 1)
                    {
                        isOkay = false;
                        break;
                    }
                }
            }
            return isOkay;
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
