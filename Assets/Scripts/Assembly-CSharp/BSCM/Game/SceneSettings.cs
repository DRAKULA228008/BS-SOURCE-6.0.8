using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BSCM.Game
{
	public class SceneSettings : MonoBehaviour
	{
		[Serializable]
		public class ModeSettings
		{
			public GameMode mode;

			public byte maxScore = 100;

			public float time = 120f;

			public float respawnNoDamage = 4f;
		}

		public ModeSettings[] modes;

		public Transform redSpawn;

		public Transform blueSpawn;

		public Transform cameraStatic;

		public static SceneSettings instance;

		private void Awake()
		{
			instance = this;
			CheckScene();
			UnityEngine.Object.Instantiate(GameSettings.instance.UIRoot).name = "UIRoot";
			if (PhotonNetwork.isMasterClient)
			{
				GameObject gameObject = PhotonNetwork.InstantiateSceneObject("RoundManager", Vector3.zero, Quaternion.identity, 0, null);
				gameObject.name = "RoundManager";
			}
		}

		public void Create()
		{
			ModeSettings settings = GetSettings(PhotonNetwork.room.GetGameMode());
			if (settings == null)
			{
				PhotonNetwork.LeaveRoom();
				return;
			}
			GameManager.maxScore = settings.maxScore;
			GameManager.startDamageTime = settings.respawnNoDamage;
			if (blueSpawn != null)
			{
				SpawnPoint teamSpawn = SpawnManager.GetTeamSpawn(Team.Blue);
				teamSpawn.spawnPosition = blueSpawn.position;
				teamSpawn.spawnRotation = blueSpawn.eulerAngles;
				teamSpawn.spawnScale = blueSpawn.localScale;
			}
			if (redSpawn != null)
			{
				SpawnPoint teamSpawn = SpawnManager.GetTeamSpawn(Team.Red);
				teamSpawn.spawnPosition = redSpawn.position;
				teamSpawn.spawnRotation = redSpawn.eulerAngles;
				teamSpawn.spawnScale = redSpawn.localScale;
			}
			if (cameraStatic != null)
			{
				SpawnPoint teamSpawn = SpawnManager.GetCameraStatic();
				teamSpawn.spawnPosition = cameraStatic.position;
				teamSpawn.spawnRotation = cameraStatic.eulerAngles;
				teamSpawn.spawnScale = cameraStatic.localScale;
			}
		}

		public ModeSettings GetSettings(GameMode mode)
		{
			for (int i = 0; i < modes.Length; i++)
			{
				if (modes[i].mode == mode)
				{
					return modes[i];
				}
			}
			return null;
		}

		private void CheckScene()
		{
			MeshRenderer[] array = UnityEngine.Object.FindObjectsOfType<MeshRenderer>();
			for (int i = 0; i < array.Length; i++)
			{
				MaterialsID component = array[i].GetComponent<MaterialsID>();
				if (component != null)
				{
					Material[] materials = array[i].materials;
					for (int j = 0; j < materials.Length; j++)
					{
						if (component.id.Count - 1 >= j)
						{
							materials[j] = GameSettings.instance.CustomMaterials[component.id[j]];
						}
						else
						{
							materials[j] = GameSettings.instance.CustomMaterialError;
						}
					}
					array[i].materials = materials;
				}
				else
				{
					Material[] materials = array[i].materials;
					for (int k = 0; k < materials.Length; k++)
					{
						materials[k] = GameSettings.instance.CustomMaterialError;
					}
					array[i].materials = materials;
				}
			}
			Canvas[] array2 = UnityEngine.Object.FindObjectsOfType<Canvas>();
			for (int l = 0; l < array2.Length; l++)
			{
				array2[l].gameObject.SetActive(false);
			}
			EventSystem[] array3 = UnityEngine.Object.FindObjectsOfType<EventSystem>();
			for (int m = 0; m < array3.Length; m++)
			{
				array3[m].gameObject.SetActive(false);
			}
			AudioSource[] array4 = UnityEngine.Object.FindObjectsOfType<AudioSource>();
			for (int n = 0; n < array4.Length; n++)
			{
				array4[n].gameObject.SetActive(false);
			}
			AudioListener[] array5 = UnityEngine.Object.FindObjectsOfType<AudioListener>();
			for (int num = 0; num < array5.Length; num++)
			{
				array5[num].enabled = false;
			}
			Text[] array6 = UnityEngine.Object.FindObjectsOfType<Text>();
			for (int num2 = 0; num2 < array6.Length; num2++)
			{
				array6[num2].enabled = false;
			}
			Texture[] array7 = UnityEngine.Object.FindObjectsOfType<Texture>();
			for (int num3 = 0; num3 < array7.Length; num3++)
			{
				//array7[num3].enabled = false;
			}
			Camera[] array8 = UnityEngine.Object.FindObjectsOfType<Camera>();
			for (int num4 = 0; num4 < array8.Length; num4++)
			{
				array8[num4].enabled = false;
			}
			ParticleSystemRenderer[] array9 = UnityEngine.Object.FindObjectsOfType<ParticleSystemRenderer>();
			for (int num5 = 0; num5 < array9.Length; num5++)
			{
				MaterialsID component = array9[num5].GetComponent<MaterialsID>();
				if (component != null)
				{
					Material[] materials = array9[num5].materials;
					for (int num6 = 0; num6 < materials.Length; num6++)
					{
						if (component.id.Count - 1 >= num6)
						{
							materials[num6] = GameSettings.instance.CustomMaterials[component.id[num6]];
						}
						else
						{
							materials[num6] = GameSettings.instance.CustomMaterialError;
						}
					}
					array9[num5].materials = materials;
				}
				else
				{
					Material[] materials = array9[num5].materials;
					for (int num7 = 0; num7 < materials.Length; num7++)
					{
						materials[num7] = GameSettings.instance.CustomMaterialError;
					}
					array9[num5].materials = materials;
				}
			}
			TrailRenderer[] array10 = UnityEngine.Object.FindObjectsOfType<TrailRenderer>();
			for (int num8 = 0; num8 < array10.Length; num8++)
			{
				MaterialsID component = array10[num8].GetComponent<MaterialsID>();
				if (component != null)
				{
					Material[] materials = array10[num8].materials;
					for (int num9 = 0; num9 < materials.Length; num9++)
					{
						if (component.id.Count - 1 >= num9)
						{
							materials[num9] = GameSettings.instance.CustomMaterials[component.id[num9]];
						}
						else
						{
							materials[num9] = GameSettings.instance.CustomMaterialError;
						}
					}
					array10[num8].materials = materials;
				}
				else
				{
					Material[] materials = array10[num8].materials;
					for (int num10 = 0; num10 < materials.Length; num10++)
					{
						materials[num10] = GameSettings.instance.CustomMaterialError;
					}
					array10[num8].materials = materials;
				}
			}
			LineRenderer[] array11 = UnityEngine.Object.FindObjectsOfType<LineRenderer>();
			for (int num11 = 0; num11 < array11.Length; num11++)
			{
				MaterialsID component = array11[num11].GetComponent<MaterialsID>();
				Material[] materials = array11[num11].materials;
				if (component != null)
				{
					for (int num12 = 0; num12 < materials.Length; num12++)
					{
						if (component.id.Count - 1 >= num12)
						{
							materials[num12] = GameSettings.instance.CustomMaterials[component.id[num12]];
						}
						else
						{
							materials[num12] = GameSettings.instance.CustomMaterialError;
						}
					}
				}
				else
				{
					for (int num13 = 0; num13 < materials.Length; num13++)
					{
						materials[num13] = GameSettings.instance.CustomMaterialError;
					}
				}
				array11[num11].materials = materials;
			}
			LensFlare[] array12 = UnityEngine.Object.FindObjectsOfType<LensFlare>();
			for (int num14 = 0; num14 < array12.Length; num14++)
			{
				array12[num14].enabled = false;
			}
			Projector[] array13 = UnityEngine.Object.FindObjectsOfType<Projector>();
			for (int num15 = 0; num15 < array13.Length; num15++)
			{
				array13[num15].enabled = false;
			}
			ParticleSystemRenderer[] array14 = UnityEngine.Object.FindObjectsOfType<ParticleSystemRenderer>();
			for (int num16 = 0; num16 < array14.Length; num16++)
			{
				array14[num16].enabled = false;
			}
			SpriteRenderer[] array15 = UnityEngine.Object.FindObjectsOfType<SpriteRenderer>();
			for (int num17 = 0; num17 < array15.Length; num17++)
			{
				array15[num17].enabled = false;
			}
		}
	}
}
