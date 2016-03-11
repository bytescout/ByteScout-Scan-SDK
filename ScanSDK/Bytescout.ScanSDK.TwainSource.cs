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
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Reflection;
using TwainLib;
using System.IO;


namespace Bytescout.Scan
{
	///<summary>
	/// Used to specify aquired images destination.
	///</summary>
	public enum SaveTo
	{
		///<summary>
		/// (0) Images will be save to ScannedImages property
		///</summary>
		ImageObject, 
		///<summary>
		/// (1) Images will be saved to files of specified format.
		///</summary>
		File
	};

	internal class TwainSource : IMessageFilter, IDisposable
	{
		private bool _messageFilterIsActive;
		private readonly TwainLib.Twain _twain;
		private int _twainCommandNullCount = 0; // hack: TwainCommand.Null is sended continuously after TWAIN device is not available or when once preview is done
		
		public SaveTo SaveTo = SaveTo.ImageObject;
		public String OutputFolder = "";
		public ImageFormat ImageFileFormat = ImageFormat.Jpeg;
		public int JpegQuality = 95;
		public long TiffCompression = (long) EncoderValue.CompressionLZW;
		public String FileNamingTemplate = "Image_{0:D2}";

		public ArrayList ScannedImages = new ArrayList();
		
		public event EventHandler TransferStarted;
		public event EventHandler TransferFinished;
		
		public TwainSource(IntPtr hwndp)
		{
			_twain = new Twain();
			_twain.Init(hwndp);
		}

		public void Dispose()
		{
			_twain.Finish();
		}
		
		public String SelectTwainSource()
		{
			return _twain.Select();
		}

		// displays the UI of the TWAIN device
		public void ShowTwainUi()
		{
			if (!_messageFilterIsActive)
			{
				_messageFilterIsActive = true;
				ScannedImages.Clear();
				Application.AddMessageFilter(this);
			}

			_twain.Acquire();
		}

		public void EndingScan()
		{
			if (_messageFilterIsActive)
			{
				Application.RemoveMessageFilter(this);
				_messageFilterIsActive = false;

				if (TransferFinished != null)
				{
					TransferFinished(this, EventArgs.Empty);
				}
			}
		}


		// receives messages and handles TWAIN device messages
		public bool PreFilterMessage(ref System.Windows.Forms.Message m)
		{
			TwainCommand twainCommand = _twain.PassMessage(ref m);

			if (twainCommand == TwainCommand.Not)
			{
				return false;
			}

			switch (twainCommand)
			{
				case TwainCommand.CloseRequest:
				{
					EndingScan();
					_twain.CloseSrc();
					break;
				}
				case TwainCommand.CloseOk:
				{
					EndingScan();
					_twain.CloseSrc();
					break;
				}
				case TwainCommand.DeviceEvent:
				{
					break;
				}
				case TwainCommand.TransferReady:
				{
					if (TransferStarted != null)
					{
						TransferStarted(this, EventArgs.Empty);
					}

					ArrayList pics = _twain.TransferPictures();

					for (int i = 0; i < pics.Count; i ++)
					{
						if (SaveTo == SaveTo.ImageObject) // save to Image object
						{
							Bitmap bmp = bitmapFromDIB((IntPtr) pics[i]);


							ScannedImages.Add(bmp);
						}
						else // save to file
						{
							Guid clsid;
							String ext;
							
							Bitmap bmp = bitmapFromDIB((IntPtr) pics[i]);
							
							GetCodecClsid(this.ImageFileFormat, out clsid, out ext);

							String fileName = this.OutputFolder;

							if (String.IsNullOrEmpty(fileName))
							{
								fileName = Path.GetTempPath();
							}

							if (fileName.Length > 0 && !fileName.EndsWith("\\"))
							{
								fileName += "\\";
							}

							String namingTemplate = "Image_{0:D2}";

							if (!String.IsNullOrEmpty(FileNamingTemplate))
							{
								namingTemplate = FileNamingTemplate;
							}

							fileName += String.Format(namingTemplate, i) + ext;

							try
							{
								File.Delete(fileName);
							}
							catch
							{
							}

							try
							{
								if (ImageFileFormat == ImageFormat.Jpeg)
								{
									ImageCodecInfo ici = GetEncoderInfo("image/jpeg");
									EncoderParameters eps = new EncoderParameters(1);
									eps.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, this.JpegQuality);

									bmp.Save(fileName, ici, eps);
								}
								else if (ImageFileFormat == ImageFormat.Tiff)
								{
									ImageCodecInfo ici = GetEncoderInfo("image/tiff");
									long compression = this.TiffCompression;
									EncoderParameters eps = new EncoderParameters(1);
									eps.Param[0] = new EncoderParameter(Encoder.Compression, compression);
									
									bmp.Save(fileName, ici, eps);
								}
								else 
								{
									bmp.Save(fileName, ImageFileFormat);
								}

								ScannedImages.Add(fileName);
							}
							catch (System.Exception e)
							{
								MessageBox.Show(e.Message);
							}
							finally
							{
								bmp.Dispose();
							}
						}
					}

					EndingScan();
					_twain.CloseSrc();
					break;
				}				
				case TwainCommand.Null:
				{
					_twainCommandNullCount++;

					if (_twainCommandNullCount > 25)
					{
						_twainCommandNullCount = 0;
						EndingScan();
						_twain.CloseSrc();
					}
					break;
				}
			}

			return true;
		}


