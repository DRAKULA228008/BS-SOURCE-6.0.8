using System;
using UnityEngine;

public class UIPopUp : MonoBehaviour
{
	public UILabel Text;

	public UILabel Title;

	public UILabel Button1;

	public UILabel Button2;

	public Action Button1Action;

	public Action Button2Action;

	public static Action ClickButton;

	private static UIPopUp instance;

	private void Start()
	{
		instance = this;
	}

	public static void ShowPopUp(string text, string title, string button1Text, Action callbackButton1, string button2Text, Action callbackButton2)
	{
		UIPanelManager.ShowPanel("Popup");
		instance.Text.text = text;
		instance.Title.text = title;
		instance.Button1.text = button1Text;
		instance.Button2.text = button2Text;
		instance.Button1Action = callbackButton1;
		instance.Button2Action = callbackButton2;
	}

	public void OnClickButton1()
	{
		Button1Action();
		if (ClickButton != null)
		{
			ClickButton();
		}
	}

	public void OnClickButton2()
	{
		Button2Action();
		if (ClickButton != null)
		{
			ClickButton();
		}
	}
}
