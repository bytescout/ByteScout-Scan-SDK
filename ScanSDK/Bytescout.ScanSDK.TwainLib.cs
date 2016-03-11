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
using System.Collections;
using System.Runtime.InteropServices;
using System.Windows.Forms;


namespace TwainLib
{
	internal enum TwainCommand
	{
		Not = -1,
		Null = 0,
		TransferReady = 1,
		CloseRequest = 2,
		CloseOk = 3,
		DeviceEvent = 4
	}
	
	internal class Twain
	{
		internal IntPtr hwnd;
		private TW_IDENTITY appid;
		private TW_IDENTITY srcds;
		private TW_EVENT evtmsg;
		private WINMSG winmsg;
		
		private const short CountryUSA = 1;
		private const short LanguageUSA = 13;

		public Twain()
		{
			appid = new TW_IDENTITY();
			appid.Id = IntPtr.Zero;
			appid.Version.MajorNum = 1;
			appid.Version.MinorNum = 0;
			appid.Version.Language = LanguageUSA;
			appid.Version.Country = CountryUSA;
			appid.Version.Info = "";
			appid.ProtocolMajor = TwProtocol.Major;
			appid.ProtocolMinor = TwProtocol.Minor;
			appid.SupportedGroups = (int) (TwDG.Image | TwDG.Control);
			appid.Manufacturer = "Bytescout Software";
			appid.ProductFamily = "Development Tools";
			appid.ProductName = "Scan SDK";

			srcds = new TW_IDENTITY();
			srcds.Id = IntPtr.Zero;

			evtmsg.EventPtr = Marshal.AllocHGlobal(Marshal.SizeOf(winmsg));
		}

		~Twain()
		{
			Marshal.FreeHGlobal(evtmsg.EventPtr);
		}

		public void Init(IntPtr hwndp)
		{
			Finish();

			TwRC rc = DSMparent(appid, IntPtr.Zero, TwDG.Control, TwDAT.Parent, TwMSG.OpenDSM, ref hwndp);

			if (rc == TwRC.Success)
			{
				rc = DSMident(appid, IntPtr.Zero, TwDG.Control, TwDAT.Identity, TwMSG.GetDefault, srcds);

				if (rc == TwRC.Success)
				{
					hwnd = hwndp;
				}
				else
				{
					rc = DSMparent(appid, IntPtr.Zero, TwDG.Control, TwDAT.Parent, TwMSG.CloseDSM, ref hwndp);
				}
			}
		}

		public String Select()
		{
			CloseSrc();

			if (appid.Id == IntPtr.Zero)
			{
				Init(hwnd);

				if (appid.Id == IntPtr.Zero)
				{
					return "";
				}
			}

			TwRC rc = DSMident(appid, IntPtr.Zero, TwDG.Control, TwDAT.Identity, TwMSG.UserSelect, srcds);

			return srcds.ProductName;
		}

		public void Acquire()
		{
			CloseSrc();

			if (appid.Id == IntPtr.Zero)
			{
				Init(hwnd);

				if (appid.Id == IntPtr.Zero)
				{
					return;
				}
			}

			TwRC rc = DSMident(appid, IntPtr.Zero, TwDG.Control, TwDAT.Identity, TwMSG.OpenDS, srcds);

			if (rc != TwRC.Success)
			{
				return;
			}

			TwCapability cap = new TwCapability(TwCap.XferCount, 1);

			rc = DScap(appid, srcds, TwDG.Control, TwDAT.Capability, TwMSG.Set, cap);

			if (rc != TwRC.Success)
			{
				CloseSrc();
				return;
			}

			TW_USERINTERFACE guif = new TW_USERINTERFACE();
			guif.ShowUI = 1;
			guif.ModalUI = 1;
			guif.ParentHand = hwnd;

			rc = DSuserif(appid, srcds, TwDG.Control, TwDAT.UserInterface, TwMSG.EnableDS, guif);

			if (rc != TwRC.Success)
			{
				CloseSrc();
				return;
			}
		}