		/// <summary>
		/// converts the TWAIN device result to .NET Bitmap
		/// </summary>
		/// <param name="dibhand">pointer to aquired image</param>
		/// <returns>TWAIN device as .NET Bitmap</returns>
		private Bitmap bitmapFromDIB(IntPtr dibhand)
		{
			IntPtr bmpptr = GlobalLock(dibhand);
			IntPtr pixptr = GetPixelInfo(bmpptr);

			IntPtr pBmp = IntPtr.Zero;
			int status = GdipCreateBitmapFromGdiDib(bmpptr, pixptr, ref pBmp);

			if ((status == 0) && (pBmp != IntPtr.Zero))
			{
				MethodInfo mi = typeof(Bitmap).GetMethod("FromGDIplus", BindingFlags.Static | BindingFlags.NonPublic);

				if (mi == null)
				{
					return null;
				}

				Bitmap result = new Bitmap(mi.Invoke(null, new object[] { pBmp }) as Bitmap);

				GlobalFree(dibhand);
				dibhand = IntPtr.Zero;

				return result;
			}
			else
			{
				return null;
			}
		}

// 		private String fileFromDIB(IntPtr dibhand, int index)
// 		{
// 			IntPtr bmpptr = GlobalLock(dibhand);
// 			IntPtr pixptr = GetPixelInfo(bmpptr);
// 
// 			IntPtr pBmp = IntPtr.Zero;
// 			int status = GdipCreateBitmapFromGdiDib(bmpptr, pixptr, ref pBmp);
// 
// 			if (status == 0 && pBmp != IntPtr.Zero)
// 			{
// 				Guid clsid;
// 				String ext;
// 
// 				GetCodecClsid(this.ImageFileFormat, out clsid, out ext);
// 
// 				String fileName = this.OutputFolder;
// 
// 				if (fileName.Length > 0 && !fileName.EndsWith("\\"))
// 				{
// 					fileName += "\\";
// 				}
// 
// 				fileName += "Image" + index.ToString() + ext;
// 
// 				try
// 				{
// 					File.Delete(fileName);
// 				}
// 				catch
// 				{
// 				}
// 
// 				status = GdipSaveImageToFile(bmpptr, fileName, ref clsid, IntPtr.Zero);
// 				
// 				GdipDisposeImage(bmpptr);
// 
// 				return fileName;
// 			}
// 			else
// 			{
// 				return null;
// 			}
// 		}

		private static bool GetCodecClsid(ImageFormat imageFormat, out Guid clsid, out String ext)
		{
			clsid = Guid.Empty;
			ext = String.Empty;

			foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageEncoders())
			{
				if (codec.FormatID == imageFormat.Guid)
				{
					clsid = codec.Clsid;
					ext = codec.FilenameExtension.Split(';')[0];
					ext = ext.Replace("*", "");
					return true;
				}
			}

			return false;
		}

		internal static ImageCodecInfo GetEncoderInfo(String mimeType)
		{
			ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();

			for (int j = 0; j < encoders.Length; ++j)
			{
				if (encoders[j].MimeType == mimeType)
				{
					return encoders[j];
				}
			}

			return null;
		}

		private IntPtr GetPixelInfo(IntPtr bmpptr)
		{
			BITMAPINFOHEADER bmi = new BITMAPINFOHEADER();
			Marshal.PtrToStructure(bmpptr, bmi);

			Rectangle bmprect = new Rectangle(0, 0, bmi.biWidth, bmi.biHeight);

			if (bmi.biSizeImage == 0)
			{
				bmi.biSizeImage = ((((bmi.biWidth * bmi.biBitCount) + 31) & ~31) >> 3) * bmi.biHeight;
			}

			int p = bmi.biClrUsed;

			if ((p == 0) && (bmi.biBitCount <= 8))
			{
				p = 1 << bmi.biBitCount;
			}

			p = (p * 4) + bmi.biSize + (int) bmpptr;

			return (IntPtr) p;
		}


		[DllImport("gdiplus.dll", ExactSpelling = true)]
		private static extern int GdipCreateBitmapFromGdiDib(IntPtr bminfo, IntPtr pixdat, ref IntPtr image);

		[DllImport("gdiplus.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
		internal static extern int GdipSaveImageToFile(IntPtr image, string filename, [In] ref Guid clsid, IntPtr encparams);

		[DllImport("gdiplus.dll", ExactSpelling = true)]
		internal static extern int GdipDisposeImage(IntPtr image);

		[DllImport("kernel32.dll", ExactSpelling = true)]
		private static extern IntPtr GlobalLock(IntPtr handle);

		[DllImport("kernel32.dll", ExactSpelling = true)]
		private static extern IntPtr GlobalFree(IntPtr handle);

		[StructLayout(LayoutKind.Sequential, Pack = 2)]
		private class BITMAPINFOHEADER
		{
			public int biSize = 0;
			public int biWidth = 0;
			public int biHeight = 0;
			public short biPlanes = 0;
			public short biBitCount = 0;
			public int biCompression = 0;
			public int biSizeImage = 0;
			public int biXPelsPerMeter = 0;
			public int biYPelsPerMeter = 0;
			public int biClrUsed = 0;
			public int biClrImportant = 0;
		}

//	typedef enum {
// 		Ok = 0,
// 		GenericError = 1,
// 		InvalidParameter = 2,
// 		OutOfMemory = 3,
// 		ObjectBusy = 4,
// 		InsufficientBuffer = 5,
// 		NotImplemented = 6,
// 		Win32Error = 7,
// 		WrongState = 8,
// 		Aborted = 9,
// 		FileNotFound = 10,
// 		ValueOverflow = 11,
// 		AccessDenied = 12,
// 		UnknownImageFormat = 13,
// 		FontFamilyNotFound = 14,
// 		FontStyleNotFound = 15,
// 		NotTrueTypeFont = 16,
// 		UnsupportedGdiplusVersion = 17,
// 		GdiplusNotInitialized = 18,
// 		PropertyNotFound = 19,
// 		PropertyNotSupported = 20,
// 		ProfileNotFound = 21
// 	} Status;
// 

	}
}