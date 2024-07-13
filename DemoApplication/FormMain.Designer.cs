namespace DemoApplication
{
    partial class FormMain
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnSerialize = new System.Windows.Forms.Button();
            rtbXMLOutput = new System.Windows.Forms.RichTextBox();
            btnDeserialize = new System.Windows.Forms.Button();
            lstSampleClasses = new System.Windows.Forms.ListBox();
            rtbParsingErrors = new System.Windows.Forms.RichTextBox();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            btnSerializeToFile = new System.Windows.Forms.Button();
            btnDeserializeFromFile = new System.Windows.Forms.Button();
            label3 = new System.Windows.Forms.Label();
            comboPolicy = new System.Windows.Forms.ComboBox();
            label4 = new System.Windows.Forms.Label();
            comboErrorType = new System.Windows.Forms.ComboBox();
            label5 = new System.Windows.Forms.Label();
            groupBox1 = new System.Windows.Forms.GroupBox();
            label8 = new System.Windows.Forms.Label();
            numMaxRecursion = new System.Windows.Forms.NumericUpDown();
            label7 = new System.Windows.Forms.Label();
            comboOptions = new System.Windows.Forms.ComboBox();
            openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            rtbDeserializeOutput = new System.Windows.Forms.RichTextBox();
            label6 = new System.Windows.Forms.Label();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            panel1 = new System.Windows.Forms.Panel();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) numMaxRecursion).BeginInit();
            ((System.ComponentModel.ISupportInitialize) splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // btnSerialize
            // 
            btnSerialize.Location = new System.Drawing.Point(7, 22);
            btnSerialize.Margin = new System.Windows.Forms.Padding(4);
            btnSerialize.Name = "btnSerialize";
            btnSerialize.Size = new System.Drawing.Size(88, 26);
            btnSerialize.TabIndex = 0;
            btnSerialize.Text = "Serialize";
            btnSerialize.UseVisualStyleBackColor = true;
            btnSerialize.Click += BtnSerialize_Click;
            // 
            // rtbXMLOutput
            // 
            rtbXMLOutput.AcceptsTab = true;
            rtbXMLOutput.Anchor =  System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            rtbXMLOutput.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            rtbXMLOutput.Location = new System.Drawing.Point(252, 22);
            rtbXMLOutput.Margin = new System.Windows.Forms.Padding(4);
            rtbXMLOutput.Name = "rtbXMLOutput";
            rtbXMLOutput.Size = new System.Drawing.Size(411, 357);
            rtbXMLOutput.TabIndex = 1;
            rtbXMLOutput.Text = "";
            rtbXMLOutput.WordWrap = false;
            // 
            // btnDeserialize
            // 
            btnDeserialize.Location = new System.Drawing.Point(102, 22);
            btnDeserialize.Margin = new System.Windows.Forms.Padding(4);
            btnDeserialize.Name = "btnDeserialize";
            btnDeserialize.Size = new System.Drawing.Size(88, 26);
            btnDeserialize.TabIndex = 2;
            btnDeserialize.Text = "Deserialize";
            btnDeserialize.UseVisualStyleBackColor = true;
            btnDeserialize.Click += BtnDeserialize_Click;
            // 
            // lstSampleClasses
            // 
            lstSampleClasses.Anchor =  System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            lstSampleClasses.FormattingEnabled = true;
            lstSampleClasses.ItemHeight = 15;
            lstSampleClasses.Location = new System.Drawing.Point(10, 22);
            lstSampleClasses.Margin = new System.Windows.Forms.Padding(4);
            lstSampleClasses.Name = "lstSampleClasses";
            lstSampleClasses.Size = new System.Drawing.Size(234, 529);
            lstSampleClasses.TabIndex = 3;
            lstSampleClasses.MouseDoubleClick += LstSampleClasses_MouseDoubleClick;
            // 
            // rtbParsingErrors
            // 
            rtbParsingErrors.Anchor =  System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            rtbParsingErrors.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            rtbParsingErrors.Location = new System.Drawing.Point(252, 401);
            rtbParsingErrors.Margin = new System.Windows.Forms.Padding(4);
            rtbParsingErrors.Name = "rtbParsingErrors";
            rtbParsingErrors.ReadOnly = true;
            rtbParsingErrors.Size = new System.Drawing.Size(411, 165);
            rtbParsingErrors.TabIndex = 4;
            rtbParsingErrors.Text = "";
            rtbParsingErrors.WordWrap = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(252, 4);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(354, 15);
            label1.TabIndex = 5;
            label1.Text = "XML Serialized (Modify and press Deserialize to see what happens)";
            // 
            // label2
            // 
            label2.Anchor =  System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(252, 383);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(82, 15);
            label2.TabIndex = 6;
            label2.Text = "Parsing Errors:";
            // 
            // btnSerializeToFile
            // 
            btnSerializeToFile.Location = new System.Drawing.Point(196, 22);
            btnSerializeToFile.Margin = new System.Windows.Forms.Padding(4);
            btnSerializeToFile.Name = "btnSerializeToFile";
            btnSerializeToFile.Size = new System.Drawing.Size(128, 26);
            btnSerializeToFile.TabIndex = 7;
            btnSerializeToFile.Text = "Serialize To File";
            btnSerializeToFile.UseVisualStyleBackColor = true;
            btnSerializeToFile.Click += BtnSerializeToFile_Click;
            // 
            // btnDeserializeFromFile
            // 
            btnDeserializeFromFile.Location = new System.Drawing.Point(331, 22);
            btnDeserializeFromFile.Margin = new System.Windows.Forms.Padding(4);
            btnDeserializeFromFile.Name = "btnDeserializeFromFile";
            btnDeserializeFromFile.Size = new System.Drawing.Size(141, 26);
            btnDeserializeFromFile.TabIndex = 8;
            btnDeserializeFromFile.Text = "Deserialize From File";
            btnDeserializeFromFile.UseVisualStyleBackColor = true;
            btnDeserializeFromFile.Click += BtnDeserializeFromFile_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(10, 4);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(187, 15);
            label3.TabIndex = 9;
            label3.Text = "Choose a type to test YAXLib with:";
            // 
            // comboPolicy
            // 
            comboPolicy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboPolicy.FormattingEnabled = true;
            comboPolicy.Location = new System.Drawing.Point(140, 60);
            comboPolicy.Margin = new System.Windows.Forms.Padding(4);
            comboPolicy.Name = "comboPolicy";
            comboPolicy.Size = new System.Drawing.Size(180, 23);
            comboPolicy.TabIndex = 10;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(7, 64);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(122, 15);
            label4.TabIndex = 12;
            label4.Text = "Error Handling Policy:";
            // 
            // comboErrorType
            // 
            comboErrorType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboErrorType.FormattingEnabled = true;
            comboErrorType.Location = new System.Drawing.Point(462, 60);
            comboErrorType.Margin = new System.Windows.Forms.Padding(4);
            comboErrorType.Name = "comboErrorType";
            comboErrorType.Size = new System.Drawing.Size(93, 23);
            comboErrorType.TabIndex = 13;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(341, 64);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(106, 15);
            label5.TabIndex = 14;
            label5.Text = "Defaullt Error Type:";
            // 
            // groupBox1
            // 
            groupBox1.Anchor =  System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            groupBox1.Controls.Add(label8);
            groupBox1.Controls.Add(numMaxRecursion);
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(comboOptions);
            groupBox1.Controls.Add(btnSerialize);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(btnDeserialize);
            groupBox1.Controls.Add(comboErrorType);
            groupBox1.Controls.Add(btnSerializeToFile);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(btnDeserializeFromFile);
            groupBox1.Controls.Add(comboPolicy);
            groupBox1.Location = new System.Drawing.Point(14, 14);
            groupBox1.Margin = new System.Windows.Forms.Padding(4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new System.Windows.Forms.Padding(4);
            groupBox1.Size = new System.Drawing.Size(999, 101);
            groupBox1.TabIndex = 15;
            groupBox1.TabStop = false;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(667, 24);
            label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(88, 15);
            label8.TabIndex = 18;
            label8.Text = "Max Recursion:";
            // 
            // numMaxRecursion
            // 
            numMaxRecursion.Location = new System.Drawing.Point(769, 22);
            numMaxRecursion.Margin = new System.Windows.Forms.Padding(4);
            numMaxRecursion.Maximum = new decimal(new int[] { 999999999, 0, 0, 0 });
            numMaxRecursion.Name = "numMaxRecursion";
            numMaxRecursion.Size = new System.Drawing.Size(113, 23);
            numMaxRecursion.TabIndex = 17;
            numMaxRecursion.Value = new decimal(new int[] { 30, 0, 0, 0 });
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(567, 64);
            label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(47, 15);
            label7.TabIndex = 16;
            label7.Text = "Option:";
            // 
            // comboOptions
            // 
            comboOptions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            comboOptions.FormattingEnabled = true;
            comboOptions.Location = new System.Drawing.Point(622, 60);
            comboOptions.Margin = new System.Windows.Forms.Padding(4);
            comboOptions.Name = "comboOptions";
            comboOptions.Size = new System.Drawing.Size(259, 23);
            comboOptions.TabIndex = 15;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            openFileDialog1.Filter = "XML Files|*.xml|All files|*.*";
            // 
            // saveFileDialog1
            // 
            saveFileDialog1.Filter = "XML Files|*.xml|All files|*.*";
            // 
            // rtbDeserializeOutput
            // 
            rtbDeserializeOutput.Anchor =  System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            rtbDeserializeOutput.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            rtbDeserializeOutput.Location = new System.Drawing.Point(7, 26);
            rtbDeserializeOutput.Margin = new System.Windows.Forms.Padding(4);
            rtbDeserializeOutput.Name = "rtbDeserializeOutput";
            rtbDeserializeOutput.ReadOnly = true;
            rtbDeserializeOutput.Size = new System.Drawing.Size(339, 541);
            rtbDeserializeOutput.TabIndex = 16;
            rtbDeserializeOutput.Text = "";
            rtbDeserializeOutput.WordWrap = false;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(4, 4);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(162, 15);
            label6.TabIndex = 17;
            label6.Text = "Deserialized object's ToString:";
            // 
            // splitContainer1
            // 
            splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.Location = new System.Drawing.Point(0, 124);
            splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(rtbXMLOutput);
            splitContainer1.Panel1.Controls.Add(lstSampleClasses);
            splitContainer1.Panel1.Controls.Add(rtbParsingErrors);
            splitContainer1.Panel1.Controls.Add(label1);
            splitContainer1.Panel1.Controls.Add(label2);
            splitContainer1.Panel1.Controls.Add(label3);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(rtbDeserializeOutput);
            splitContainer1.Panel2.Controls.Add(label6);
            splitContainer1.Size = new System.Drawing.Size(1027, 573);
            splitContainer1.SplitterDistance = 669;
            splitContainer1.SplitterWidth = 5;
            splitContainer1.TabIndex = 18;
            // 
            // panel1
            // 
            panel1.Controls.Add(groupBox1);
            panel1.Dock = System.Windows.Forms.DockStyle.Top;
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Margin = new System.Windows.Forms.Padding(4);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(1027, 124);
            panel1.TabIndex = 19;
            // 
            // FormMain
            // 
            AcceptButton = btnSerialize;
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1027, 697);
            Controls.Add(splitContainer1);
            Controls.Add(panel1);
            Margin = new System.Windows.Forms.Padding(4);
            MinimumSize = new System.Drawing.Size(588, 448);
            Name = "FormMain";
            Text = "YAXLib Demo Application";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) numMaxRecursion).EndInit();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button btnSerialize;
        private System.Windows.Forms.RichTextBox rtbXMLOutput;
        private System.Windows.Forms.Button btnDeserialize;
        private System.Windows.Forms.ListBox lstSampleClasses;
        private System.Windows.Forms.RichTextBox rtbParsingErrors;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSerializeToFile;
        private System.Windows.Forms.Button btnDeserializeFromFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboPolicy;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboErrorType;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.RichTextBox rtbDeserializeOutput;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboOptions;
        private System.Windows.Forms.NumericUpDown numMaxRecursion;
        private System.Windows.Forms.Label label8;
    }
}

