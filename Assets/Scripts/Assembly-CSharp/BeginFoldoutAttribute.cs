using UnityEngine;

public class BeginFoldoutAttribute : PropertyAttribute
{
	public bool isFoldout;

	public BeginFoldoutAttribute(bool foldout)
	{
		isFoldout = foldout;
	}
}
