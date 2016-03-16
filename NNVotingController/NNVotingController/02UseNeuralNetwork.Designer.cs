namespace NNVotingController
{
    partial class _02UseNeuralNetwork
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
            this.listNetworks = new System.Windows.Forms.ListBox();
            this.panelNetwork = new System.Windows.Forms.Panel();
            this.lblNetwork = new System.Windows.Forms.Label();
            this.panelInput = new System.Windows.Forms.Panel();
            this.lblInput = new System.Windows.Forms.Label();
            this.listInputs = new System.Windows.Forms.ListBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.panelNetwork.SuspendLayout();
            this.panelInput.SuspendLayout();
            this.SuspendLayout();
            // 
            // listNetworks
            // 
            this.listNetworks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listNetworks.FormattingEnabled = true;
            this.listNetworks.Location = new System.Drawing.Point(0, 19);
            this.listNetworks.Name = "listNetworks";
            this.listNetworks.ScrollAlwaysVisible = true;
            this.listNetworks.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listNetworks.Size = new System.Drawing.Size(400, 95);
            this.listNetworks.TabIndex = 1;
            // 
            // panelNetwork
            // 
            this.panelNetwork.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.panelNetwork.Controls.Add(this.lblNetwork);
            this.panelNetwork.Controls.Add(this.listNetworks);
            this.panelNetwork.Location = new System.Drawing.Point(3, 124);
            this.panelNetwork.Name = "panelNetwork";
            this.panelNetwork.Size = new System.Drawing.Size(400, 116);
            this.panelNetwork.TabIndex = 2;
            // 
            // lblNetwork
            // 
            this.lblNetwork.AutoSize = true;
            this.lblNetwork.Location = new System.Drawing.Point(3, 3);
            this.lblNetwork.Name = "lblNetwork";
            this.lblNetwork.Size = new System.Drawing.Size(52, 13);
            this.lblNetwork.TabIndex = 2;
            this.lblNetwork.Text = "Networks";
            // 
            // panelInput
            // 
            this.panelInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.panelInput.Controls.Add(this.lblInput);
            this.panelInput.Controls.Add(this.listInputs);
            this.panelInput.Location = new System.Drawing.Point(3, 2);
            this.panelInput.Name = "panelInput";
            this.panelInput.Size = new System.Drawing.Size(400, 116);
            this.panelInput.TabIndex = 3;
            // 
            // lblInput
            // 
            this.lblInput.AutoSize = true;
            this.lblInput.Location = new System.Drawing.Point(3, 3);
            this.lblInput.Name = "lblInput";
            this.lblInput.Size = new System.Drawing.Size(36, 13);
            this.lblInput.TabIndex = 2;
            this.lblInput.Text = "Inputs";
            // 
            // listInputs
            // 
            this.listInputs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listInputs.FormattingEnabled = true;
            this.listInputs.Location = new System.Drawing.Point(0, 19);
            this.listInputs.Name = "listInputs";
            this.listInputs.ScrollAlwaysVisible = true;
            this.listInputs.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listInputs.Size = new System.Drawing.Size(400, 95);
            this.listInputs.TabIndex = 1;
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStart.Location = new System.Drawing.Point(256, 273);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(150, 100);
            this.btnStart.TabIndex = 4;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // _02UseNeuralNetwork
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.panelInput);
            this.Controls.Add(this.panelNetwork);
            this.Name = "_02UseNeuralNetwork";
            this.Size = new System.Drawing.Size(406, 373);
            this.Resize += new System.EventHandler(this._02UseNeuralNetwork_Resize);
            this.panelNetwork.ResumeLayout(false);
            this.panelNetwork.PerformLayout();
            this.panelInput.ResumeLayout(false);
            this.panelInput.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listNetworks;
        private System.Windows.Forms.Panel panelNetwork;
        private System.Windows.Forms.Label lblNetwork;
        private System.Windows.Forms.Panel panelInput;
        private System.Windows.Forms.Label lblInput;
        private System.Windows.Forms.ListBox listInputs;
        private System.Windows.Forms.Button btnStart;

    }
}
