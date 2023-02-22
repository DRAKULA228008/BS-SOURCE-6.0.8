namespace UnityEngine.SceneManagement
{
	public class SceneManager
	{
		public static void LoadScene(string name)
		{
			Application.LoadLevel(name);
		}

		public static void LoadScene(int buildIndex)
		{
			Application.LoadLevel(buildIndex);
		}

		public static AsyncOperation LoadSceneAsync(string name, LoadSceneMode mode = LoadSceneMode.Single)
		{
			if (mode == LoadSceneMode.Single)
			{
				return Application.LoadLevelAsync(name);
			}
			return Application.LoadLevelAdditiveAsync(name);
		}

		public static AsyncOperation LoadSceneAsync(int buildIndex, LoadSceneMode mode = LoadSceneMode.Single)
		{
			if (mode == LoadSceneMode.Single)
			{
				return Application.LoadLevelAsync(buildIndex);
			}
			return Application.LoadLevelAdditiveAsync(buildIndex);
		}
	}
}
