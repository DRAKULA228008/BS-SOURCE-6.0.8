using System;
using System.IO;
using BestHTTP.Logger;

namespace BestHTTP.PlatformSupport.FileSystem
{
	public sealed class DefaultIOService : IIOService
	{
		public Stream CreateFileStream(string path, FileStreamModes mode)
		{
			if (HTTPManager.Logger.Level == Loglevels.All)
			{
				HTTPManager.Logger.Verbose("DefaultIOService", string.Format("CreateFileStream path: '{0}' mode: {1}", path, mode));
			}
			switch (mode)
			{
			case FileStreamModes.Create:
				return new FileStream(path, FileMode.Create);
			case FileStreamModes.Open:
				return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
			case FileStreamModes.Append:
				return new FileStream(path, FileMode.Append);
			default:
				throw new NotImplementedException("DefaultIOService.CreateFileStream - mode not implemented: " + mode);
			}
		}

		public void DirectoryCreate(string path)
		{
			if (HTTPManager.Logger.Level == Loglevels.All)
			{
				HTTPManager.Logger.Verbose("DefaultIOService", string.Format("DirectoryCreate path: '{0}'", path));
			}
			Directory.CreateDirectory(path);
		}

		public bool DirectoryExists(string path)
		{
			bool flag = Directory.Exists(path);
			if (HTTPManager.Logger.Level == Loglevels.All)
			{
				HTTPManager.Logger.Verbose("DefaultIOService", string.Format("DirectoryExists path: '{0}' exists: {1}", path, flag));
			}
			return flag;
		}

		public string[] GetFiles(string path)
		{
			string[] files = Directory.GetFiles(path);
			if (HTTPManager.Logger.Level == Loglevels.All)
			{
				HTTPManager.Logger.Verbose("DefaultIOService", string.Format("GetFiles path: '{0}' files count: {1}", path, files.Length));
			}
			return files;
		}

		public void FileDelete(string path)
		{
			if (HTTPManager.Logger.Level == Loglevels.All)
			{
				HTTPManager.Logger.Verbose("DefaultIOService", string.Format("FileDelete path: '{0}'", path));
			}
			File.Delete(path);
		}

		public bool FileExists(string path)
		{
			bool flag = File.Exists(path);
			if (HTTPManager.Logger.Level == Loglevels.All)
			{
				HTTPManager.Logger.Verbose("DefaultIOService", string.Format("FileExists path: '{0}' exists: {1}", path, flag));
			}
			return flag;
		}
	}
}
