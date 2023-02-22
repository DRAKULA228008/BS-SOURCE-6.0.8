using System;
using System.Collections.Generic;
using UnityEngine;

public class UINotification : MonoBehaviour
{
	[Serializable]
	public class Data
	{
		public int ID;

		public string Text;

		public Action PositiveAction;

		public Action NegativeAction;
	}

	public GameObject Button;

	public UILabel ButtonCountLabel;

	public GameObject Panel;

	public UILabel Label;

	private List<Data> Datas = new List<Data>();

	private Data SelectData;

	private bool isShowButton;

	private bool isShowPanel;

	private int NextID = -1;

	private static UINotification instance;

	private void Start()
	{
		instance = this;
	}

	private void OnEnable()
	{
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Combine(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(GetButtonDown));
	}

	private void OnDisable()
	{
		InputManager.GetButtonDownEvent = (InputManager.ButtonDelegate)Delegate.Remove(InputManager.GetButtonDownEvent, new InputManager.ButtonDelegate(GetButtonDown));
	}

	public static int Add(string text, Action positive, Action negative)
	{
		Data data = new Data();
		instance.NextID++;
		data.ID = instance.NextID;
		data.Text = text;
		data.PositiveAction = positive;
		data.NegativeAction = negative;
		instance.Datas.Add(data);
		if (!instance.isShowButton)
		{
			instance.SetShowButton(true);
		}
		instance.ButtonCountLabel.text = instance.Datas.Count.ToString();
		return instance.NextID;
	}

	private void GetButtonDown(string button)
	{
		if (button == "Notification")
		{
			Show();
		}
	}

	public void Show()
	{
		if (isShowPanel)
		{
			return;
		}
		if (Datas.Count == 0)
		{
			SetShowButton(false);
			SetShowPanel(false);
			return;
		}
		SelectData = Datas[0];
		Datas.Remove(SelectData);
		if (Datas.Count == 0)
		{
			SetShowButton(false);
		}
		else
		{
			ButtonCountLabel.text = Datas.Count.ToString();
		}
		SetShowPanel(true);
	}

	private void SetShowPanel(bool active)
	{
		isShowPanel = active;
		Panel.SetActive(active);
		if (isShowPanel)
		{
			Label.text = SelectData.Text;
		}
	}

	private void SetShowButton(bool active)
	{
		isShowButton = active;
		Button.SetActive(active);
	}

	public void OnClickPositive()
	{
		if (SelectData != null || SelectData.PositiveAction != null)
		{
			SelectData.PositiveAction();
		}
		Close();
	}

	public void OnClickNegative()
	{
		if (SelectData != null || SelectData.NegativeAction != null)
		{
			SelectData.NegativeAction();
		}
		Close();
	}

	public static void Close()
	{
		instance.SetShowPanel(false);
		if (instance.Datas.Count == 0)
		{
			instance.SetShowButton(false);
		}
		else
		{
			instance.ButtonCountLabel.text = instance.Datas.Count.ToString();
		}
		instance.SelectData = null;
	}

	public static void Remove(int id)
	{
		if (instance.SelectData != null && instance.SelectData.ID == id)
		{
			Close();
			return;
		}
		for (int i = 0; i < instance.Datas.Count; i++)
		{
			if (instance.Datas[i].ID == id)
			{
				instance.Datas.RemoveAt(i);
			}
		}
		if (instance.Datas.Count == 0)
		{
			instance.SetShowButton(false);
			instance.SetShowPanel(false);
		}
		else
		{
			instance.ButtonCountLabel.text = instance.Datas.Count.ToString();
		}
	}
}
