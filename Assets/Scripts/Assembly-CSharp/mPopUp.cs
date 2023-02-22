using System;
using UnityEngine;

public class mPopUp : MonoBehaviour
{
	[Header("Popup")]
	public UIPanel PopupPanel;

	public UILabel PopupTitleLabel;

	public UILabel PopupTextLabel;

	public UILabel PopupButton1;

	public UILabel PopupButton2;

	public static bool activePopup;

	[Header("Text Line")]
	public UIPanel TextLinePanel;

	public UILabel TextLineLabel;

	public static bool activeText;

	[Header("Input")]
	public UIPanel InputPanel;

	public UILabel InputTitleLabel;

	public UILabel InputTextLabel;

	public UIInput InputField;

	public UILabel InputButton1;

	public UILabel InputButton2;

	private Action InputAction;

	private Action InputChange;

	public static bool activeInput;

	[Header("Wait")]
	public GameObject WaitSprite;

	public UILabel WaitLabel;

	public GameObject WaitPanel;

	public UILabel WaitPanelLabel;

	private Action Button1Action;

	private Action Button2Action;

	private static mPopUp instance;

	private void Awake()
	{
		instance = this;
	}

	public static void SetActiveWait(bool active)
	{
		SetActiveWait(active, string.Empty);
	}

	public static void SetActiveWait(bool active, string text)
	{
		instance.WaitSprite.SetActive(active);
		if (active)
		{
			instance.WaitLabel.text = text;
		}
	}

	public static void SetActiveWaitPanel(bool active)
	{
		SetActiveWaitPanel(active, string.Empty);
	}

	public static void SetActiveWaitPanel(bool active, string text)
	{
		instance.WaitPanel.SetActive(active);
		if (active)
		{
			instance.WaitPanelLabel.text = text;
		}
	}

	public static void ShowText(string text)
	{
		ShowText(text, 0f, string.Empty);
	}

	public static void ShowText(string text, float duration, string panel)
	{
		Hide();
		activeText = true;
		mPanelManager.Hide();
		instance.TextLinePanel.alpha = 0f;
		mPanelManager.ShowTween(instance.TextLinePanel.cachedGameObject);
		instance.TextLineLabel.text = text;
		if (duration != 0f)
		{
			TimerManager.In(duration, delegate
			{
				HideAll(panel);
			});
		}
	}

	public static void ShowPopup(string text, string title, string buttonText, Action callbackButton)
	{
		ShowPopup(text, title, string.Empty, null, buttonText, callbackButton);
	}

	public static void ShowPopup(string text, string title, string button1Text, Action callbackButton1, string button2Text, Action callbackButton2)
	{
		Hide();
		activePopup = true;
		instance.PopupPanel.alpha = 0f;
		mPanelManager.ShowTween(instance.PopupPanel.cachedGameObject);
		instance.PopupTextLabel.text = text;
		instance.PopupTitleLabel.text = title;
		if (string.IsNullOrEmpty(button1Text))
		{
			instance.PopupButton1.cachedGameObject.SetActive(false);
		}
		else
		{
			instance.PopupButton1.cachedGameObject.SetActive(true);
			instance.PopupButton1.text = button1Text;
			instance.Button1Action = callbackButton1;
		}
		instance.PopupButton2.text = button2Text;
		instance.Button2Action = callbackButton2;
	}

	public static void SetPopupText(string text)
	{
		instance.PopupTextLabel.text = text;
	}

	public void OnClickButton(int button)
	{
		switch (button)
		{
		case 1:
			if (Button1Action != null)
			{
				Button1Action();
			}
			break;
		case 2:
			if (Button2Action != null)
			{
				Button2Action();
			}
			break;
		}
	}

	public static void ShowInput(string text, string title, int limit, UIInput.KeyboardType keyboardType, Action callbackInput, Action callbackChange, string button1Text, Action callbackButton1, string button2Text, Action callbackButton2)
	{
		Hide();
		activeInput = true;
		mPanelManager.ShowTween(instance.InputPanel.cachedGameObject);
		instance.InputTextLabel.text = text;
		instance.InputTitleLabel.text = title;
		instance.InputField.characterLimit = limit;
		instance.InputField.keyboardType = keyboardType;
		instance.InputAction = callbackInput;
		instance.InputChange = callbackChange;
		instance.InputField.value = text;
		instance.InputButton1.text = button1Text;
		instance.Button1Action = callbackButton1;
		instance.InputButton2.text = button2Text;
		instance.Button2Action = callbackButton2;
	}

	public void OnInputSubmit()
	{
		if (InputAction != null)
		{
			InputAction();
		}
	}

	public void OnInputChange()
	{
		if (InputChange != null)
		{
			InputChange();
		}
	}

	public static string GetInputText()
	{
		return instance.InputField.value;
	}

	public static void SetInputText(string text)
	{
		instance.InputField.value = text;
	}

	private static void Hide()
	{
		if (activeText)
		{
			mPanelManager.HideTween(instance.TextLinePanel.cachedGameObject);
		}
		if (activePopup)
		{
			mPanelManager.HideTween(instance.PopupPanel.cachedGameObject);
		}
		if (activeInput)
		{
			mPanelManager.HideTween(instance.InputPanel.cachedGameObject);
		}
		activeText = false;
		activePopup = false;
		activeInput = false;
	}

	public static void HideAll()
	{
		HideAll(string.Empty);
	}

	public static void HideAll(string panel)
	{
		HideAll(panel, true);
	}

	public static void HideAll(string panel, bool playerData)
	{
		if (activeText)
		{
			mPanelManager.HideTween(instance.TextLinePanel.cachedGameObject);
		}
		if (activePopup)
		{
			mPanelManager.HideTween(instance.PopupPanel.cachedGameObject);
		}
		if (activeInput)
		{
			mPanelManager.HideTween(instance.InputPanel.cachedGameObject);
		}
		activeText = false;
		activePopup = false;
		activeInput = false;
		if (!string.IsNullOrEmpty(panel))
		{
			mPanelManager.Show(panel, playerData);
		}
		else if (mPanelManager.select == null)
		{
			if (mPanelManager.last == null)
			{
				mPanelManager.Show("Menu", playerData);
			}
			else
			{
				mPanelManager.Show(mPanelManager.last.name, playerData);
			}
		}
	}
}
