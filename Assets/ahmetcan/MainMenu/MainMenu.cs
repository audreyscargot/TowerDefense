using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
  public GameObject Page1;
  public GameObject Page2;
  public void PlayGame() { SceneManager.LoadSceneAsync(2);} // main game map
  public void ExitGame(){Application.Quit();} // exit game

  public void ToPage2()
  {
    Page1.SetActive(false);
    Page2.SetActive(true);
    SaveAndLoad SL = gameObject.GetComponent<SaveAndLoad>();
    SL.ShowPlayers();
  }
  public void ToPage1()
  {
    Page1.SetActive(true);                                                                          
    Page2.SetActive(false);
  }
  
}
