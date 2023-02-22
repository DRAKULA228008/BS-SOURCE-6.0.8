using UnityEngine;

public class StringPopup : PropertyAttribute
{
	public string[] list;

	public int selected;

	public StringPopup(params string[] textList)
	{
		list = textList;
	}
}
