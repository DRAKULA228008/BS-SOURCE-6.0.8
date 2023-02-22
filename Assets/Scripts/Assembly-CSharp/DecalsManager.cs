using UnityEngine;

public class DecalsManager : MonoBehaviour
{
	public float FrontOfWall = 0.02f;

	public MeshFilter bulletMeshFilter;

	public int bulletMaxCount = 50;

	private int bulletCount;

	private Mesh bulletMesh;

	public Transform BloodEffect;

	private int BloodEffectTimerID;

	private static DecalsManager instance;

	private Vector3[] vertices = new Vector3[50];

	private int[] triangles = new int[0];

	private Vector3[] normals;

	private Vector2[] uv;

	private static Vector3 quadNormal = new Vector3(0f, 0f, -1f);

	private static Vector3[] quadVertices = new Vector3[4]
	{
		new Vector3(-0.05f, -0.05f, 0f),
		new Vector3(0.05f, 0.05f, 0f),
		new Vector3(0.05f, -0.05f, 0f),
		new Vector3(-0.05f, 0.05f, 0f)
	};

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		CreateBulletMesh();
		EventManager.AddListener("OnSettings", UpdateSettings);
		UpdateSettings();
	}

	private void CreateBulletMesh()
	{
		bulletMesh = new Mesh();
		bulletMeshFilter.sharedMesh = bulletMesh;
		vertices = new Vector3[bulletMaxCount * 4];
		triangles = new int[bulletMaxCount * 6];
		normals = new Vector3[bulletMaxCount * 4];
		uv = new Vector2[bulletMaxCount * 4];
		Vector2[] array = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 1f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f)
		};
		for (int i = 0; i < bulletMaxCount; i++)
		{
			vertices[i * 4] = quadVertices[0] + Vector3.left * 1000f;
			vertices[i * 4 + 1] = quadVertices[1] + Vector3.left * 1000f;
			vertices[i * 4 + 2] = quadVertices[2] + Vector3.left * 1000f;
			vertices[i * 4 + 3] = quadVertices[3] + Vector3.left * 1000f;
			triangles[i * 6] = 0 + i * 4;
			triangles[i * 6 + 1] = 1 + i * 4;
			triangles[i * 6 + 2] = 2 + i * 4;
			triangles[i * 6 + 3] = 1 + i * 4;
			triangles[i * 6 + 4] = 0 + i * 4;
			triangles[i * 6 + 5] = 3 + i * 4;
			normals[i * 4] = quadNormal;
			normals[i * 4 + 1] = quadNormal;
			normals[i * 4 + 2] = quadNormal;
			normals[i * 4 + 3] = quadNormal;
			uv[i * 4] = array[0];
			uv[i * 4 + 1] = array[1];
			uv[i * 4 + 2] = array[2];
			uv[i * 4 + 3] = array[3];
		}
		bulletMesh.vertices = vertices;
		bulletMesh.triangles = triangles;
		bulletMesh.normals = normals;
		bulletMesh.uv = uv;
		bulletMesh.RecalculateBounds();
	}

	public static void FireWeapon(DecalInfo decalInfo)
	{
		nProfiler.BeginSample("DecalsManager.FireWeapon");
		for (int i = 0; i < decalInfo.Points.size; i++)
		{
			if (decalInfo.BloodDecal == i)
			{
				if (Settings.Blood)
				{
					instance.CreateBloodEffect(decalInfo.Points[i]);
				}
			}
			else if (Settings.BulletHole && !decalInfo.isKnife)
			{
				instance.CreateBulletHole(decalInfo.Points[i], decalInfo.Normals[i]);
			}
		}
		decalInfo.Dispose();
		nProfiler.EndSample();
	}

	public void CreateBulletHole(Vector3 point, Vector3 normal)
	{
		nProfiler.BeginSample("DecalsManager.CreateBulletHole");
		if (!(point == Vector3.zero) || !(normal == Vector3.zero))
		{
			Vector3 vector = point + normal * instance.FrontOfWall;
			Vector3 eulerAngles = Quaternion.FromToRotation(Vector3.back, normal).eulerAngles;
			Quaternion rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, Random.value * 360f);
			vertices[bulletCount * 4] = X(rotation, quadVertices[0]) + vector;
			vertices[bulletCount * 4 + 1] = X(rotation, quadVertices[1]) + vector;
			vertices[bulletCount * 4 + 2] = X(rotation, quadVertices[2]) + vector;
			vertices[bulletCount * 4 + 3] = X(rotation, quadVertices[3]) + vector;
			bulletMesh.vertices = vertices;
			bulletMesh.RecalculateBounds();
			bulletCount++;
			if (bulletCount >= bulletMaxCount)
			{
				bulletCount = 0;
			}
			nProfiler.EndSample();
		}
	}

	public static void ClearBulletHoles()
	{
		for (int i = 0; i < instance.bulletMaxCount; i++)
		{
			instance.vertices[i * 4] = quadVertices[0] + Vector3.left * 1000f;
			instance.vertices[i * 4 + 1] = quadVertices[1] + Vector3.left * 1000f;
			instance.vertices[i * 4 + 2] = quadVertices[2] + Vector3.left * 1000f;
			instance.vertices[i * 4 + 3] = quadVertices[3] + Vector3.left * 1000f;
		}
		instance.bulletCount = 0;
	}

	public void CreateBloodEffect(Vector3 pos)
	{
		nProfiler.BeginSample("DecalsManager.CreateBloodEffect");
		Transform activeCamera = CameraManager.ActiveCamera;
		if (!(activeCamera == null))
		{
			if (TimerManager.IsActive(BloodEffectTimerID))
			{
				TimerManager.Cancel(BloodEffectTimerID);
			}
			BloodEffect.position = pos;
			BloodEffect.LookAt(activeCamera.position);
			Vector3 eulerAngles = BloodEffect.eulerAngles;
			BloodEffect.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, Random.value * 360f);
			BloodEffectTimerID = TimerManager.In(0.05f, delegate
			{
				BloodEffect.position = Vector3.right * 5000f;
			});
			nProfiler.EndSample();
		}
	}

	private void UpdateSettings()
	{
		if (!Settings.BulletHole)
		{
			ClearBulletHoles();
		}
	}

	private static Vector3 X(Quaternion rotation, Vector3 point)
	{
		float num = rotation.x * 2f;
		float num2 = rotation.y * 2f;
		float num3 = rotation.z * 2f;
		float num4 = rotation.x * num;
		float num5 = rotation.y * num2;
		float num6 = rotation.z * num3;
		float num7 = rotation.x * num2;
		float num8 = rotation.x * num3;
		float num9 = rotation.y * num3;
		float num10 = rotation.w * num;
		float num11 = rotation.w * num2;
		float num12 = rotation.w * num3;
		Vector3 result = default(Vector3);
		result.x = (1f - (num5 + num6)) * point.x + (num7 - num12) * point.y + (num8 + num11) * point.z;
		result.y = (num7 + num12) * point.x + (1f - (num4 + num6)) * point.y + (num9 - num10) * point.z;
		result.z = (num8 - num11) * point.x + (num9 + num10) * point.y + (1f - (num4 + num5)) * point.z;
		return result;
	}
}
