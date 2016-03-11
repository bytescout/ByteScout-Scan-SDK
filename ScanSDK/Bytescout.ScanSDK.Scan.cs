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
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Collections;
using System.ComponentModel;
using Microsoft.Win32;

namespace Bytescout.Scan
{
	internal delegate void AsyncMethodCaller(IntPtr windowsHandle);
	
	///<summary>
	/// Represents the method that will handle the TransferFinished event.
	///</summary>
	///<param name="sender">Source of the event.</param>
	///<param name="scannedImages">Array of scanned images. May contain Bitmap objects 
	/// or String objects depending on the SaveTo property value.</param>
	public delegate void TransferFinishedEventHandler(Scan sender, ArrayList scannedImages);

	///<summary>
	/// Used to specify output image format.
	///</summary>
	public enum OutputFormat
	{
		///<summary>
		/// (0) JPEG format.
		///</summary>
		JPEG,
		///<summary>
		/// (1) PNG format.
		///</summary>
		PNG,
		///<summary>
		/// (2) TIFF format.
		///</summary>
		TIFF,
		///<summary>
		/// (3) BMP format.
		///</summary>
		BMP,
		//GIF
	};

	///<summary>
	/// Used to specify TIFF image compression.
	///</summary>
	public enum TiffCompression
	{
		///<summary>
		/// (0) No compression.
		///</summary>
		None,
		///<summary>
		/// (1) RLE compression.
		///</summary>
		RLE,
		///<summary>
		/// (2) CCITT3 compression.
		///</summary>
		CCITT3,
		///<summary>
		/// (3) CCITT4 compression.
		///</summary>
		CCITT4,
		///<summary>
		/// (4) LZW compression.
		///</summary>
		LZW
	};

	///<summary>
	/// Defines TWAIN scanner class.
	///</summary>
	public class Scan : Component
	{
		private TwainSource _twainSource = null;

		private String _regKey = "Bytescout\\Scan SDK";
		private String _defaultDevice = "";
		private SaveTo _saveTo = SaveTo.File;
		private String _outputFolder = "";
		private String _fileNamingTemplate = "Image_{0:D2}";
		private OutputFormat _outputFormat = OutputFormat.JPEG;
		private int _jpegQuality = 95;
		private TiffCompression _tiffCompression = TiffCompression.LZW;

		///<summary>
		/// Occurs when image transfer is started.
		///</summary>
		public event EventHandler TransferStarted;
		///<summary>
		/// Occurs when image transfer is finished.
		///</summary>
		public event TransferFinishedEventHandler TransferFinished;

		internal static bool CompatibilityMode = true; // this is Registered state ;)

		///<summary>
		/// Gets or sets the registry key to store options. This key will be created in HKCU registry branch.
		/// Default is "Bytescout\Scan SDK".
		///</summary>
		[DefaultValue("Bytescout\\Scan SDK")]
		public System.String RegistryKeyToStoreOptions
		{
			get { return _regKey; }
			set { _regKey = value; }
		}

		[DefaultValue("")]
		public System.String DefaultDevice
		{
			get { return _defaultDevice; }
			set { _defaultDevice = value; }
		}
		
		///<summary>
		/// Gets or sets SaveTo enum value specifing where to save the acquired images.
		///</summary>
		[DefaultValue(SaveTo.ImageObject)]
		public Bytescout.Scan.SaveTo SaveTo
		{
			get { return _saveTo; }
			set { _saveTo = value; }
		}
		
		///<summary>
		/// Gets or sets the output folder for generated image files. 
		/// If empty, files will be saved to user TEMP directory.
		///</summary>
		[DefaultValue("")]
		public System.String OutputFolder
		{
			get { return _outputFolder; }
			set { _outputFolder = value; }
		}

		///<summary>
		/// Gets ot sets a template to generate file names.
		/// Default is "Image_{0:D2}". 
		///</summary>
		[DefaultValue("Image_{0:D2}")]
		public System.String FileNamingTemplate
		{
			get { return _fileNamingTemplate; }
			set { _fileNamingTemplate = value; }
		}

		///<summary>
		/// Gets or sets format of the output images.
		///</summary>
		[DefaultValue(OutputFormat.JPEG)]
		public Bytescout.Scan.OutputFormat OutputFormat
		{
			get { return _outputFormat; }
			set { _outputFormat = value; }
		}

