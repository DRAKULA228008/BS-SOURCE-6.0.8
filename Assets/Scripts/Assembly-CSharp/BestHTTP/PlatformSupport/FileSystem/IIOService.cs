using System.IO;

namespace BestHTTP.PlatformSupport.FileSystem
{
	public interface IIOService
	{
		void DirectoryCreate(string path);

		bool DirectoryExists(string path);

		string[] GetFiles(string path);

		void FileDelete(string path);

		bool FileExists(string path);

		Stream CreateFileStream(string path, FileStreamModes mode);
	}
}
