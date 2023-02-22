using System;
using System.Diagnostics;
using UnityEngine;

public class AndroidShell
{
	public static void RunCommand(string command, Action<string> complete, Action<string> error)
	{
		RunCommand("sh", command, complete, error);
	}

	public static void RunCommand(string file, string command, Action<string> complete, Action<string> error)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		Process process = new Process();
		process.StartInfo.FileName = file;
		process.StartInfo.RedirectStandardInput = true;
		process.StartInfo.RedirectStandardOutput = true;
		process.StartInfo.RedirectStandardError = true;
		process.StartInfo.UseShellExecute = false;
		process.Start();
		process.StandardInput.WriteLine(command);
		process.StandardInput.Flush();
		process.StandardInput.Close();
		process.WaitForExit();
		string text = process.StandardOutput.ReadToEnd();
		if (!string.IsNullOrEmpty(text))
		{
			if (complete != null)
			{
				complete(text);
			}
			return;
		}
		text = string.Empty;
		text = process.StandardError.ReadToEnd();
		if (!string.IsNullOrEmpty(text) && error != null)
		{
			error(text);
		}
	}
}
