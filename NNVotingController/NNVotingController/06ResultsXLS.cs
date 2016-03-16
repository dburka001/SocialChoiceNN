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
using Excel = Microsoft.Office.Interop.Excel;
using VBIDE = Microsoft.Vbe.Interop;
using System.Diagnostics; 

namespace NNVotingController
{
    public partial class _06ResultsXLS : UserControl
    {     
        int nbrOfVotingRules;
        int nbrOfNominees;
        int nbrOfVoters;
        int[,] permutations;
        Excel.Application xlApp;
        Excel.Workbook templateWorkBook;
        Excel.Workbook outputWorkBook;
        Excel.Sheets worksheets;
        object misValue = System.Reflection.Missing.Value;
        string resultPath = Properties.Settings.Default.MainPath + Properties.Settings.Default.ResultPath;
        List<string> resultString;
        DataTable results = new DataTable();        

        public _06ResultsXLS()
        {
            InitializeComponent();
            resultString = Directory.GetFiles(resultPath, "*.result", SearchOption.TopDirectoryOnly).ToList<string>();
            results.Columns.Add("FileName", typeof(string));
            for (int i = 0; i < resultString.Count; i++)
            {
                results.Rows.Add(Path.GetFileNameWithoutExtension(resultString[i]));
            }                                    
            listResults.DataSource = results;
            listResults.DisplayMember = "FileName";
        }

