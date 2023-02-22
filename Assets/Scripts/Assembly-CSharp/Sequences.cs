using DG.Tweening;
using UnityEngine;

public class Sequences : MonoBehaviour
{
	public Transform target;

	private void Start()
	{
		Sequence sequence = DOTween.Sequence();
		sequence.Append(target.DOMoveY(2f, 1f));
		sequence.Join(target.DORotate(new Vector3(0f, 135f, 0f), 1f));
		sequence.Append(target.DOScaleY(0.2f, 1f));
		sequence.Insert(0f, target.DOMoveX(4f, sequence.Duration()).SetRelative());
		sequence.SetLoops(4, LoopType.Yoyo);
	}
}
