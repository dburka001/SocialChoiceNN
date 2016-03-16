namespace NNVotingController
{
    partial class _01TrainNeuralNetwork
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnStart = new System.Windows.Forms.Button();
            this.lblFileName = new System.Windows.Forms.Label();
            this.nudSeedTo = new System.Windows.Forms.NumericUpDown();
            this.lblSeedTo = new System.Windows.Forms.Label();
            this.nudSeedFrom = new System.Windows.Forms.NumericUpDown();
            this.lblSeedFrom = new System.Windows.Forms.Label();
            this.listTraining = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudSeedTo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSeedFrom)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStart.Location = new System.Drawing.Point(328, 243);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(150, 100);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // lblFileName
            // 
            this.lblFileName.AutoSize = true;
            this.lblFileName.Location = new System.Drawing.Point(3, 12);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(122, 13);
            this.lblFileName.TabIndex = 2;
            this.lblFileName.Text = "Filename of training data";
            // 
            // nudSeedTo
            // 
            this.nudSeedTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudSeedTo.Location = new System.Drawing.Point(352, 8);
            this.nudSeedTo.Name = "nudSeedTo";
            this.nudSeedTo.Size = new System.Drawing.Size(120, 20);
            this.nudSeedTo.TabIndex = 8;
            // 
            // lblSeedTo
            // 
            this.lblSeedTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSeedTo.AutoSize = true;
            this.lblSeedTo.Location = new System.Drawing.Point(332, 12);
            this.lblSeedTo.Name = "lblSeedTo";
            this.lblSeedTo.Size = new System.Drawing.Size(10, 13);
            this.lblSeedTo.TabIndex = 6;
            this.lblSeedTo.Text = "-";
            // 
            // nudSeedFrom
            // 
            this.nudSeedFrom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudSeedFrom.Location = new System.Drawing.Point(202, 8);
            this.nudSeedFrom.Name = "nudSeedFrom";
            this.nudSeedFrom.Size = new System.Drawing.Size(120, 20);
            this.nudSeedFrom.TabIndex = 5;
            // 
            // lblSeedFrom
            // 
            this.lblSeedFrom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSeedFrom.AutoSize = true;
            this.lblSeedFrom.Location = new System.Drawing.Point(160, 12);
            this.lblSeedFrom.Name = "lblSeedFrom";
            this.lblSeedFrom.Size = new System.Drawing.Size(32, 13);
            this.lblSeedFrom.TabIndex = 4;
            this.lblSeedFrom.Text = "Seed";
            // 
            // listTraining
            // 
            this.listTraining.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listTraining.FormattingEnabled = true;
            this.listTraining.Location = new System.Drawing.Point(0, 34);
            this.listTraining.Name = "listTraining";
            this.listTraining.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listTraining.Size = new System.Drawing.Size(475, 199);
            this.listTraining.TabIndex = 9;
            // 
            // _01TrainNeuralNetwork
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listTraining);
            this.Controls.Add(this.nudSeedTo);
            this.Controls.Add(this.lblSeedTo);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.nudSeedFrom);
            this.Controls.Add(this.lblSeedFrom);
            this.Controls.Add(this.lblFileName);
            this.Name = "_01TrainNeuralNetwork";
            this.Size = new System.Drawing.Size(478, 343);
            ((System.ComponentModel.ISupportInitialize)(this.nudSeedTo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSeedFrom)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.NumericUpDown nudSeedTo;
        private System.Windows.Forms.Label lblSeedTo;
        private System.Windows.Forms.NumericUpDown nudSeedFrom;
        private System.Windows.Forms.Label lblSeedFrom;
        private System.Windows.Forms.ListBox listTraining;
    }
}