        #region Setup
        private void listResults_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblDone.Visible = false;
        }
        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            DataView dw = results.DefaultView;
            dw.RowFilter = string.Format("FileName Like '%{0}%'", txtFilter.Text);
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            lblDone.Hide();
            string outputType = getOutputType();
            if (checkOutputType(outputType)) return;
            nbrOfNominees = Convert.ToInt32(outputType.Substring(0,2));
            nbrOfVoters = Convert.ToInt32(outputType.Substring(outputType.IndexOf("szavazo") - 2,2));
            nbrOfVotingRules = 10 + nbrOfNominees - 3;
            xlApp = new Excel.Application();            
            if (xlApp == null)
            {
                MessageBox.Show("Excel is not properly installed!!");
                return;
            }
            xlApp.DisplayAlerts = false;
            try
            {
                Excel.Worksheet currentTemplate = getCurrentTemplate(outputType);
                if (currentTemplate != null)
                {
                    createWorkBook();                    
                    int counter = 1;
                    foreach (DataRowView drw in listResults.SelectedItems)
                    {
                        double[,] data = getCurrentData(drw[0].ToString() + ".result");
                        createNewSheet(currentTemplate, data, counter, drw[0].ToString() + ".result");
                        counter++;
                    }
                    RunMacro(outputWorkBook);
                    outputWorkBook.SaveAs(resultPath + outputType + txtDescription.Text + ".xlsm", Excel.XlFileFormat.xlOpenXMLWorkbookMacroEnabled, misValue, misValue, misValue, misValue, Excel.XlSaveAsAccessMode.xlExclusive, misValue, misValue, misValue, misValue, misValue);
                    outputWorkBook.Close(true, misValue, misValue);
                    templateWorkBook.Close(false, misValue, misValue);                    
                    releaseObject(currentTemplate);
                    releaseObject(templateWorkBook);
                }
            }
            catch (Exception ex)
            {
                // Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(st.FrameCount - 1);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();
                MessageBox.Show("Line Number: " + line.ToString() + " \n" + ex.Message);                
            }
            finally
            {                
                xlApp.Quit();
                releaseObject(worksheets);
                releaseObject(outputWorkBook);
                releaseObject(xlApp);
            }
            lblDone.Visible = true;
        }
        private string getOutputType()
        {
            string currentType = ((DataRowView)listResults.SelectedItems[0])[0].ToString();
            currentType = currentType.Substring(0, currentType.IndexOf("szavazo") + 7);                   
            return currentType;
        }
        private bool checkOutputType(string currentType)
        {
            foreach (DataRowView drw in listResults.SelectedItems)
            {
                if (!drw[0].ToString().StartsWith(currentType))
                {
                    MessageBox.Show("Egyszerre csak egy adott jelölt- és szavazószám kombináció adható meg!");
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Excel
        private void createNewSheet(Excel.Worksheet template, double[,] data, int counter, string fileName)
        {
            string templateRange = "A1:EX10100";
            int cellRow = 2;
            int cellCol = 1;
            Excel.Worksheet sheet;
            if (counter == 1)
            {
                sheet = (Excel.Worksheet)outputWorkBook.Worksheets.get_Item(counter);             
            }
            else
            {
                sheet = (Excel.Worksheet)worksheets.Add(Type.Missing, worksheets[counter - 1], Type.Missing, Type.Missing);                
            }
            sheet.Name = counter.ToString();
            template.get_Range(templateRange).Copy(sheet.get_Range(templateRange));
            sheet.Cells[1, 1].Value = fileName;
            var startCell = sheet.Cells[cellRow, cellCol];
            var endCell = sheet.Cells[cellRow + data.GetLength(0) - 1, cellCol + data.GetLength(1) - 1];
            var writeRange = (Excel.Range)sheet.Range[startCell, endCell];
            writeRange.Value = data;            
            releaseObject(sheet);
        }
        private void createWorkBook()
        {
            outputWorkBook = xlApp.Workbooks.Add(misValue);
            worksheets = outputWorkBook.Worksheets;
            worksheets[1].Delete();
            worksheets[2].Delete();            
        }
        private Excel.Worksheet getCurrentTemplate(string currentType)
        {
            templateWorkBook = xlApp.Workbooks.Open(Properties.Settings.Default.MainPath + "Templates\\" + "Templates.xlsm", 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0); ;
            Excel.Worksheet currentSheet = null;

            foreach (Excel.Worksheet sheet in templateWorkBook.Worksheets)
            {
                if (sheet.Name == currentType)
                {
                    currentSheet = sheet;
                    break;
                }
            }

            return currentSheet;
        }
        private void RunMacro(Excel.Workbook wb)
        {
            VBIDE.VBComponent module = wb.VBProject.VBComponents.Add(VBIDE.vbext_ComponentType.vbext_ct_StdModule);
            module.CodeModule.AddFromString(getMacro());
            wb.Application.Run("jelolt" + nbrOfNominees.ToString(), misValue, misValue, misValue,
                             misValue, misValue, misValue, misValue,
                             misValue, misValue, misValue, misValue,
                             misValue, misValue, misValue, misValue,
                             misValue, misValue, misValue, misValue,
                             misValue, misValue, misValue, misValue,
                             misValue, misValue, misValue, misValue,
                             misValue, misValue, misValue);
        }
        private string getMacro()
        {           
            string macroString = "";

            StreamReader sr = new StreamReader(Properties.Settings.Default.MainPath + "Templates\\macro.txt", Encoding.Default);

            while (!sr.EndOfStream)
            {
                macroString += sr.ReadLine() + "\n";
            }

            sr.Close();

            return macroString;
        }
        private void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
        #endregion

        #region Data
        double[][,] comparsionMatrices;
        // ONLY WORKS FOR BINARY CODING!!!!!!!!!!!!!!!!!!
        private double[,] getCurrentData(string fileName)
        {
            Console.WriteLine(fileName);

            StreamReader sr = new StreamReader(resultPath + fileName, Encoding.Default);
            StreamReader srSum = null;

            if (fileName.Contains("_SUM"))
            {
                string sumFileName = fileName.Substring(0, fileName.IndexOf("_SUM")) + ".noncond";
                srSum = new StreamReader(resultPath + sumFileName, Encoding.Default);
            }

            int rows = 0;
            int cols = (nbrOfVoters * nbrOfNominees * (nbrOfNominees - 1) / 2) + nbrOfNominees;
            while (!sr.EndOfStream) { string s = sr.ReadLine(); rows++; }
            sr.BaseStream.Position = 0;
            sr.DiscardBufferedData();

            rows = Math.Min(1000, rows);
            double[,] data = new double[rows, cols + nbrOfNominees * nbrOfVotingRules];
            permutations = SupportFunction.GetPermutations(nbrOfNominees);

            int currentRow = 0;
            while (!sr.EndOfStream)
            {
                if (currentRow == rows) break;
                int currentCol = cols;                
                double[] currentValues = new double[data.GetLength(1)];
                string[] currentLine = sr.ReadLine().Split(';');
                for (int i = 0; i < currentLine.Length; i++)
                {
                    currentValues[cols - currentLine.Length + i] = Convert.ToDouble(currentLine[i]);
                }
                if (srSum != null)
                {
                    currentLine = srSum.ReadLine().Split(';');
                    for (int i = 0; i < currentLine.Length; i++)
                    {
                        currentValues[i] = Convert.ToDouble(currentLine[i]);
                    }
                }
                
                createComparsionMatrices(currentValues);
                getCondrocet(ref currentValues, currentCol); currentCol += nbrOfNominees;
                getCopeland(ref currentValues, currentCol); currentCol += nbrOfNominees;
                getBorda(ref currentValues, currentCol); currentCol += nbrOfNominees;
                getPlurality(ref currentValues, currentCol); currentCol += nbrOfNominees;
                getKemeny(ref currentValues, currentCol); currentCol += nbrOfNominees * 2;        
                getPluralityRunOff(ref currentValues, currentCol); currentCol += nbrOfNominees;
                getNApproval(ref currentValues, ref currentCol);
                getUnanimity(ref currentValues, currentCol); currentCol += nbrOfNominees;
                getPareto(ref currentValues, currentCol); currentCol += nbrOfNominees;                
                
                for (int i = 0; i < currentValues.Length; i++)
                {
                    data[currentRow, i] = currentValues[i];
                }
                currentRow++;
            }

            sr.Close();
            if (srSum != null) srSum.Close();
            
            //return data;

            double[,] newData = new double[rows, nbrOfNominees * (nbrOfVotingRules + 1)];

            for (int i = 0; i < newData.GetLength(0); i++)
            {
                for (int j = 0; j < newData.GetLength(1); j++)
                {
                    newData[i, j] = data[i, j + cols - nbrOfNominees];
                }
            }

            return newData;
        }        
        // Voting Rules
        private void getCondrocet(ref double[] currentValues, int startCol)
        {
            for (int nomineeID = 0; nomineeID < nbrOfNominees; nomineeID++)
            {
                bool isCondorcet = true;
                for (int i = 0; i < nbrOfNominees; i++)
                {
                    if (i == nomineeID) continue;
                    double currentSum = 0;
                    for (int voterID = 0; voterID < nbrOfVoters; voterID++)
                    {
                        currentSum += comparsionMatrices[voterID][nomineeID, i];
                    }
                    if (currentSum <= nbrOfVoters / 2)
                    {
                        isCondorcet = false;
                    }
                }
                if (isCondorcet) currentValues[startCol + nomineeID] = 1;
                else currentValues[startCol + nomineeID] = 0;                
            }
        }
        private void getCopeland(ref double[] currentValues,  int startCol)
        {
            //Summed values of the comparsionmatrices
            double[,] sumValues = getComparsionSum();
            
            // Copeland Vaues
            double[] copelandValues = new double[nbrOfNominees];
            for (int i = 0; i < nbrOfNominees; i++)
            {
                for (int j = i + 1; j < nbrOfNominees; j++)
                {
                    if (sumValues[i, j] > sumValues[j, i])
                    {
                        copelandValues[i]++;
                        copelandValues[j]--;
                    }
                    if (sumValues[i, j] < sumValues[j, i])
                    {
                        copelandValues[i]--;
                        copelandValues[j]++;
                    }
                }
            }

            // Fidning Copeland winner
            int[] currentOrder = getOrder(copelandValues);
            for (int i = 0; i < copelandValues.Length; i++)
            {                
                currentValues[startCol + i] = currentOrder[i];
            }
        }
        private void getBorda(ref double[] currentValues, int startCol)
        {
            // Getting the Borda values            
            double[] bordaValues = new double[nbrOfNominees];
            for (int voterID = 0; voterID < nbrOfVoters; voterID++)
            {
                for (int i = 0; i < nbrOfNominees; i++)
                {
                    bordaValues[i]++;
                    for (int j = 0; j < nbrOfNominees; j++)
                    {
                        bordaValues[i] += comparsionMatrices[voterID][i, j];
                    }
                }
            }

            // Fidning Borda winner                        
            int[] currentOrder = getOrder(bordaValues);
            for (int i = 0; i < bordaValues.Length; i++)
            {
                currentValues[startCol + i] = currentOrder[i];
            }
        }
        private void getPlurality(ref double[] currentValues, int startCol)
        {
            // Getting the Plurality values
            double[] pluralityValues = getPluralityValues(1);

            // Fidning Plurality winner            
            int[] currentOrder = getOrder(pluralityValues);
            for (int i = 0; i < pluralityValues.Length; i++)
            {                
                currentValues[startCol + i] = currentOrder[i];
            }
        }
        private void getKemeny(ref double[] currentValues, int startCol)
        {
            //Summed values of the comparsionmatrices
            double[,] sumValues = getComparsionSum();

            //Matrices for calculating Kemeny Values
            double[][,] kemenyMatrices = new double[permutations.GetLength(0)][,];
            for (int permID = 0; permID < kemenyMatrices.Length; permID++)
            {
                kemenyMatrices[permID] = new double[nbrOfNominees, nbrOfNominees];
                for (int i = 0; i < permutations.GetLength(1); i++)
                {
                    for (int j = i + 1; j < permutations.GetLength(1); j++)
                    {
                        kemenyMatrices[permID][permutations[permID, i] - 1, permutations[permID, j] - 1] = 1;
                    }
                }
            }

            // Getting Kemeny values
            double[] kemenyValues = new double[kemenyMatrices.Length];
            for (int permID = 0; permID < kemenyMatrices.Length; permID++)
            {
                for (int i = 0; i < nbrOfNominees; i++)
                {
                    for (int j = 0; j < nbrOfNominees; j++)
                    {
                        kemenyValues[permID] += kemenyMatrices[permID][i, j] * sumValues[i, j];
                    }
                }
            }

            // Find Kemeny winners
            double currentMax = kemenyValues.Max();
            List<int> winnerIDs = new List<int>();
            for (int i = 0; i < kemenyValues.Length; i++)
            {
                if (kemenyValues[i] == currentMax) winnerIDs.Add(i);
            }
            if (winnerIDs.Count == 1)
            {
                for (int i = 0; i < nbrOfNominees; i++)
                {
                    currentValues[startCol + nbrOfNominees + permutations[winnerIDs[0], i] - 1] = i + 1;
                }    
            }
            foreach (int id in winnerIDs)
            {
                currentValues[startCol + permutations[id, 0] - 1] = 1;
            }
        }        
        private void getPluralityRunOff(ref double[] currentValues, int startCol)
        {
            // Getting the Plurality values
            double[] pluralityValues = getPluralityValues(1);

            // Find winners            
            List<int> winnerIDs = new List<int>();
            bool excludeFirst = false;
            // Find highest point
            double currentMax = pluralityValues.Max();
            for (int i = 0; i < pluralityValues.Length; i++)
            {
                if (pluralityValues[i] == currentMax)
                {
                    winnerIDs.Add(i);
                    pluralityValues[i] = 0;
                }
            }
            // If no tie, find second highest
            if (winnerIDs.Count == 1)
            {
                excludeFirst = true;
                currentMax = pluralityValues.Max();
                for (int i = 0; i < pluralityValues.Length; i++)
                {
                    if (pluralityValues[i] == currentMax)
                    {
                        winnerIDs.Add(i);
                    }
                }
            }
            // If too many winners, then throw out wrong ones
            int counter = 1;
            if (excludeFirst) counter++;
            while (winnerIDs.Count > 2 && counter <= nbrOfNominees)
            {
                pluralityValues = getPluralityValues(counter);
                if (excludeFirst) pluralityValues[0] = 0;
                currentMax = pluralityValues.Max();
                bool isFirst = true;
                List<int> RemoveIDs = new List<int>();
                foreach (int id in winnerIDs)
                {
                    if (excludeFirst && isFirst)
                    {
                        isFirst = false;
                        continue;                        
                    }
                    if (pluralityValues[id] != currentMax)
                    {
                        RemoveIDs.Add(id);
                    }
                }
                if(winnerIDs.Count - RemoveIDs.Count == 2)
                foreach (int id in RemoveIDs)
                {
                    winnerIDs.Remove(id);
                }
                counter++;
            }
            if (winnerIDs.Count != 2) return;

            // Find winner
            pluralityValues = new double[2];
            for (int voterID = 0; voterID < nbrOfVoters; voterID++)
            {
                if (comparsionMatrices[voterID][winnerIDs[0], winnerIDs[1]] == 1)
                {
                    pluralityValues[0]++;
                }
                else
                {
                    pluralityValues[1]++;
                }
            }
            currentMax = pluralityValues.Max();
            for (int i = 0; i < pluralityValues.Length; i++)
            {
                if (pluralityValues[i] == currentMax)                
                    currentValues[startCol + winnerIDs[i]] = 1;
                else
                    currentValues[startCol + winnerIDs[i]] = 2;
            }
        }
        private void getNApproval(ref double[] currentValues, ref int startCol)
        {           
            for (int n = 2; n < nbrOfNominees; n++)
            {
                double[] approvalValues = getApprovalValues(n);
                int[] currentOrder = getOrder(approvalValues);
                for (int i = 0; i < approvalValues.Length; i++)
                {
                    currentValues[startCol + i] = currentOrder[i];
                }
                startCol += nbrOfNominees;
            }
        }
        private void getUnanimity(ref double[] currentValues, int startCol)
        {
            //Summed values of the comparsionmatrices
            double[,] sumValues = getComparsionSum();

            for (int i = 0; i < nbrOfNominees; i++)
            {
                double rowSum = 0;
                for (int j = 0; j < nbrOfNominees; j++)
                {
                    rowSum += sumValues[i, j];
                }
                if (rowSum == (nbrOfNominees - 1)*nbrOfVoters)
                {
                    currentValues[startCol + i] = 1;
                }
            }
        }
        private void getPareto(ref double[] currentValues, int startCol)
        {
            //Summed values of the comparsionmatrices
            double[,] sumValues = getComparsionSum();

            int currentDominated = -1;            

            for (int i = 0; i < nbrOfNominees; i++)
            {
                if (currentDominated == -1)
                for (int j = 0; j < nbrOfNominees; j++)
                {
                    if (sumValues[i,j] == nbrOfVoters)
                    {
                        bool isPareto = true;
                        for (int k = 0; k < nbrOfNominees; k++)
                        {
                            for (int l = 0; l < nbrOfNominees; l++)
                            {
                                if (!(i == k && j == l) && sumValues[k, l] == nbrOfVoters)
                                {
                                    if (l != j && !checkDominance(sumValues, l, j))
                                    {
                                        isPareto = false;
                                    }
                                }
                            }
                        }
                        if (isPareto) 
                        {
                            currentDominated = j;
                            break;
                        }
                    }    
                }
            }
            if (currentDominated != -1) currentValues[startCol + currentDominated] = 1;
        }
        // Support functions
        private bool checkDominance(double[,] sumValues, int currentDominant, int currentDominated)
        {
            for (int i = 0; i < nbrOfNominees; i++)
            {
                if (sumValues[currentDominant, i] == nbrOfVoters)
                {
                    if (i == currentDominated)
                    {
                        return true;
                    }
                    else
                    {
                        if(checkDominance(sumValues, i, currentDominated)) return true;                        
                    }
                }
            }
            return false;
        }
        private void createComparsionMatrices(double[] currentValues)
        {
            comparsionMatrices = new double[nbrOfVoters][,];
            int currentCol = 0;
            for (int voterID = 0; voterID < nbrOfVoters; voterID++)
            {
                comparsionMatrices[voterID] = new double[nbrOfNominees, nbrOfNominees];
                for (int i = 0; i < nbrOfNominees; i++)
                {
                    for (int j = i; j < nbrOfNominees; j++)
                    {
                        if (i == j)
                        {
                            comparsionMatrices[voterID][i, j] = 0;
                        }
                        else
                        {
                            comparsionMatrices[voterID][i, j] = currentValues[currentCol];
                            comparsionMatrices[voterID][j, i] = Math.Abs(currentValues[currentCol] - 1);
                            currentCol++;
                        }
                    }
                }
            }
        }
        private double[,] getComparsionSum()
        {
            //Summed values of the comparsionmatrices
            double[,] sumValues = new double[nbrOfNominees, nbrOfNominees];
            for (int voterID = 0; voterID < nbrOfVoters; voterID++)
            {
                for (int i = 0; i < nbrOfNominees; i++)
                {
                    for (int j = 0; j < nbrOfNominees; j++)
                    {
                        sumValues[i, j] += comparsionMatrices[voterID][i, j];
                    }
                }
            }
            return sumValues;
        }
        private double[] getPluralityValues(int position)
        {
            // Getting the Plurality values
            double[] pluralityValues = new double[nbrOfNominees];
            for (int voterID = 0; voterID < nbrOfVoters; voterID++)
            {
                for (int i = 0; i < nbrOfNominees; i++)
                {
                    double currentSum = 0;
                    for (int j = 0; j < nbrOfNominees; j++)
                    {
                        currentSum += comparsionMatrices[voterID][i, j];
                    }
                    if (currentSum == nbrOfNominees - position)
                    {
                        pluralityValues[i]++;
                    }
                }
            }
            return pluralityValues;
        }
        private double[] getApprovalValues(int n)
        {
            // Getting the Plurality values
            double[] approvalValues = new double[nbrOfNominees];
            for (int voterID = 0; voterID < nbrOfVoters; voterID++)
            {
                for (int i = 0; i < nbrOfNominees; i++)
                {
                    double currentSum = 1;
                    for (int j = 0; j < nbrOfNominees; j++)
                    {
                        currentSum += comparsionMatrices[voterID][i, j];
                    }
                    currentSum -= (nbrOfNominees - n);
                    if (currentSum > 0)
                    {
                        approvalValues[i]++;
                    }
                }
            }
            return approvalValues;
        }
        private int[] getOrder(double[] orderValues)
        {
            int[] result = new int[orderValues.Length];

            double currentMin = orderValues.Min();
            for (int i = 0; i < orderValues.Length; i++)
            {
                orderValues[i] += Math.Abs(currentMin) + 1;
            }

            int currentPosition = 1;
            do
            {
                double currentMax = orderValues.Max();
                int counter = 0;
                for (int i = 0; i < orderValues.Length; i++)
                {
                    if (currentMax == orderValues[i])
                    {
                        orderValues[i] = 0;
                        result[i] = currentPosition;
                        counter++;
                    }
                }
                currentPosition += counter;
            } while (!(orderValues.Sum() == 0));

            return result;
        }
        #endregion
    }
}