		public ArrayList TransferPictures()
		{
			ArrayList pics = new ArrayList();

			if (srcds.Id == IntPtr.Zero)
			{
				return pics;
			}

			TwRC rc;
			IntPtr hbitmap = IntPtr.Zero;
			TW_PENDINGXFERS pxfr = new TW_PENDINGXFERS();

			do
			{
				pxfr.Count = 0;
				hbitmap = IntPtr.Zero;

				TW_IMAGEINFO iinf = new TW_IMAGEINFO();
				rc = DSiinf(appid, srcds, TwDG.Image, TwDAT.ImageInfo, TwMSG.Get, iinf);

				if (rc != TwRC.Success)
				{
					CloseSrc();
					return pics;
				}

				rc = DSixfer(appid, srcds, TwDG.Image, TwDAT.ImageNativeXfer, TwMSG.Get, ref hbitmap);

				if (rc != TwRC.XferDone)
				{
					CloseSrc();
					return pics;
				}

				rc = DSpxfer(appid, srcds, TwDG.Control, TwDAT.PendingXfers, TwMSG.EndXfer, pxfr);

				if (rc != TwRC.Success)
				{
					CloseSrc();
					return pics;
				}

				pics.Add(hbitmap);
			}
			while (pxfr.Count != 0);

			rc = DSpxfer(appid, srcds, TwDG.Control, TwDAT.PendingXfers, TwMSG.Reset, pxfr);

			return pics;
		}
		
		public TwainCommand PassMessage(ref Message m)
		{
			if (srcds.Id == IntPtr.Zero)
			{
				return TwainCommand.Not;
			}

			int pos = GetMessagePos();

			winmsg.hwnd = m.HWnd;
			winmsg.message = m.Msg;
			winmsg.wParam = m.WParam;
			winmsg.lParam = m.LParam;
			winmsg.time = GetMessageTime();

			unchecked
			{
				winmsg.x = (short) pos;
				winmsg.y = (short) (pos >> 16);
			}

			Marshal.StructureToPtr(winmsg, evtmsg.EventPtr, false);
			evtmsg.Message = 0;

			TwRC rc = DSevent(appid, srcds, TwDG.Control, TwDAT.Event, TwMSG.ProcessEvent, ref evtmsg);

			if (rc == TwRC.NotDSEvent)
			{
				return TwainCommand.Not;
			}

			if (evtmsg.Message == (short) TwMSG.XFerReady)
			{
				return TwainCommand.TransferReady;
			}

			if (evtmsg.Message == (short) TwMSG.CloseDSReq)
			{
				return TwainCommand.CloseRequest;
			}

			if (evtmsg.Message == (short) TwMSG.CloseDSOK)
			{
				return TwainCommand.CloseOk;
			}

			if (evtmsg.Message == (short) TwMSG.DeviceEvent)
			{
				return TwainCommand.DeviceEvent;
			}
			
			return TwainCommand.Null;
		}
		
		public void CloseSrc()
		{
			TwRC rc;

			if (srcds.Id != IntPtr.Zero)
			{
				TW_USERINTERFACE guif = new TW_USERINTERFACE();
				rc = DSuserif(appid, srcds, TwDG.Control, TwDAT.UserInterface, TwMSG.DisableDS, guif);
				rc = DSMident(appid, IntPtr.Zero, TwDG.Control, TwDAT.Identity, TwMSG.CloseDS, srcds);
			}
		}
		
		public void Finish()
		{
			TwRC rc;

			CloseSrc();

			if (appid.Id != IntPtr.Zero)
			{
				rc = DSMparent(appid, IntPtr.Zero, TwDG.Control, TwDAT.Parent, TwMSG.CloseDSM, ref hwnd);
			}

			appid.Id = IntPtr.Zero;
		}
		
		// ------ DSM entry point DAT_ variants:
		[DllImport("twain_32.dll", EntryPoint = "#1")]
		private static extern TwRC DSMparent([In, Out] TW_IDENTITY origin, IntPtr zeroptr, TwDG dg, TwDAT dat, TwMSG msg, ref IntPtr refptr);
		[DllImport("twain_32.dll", EntryPoint = "#1")]
		private static extern TwRC DSMident([In, Out] TW_IDENTITY origin, IntPtr zeroptr, TwDG dg, TwDAT dat, TwMSG msg, [In, Out] TW_IDENTITY idds);
		[DllImport("twain_32.dll", EntryPoint = "#1")]
		private static extern TwRC DSMstatus([In, Out] TW_IDENTITY origin, IntPtr zeroptr, TwDG dg, TwDAT dat, TwMSG msg, [In, Out] TW_STATUS dsmstat);

