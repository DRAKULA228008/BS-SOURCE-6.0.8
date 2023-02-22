using UnityEngine;

public class UIUpdateManager : MonoBehaviour
{
	private static UIUpdateManager instance;

	private int regularUpdateArrayCount;

	private int lateUpdateArrayCount;

	private UIMonoBehaviour[] regularArray = new UIMonoBehaviour[0];

	private UIMonoBehaviour[] lateArray = new UIMonoBehaviour[0];

	private int frameCount;

	public UIUpdateManager()
	{
		instance = this;
	}

	public static void AddItem(UIMonoBehaviour behaviour)
	{
		instance.AddItemToArray(behaviour);
	}

	public static void RemoveSpecificItem(UIMonoBehaviour behaviour)
	{
		instance.RemoveSpecificItemFromArray(behaviour);
	}

	public static void RemoveSpecificItemAndDestroyIt(UIMonoBehaviour behaviour)
	{
		instance.RemoveSpecificItemFromArray(behaviour);
		Object.Destroy(behaviour.gameObject);
	}

	private void AddItemToArray(UIMonoBehaviour behaviour)
	{
		if (behaviour.GetType().GetMethod("nUpdate").DeclaringType != typeof(UIMonoBehaviour))
		{
			regularArray = ExtendAndAddItemToArray(regularArray, behaviour);
			regularUpdateArrayCount++;
		}
		if (behaviour.GetType().GetMethod("nLateUpdate").DeclaringType != typeof(UIMonoBehaviour))
		{
			lateArray = ExtendAndAddItemToArray(lateArray, behaviour);
			lateUpdateArrayCount++;
		}
	}

	public UIMonoBehaviour[] ExtendAndAddItemToArray(UIMonoBehaviour[] original, UIMonoBehaviour itemToAdd)
	{
		int num = original.Length;
		UIMonoBehaviour[] array = new UIMonoBehaviour[num + 1];
		for (int i = 0; i < num; i++)
		{
			array[i] = original[i];
		}
		array[array.Length - 1] = itemToAdd;
		return array;
	}

	private void RemoveSpecificItemFromArray(UIMonoBehaviour behaviour)
	{
		if (CheckIfArrayContainsItem(regularArray, behaviour))
		{
			regularArray = ShrinkAndRemoveItemToArray(regularArray, behaviour);
			regularUpdateArrayCount--;
		}
		if (CheckIfArrayContainsItem(lateArray, behaviour))
		{
			lateArray = ShrinkAndRemoveItemToArray(lateArray, behaviour);
			lateUpdateArrayCount--;
		}
	}

	public bool CheckIfArrayContainsItem(UIMonoBehaviour[] arrayToCheck, UIMonoBehaviour objectToCheckFor)
	{
		int num = arrayToCheck.Length;
		for (int i = 0; i < num; i++)
		{
			if (objectToCheckFor == arrayToCheck[i])
			{
				return true;
			}
		}
		return false;
	}

	public UIMonoBehaviour[] ShrinkAndRemoveItemToArray(UIMonoBehaviour[] original, UIMonoBehaviour itemToRemove)
	{
		int num = original.Length;
		UIMonoBehaviour[] array = new UIMonoBehaviour[num - 1];
		for (int i = 0; i < num; i++)
		{
			if (!(original[i] == itemToRemove))
			{
				array[i] = original[i];
			}
		}
		return array;
	}

	private void Update()
	{
		frameCount = Time.frameCount;
		if (regularUpdateArrayCount == 0)
		{
			return;
		}
		for (int i = 0; i < regularUpdateArrayCount; i++)
		{
			if (!(regularArray[i] == null))
			{
				regularArray[i].nUpdate(frameCount);
			}
		}
	}

	private void LateUpdate()
	{
		if (lateUpdateArrayCount == 0)
		{
			return;
		}
		for (int i = 0; i < lateUpdateArrayCount; i++)
		{
			if (!(lateArray[i] == null))
			{
				lateArray[i].nLateUpdate(frameCount);
			}
		}
	}
}
