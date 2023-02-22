using System;
using System.Threading;
using UnityEngine;

namespace BestHTTP
{
	[ExecuteInEditMode]
	public sealed class HTTPUpdateDelegator : MonoBehaviour
	{
		public static Func<bool> OnBeforeApplicationQuit;

		public static Action<bool> OnApplicationForegroundStateChanged;

		private static bool IsSetupCalled;

		public static HTTPUpdateDelegator Instance { get; private set; }

		public static bool IsCreated { get; private set; }

		public static bool IsThreaded { get; set; }

		public static bool IsThreadRunning { get; private set; }

		public static int ThreadFrequencyInMS { get; set; }

		static HTTPUpdateDelegator()
		{
			ThreadFrequencyInMS = 100;
		}

		public static void CheckInstance()
		{
			try
			{
				if (!IsCreated)
				{
					GameObject gameObject = GameObject.Find("HTTP Update Delegator");
					if (gameObject != null)
					{
						Instance = gameObject.GetComponent<HTTPUpdateDelegator>();
					}
					if (Instance == null)
					{
						gameObject = new GameObject("HTTP Update Delegator");
						gameObject.hideFlags = HideFlags.DontSave;
						Instance = gameObject.AddComponent<HTTPUpdateDelegator>();
					}
					IsCreated = true;
					HTTPManager.Logger.Information("HTTPUpdateDelegator", "Instance Created!");
				}
			}
			catch
			{
				HTTPManager.Logger.Error("HTTPUpdateDelegator", "Please call the BestHTTP.HTTPManager.Setup() from one of Unity's event(eg. awake, start) before you send any request!");
			}
		}

		private void Setup()
		{
			IsThreaded = false;
			if (IsThreaded)
			{
				ThreadPool.QueueUserWorkItem(ThreadFunc);
			}
			IsSetupCalled = true;
			if (!Application.isEditor || Application.isPlaying)
			{
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
			HTTPManager.Logger.Information("HTTPUpdateDelegator", "Setup done!");
		}

		private void ThreadFunc(object obj)
		{
			HTTPManager.Logger.Information("HTTPUpdateDelegator", "Update Thread Started");
			try
			{
				IsThreadRunning = true;
				while (IsThreadRunning)
				{
					HTTPManager.OnUpdate();
					Thread.Sleep(ThreadFrequencyInMS);
				}
			}
			finally
			{
				HTTPManager.Logger.Information("HTTPUpdateDelegator", "Update Thread Ended");
			}
		}

		private void Update()
		{
			if (!IsSetupCalled)
			{
				IsSetupCalled = true;
				Setup();
			}
			if (!IsThreaded)
			{
				HTTPManager.OnUpdate();
			}
		}

		private void OnDisable()
		{
			HTTPManager.Logger.Information("HTTPUpdateDelegator", "OnDisable Called!");
			OnApplicationQuit();
		}

		private void OnApplicationPause(bool isPaused)
		{
			if (OnApplicationForegroundStateChanged != null)
			{
				OnApplicationForegroundStateChanged(isPaused);
			}
		}

		private void OnApplicationQuit()
		{
			HTTPManager.Logger.Information("HTTPUpdateDelegator", "OnApplicationQuit Called!");
			if (OnBeforeApplicationQuit != null)
			{
				try
				{
					if (!OnBeforeApplicationQuit())
					{
						HTTPManager.Logger.Information("HTTPUpdateDelegator", "OnBeforeApplicationQuit call returned false, postponing plugin shutdown.");
						return;
					}
				}
				catch (Exception ex)
				{
					HTTPManager.Logger.Exception("HTTPUpdateDelegator", string.Empty, ex);
				}
			}
			IsThreadRunning = false;
			if (IsCreated)
			{
				IsCreated = false;
				HTTPManager.OnQuit();
			}
		}
	}
}
