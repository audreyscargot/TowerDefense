using UnityEngine;
using UnityEngine.SceneManagement;
public class GameOver : MonoBehaviour
{
  public void PlayGame() { SceneManager.LoadSceneAsync(2);} // main game map
  
  public void ReturnToMainMenu(){ SceneManager.LoadSceneAsync(3);} // main menu map
}
