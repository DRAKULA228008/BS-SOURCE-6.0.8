using System;
using FreeJSON;
using UnityEngine;

public class AndroidGoogleSignIn : MonoBehaviour
{
	public static AndroidGoogleSignInAccount Account = new AndroidGoogleSignInAccount();

	private static AndroidGoogleSignIn instance;

	private static Action successCallback;

	private static Action<string> errorCallback;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	private static void Init()
	{
		if (!(instance != null))
		{
			GameObject gameObject = new GameObject("GoogleSignIn");
			gameObject.AddComponent<AndroidGoogleSignIn>();
		}
	}

	public static void SignIn(string webClientId, Action success, Action<string> error)
	{
		if (Application.platform != RuntimePlatform.Android)
		{
			return;
		}
		Init();
		successCallback = success;
		errorCallback = error;
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				using (AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.rexetstudio.google.GoogleSignInFragment"))
				{
					androidJavaClass2.SetStatic("UnityGameObjectName", instance.gameObject.name);
					androidJavaClass2.CallStatic("SignIn", androidJavaObject, webClientId);
				}
			}
		}
	}

	public void UnityGoogleSignInSuccessCallback(string googleSignInAccountJson)
	{
		googleSignInAccountJson = googleSignInAccountJson.Replace(", ", ",");
		googleSignInAccountJson = googleSignInAccountJson.Replace(": ", ":");
		AndroidGoogleSignInAccount androidGoogleSignInAccount = new AndroidGoogleSignInAccount();
		JsonObject jsonObject = JsonObject.Parse(googleSignInAccountJson);
		androidGoogleSignInAccount.DisplayName = jsonObject.Get<string>("DisplayName");
		androidGoogleSignInAccount.Email = jsonObject.Get<string>("Email");
		androidGoogleSignInAccount.FamilyName = jsonObject.Get<string>("FamilyName");
		androidGoogleSignInAccount.Id = jsonObject.Get<string>("Id");
		androidGoogleSignInAccount.PhotoUrl = jsonObject.Get<string>("PhotoUrl");
		androidGoogleSignInAccount.Token = jsonObject.Get<string>("Token");
		if (androidGoogleSignInAccount == null)
		{
			UnityGoogleSignInErrorCallback(string.Empty);
			return;
		}
		if (string.IsNullOrEmpty(androidGoogleSignInAccount.Email))
		{
			UnityGoogleSignInErrorCallback(string.Empty);
			return;
		}
		Account = androidGoogleSignInAccount;
		if (successCallback != null)
		{
			successCallback();
		}
		ClearReferences();
	}

	public void UnityGoogleSignInErrorCallback(string errorMsg)
	{
		if (errorCallback != null)
		{
			errorCallback(errorMsg);
		}
		ClearReferences();
	}

	private void ClearReferences()
	{
		successCallback = null;
		errorCallback = null;
	}
}