		///<summary>
		/// Gets or sets the quality of JPEG images. From 0 (lowest) to 100 (highest). 
		/// Default is 95.
		///</summary>
		[DefaultValue(95)]
		public int JpegQuality
		{
			get { return _jpegQuality; }
			set { _jpegQuality = value; }
		}

		///<summary>
		/// Gets or sets TIFF image compression algorithm. 
		/// Default is LZW compression.
		///</summary>
		[DefaultValue(TiffCompression.LZW)]
		public Bytescout.Scan.TiffCompression TiffCompression
		{
			get { return _tiffCompression; }
			set { _tiffCompression = value; }
		}
		
		///<summary>
		/// Initializes a new instance of the Scan class with default settings.
		///</summary>
		public Scan()
		{
			LoadOptions();
		}

		///<summary>
		/// Initializes a new instance of the Scan class using the specified IContainer object.
		///</summary>
		///<param name="container"></param>
		public Scan(IContainer container)
		{
			container.Add(this);

			LoadOptions();
		}

		/// <summary>
		/// Inits the library with registration information.
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		/// <param name="key">The key.</param>
		public void InitLibrary(string userName, string key)
		{
            CompatibilityMode = true; //CompatibilityChecker.CheckCompatibility(userName, key);
		}

		/// <summary>
		/// Releases the unmanaged resources used by the <see cref="T:System.ComponentModel.Component"/> and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources. 
		///                 </param>
		protected override void Dispose(bool disposing)
		{
			SaveOptions();
			
			if (_twainSource != null)
			{
				_twainSource.TransferStarted -= twainSource_TransferStarted;
				_twainSource.TransferFinished -= twainSource_TransferFinished;
				_twainSource.Dispose();
				_twainSource = null;
			}

			base.Dispose(disposing);
		}

		private void LoadOptions()
		{
			RegistryKey userKey = null;
			RegistryKey machineKey = null;
			RegistryKey key;

			if (String.IsNullOrEmpty(RegistryKeyToStoreOptions))
			{
				return;
			}

			try
			{
				userKey = Registry.CurrentUser.OpenSubKey("Software\\" + RegistryKeyToStoreOptions);
				machineKey = Registry.LocalMachine.OpenSubKey("Software\\" + RegistryKeyToStoreOptions);
			}
			catch //(System.Exception e)
			{
			}

			if (userKey == null && machineKey == null)
			{
				return;
			}

			if (userKey != null)
			{
				key = userKey;
			}
			else
			{
				key = machineKey;
			}

			try
			{
				_defaultDevice = (String) key.GetValue("Default Device", "");
				_saveTo = (SaveTo) Enum.Parse(typeof(SaveTo), (String) key.GetValue("Save To", SaveTo.File));
				_outputFolder = (String) key.GetValue("Output Folder", SaveTo.File);
				_fileNamingTemplate = (String) key.GetValue("File Naming Template", "Image_{0:D2}");
				_outputFormat = (OutputFormat) Enum.Parse(typeof(OutputFormat), (String) key.GetValue("Output Format", OutputFormat.JPEG));
				_jpegQuality = (int) key.GetValue("JPEG Quality", 95);
				_tiffCompression = (TiffCompression) Enum.Parse(typeof(TiffCompression), (String) key.GetValue("TIFF Compression", TiffCompression.LZW));
			}
			catch (Exception e)
			{
				throw new ScanException("Failed to get options.", e);
			}
		}

		private void SaveOptions()
		{
			if (String.IsNullOrEmpty(RegistryKeyToStoreOptions))
			{
				return;
			}

			try
			{
				RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\" + RegistryKeyToStoreOptions);

				key.SetValue("Default Device", _defaultDevice, RegistryValueKind.String);
				key.SetValue("Save To", _saveTo.ToString(), RegistryValueKind.String);
				key.SetValue("Output Folder", _outputFolder, RegistryValueKind.String);
				key.SetValue("File Naming Template", _fileNamingTemplate, RegistryValueKind.String);
				key.SetValue("Output Format", _outputFormat.ToString(), RegistryValueKind.String);
				key.SetValue("JPEG Quality", _jpegQuality, RegistryValueKind.DWord);
				key.SetValue("TIFF Compression", _tiffCompression.ToString(), RegistryValueKind.String);
			}
			catch (Exception e)
			{
				throw new ScanException("Failed to save options", e);
			}
		}

// 		public ArrayList AcquireImages(IntPtr windowHandle)
// 		{
// 			
// 		}

