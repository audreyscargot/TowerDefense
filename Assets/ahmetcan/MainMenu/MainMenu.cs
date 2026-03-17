using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
  public void PlayGame() { SceneManager.LoadSceneAsync(2);} // main game map
  public void Scores() {Debug.Log("scores");} // scores
  public void ExitGame(){Application.Quit();} // exit game
}
