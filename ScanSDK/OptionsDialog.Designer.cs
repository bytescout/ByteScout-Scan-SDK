namespace Bytescout.Scan
{
	partial class OptionsDialog
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
			this.cmbOutputFormat = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.nudJpegQuality = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.tbOutputFolder = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.tbNamingTemplate = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.lblExample = new System.Windows.Forms.Label();
			this.cmbTiffCompression = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.tbDevice = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize) (this.nudJpegQuality)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// cmbOutputFormat
			// 
			this.cmbOutputFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbOutputFormat.FormattingEnabled = true;
			this.cmbOutputFormat.Location = new System.Drawing.Point(104, 19);
			this.cmbOutputFormat.Name = "cmbOutputFormat";
			this.cmbOutputFormat.Size = new System.Drawing.Size(118, 21);
			this.cmbOutputFormat.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(71, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "Output format";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 15);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(78, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Default Device";
			// 
			// nudJpegQuality
			// 
			this.nudJpegQuality.Location = new System.Drawing.Point(104, 46);
			this.nudJpegQuality.Name = "nudJpegQuality";
			this.nudJpegQuality.Size = new System.Drawing.Size(43, 20);
			this.nudJpegQuality.TabIndex = 1;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 48);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(69, 13);
			this.label3.TabIndex = 5;
			this.label3.Text = "JPEG Quality";
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(322, 205);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(241, 205);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 5;
			this.btnOK.Text = "OK";
			this.btnOK.UseVisualStyleBackColor = true;
			// 
			// tbOutputFolder
			// 
			this.tbOutputFolder.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tbOutputFolder.Location = new System.Drawing.Point(116, 39);
			this.tbOutputFolder.Name = "tbOutputFolder";
			this.tbOutputFolder.Size = new System.Drawing.Size(243, 20);
			this.tbOutputFolder.TabIndex = 1;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 42);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(68, 13);
			this.label4.TabIndex = 7;
			this.label4.Text = "Output folder";
			// 
			// btnBrowse
			// 
			this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles) ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrowse.Location = new System.Drawing.Point(366, 37);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(32, 23);
			this.btnBrowse.TabIndex = 2;
			this.btnBrowse.Text = "...";
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// tbNamingTemplate
			// 
			this.tbNamingTemplate.Location = new System.Drawing.Point(116, 65);
			this.tbNamingTemplate.Name = "tbNamingTemplate";
			this.tbNamingTemplate.Size = new System.Drawing.Size(118, 20);
			this.tbNamingTemplate.TabIndex = 3;
			this.tbNamingTemplate.TextChanged += new System.EventHandler(this.tbNamingTemplate_TextChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(12, 68);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(86, 13);
			this.label5.TabIndex = 10;
			this.label5.Text = "Naming template";
			// 
			// lblExample
			// 
			this.lblExample.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.lblExample.Location = new System.Drawing.Point(240, 68);
			this.lblExample.Name = "lblExample";
			this.lblExample.Size = new System.Drawing.Size(157, 17);
			this.lblExample.TabIndex = 11;
			this.lblExample.Text = "Example:";
			// 
			// cmbTiffCompression
			// 
			this.cmbTiffCompression.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbTiffCompression.FormattingEnabled = true;
			this.cmbTiffCompression.Location = new System.Drawing.Point(104, 72);
			this.cmbTiffCompression.Name = "cmbTiffCompression";
			this.cmbTiffCompression.Size = new System.Drawing.Size(118, 21);
			this.cmbTiffCompression.TabIndex = 2;
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(6, 75);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(92, 13);
			this.label7.TabIndex = 13;
			this.label7.Text = "TIFF Compression";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.cmbOutputFormat);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.cmbTiffCompression);
			this.groupBox1.Controls.Add(this.nudJpegQuality);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Location = new System.Drawing.Point(12, 91);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(385, 100);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Output format";
			// 
			// tbDevice
			// 
			this.tbDevice.Anchor = ((System.Windows.Forms.AnchorStyles) (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tbDevice.Location = new System.Drawing.Point(116, 13);
			this.tbDevice.Name = "tbDevice";
			this.tbDevice.Size = new System.Drawing.Size(281, 20);
			this.tbDevice.TabIndex = 0;
			// 
			// OptionsDialog
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(409, 240);
			this.Controls.Add(this.tbDevice);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.lblExample);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.tbNamingTemplate);
			this.Controls.Add(this.btnBrowse);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.tbOutputFolder);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.label2);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "OptionsDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Options";
			((System.ComponentModel.ISupportInitialize) (this.nudJpegQuality)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox cmbOutputFormat;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown nudJpegQuality;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.TextBox tbOutputFolder;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.TextBox tbNamingTemplate;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label lblExample;
		private System.Windows.Forms.ComboBox cmbTiffCompression;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
		private System.Windows.Forms.TextBox tbDevice;
	}
}