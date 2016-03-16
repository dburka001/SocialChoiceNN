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
    public partial class _03GenerateMainInput : UserControl
    {
        private int inputSeed, nbrOfVoters, nbrOfNominees, nbrOfSamples, nbrOfExtraRows;
        private int[,] data, binaryData;
        private int nbrOfRankingColumns = 0;
        private int currentRow = 0;
        private bool allowDiffVoterOrder = true;
        private bool isBinary = true;

        Random rnd;

        enum PluralityType { Normal, Borda }

        public _03GenerateMainInput()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {            
            if (!getInputData()) return;
            rnd = new Random(inputSeed);
            createDataMatrix();
            fillColumns();
            saveFiles();
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
                inputSeed  = Convert.ToInt32(nudSeed.Value);
                nbrOfVoters = Convert.ToInt32(nudVoters.Value);
                nbrOfNominees = Convert.ToInt32(nudNominees.Value);
                nbrOfSamples = Convert.ToInt32(nudSamples.Value);
                nbrOfExtraRows = Convert.ToInt32(nudRepetition.Value);
                allowDiffVoterOrder = chkDiffVoterOrder.Checked;
                if (rdbBinary.Checked || rdbSum.Checked) isBinary = true; else isBinary = false;
                /*
                bool isThereSelectedType = false;
                foreach (CheckBox chkb in panelType.Controls.OfType<CheckBox>())
                {
                    if (chkb.Checked) isThereSelectedType = true;
                }
                if (!isThereSelectedType)
                {
                    MessageBox.Show("At least one result type has to be selected!");
                    return false;
                }
                */
                return true;
            }
            catch (Exception)
            {
                MessageBox.Show("Error in input data");
                return false;
            }
        }

        #region Generate input data
        private void createDataMatrix()
        {
            data = new int[nbrOfSamples * (nbrOfExtraRows + 1), nbrOfVoters * nbrOfNominees];
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
                    int[,] availOrders = SupportFunction.GetPermutations(nbrOfNominees);
                    int[] voterPreferenceDistributon = SupportFunction.CreatePreferenceDistribution(availOrders.GetLength(0), nbrOfVoters, rnd);
                    orderArray = SupportFunction.CreateOrderArray(availOrders, voterPreferenceDistributon, RandomOrder(nbrOfVoters));
                }

                // Fill current row                
                int[] order = null;
                for (int currentVoter = 0; currentVoter < nbrOfVoters; currentVoter++)
                {
                    if (!allowDiffVoterOrder) order = orderArray[currentVoter];
                    fillRankingColumns(nbrOfNominees * currentVoter, order);
                }
                currentRow++;

                // Add last row again with differenet sorting of voters
                if(allowDiffVoterOrder) AddExtraRows();
            }
        }
        private void fillRankingColumns(int startCol, int[] order)
        {
            if (allowDiffVoterOrder)
            {
                order = RandomOrder(nbrOfNominees);
            }

            for (int counter = 0; counter < nbrOfNominees; counter++)
            {
                data[currentRow, startCol + counter] = order[counter] + 1;
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
                currentRow++;
                nbrOfSamples++;
            }
        }
        #endregion

        #region Calculate results
        private int[] calculateResult(int rowID)
        {
            decimal[] rowResult = new decimal[nbrOfNominees];
            for (int i = 0; i < rowResult.Length; i++)
            {
                rowResult[i] = 0;
            }
            List<int[]> resultList = new List<int[]>();
            if (chkCondorcet.Checked) resultList.Add(getCondorcetResult(rowID));
            if (chkBorda.Checked) resultList.Add(getBordaResult(rowID));
            if (chkPlurality.Checked) resultList.Add(getPluralityResult(rowID));
            recodeRow(rowID);
            // Check results
            if (resultList.Count == 0) return null;
            foreach (int[] currentResult in resultList)
            {
                if (currentResult == null) return null;
                for (int i = 0; i < rowResult.Length; i++)
                {
                    rowResult[i] += (decimal)currentResult[i];
                }                
            }
            for (int i = 0; i < rowResult.Length; i++)
            {
                rowResult[i] /= (decimal)resultList.Count;
            }
            int[] rowResultInt = new int[rowResult.Length];            
            for (int i = 0; i < rowResult.Length; i++)
            {
                if ((rowResult[i] % 1) == 0) // Checking if it is an integer
                    rowResultInt[i] = (int)rowResult[i];
                else
                    return null;
            }
            if (rowResultInt.Sum() == 1)
                return rowResultInt;
            else return null;
        }
        private int[] getCondorcetResult(int rowID)
        {
            int[,] matrixResult = new int[nbrOfNominees, nbrOfNominees];
            int[] calculatedResult = new int[nbrOfNominees];

            for (int matrixRow = 0; matrixRow < nbrOfNominees; matrixRow++)
            {
                for (int matrixCol = matrixRow; matrixCol < nbrOfNominees; matrixCol++)
                {
                    if (matrixCol == matrixRow)
                        matrixResult[matrixRow, matrixCol] = 1;
                    else
                    {
                        int currentResult = 0;
                        for (int col = 0; col < data.GetLength(1); col += nbrOfNominees)
                        {
                            if (data[rowID, col + matrixRow] > data[rowID, col + matrixCol])
                                currentResult++;
                            else
                                currentResult--;
                        }
                        if (currentResult > 0)
                            currentResult = 1;
                        if (currentResult < 0)
                            currentResult = -1;
                        matrixResult[matrixRow, matrixCol] = currentResult;
                        matrixResult[matrixCol, matrixRow] = -currentResult;
                    }
                }
            }

            for (int currentNominee = 0; currentNominee < nbrOfNominees; currentNominee++)
            {
                int sum = 0;
                for (int matrixCol = 0; matrixCol < nbrOfNominees; matrixCol++)
                {
                    sum += matrixResult[currentNominee, matrixCol];
                }
                if (sum == nbrOfNominees) // Current nominee beat everyone               
                    calculatedResult[currentNominee] = 1;
                else
                    calculatedResult[currentNominee] = 0;
            }

            if (calculatedResult.Sum() == 1)
                return calculatedResult;
            else
                return null;
        }
        private int[] getBordaResult(int rowID)
        {
            return getPluralityTypeResult(rowID, PluralityType.Borda);
        }
        private int[] getPluralityResult(int rowID)
        {
            return getPluralityTypeResult(rowID, PluralityType.Normal);
        }
        private int[] getPluralityTypeResult(int rowID, PluralityType ptype)
        {
            // Getting counts
            int[] winCounts = new int[nbrOfNominees];
            for (int winId = 0; winId < winCounts.Length; winId++)
            {
                winCounts[winId] = 0;
            }
            for (int col = 0; col < data.GetLength(1); col += nbrOfNominees)
            {
                for (int winId = 0; winId < winCounts.Length; winId++)
                {
                    if (ptype == PluralityType.Borda) winCounts[winId] += data[rowID, col + winId];
                    if (ptype == PluralityType.Normal && data[rowID, col + winId] == nbrOfNominees) 
                        winCounts[winId] += 1;                    
                }
            }
            // Determine if there is a tie
            int maxWin = winCounts.Max();
            int[] winners = new int[winCounts.Length];
            for (int winId = 0; winId < winners.Length; winId++)
            {
                if (winCounts[winId] == maxWin)
                    winners[winId] = 1;
                else
                    winners[winId] = 0;
            }
            return winners;
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
        private void saveFiles()
        {
            StreamWriter currentWriter;
            StreamWriter swCondorcet;
            StreamWriter swNonCondorcet;

            try
            {
                string filename = Properties.Settings.Default.MainPath + Properties.Settings.Default.ResultPath;
                filename += timeStamp()
                    + nbrOfNominees.ToString().PadLeft(2, '0') + "jelolt_" 
                    + nbrOfVoters.ToString().PadLeft(2, '0') + "szavazo_" 
                    + nbrOfSamples.ToString() + "minta_" 
                    + inputSeed.ToString() + "seed"
                    + txtDescription.Text;

                swCondorcet = new StreamWriter(filename + "." + nbrOfNominees.ToString(), false, Encoding.Default);
                swNonCondorcet = new StreamWriter(filename + ".noncond", false, Encoding.Default);

                for (int row = 0; row < data.GetLength(0); row++)
                {
                    int[] currentResult = calculateResult(row);
                    if (currentResult != null) currentWriter = swCondorcet; else currentWriter = swNonCondorcet;
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
                    if (currentResult != null)
                    for (int col = 0; col < currentResult.Length; col++)
                    {
                        currentWriter.Write(";");
                        currentWriter.Write(currentResult[col].ToString());
                    }
                    currentWriter.Write("\n");
                }

                swCondorcet.Close();
                swNonCondorcet.Close();

                if (rdbSum.Checked)
                {
                    SupportFunction.ConvertToSum(filename, "." + nbrOfNominees.ToString(), nbrOfNominees, nbrOfVoters);
                    SupportFunction.ConvertToSum(filename, ".noncond", nbrOfNominees, nbrOfVoters);
                }
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