		///<summary>
		/// Displays TWAIN device selection dialog.
		///</summary>
		public void SelectDevice(IntPtr windowHandle)
		{
			// initialize TwainSource
			if (_twainSource == null)
			{
				_twainSource = new TwainSource(windowHandle);
			}

			DefaultDevice = _twainSource.SelectTwainSource();
		}

		///<summary>
		/// Starts image acquisition asynchronously. 
		///</summary>
		///<param name="windowHandle"></param>
		public void AcquireImagesAsync(IntPtr windowHandle)
		{
			// select source and show TWAIN UI
			DoScan(windowHandle);
		}

		private void DoScan(IntPtr windowHandle)
		{
			// initialize TwainSource
			if (_twainSource == null)
			{
				_twainSource = new TwainSource(windowHandle);
			}

			_twainSource.TransferStarted -= new EventHandler(twainSource_TransferStarted);
			_twainSource.TransferFinished -= new EventHandler(twainSource_TransferFinished);
			_twainSource.TransferStarted += new EventHandler(twainSource_TransferStarted);
			_twainSource.TransferFinished += new EventHandler(twainSource_TransferFinished);

			_twainSource.SaveTo = this.SaveTo;
			_twainSource.OutputFolder = this.OutputFolder;
			_twainSource.JpegQuality = this.JpegQuality;
			_twainSource.TiffCompression = TranslateTiffCompression(this.TiffCompression);
			_twainSource.FileNamingTemplate = _fileNamingTemplate;

			switch (this.OutputFormat)
			{
				case OutputFormat.JPEG:
				_twainSource.ImageFileFormat = ImageFormat.Jpeg;
				break;
				case OutputFormat.PNG:
				_twainSource.ImageFileFormat = ImageFormat.Png;
				break;
				case OutputFormat.TIFF:
				_twainSource.ImageFileFormat = ImageFormat.Tiff;
				break;
				case OutputFormat.BMP:
				_twainSource.ImageFileFormat = ImageFormat.Bmp;
				break;
				// 				case OutputFormats.GIF:
				// 					twainSource.ImageFileFormat = ImageFormat.Gif;
				// 					break;
			}

			_twainSource.ShowTwainUi();
		}

		private void twainSource_TransferStarted(object sender, EventArgs e)
		{
			if (this.TransferStarted != null)
			{
				this.TransferStarted(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// When TWAIN finished the transfer, puts results into workspace
		/// </summary>
		private void twainSource_TransferFinished(object sender, EventArgs e)
		{
			if (this.TransferFinished != null)
			{
				this.TransferFinished(this, _twainSource.ScannedImages);
			}
		}

		private static long TranslateTiffCompression(TiffCompression compression)
		{
			switch (compression)
			{
				case TiffCompression.None: return (long) EncoderValue.CompressionNone;
				case TiffCompression.RLE: return (long) EncoderValue.CompressionRle;
				case TiffCompression.CCITT3: return (long) EncoderValue.CompressionCCITT3;
				case TiffCompression.CCITT4: return (long) EncoderValue.CompressionCCITT4;
				case TiffCompression.LZW: return (long) EncoderValue.CompressionLZW;
				default: return (long) EncoderValue.CompressionLZW;
			}
		}

		///<summary>
		/// Opens Options dialog.
		///</summary>
		///<param name="scanInstance">Scan object to show options for.</param>
		///<exception cref="ScanException"></exception>
		public static void ShowOptionsDialog(Scan scanInstance)
		{
			if (scanInstance == null)
			{
				throw new ScanException("Invalid constructor parameter.");
			}

			using (OptionsDialog dlg = new OptionsDialog(scanInstance))
			{
				if (dlg.ShowDialog() == DialogResult.OK)
				{
					scanInstance.SaveOptions();
				}
			}
		}
	}
}
