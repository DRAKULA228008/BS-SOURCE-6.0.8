using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class lzip
{
	public struct fileStat
	{
		public int index;

		public int compSize;

		public int uncompSize;

		public int nameSize;

		public string name;

		public int commentSize;

		public string comment;

		public bool isDirectory;

		public bool isSupported;

		public bool isEncrypted;
	}

	private const string libname = "zipw";

	public static List<string> ninfo = new List<string>();

	public static List<long> uinfo = new List<long>();

	public static List<long> cinfo = new List<long>();

	public static int zipFiles;

	public static int zipFolders;

	public static int cProgress = 0;

	[DllImport("zipw", CallingConvention = CallingConvention.Cdecl)]
	internal static extern int zsetPermissions(string filePath, string _user, string _group, string _other);

	[DllImport("zipw")]
	internal static extern bool zipValidateFile(string zipArchive, IntPtr FileBuffer, int fileBufferLength);

	[DllImport("zipw")]
	internal static extern int zipGetTotalFiles(string zipArchive, IntPtr FileBuffer, int fileBufferLength);

	[DllImport("zipw")]
	internal static extern int zipGetTotalEntries(string zipArchive, IntPtr FileBuffer, int fileBufferLength);

	[DllImport("zipw")]
	internal static extern int zipGetInfoA(string zipArchive, IntPtr total, IntPtr FileBuffer, int fileBufferLength);

	[DllImport("zipw")]
	internal static extern IntPtr zipGetInfo(string zipArchive, int size, IntPtr unc, IntPtr comp, IntPtr FileBuffer, int fileBufferLength);

	[DllImport("zipw")]
	internal static extern void releaseBuffer(IntPtr buffer);

	[DllImport("zipw")]
	internal static extern int zipGetEntrySize(string zipArchive, string entry, IntPtr FileBuffer, int fileBufferLength);

	[DllImport("zipw")]
	internal static extern bool zipEntryExists(string zipArchive, string entry, IntPtr FileBuffer, int fileBufferLength);

	[DllImport("zipw")]
	internal static extern int zipCD(int levelOfCompression, string zipArchive, string inFilePath, string fileName, string comment, [MarshalAs(UnmanagedType.LPStr)] string password, bool useBz2);

	[DllImport("zipw")]
	internal static extern bool zipBuf2File(int levelOfCompression, string zipArchive, string arc_filename, IntPtr buffer, int bufferSize, string comment, [MarshalAs(UnmanagedType.LPStr)] string password, bool useBz2);

	[DllImport("zipw")]
	internal static extern int zipDeleteFile(string zipArchive, string arc_filename, string tempArchive);

	[DllImport("zipw")]
	internal static extern int zipEntry2Buffer(string zipArchive, string entry, IntPtr buffer, int bufferSize, IntPtr FileBuffer, int fileBufferLength, [MarshalAs(UnmanagedType.LPStr)] string password);

	[DllImport("zipw")]
	internal static extern IntPtr zipCompressBuffer(IntPtr source, int sourceLen, int levelOfCompression, ref int v);

	[DllImport("zipw")]
	internal static extern IntPtr zipDecompressBuffer(IntPtr source, int sourceLen, ref int v);

	[DllImport("zipw")]
	internal static extern int zipEX(string zipArchive, string outPath, IntPtr progress, IntPtr FileBuffer, int fileBufferLength, IntPtr proc, [MarshalAs(UnmanagedType.LPStr)] string password);

	[DllImport("zipw")]
	internal static extern int zipEntry(string zipArchive, string arc_filename, string outpath, IntPtr FileBuffer, int fileBufferLength, IntPtr proc, [MarshalAs(UnmanagedType.LPStr)] string password);

	[DllImport("zipw")]
	internal static extern uint getEntryDateTime(string zipArchive, string arc_filename, IntPtr FileBuffer, int fileBufferLength);

	[DllImport("zipw")]
	internal static extern int zipGzip(IntPtr source, int sourceLen, IntPtr outBuffer, int levelOfCompression, bool addHeader, bool addFooter);

	[DllImport("zipw")]
	internal static extern int zipUnGzip(IntPtr source, int sourceLen, IntPtr outBuffer, int outLen, bool hasHeader, bool hasFooter);

	[DllImport("zipw")]
	internal static extern int zipUnGzip2(IntPtr source, int sourceLen, IntPtr outBuffer, int outLen);

	public static int setFilePermissions(string filePath, string _user, string _group, string _other)
	{
		return zsetPermissions(filePath, _user, _group, _other);
	}

	public static bool validateFile(string zipArchive, byte[] FileBuffer = null)
	{
		bool flag = false;
		if (FileBuffer != null)
		{
			GCHandle gCHandle = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
			flag = zipValidateFile(null, gCHandle.AddrOfPinnedObject(), FileBuffer.Length);
			gCHandle.Free();
			return flag;
		}
		return zipValidateFile(zipArchive, IntPtr.Zero, 0);
	}

	public static int getTotalFiles(string zipArchive, byte[] FileBuffer = null)
	{
		if (FileBuffer != null)
		{
			GCHandle gCHandle = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
			int result = zipGetTotalFiles(null, gCHandle.AddrOfPinnedObject(), FileBuffer.Length);
			gCHandle.Free();
			return result;
		}
		return zipGetTotalFiles(zipArchive, IntPtr.Zero, 0);
	}

	public static int getTotalEntries(string zipArchive, byte[] FileBuffer = null)
	{
		if (FileBuffer != null)
		{
			GCHandle gCHandle = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
			int result = zipGetTotalEntries(null, gCHandle.AddrOfPinnedObject(), FileBuffer.Length);
			gCHandle.Free();
			return result;
		}
		return zipGetTotalEntries(zipArchive, IntPtr.Zero, 0);
	}

	public static long getFileInfo(string zipArchive, string path = null, byte[] FileBuffer = null)
	{
		ninfo.Clear();
		uinfo.Clear();
		cinfo.Clear();
		zipFiles = 0;
		zipFolders = 0;
		int[] array = new int[1];
		GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
		int num;
		if (FileBuffer != null)
		{
			GCHandle gCHandle2 = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
			num = zipGetInfoA(null, gCHandle.AddrOfPinnedObject(), gCHandle2.AddrOfPinnedObject(), FileBuffer.Length);
			gCHandle2.Free();
		}
		else
		{
			num = zipGetInfoA(zipArchive, gCHandle.AddrOfPinnedObject(), IntPtr.Zero, 0);
		}
		gCHandle.Free();
		if (num <= 0)
		{
			return -1L;
		}
		IntPtr zero = IntPtr.Zero;
		uint[] array2 = new uint[array[0]];
		uint[] array3 = new uint[array[0]];
		GCHandle gCHandle3 = GCHandle.Alloc(array2, GCHandleType.Pinned);
		GCHandle gCHandle4 = GCHandle.Alloc(array3, GCHandleType.Pinned);
		if (FileBuffer != null)
		{
			GCHandle gCHandle5 = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
			zero = zipGetInfo(null, num, gCHandle3.AddrOfPinnedObject(), gCHandle4.AddrOfPinnedObject(), gCHandle5.AddrOfPinnedObject(), FileBuffer.Length);
			gCHandle5.Free();
		}
		else
		{
			zero = zipGetInfo(zipArchive, num, gCHandle3.AddrOfPinnedObject(), gCHandle4.AddrOfPinnedObject(), IntPtr.Zero, 0);
		}
		if (zero == IntPtr.Zero)
		{
			gCHandle3.Free();
			gCHandle4.Free();
			return -2L;
		}
		string s = Marshal.PtrToStringAuto(zero);
		StringReader stringReader = new StringReader(s);
		long num2 = 0L;
		for (int i = 0; i < array[0]; i++)
		{
			string item;
			if ((item = stringReader.ReadLine()) != null)
			{
				ninfo.Add(item);
			}
			if (array2 != null)
			{
				uinfo.Add(array2[i]);
				num2 += array2[i];
				if (array2[i] != 0)
				{
					zipFiles++;
				}
				else
				{
					zipFolders++;
				}
			}
			if (array3 != null)
			{
				cinfo.Add(array3[i]);
			}
		}
		stringReader.Close();
		stringReader.Dispose();
		gCHandle3.Free();
		gCHandle4.Free();
		releaseBuffer(zero);
		array = null;
		array2 = null;
		array3 = null;
		return num2;
	}

	public static int getEntrySize(string zipArchive, string entry, byte[] FileBuffer = null)
	{
		if (FileBuffer != null)
		{
			GCHandle gCHandle = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
			int result = zipGetEntrySize(null, entry, gCHandle.AddrOfPinnedObject(), FileBuffer.Length);
			gCHandle.Free();
			return result;
		}
		return zipGetEntrySize(zipArchive, entry, IntPtr.Zero, 0);
	}

	public static bool entryExists(string zipArchive, string entry, byte[] FileBuffer = null)
	{
		if (FileBuffer != null)
		{
			GCHandle gCHandle = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
			bool result = zipEntryExists(null, entry, gCHandle.AddrOfPinnedObject(), FileBuffer.Length);
			gCHandle.Free();
			return result;
		}
		return zipEntryExists(zipArchive, entry, IntPtr.Zero, 0);
	}

	public static bool compressBuffer(byte[] source, ref byte[] outBuffer, int levelOfCompression)
	{
		if (levelOfCompression < 0)
		{
			levelOfCompression = 0;
		}
		if (levelOfCompression > 10)
		{
			levelOfCompression = 10;
		}
		GCHandle gCHandle = GCHandle.Alloc(source, GCHandleType.Pinned);
		int v = 0;
		IntPtr intPtr = zipCompressBuffer(gCHandle.AddrOfPinnedObject(), source.Length, levelOfCompression, ref v);
		if (v == 0 || intPtr == IntPtr.Zero)
		{
			gCHandle.Free();
			releaseBuffer(intPtr);
			return false;
		}
		Array.Resize(ref outBuffer, v);
		Marshal.Copy(intPtr, outBuffer, 0, v);
		gCHandle.Free();
		releaseBuffer(intPtr);
		return true;
	}

	public static int compressBufferFixed(byte[] source, ref byte[] outBuffer, int levelOfCompression, bool safe = true)
	{
		if (levelOfCompression < 0)
		{
			levelOfCompression = 0;
		}
		if (levelOfCompression > 10)
		{
			levelOfCompression = 10;
		}
		GCHandle gCHandle = GCHandle.Alloc(source, GCHandleType.Pinned);
		int v = 0;
		IntPtr intPtr = zipCompressBuffer(gCHandle.AddrOfPinnedObject(), source.Length, levelOfCompression, ref v);
		if (v == 0 || intPtr == IntPtr.Zero)
		{
			gCHandle.Free();
			releaseBuffer(intPtr);
			return 0;
		}
		if (v > outBuffer.Length)
		{
			if (safe)
			{
				gCHandle.Free();
				releaseBuffer(intPtr);
				return 0;
			}
			v = outBuffer.Length;
		}
		Marshal.Copy(intPtr, outBuffer, 0, v);
		gCHandle.Free();
		releaseBuffer(intPtr);
		return v;
	}

	public static byte[] compressBuffer(byte[] source, int levelOfCompression)
	{
		if (levelOfCompression < 0)
		{
			levelOfCompression = 0;
		}
		if (levelOfCompression > 10)
		{
			levelOfCompression = 10;
		}
		GCHandle gCHandle = GCHandle.Alloc(source, GCHandleType.Pinned);
		int v = 0;
		IntPtr intPtr = zipCompressBuffer(gCHandle.AddrOfPinnedObject(), source.Length, levelOfCompression, ref v);
		if (v == 0 || intPtr == IntPtr.Zero)
		{
			gCHandle.Free();
			releaseBuffer(intPtr);
			return null;
		}
		byte[] array = new byte[v];
		Marshal.Copy(intPtr, array, 0, v);
		gCHandle.Free();
		releaseBuffer(intPtr);
		return array;
	}

	public static bool decompressBuffer(byte[] source, ref byte[] outBuffer)
	{
		GCHandle gCHandle = GCHandle.Alloc(source, GCHandleType.Pinned);
		int v = 0;
		IntPtr intPtr = zipDecompressBuffer(gCHandle.AddrOfPinnedObject(), source.Length, ref v);
		if (v == 0 || intPtr == IntPtr.Zero)
		{
			gCHandle.Free();
			releaseBuffer(intPtr);
			return false;
		}
		Array.Resize(ref outBuffer, v);
		Marshal.Copy(intPtr, outBuffer, 0, v);
		gCHandle.Free();
		releaseBuffer(intPtr);
		return true;
	}

	public static int decompressBufferFixed(byte[] source, ref byte[] outBuffer, bool safe = true)
	{
		GCHandle gCHandle = GCHandle.Alloc(source, GCHandleType.Pinned);
		int v = 0;
		IntPtr intPtr = zipDecompressBuffer(gCHandle.AddrOfPinnedObject(), source.Length, ref v);
		if (v == 0 || intPtr == IntPtr.Zero)
		{
			gCHandle.Free();
			releaseBuffer(intPtr);
			return 0;
		}
		if (v > outBuffer.Length)
		{
			if (safe)
			{
				gCHandle.Free();
				releaseBuffer(intPtr);
				return 0;
			}
			v = outBuffer.Length;
		}
		Marshal.Copy(intPtr, outBuffer, 0, v);
		gCHandle.Free();
		releaseBuffer(intPtr);
		return v;
	}

	public static byte[] decompressBuffer(byte[] source)
	{
		GCHandle gCHandle = GCHandle.Alloc(source, GCHandleType.Pinned);
		int v = 0;
		IntPtr intPtr = zipDecompressBuffer(gCHandle.AddrOfPinnedObject(), source.Length, ref v);
		if (v == 0 || intPtr == IntPtr.Zero)
		{
			gCHandle.Free();
			releaseBuffer(intPtr);
			return null;
		}
		byte[] array = new byte[v];
		Marshal.Copy(intPtr, array, 0, v);
		gCHandle.Free();
		releaseBuffer(intPtr);
		return array;
	}

	public static int entry2Buffer(string zipArchive, string entry, ref byte[] buffer, byte[] FileBuffer = null, string password = null)
	{
		if (password == string.Empty)
		{
			password = null;
		}
		int num;
		if (FileBuffer != null)
		{
			GCHandle gCHandle = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
			num = zipGetEntrySize(null, entry, gCHandle.AddrOfPinnedObject(), FileBuffer.Length);
			gCHandle.Free();
		}
		else
		{
			num = zipGetEntrySize(zipArchive, entry, IntPtr.Zero, 0);
		}
		if (num <= 0)
		{
			return -18;
		}
		Array.Resize(ref buffer, num);
		GCHandle gCHandle2 = GCHandle.Alloc(buffer, GCHandleType.Pinned);
		int result;
		if (FileBuffer != null)
		{
			GCHandle gCHandle3 = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
			result = zipEntry2Buffer(null, entry, gCHandle2.AddrOfPinnedObject(), num, gCHandle3.AddrOfPinnedObject(), FileBuffer.Length, password);
			gCHandle3.Free();
		}
		else
		{
			result = zipEntry2Buffer(zipArchive, entry, gCHandle2.AddrOfPinnedObject(), num, IntPtr.Zero, 0, password);
		}
		gCHandle2.Free();
		return result;
	}

	public static int entry2FixedBuffer(string zipArchive, string entry, ref byte[] fixedBuffer, byte[] FileBuffer = null, string password = null)
	{
		if (password == string.Empty)
		{
			password = null;
		}
		int num;
		if (FileBuffer != null)
		{
			GCHandle gCHandle = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
			num = zipGetEntrySize(null, entry, gCHandle.AddrOfPinnedObject(), FileBuffer.Length);
			gCHandle.Free();
		}
		else
		{
			num = zipGetEntrySize(zipArchive, entry, IntPtr.Zero, 0);
		}
		if (num <= 0)
		{
			return -18;
		}
		if (fixedBuffer.Length < num)
		{
			return -19;
		}
		GCHandle gCHandle2 = GCHandle.Alloc(fixedBuffer, GCHandleType.Pinned);
		int num2;
		if (FileBuffer != null)
		{
			GCHandle gCHandle3 = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
			num2 = zipEntry2Buffer(null, entry, gCHandle2.AddrOfPinnedObject(), num, gCHandle3.AddrOfPinnedObject(), FileBuffer.Length, password);
			gCHandle3.Free();
		}
		else
		{
			num2 = zipEntry2Buffer(zipArchive, entry, gCHandle2.AddrOfPinnedObject(), num, IntPtr.Zero, 0, password);
		}
		gCHandle2.Free();
		if (num2 != 1)
		{
			return num2;
		}
		return num;
	}

	public static byte[] entry2Buffer(string zipArchive, string entry, byte[] FileBuffer = null, string password = null)
	{
		if (password == string.Empty)
		{
			password = null;
		}
		int num;
		if (FileBuffer != null)
		{
			GCHandle gCHandle = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
			num = zipGetEntrySize(null, entry, gCHandle.AddrOfPinnedObject(), FileBuffer.Length);
			gCHandle.Free();
		}
		else
		{
			num = zipGetEntrySize(zipArchive, entry, IntPtr.Zero, 0);
		}
		if (num <= 0)
		{
			return null;
		}
		byte[] array = new byte[num];
		GCHandle gCHandle2 = GCHandle.Alloc(array, GCHandleType.Pinned);
		int num2;
		if (FileBuffer != null)
		{
			GCHandle gCHandle3 = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
			num2 = zipEntry2Buffer(null, entry, gCHandle2.AddrOfPinnedObject(), num, gCHandle3.AddrOfPinnedObject(), FileBuffer.Length, password);
			gCHandle3.Free();
		}
		else
		{
			num2 = zipEntry2Buffer(zipArchive, entry, gCHandle2.AddrOfPinnedObject(), num, IntPtr.Zero, 0, password);
		}
		gCHandle2.Free();
		if (num2 != 1)
		{
			return null;
		}
		return array;
	}

	public static bool buffer2File(int levelOfCompression, string zipArchive, string arc_filename, byte[] buffer, bool append = false, string comment = null, string password = null, bool useBz2 = false)
	{
		if (!append && File.Exists(zipArchive))
		{
			File.Delete(zipArchive);
		}
		GCHandle gCHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
		if (levelOfCompression < 0)
		{
			levelOfCompression = 0;
		}
		if (levelOfCompression > 9)
		{
			levelOfCompression = 9;
		}
		if (password == string.Empty)
		{
			password = null;
		}
		if (comment == string.Empty)
		{
			comment = null;
		}
		bool result = zipBuf2File(levelOfCompression, zipArchive, arc_filename, gCHandle.AddrOfPinnedObject(), buffer.Length, comment, password, useBz2);
		gCHandle.Free();
		return result;
	}

	public static int delete_entry(string zipArchive, string arc_filename)
	{
		string text = zipArchive + ".tmp";
		int num = zipDeleteFile(zipArchive, arc_filename, text);
		if (num > 0)
		{
			File.Delete(zipArchive);
			File.Move(text, zipArchive);
		}
		else if (File.Exists(text))
		{
			File.Delete(text);
		}
		return num;
	}

	public static int replace_entry(string zipArchive, string arc_filename, string newFilePath, int level = 9, string comment = null, string password = null, bool useBz2 = false)
	{
		int num = delete_entry(zipArchive, arc_filename);
		if (num < 0)
		{
			return -3;
		}
		if (password == string.Empty)
		{
			password = null;
		}
		if (comment == string.Empty)
		{
			comment = null;
		}
		return zipCD(level, zipArchive, newFilePath, arc_filename, comment, password, useBz2);
	}

	public static int replace_entry(string zipArchive, string arc_filename, byte[] newFileBuffer, int level = 9, string password = null, bool useBz2 = false)
	{
		int num = delete_entry(zipArchive, arc_filename);
		if (num < 0)
		{
			return -5;
		}
		if (buffer2File(level, zipArchive, arc_filename, newFileBuffer, true, null, password, useBz2))
		{
			return 1;
		}
		return -6;
	}

	public static int extract_entry(string zipArchive, string arc_filename, string outpath, byte[] FileBuffer = null, int[] proc = null, string password = null)
	{
		if (!Directory.Exists(Path.GetDirectoryName(outpath)))
		{
			return -1;
		}
		int num = -1;
		if (proc == null)
		{
			proc = new int[1];
		}
		GCHandle gCHandle = GCHandle.Alloc(proc, GCHandleType.Pinned);
		if (FileBuffer != null)
		{
			GCHandle gCHandle2 = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
			num = ((proc == null) ? zipEntry(null, arc_filename, outpath, gCHandle2.AddrOfPinnedObject(), FileBuffer.Length, IntPtr.Zero, password) : zipEntry(null, arc_filename, outpath, gCHandle2.AddrOfPinnedObject(), FileBuffer.Length, gCHandle.AddrOfPinnedObject(), password));
			gCHandle2.Free();
			gCHandle.Free();
			return num;
		}
		num = ((proc == null) ? zipEntry(zipArchive, arc_filename, outpath, IntPtr.Zero, 0, IntPtr.Zero, password) : zipEntry(zipArchive, arc_filename, outpath, IntPtr.Zero, 0, gCHandle.AddrOfPinnedObject(), password));
		gCHandle.Free();
		return num;
	}

	public static int decompress_File(string zipArchive, string outPath, int[] progress, byte[] FileBuffer = null, int[] proc = null, string password = null)
	{
		if (outPath.Substring(outPath.Length - 1, 1) != "/")
		{
			outPath += "/";
		}
		GCHandle gCHandle = GCHandle.Alloc(progress, GCHandleType.Pinned);
		if (proc == null)
		{
			proc = new int[1];
		}
		GCHandle gCHandle2 = GCHandle.Alloc(proc, GCHandleType.Pinned);
		int result;
		if (FileBuffer != null)
		{
			GCHandle gCHandle3 = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
			result = ((proc == null) ? zipEX(null, outPath, gCHandle.AddrOfPinnedObject(), gCHandle3.AddrOfPinnedObject(), FileBuffer.Length, IntPtr.Zero, password) : zipEX(null, outPath, gCHandle.AddrOfPinnedObject(), gCHandle3.AddrOfPinnedObject(), FileBuffer.Length, gCHandle2.AddrOfPinnedObject(), password));
			gCHandle3.Free();
			gCHandle.Free();
			gCHandle2.Free();
			return result;
		}
		result = ((proc == null) ? zipEX(zipArchive, outPath, gCHandle.AddrOfPinnedObject(), IntPtr.Zero, 0, IntPtr.Zero, password) : zipEX(zipArchive, outPath, gCHandle.AddrOfPinnedObject(), IntPtr.Zero, 0, gCHandle2.AddrOfPinnedObject(), password));
		gCHandle.Free();
		gCHandle2.Free();
		return result;
	}

	public static int compress_File(int levelOfCompression, string zipArchive, string inFilePath, bool append = false, string fileName = "", string comment = null, string password = null, bool useBz2 = false)
	{
		if (!File.Exists(inFilePath))
		{
			return -10;
		}
		if (!append && File.Exists(zipArchive))
		{
			File.Delete(zipArchive);
		}
		if (fileName == string.Empty)
		{
			fileName = Path.GetFileName(inFilePath);
		}
		if (levelOfCompression < 0)
		{
			levelOfCompression = 0;
		}
		if (levelOfCompression > 9)
		{
			levelOfCompression = 9;
		}
		if (password == string.Empty)
		{
			password = null;
		}
		if (comment == string.Empty)
		{
			comment = null;
		}
		return zipCD(levelOfCompression, zipArchive, inFilePath, fileName, comment, password, useBz2);
	}

	public static int compress_File_List(int levelOfCompression, string zipArchive, string[] inFilePath, int[] progress = null, bool append = false, string[] fileName = null, string password = null, bool useBz2 = false)
	{
		if (inFilePath == null)
		{
			return -3;
		}
		if (fileName != null && fileName.Length != inFilePath.Length)
		{
			return -4;
		}
		for (int i = 0; i < inFilePath.Length; i++)
		{
			if (!File.Exists(inFilePath[i]))
			{
				return -10;
			}
		}
		if (!append && File.Exists(zipArchive))
		{
			File.Delete(zipArchive);
		}
		if (levelOfCompression < 0)
		{
			levelOfCompression = 0;
		}
		if (levelOfCompression > 9)
		{
			levelOfCompression = 9;
		}
		if (password == string.Empty)
		{
			password = null;
		}
		int result = 0;
		string[] array = null;
		string directoryName = Path.GetDirectoryName(zipArchive);
		if (fileName == null)
		{
			array = new string[inFilePath.Length];
			for (int j = 0; j < inFilePath.Length; j++)
			{
				array[j] = inFilePath[j].Replace(directoryName, string.Empty);
			}
		}
		else
		{
			array = fileName;
		}
		for (int k = 0; k < inFilePath.Length; k++)
		{
			if (array[k] == null)
			{
				array[k] = inFilePath[k].Replace(directoryName, string.Empty);
			}
		}
		for (int l = 0; l < inFilePath.Length; l++)
		{
			if (progress != null)
			{
				progress[0]++;
			}
			result = compress_File(levelOfCompression, zipArchive, inFilePath[l], true, array[l], null, password, useBz2);
		}
		directoryName = null;
		return result;
	}

	public static void compressDir(string sourceDir, int levelOfCompression, string zipArchive, bool includeRoot = false, string password = null, bool useBz2 = false)
	{
		string text = sourceDir.Replace("\\", "/");
		if (!Directory.Exists(text))
		{
			return;
		}
		if (File.Exists(zipArchive))
		{
			File.Delete(zipArchive);
		}
		string[] array = text.Split('/');
		string text2 = array[array.Length - 1];
		string text3 = text2;
		cProgress = 0;
		if (levelOfCompression < 0)
		{
			levelOfCompression = 0;
		}
		if (levelOfCompression > 9)
		{
			levelOfCompression = 9;
		}
		string[] files = Directory.GetFiles(text, "*", SearchOption.AllDirectories);
		foreach (string text4 in files)
		{
			string text5 = text4.Replace(text, text2).Replace("\\", "/");
			if (!includeRoot)
			{
				text5 = text5.Substring(text3.Length + 1);
			}
			compress_File(levelOfCompression, zipArchive, text4, true, text5, null, password, useBz2);
			cProgress++;
		}
	}

	public static int getAllFiles(string Dir)
	{
		string[] files = Directory.GetFiles(Dir, "*", SearchOption.AllDirectories);
		int result = files.Length;
		files = null;
		return result;
	}

	public static int gzip(byte[] source, byte[] outBuffer, int level, bool addHeader = true, bool addFooter = true)
	{
		GCHandle gCHandle = GCHandle.Alloc(source, GCHandleType.Pinned);
		GCHandle gCHandle2 = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);
		int num = zipGzip(gCHandle.AddrOfPinnedObject(), source.Length, gCHandle2.AddrOfPinnedObject(), level, addHeader, addFooter);
		gCHandle.Free();
		gCHandle2.Free();
		int num2 = 0;
		if (addHeader)
		{
			num2 += 10;
		}
		if (addFooter)
		{
			num2 += 8;
		}
		return num + num2;
	}

	public static int gzipUncompressedSize(byte[] source)
	{
		int num = source.Length;
		return (source[num - 4] & 0xFF) | ((source[num - 3] & 0xFF) << 8) | ((source[num - 2] & 0xFF) << 16) | ((source[num - 1] & 0xFF) << 24);
	}

	public static int unGzip(byte[] source, byte[] outBuffer, bool hasHeader = true, bool hasFooter = true)
	{
		GCHandle gCHandle = GCHandle.Alloc(source, GCHandleType.Pinned);
		GCHandle gCHandle2 = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);
		int result = zipUnGzip(gCHandle.AddrOfPinnedObject(), source.Length, gCHandle2.AddrOfPinnedObject(), outBuffer.Length, hasHeader, hasFooter);
		gCHandle.Free();
		gCHandle2.Free();
		return result;
	}

	public static int unGzip2(byte[] source, byte[] outBuffer)
	{
		GCHandle gCHandle = GCHandle.Alloc(source, GCHandleType.Pinned);
		GCHandle gCHandle2 = GCHandle.Alloc(outBuffer, GCHandleType.Pinned);
		int result = zipUnGzip2(gCHandle.AddrOfPinnedObject(), source.Length, gCHandle2.AddrOfPinnedObject(), outBuffer.Length);
		gCHandle.Free();
		gCHandle2.Free();
		return result;
	}

	public static DateTime entryDateTime(string zipArchive, string entry, byte[] FileBuffer = null)
	{
		uint num = 0u;
		if (FileBuffer != null)
		{
			GCHandle gCHandle = GCHandle.Alloc(FileBuffer, GCHandleType.Pinned);
			num = getEntryDateTime(null, entry, gCHandle.AddrOfPinnedObject(), FileBuffer.Length);
			gCHandle.Free();
		}
		if (FileBuffer == null)
		{
			num = getEntryDateTime(zipArchive, entry, IntPtr.Zero, 0);
		}
		uint num2 = (num & 0xFFFF0000u) >> 16;
		uint num3 = num & 0xFFFFu;
		uint year = (num2 >> 9) + 1980;
		uint month = (num2 & 0x1E0) >> 5;
		uint day = num2 & 0x1Fu;
		uint hour = num3 >> 11;
		uint minute = (num3 & 0x7E0) >> 5;
		uint second = (num3 & 0x1F) * 2;
		if (num == 0 || num == 1 || num == 2)
		{
			Debug.Log("Error in getting DateTime");
			return DateTime.Now;
		}
		return new DateTime((int)year, (int)month, (int)day, (int)hour, (int)minute, (int)second);
	}
}
