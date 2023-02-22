using System;
using System.Text;
using UnityEngine;

namespace BestHTTP.Logger
{
	public class DefaultLogger : ILogger
	{
		public Loglevels Level { get; set; }

		public string FormatVerbose { get; set; }

		public string FormatInfo { get; set; }

		public string FormatWarn { get; set; }

		public string FormatErr { get; set; }

		public string FormatEx { get; set; }

		public DefaultLogger()
		{
			FormatVerbose = "[{0}] D [{1}]: {2}";
			FormatInfo = "[{0}] I [{1}]: {2}";
			FormatWarn = "[{0}] W [{1}]: {2}";
			FormatErr = "[{0}] Err [{1}]: {2}";
			FormatEx = "[{0}] Ex [{1}]: {2} - Message: {3}  StackTrace: {4}";
			Level = ((!Debug.isDebugBuild) ? Loglevels.Error : Loglevels.Warning);
		}

		public void Verbose(string division, string verb)
		{
			if ((int)Level <= 0)
			{
				try
				{
					Debug.Log(string.Format(FormatVerbose, GetFormattedTime(), division, verb));
				}
				catch
				{
				}
			}
		}

		public void Information(string division, string info)
		{
			if ((int)Level <= 1)
			{
				try
				{
					Debug.Log(string.Format(FormatInfo, GetFormattedTime(), division, info));
				}
				catch
				{
				}
			}
		}

		public void Warning(string division, string warn)
		{
			if ((int)Level <= 2)
			{
				try
				{
					Debug.LogWarning(string.Format(FormatWarn, GetFormattedTime(), division, warn));
				}
				catch
				{
				}
			}
		}

		public void Error(string division, string err)
		{
			if ((int)Level <= 3)
			{
				try
				{
					Debug.LogError(string.Format(FormatErr, GetFormattedTime(), division, err));
				}
				catch
				{
				}
			}
		}

		public void Exception(string division, string msg, Exception ex)
		{
			if ((int)Level > 4)
			{
				return;
			}
			try
			{
				string empty = string.Empty;
				if (ex == null)
				{
					empty = "null";
				}
				else
				{
					StringBuilder stringBuilder = new StringBuilder();
					Exception ex2 = ex;
					int num = 1;
					while (ex2 != null)
					{
						stringBuilder.AppendFormat("{0}: {1} {2}", num++.ToString(), ex2.Message, ex2.StackTrace);
						ex2 = ex2.InnerException;
						if (ex2 != null)
						{
							stringBuilder.AppendLine();
						}
					}
					empty = stringBuilder.ToString();
				}
				Debug.LogError(string.Format(FormatEx, GetFormattedTime(), division, msg, empty, (ex == null) ? "null" : ex.StackTrace));
			}
			catch
			{
			}
		}

		private string GetFormattedTime()
		{
			return DateTime.Now.Ticks.ToString();
		}
	}
}
