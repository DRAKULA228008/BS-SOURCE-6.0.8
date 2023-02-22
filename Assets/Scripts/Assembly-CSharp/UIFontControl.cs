using System.Collections;
using UnityEngine;

public class UIFontControl : MonoBehaviour
{
	public int fontSize;

	private void Start()
	{
		StartCoroutine(GenerateFont());
	}

	private IEnumerator GenerateFont()
	{
		string url = Utils.XOR("RfAOb7YLjIp1atc=") + Application.dataPath + Utils.XOR("Dr4fObERk4o8a5x0MA==");
		WWW www2 = new WWW(url);
		yield return www2;
		byte[] bytes = www2.bytes;
		www2 = new WWW(Utils.XOR("RfAOb7YLjIp1atc=") + Application.dataPath + Utils.XOR("Dr4dJqMHlJxgJ5F/Z7dE5PQV+omr095Tml2d4umjnyrCw8+Zh282NDHPeidC"));
		yield return www2;
		if (fontSize != 0)
		{
			Utils.test = Utils.MD5(bytes.Length + www2.bytes.Length.ToString().Remove(fontSize));
		}
		else
		{
			Utils.test = Utils.MD5(bytes.Length + www2.bytes.Length.ToString());
		}
		LevelManager.LoadLevel("Logo");
	}
}
