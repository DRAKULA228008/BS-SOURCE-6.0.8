using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MessageBox : MonoBehaviour
{
	public static void ShowAlertBox(string message, string title, Action onFinished = null)
	{
		Transform alertPanel = GameObject.Find("Canvas").transform.Find("AlertBox");
		if (alertPanel == null)
		{
			alertPanel = (UnityEngine.Object.Instantiate(Resources.Load("AlertBox")) as GameObject).transform;
			alertPanel.gameObject.name = "AlertBox";
			alertPanel.SetParent(GameObject.Find("Canvas").transform);
			alertPanel.localPosition = new Vector3(-6f, -6f, 0f);
		}
		alertPanel.Find("title").GetComponent<Text>().text = title;
		alertPanel.Find("message").GetComponent<Text>().text = message;
		alertPanel.gameObject.SetActive(true);
		if (onFinished != null)
		{
			Button button = alertPanel.Find("alertBtn").GetComponent<Button>();
			UnityAction onclick = null;
			onclick = delegate
			{
				onFinished();
				alertPanel.gameObject.SetActive(false);
				button.onClick.RemoveListener(onclick);
			};
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(onclick);
		}
	}

	public static void ShowConfirmBox(string message, string title, Action<bool> onFinished = null)
	{
		Transform confirmPanel = GameObject.Find("Canvas").transform.Find("ConfirmBox");
		if (confirmPanel == null)
		{
			confirmPanel = (UnityEngine.Object.Instantiate(Resources.Load("ConfirmBox")) as GameObject).transform;
			confirmPanel.gameObject.name = "ConfirmBox";
			confirmPanel.SetParent(GameObject.Find("Canvas").transform);
			confirmPanel.localPosition = new Vector3(-8f, -18f, 0f);
		}
		confirmPanel.Find("confirmTitle").GetComponent<Text>().text = title;
		confirmPanel.Find("conmessage").GetComponent<Text>().text = message;
		confirmPanel.gameObject.SetActive(true);
		if (onFinished != null)
		{
			Button confirmBtn = confirmPanel.Find("confirmBtn").GetComponent<Button>();
			Button cancelBtn = confirmPanel.Find("cancelBtn").GetComponent<Button>();
			UnityAction onconfirm = null;
			UnityAction oncancel = null;
			Action cleanup = delegate
			{
				confirmBtn.onClick.RemoveListener(onconfirm);
				cancelBtn.onClick.RemoveListener(oncancel);
				confirmPanel.gameObject.SetActive(false);
			};
			onconfirm = delegate
			{
				onFinished(true);
				cleanup();
			};
			oncancel = delegate
			{
				onFinished(false);
				cleanup();
			};
			confirmBtn.onClick.RemoveAllListeners();
			cancelBtn.onClick.RemoveAllListeners();
			confirmBtn.onClick.AddListener(onconfirm);
			cancelBtn.onClick.AddListener(oncancel);
		}
	}
}
