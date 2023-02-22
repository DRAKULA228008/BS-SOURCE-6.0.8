using DG.Tweening;
using UnityEngine;

public class Basics : MonoBehaviour
{
	public Transform cubeA;

	public Transform cubeB;

	private void Start()
	{
		DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
		cubeA.DOMove(new Vector3(-2f, 2f, 0f), 1f).SetRelative().SetLoops(-1, LoopType.Yoyo);
		DOTween.To(() => cubeB.position, delegate(Vector3 x)
		{
			cubeB.position = x;
		}, new Vector3(-2f, 2f, 0f), 1f).SetRelative().SetLoops(-1, LoopType.Yoyo);
	}
}
