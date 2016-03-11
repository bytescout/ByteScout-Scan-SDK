/**************************************************
ByteScout Scan SDK
Copyright (c) 2010, http://ByteScout.com
All rights reserved.

Commercial usage, redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
**************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Bytescout.Scan
{
	internal partial class OptionsDialog : Form
	{
		private Scan scan = null;

		public OptionsDialog(Scan scan)
		{
			if (scan == null)
			{
				throw new ScanException("Invalid constructor parameter.");
			}

			this.scan = scan;

			InitializeComponent();

			foreach (OutputFormat f in Enum.GetValues(typeof(OutputFormat)))
			{
				cmbOutputFormat.Items.Add(f);
			}

			foreach (TiffCompression f in Enum.GetValues(typeof(TiffCompression)))
			{
				cmbTiffCompression.Items.Add(f);
			}

			tbDevice.Text = scan.DefaultDevice;
			tbOutputFolder.Text = scan.OutputFolder;
			tbNamingTemplate.Text = scan.FileNamingTemplate;
			cmbOutputFormat.SelectedItem = scan.OutputFormat;
			nudJpegQuality.Value = (decimal) scan.JpegQuality;
			cmbTiffCompression.SelectedItem = scan.TiffCompression;

			lblExample.Text = "Example: " + String.Format(scan.FileNamingTemplate, 1) /*+ ", " + String.Format(scan.FileNamingTemplate, 2)*/;

		}

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			folderBrowserDialog.SelectedPath = tbOutputFolder.Text;

			if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
			{
				tbOutputFolder.Text = folderBrowserDialog.SelectedPath;
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (this.DialogResult == DialogResult.OK)
			{
				scan.DefaultDevice = tbDevice.Text;
				scan.OutputFolder = tbOutputFolder.Text;
				scan.FileNamingTemplate = tbNamingTemplate.Text;
				scan.OutputFormat = (OutputFormat) cmbOutputFormat.SelectedItem;
				scan.JpegQuality = (int) nudJpegQuality.Value;
				scan.TiffCompression = (TiffCompression) cmbTiffCompression.SelectedItem;
			}

			base.OnClosing(e);
		}

		private void tbNamingTemplate_TextChanged(object sender, EventArgs e)
		{
			String s = "";

			try
			{
				s = "Example: " + String.Format(tbNamingTemplate.Text, 1);
			}
			catch
			{
				s = "Example: error";
			}

			lblExample.Text = s;
		}
	}
}