		// ------ DSM entry point DAT_ variants to DS:
		[DllImport("twain_32.dll", EntryPoint = "#1")]
		private static extern TwRC DSuserif([In, Out] TW_IDENTITY origin, [In, Out] TW_IDENTITY dest, TwDG dg, TwDAT dat, TwMSG msg, TW_USERINTERFACE guif);
		[DllImport("twain_32.dll", EntryPoint = "#1")]
		private static extern TwRC DSevent([In, Out] TW_IDENTITY origin, [In, Out] TW_IDENTITY dest, TwDG dg, TwDAT dat, TwMSG msg, ref TW_EVENT evt);
		[DllImport("twain_32.dll", EntryPoint = "#1")]
		private static extern TwRC DSstatus([In, Out] TW_IDENTITY origin, [In] TW_IDENTITY dest, TwDG dg, TwDAT dat, TwMSG msg, [In, Out] TW_STATUS dsmstat);
		[DllImport("twain_32.dll", EntryPoint = "#1")]
		private static extern TwRC DScap([In, Out] TW_IDENTITY origin, [In] TW_IDENTITY dest, TwDG dg, TwDAT dat, TwMSG msg, [In, Out] TwCapability capa);
		[DllImport("twain_32.dll", EntryPoint = "#1")]
		private static extern TwRC DSiinf([In, Out] TW_IDENTITY origin, [In] TW_IDENTITY dest, TwDG dg, TwDAT dat, TwMSG msg, [In, Out] TW_IMAGEINFO imginf);
		[DllImport("twain_32.dll", EntryPoint = "#1")]
		private static extern TwRC DSixfer([In, Out] TW_IDENTITY origin, [In] TW_IDENTITY dest, TwDG dg, TwDAT dat, TwMSG msg, ref IntPtr hbitmap);
		[DllImport("twain_32.dll", EntryPoint = "#1")]
		private static extern TwRC DSpxfer([In, Out] TW_IDENTITY origin, [In] TW_IDENTITY dest, TwDG dg, TwDAT dat, TwMSG msg, [In, Out] TW_PENDINGXFERS pxfr);
		
		[DllImport("kernel32.dll", ExactSpelling = true)]
		internal static extern IntPtr GlobalAlloc(int flags, int size);
		[DllImport("kernel32.dll", ExactSpelling = true)]
		internal static extern IntPtr GlobalLock(IntPtr handle);
		[DllImport("kernel32.dll", ExactSpelling = true)]
		internal static extern bool GlobalUnlock(IntPtr handle);
		[DllImport("kernel32.dll", ExactSpelling = true)]
		internal static extern IntPtr GlobalFree(IntPtr handle);

		[DllImport("user32.dll", ExactSpelling = true)]
		private static extern int GetMessagePos();
		[DllImport("user32.dll", ExactSpelling = true)]
		private static extern int GetMessageTime();
		
		[DllImport("gdi32.dll", ExactSpelling = true)]
		private static extern int GetDeviceCaps(IntPtr hDC, int nIndex);

		[DllImport("gdi32.dll", CharSet = CharSet.Auto)]
		private static extern IntPtr CreateDC(string szdriver, string szdevice, string szoutput, IntPtr devmode);

		[DllImport("gdi32.dll", ExactSpelling = true)]
		private static extern bool DeleteDC(IntPtr hdc);
		
		public static int ScreenBitDepth
		{
			get
			{
				IntPtr screenDC = CreateDC("DISPLAY", null, null, IntPtr.Zero);
				int bitDepth = GetDeviceCaps(screenDC, 12);
				bitDepth *= GetDeviceCaps(screenDC, 14);
				DeleteDC(screenDC);
				return bitDepth;
			}
		}
		
