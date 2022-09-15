using UnityEngine;

namespace WaveCaveGames.CheckersGame{
	
	public class MenuManager : MonoBehaviour
	{
		public void LoadScene(string sceneName){
			UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
		}
		public void Quit(){
			#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
			#else
			Application.Quit();
			#endif
		}
		public void OpenURL(string url){
			Application.OpenURL(url);
		}
	}
}
