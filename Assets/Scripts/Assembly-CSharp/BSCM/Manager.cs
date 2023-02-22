using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace BSCM
{
	public class Manager
	{
		public static bool enabled = true;

		public static GameMode[] modes;

		public static int hash;

		public static string bundleUrl;

		private static AssetBundle bundle;

		private static Dictionary<GameMode, List<string>> scenesGameMode = new Dictionary<GameMode, List<string>>();

		private static string directoryPath = AndroidNativeFunctions.GetAbsolutePath() + "/Block Strike/Custom Maps";

		public static void Start()
		{
			if (!enabled)
			{
				return;
			}
			CreateDirectory();
			scenesGameMode = new Dictionary<GameMode, List<string>>();
			string[] files = Directory.GetFiles(directoryPath, "*.txt");
			string empty = string.Empty;
			for (int i = 0; i < files.Length; i++)
			{
				if (!CheckFileName(files[i]))
				{
					continue;
				}
				empty = GetAssetBundlePath(files[i]);
				if (string.IsNullOrEmpty(empty) || new FileInfo(empty).Length > 6000000)
				{
					continue;
				}
				GameMode[] bundleModesPath = GetBundleModesPath(files[i]);
				if (bundleModesPath.Length == 0)
				{
					continue;
				}
				for (int j = 0; j < bundleModesPath.Length; j++)
				{
					if (!scenesGameMode.ContainsKey(bundleModesPath[j]))
					{
						scenesGameMode.Add(bundleModesPath[j], new List<string>());
					}
					scenesGameMode[bundleModesPath[j]].Add(empty);
				}
			}
		}

		private static bool CheckFileName(string path)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
			if (string.IsNullOrEmpty(fileNameWithoutExtension))
			{
				return false;
			}
			if (LevelManager.HasScene(fileNameWithoutExtension))
			{
				return false;
			}
			return true;
		}

		private static string GetAssetBundlePath(string fileInfoPath)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileInfoPath);
			if (File.Exists(directoryPath + "/" + fileNameWithoutExtension + ".bscm"))
			{
				return directoryPath + "/" + fileNameWithoutExtension + ".bscm";
			}
			return string.Empty;
		}

		public static bool HasMaps()
		{
			return (scenesGameMode.Count != 0) ? true : false;
		}

		public static string[] GetMapsList(GameMode mode)
		{
			if (!scenesGameMode.ContainsKey(mode))
			{
				return new string[0];
			}
			return scenesGameMode[mode].ToArray();
		}

		public static void LoadBundle(string path)
		{
			byte[] binary = File.ReadAllBytes(path);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);
			modes = GetBundleModes(fileNameWithoutExtension);
			hash = GetBundleHash(fileNameWithoutExtension);
			bundleUrl = GetBundleUrl(fileNameWithoutExtension);
			UnloadBundle();
			bundle = AssetBundle.LoadFromMemory(binary);
		}

		public static void UnloadBundle()
		{
			if (bundle != null)
			{
				bundle.Unload(true);
			}
		}

		public static string GetBundleName(string path)
		{
			return Path.GetFileNameWithoutExtension(path);
		}

		public static string GetBundlePath(string bundleName)
		{
			if (File.Exists(directoryPath + "/" + bundleName + ".bscm"))
			{
				return directoryPath + "/" + bundleName + ".bscm";
			}
			return string.Empty;
		}

		public static string SaveBundle(string name, int[] modes, int hash, string url, byte[] map)
		{
			CreateDirectory();
			File.WriteAllBytes(directoryPath + "/" + name + ".bscm", map);
			StringBuilder stringBuilder = new StringBuilder();
			string text = string.Empty;
			for (int i = 0; i < modes.Length; i++)
			{
				text = ((i >= modes.Length - 1) ? (text + modes[i]) : (text + modes[i] + ","));
			}
			stringBuilder.AppendLine("mode=" + text);
			stringBuilder.AppendLine("hash=" + hash);
			stringBuilder.AppendLine("id=" + url);
			File.WriteAllText(directoryPath + "/" + name + ".txt", stringBuilder.ToString());
			return directoryPath + "/" + name + ".bscm";
		}

		public static bool DeleteBundle(string name)
		{
			if (File.Exists(directoryPath + "/" + name + ".bscm"))
			{
				File.Delete(directoryPath + "/" + name + ".bscm");
				File.Delete(directoryPath + "/" + name + ".txt");
				Start();
				return true;
			}
			return false;
		}

		private static void CreateDirectory()
		{
			if (!Directory.Exists(directoryPath))
			{
				Directory.CreateDirectory(directoryPath);
			}
		}

		public static int GetBundleHash(string bundleName)
		{
			//Discarded unreachable code: IL_006a, IL_0077
			string path = directoryPath + "/" + bundleName + ".txt";
			if (File.Exists(path))
			{
				try
				{
					string[] array = File.ReadAllText(path).Split("\n"[0]);
					array[1] = array[1].Replace("hash=", string.Empty);
					int result = 0;
					int.TryParse(array[1], out result);
					return result;
				}
				catch
				{
					return 0;
				}
			}
			return 0;
		}

		private static string GetBundleUrl(string bundleName)
		{
			//Discarded unreachable code: IL_0059, IL_006a
			string path = directoryPath + "/" + bundleName + ".txt";
			if (File.Exists(path))
			{
				try
				{
					string[] array = File.ReadAllText(path).Split("\n"[0]);
					return array[2].Replace("id=", string.Empty);
				}
				catch
				{
					return string.Empty;
				}
			}
			return string.Empty;
		}

		private static GameMode[] GetBundleModesPath(string bundlePath)
		{
			return GetBundleModes(Path.GetFileNameWithoutExtension(bundlePath));
		}

		private static GameMode[] GetBundleModes(string bundleName)
		{
			string path = directoryPath + "/" + bundleName + ".txt";
			List<GameMode> list = new List<GameMode>();
			try
			{
				string[] array = File.ReadAllText(path).Split("\n"[0]);
				array[0] = array[0].Replace("mode=", string.Empty);
				array = array[0].Split(","[0]);
				int result = 0;
				for (int i = 0; i < array.Length; i++)
				{
					if (int.TryParse(array[i], out result))
					{
						list.Add((GameMode)result);
					}
				}
			}
			catch
			{
			}
			return list.ToArray();
		}
	}
}