		[StructLayout(LayoutKind.Sequential, Pack = 4)]
		internal struct WINMSG
		{
			public IntPtr hwnd;
			public int message;
			public IntPtr wParam;
			public IntPtr lParam;
			public int time;
			public int x;
			public int y;
		}
	} 

	internal class TwProtocol
	{                           
		// TWON_PROTOCOL...
		public const short Major = 1;
		public const short Minor = 9;
	}



	[Flags]
	internal enum TwDG : short
	{   
		// DG_.....
		Control = 0x0001,
		Image = 0x0002,
		Audio = 0x0004
	}

	internal enum TwDAT : short
	{                           
		// DAT_....
		Null = 0x0000,
		Capability = 0x0001,
		Event = 0x0002,
		Identity = 0x0003,
		Parent = 0x0004,
		PendingXfers = 0x0005,
		SetupMemXfer = 0x0006,
		SetupFileXfer = 0x0007,
		Status = 0x0008,
		UserInterface = 0x0009,
		XferGroup = 0x000a,
		TwunkIdentity = 0x000b,
		CustomDSData = 0x000c,
		DeviceEvent = 0x000d,
		FileSystem = 0x000e,
		PassThru = 0x000f,

		ImageInfo = 0x0101,
		ImageLayout = 0x0102,
		ImageMemXfer = 0x0103,
		ImageNativeXfer = 0x0104,
		ImageFileXfer = 0x0105,
		CieColor = 0x0106,
		GrayResponse = 0x0107,
		RGBResponse = 0x0108,
		JpegCompression = 0x0109,
		Palette8 = 0x010a,
		ExtImageInfo = 0x010b,

		SetupFileXfer2 = 0x0301
	}

	internal enum TwMSG : short
	{                           
		// MSG_.....
		Null = 0x0000,
		Get = 0x0001,
		GetCurrent = 0x0002,
		GetDefault = 0x0003,
		GetFirst = 0x0004,
		GetNext = 0x0005,
		Set = 0x0006,
		Reset = 0x0007,
		QuerySupport = 0x0008,

		XFerReady = 0x0101,
		CloseDSReq = 0x0102,
		CloseDSOK = 0x0103,
		DeviceEvent = 0x0104,

		CheckStatus = 0x0201,

		OpenDSM = 0x0301,
		CloseDSM = 0x0302,

		OpenDS = 0x0401,
		CloseDS = 0x0402,
		UserSelect = 0x0403,

		DisableDS = 0x0501,
		EnableDS = 0x0502,
		EnableDSUIOnly = 0x0503,

		ProcessEvent = 0x0601,

		EndXfer = 0x0701,
		StopFeeder = 0x0702,

		ChangeDirectory = 0x0801,
		CreateDirectory = 0x0802,
		Delete = 0x0803,
		FormatMedia = 0x0804,
		GetClose = 0x0805,
		GetFirstFile = 0x0806,
		GetInfo = 0x0807,
		GetNextFile = 0x0808,
		Rename = 0x0809,
		Copy = 0x080A,
		AutoCaptureDir = 0x080B,

		PassThru = 0x0901
	}

	internal enum TwRC : short
	{                           
		// TWRC_....
		Success = 0x0000,
		Failure = 0x0001,
		CheckStatus = 0x0002,
		Cancel = 0x0003,
		DSEvent = 0x0004,
		NotDSEvent = 0x0005,
		XferDone = 0x0006,
		EndOfList = 0x0007,
		InfoNotSupported = 0x0008,
		DataNotAvailable = 0x0009
	}

	internal enum TwCC : short
	{                           
		// TWCC_....
		Success = 0x0000,
		Bummer = 0x0001,
		LowMemory = 0x0002,
		NoDS = 0x0003,
		MaxConnections = 0x0004,
		OperationError = 0x0005,
		BadCap = 0x0006,
		BadProtocol = 0x0009,
		BadValue = 0x000a,
		SeqError = 0x000b,
		BadDest = 0x000c,
		CapUnsupported = 0x000d,
		CapBadOperation = 0x000e,
		CapSeqError = 0x000f,
		Denied = 0x0010,
		FileExists = 0x0011,
		FileNotFound = 0x0012,
		NotEmpty = 0x0013,
		PaperJam = 0x0014,
		PaperDoubleFeed = 0x0015,
		FileWriteError = 0x0016,
		CheckDeviceOnline = 0x0017
	}

