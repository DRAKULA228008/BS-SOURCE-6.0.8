using UnityEngine;

public class PaintColor : MonoBehaviour
{
	public MeshFilter face;

	public int id;

	private void Start()
	{
		Color32 color = PaintManager.GetColor(id);
		Color32[] colors = new Color32[4] { color, color, color, color };
		face.mesh.colors32 = colors;
	}

	private void OnPaint()
	{
		PaintManager.SetColor(id);
	}
}