	internal enum TwOn : short
	{                           
		// TWON_....
		Array = 0x0003,
		Enum = 0x0004,
		One = 0x0005,
		Range = 0x0006,
		DontCare = -1
	}

	internal enum TwType : short
	{                           
		// TWTY_....
		Int8 = 0x0000,
		Int16 = 0x0001,
		Int32 = 0x0002,
		UInt8 = 0x0003,
		UInt16 = 0x0004,
		UInt32 = 0x0005,
		Bool = 0x0006,
		Fix32 = 0x0007,
		Frame = 0x0008,
		Str32 = 0x0009,
		Str64 = 0x000a,
		Str128 = 0x000b,
		Str255 = 0x000c,
		Str1024 = 0x000d,
		Str512 = 0x000e
	}

	internal enum TwCap : short
	{
		XferCount = 0x0001,		// CAP_XFERCOUNT
		// ICAP_...
		ICompression = 0x0100,
		IPixelType = 0x0101,
		IUnits = 0x0102,
		IXferMech = 0x0103
	}

	[StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
	internal class TW_IDENTITY
	{                           
		// TW_IDENTITY
		public IntPtr Id;
		public TW_VERSION Version;
		public short ProtocolMajor;
		public short ProtocolMinor;
		public int SupportedGroups;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 34)]
		public string Manufacturer;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 34)]
		public string ProductFamily;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 34)]
		public string ProductName;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 2, CharSet = CharSet.Ansi)]
	internal struct TW_VERSION
	{                           
		public short MajorNum;
		public short MinorNum;
		public short Language;
		public short Country;
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 34)]
		public string Info;
	}
	
	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	internal class TW_USERINTERFACE
	{                           
		public short ShowUI; // bool is strictly 32 bit, so use short
		public short ModalUI;
		public IntPtr ParentHand;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	internal class TW_STATUS
	{                           
		public short ConditionCode; // TwCC
		public short Reserved;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	internal struct TW_EVENT
	{                           
		public IntPtr EventPtr;
		public short Message;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	internal class TW_IMAGEINFO
	{                           
		public int XResolution;
		public int YResolution;
		public int ImageWidth;
		public int ImageLength;
		public short SamplesPerPixel;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
		public short[] BitsPerSample;
		public short BitsPerPixel;
		public short Planar;
		public short PixelType;
		public short Compression;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	internal class TW_PENDINGXFERS
	{                           
		public short Count;
		public int EOJ;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	internal struct TW_FIX32
	{                                    
		public short Whole;
		public ushort Frac;

		public float ToFloat()
		{
			return (float) Whole + ((float) Frac / 65536.0f);
		}

		public void FromFloat(float f)
		{
			int i = (int) ((f * 65536.0f) + 0.5f);
			Whole = (short) (i >> 16);
			Frac = (ushort) (i & 0x0000ffff);
		}
	}

	[StructLayout(LayoutKind.Sequential, Pack = 2)]
	internal class TwCapability
	{
		// TW_CAPABILITY
		public short Cap;
		public short ConType;
		public IntPtr Handle;

		public TwCapability(TwCap cap)
		{
			Cap = (short) cap;
			ConType = -1;
		}

		public TwCapability(TwCap cap, short sval)
		{
			Cap = (short) cap;
			ConType = (short) TwOn.One;
			Handle = Twain.GlobalAlloc(0x42, 6);
			IntPtr pv = Twain.GlobalLock(Handle);
			Marshal.WriteInt16(pv, 0, (short) TwType.Int16);
			Marshal.WriteInt32(pv, 2, (int) sval);
			Twain.GlobalUnlock(Handle);
		}

		~TwCapability()
		{
			if (Handle != IntPtr.Zero)
			{
				Twain.GlobalFree(Handle);
			}
		}
	}
}