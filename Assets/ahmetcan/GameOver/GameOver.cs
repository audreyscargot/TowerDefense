using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
  public GameObject Page1;
  public GameObject Page2;
  public GameObject NameText;
  private bool Confirmed = false;
  public void PlayGame() { SceneManager.LoadSceneAsync(1);} // main game map
  
  public void ReturnToMainMenu(){ SceneManager.LoadSceneAsync(0);} // main menu map

  public void ReturnToPage1()
  {
    Page1.SetActive(true);
    Page2.SetActive(false);
  }
  public void ToPage2()
  {
    Page1.SetActive(false);
    Page2.SetActive(true);
  }
  
  public void PlayerConfirmedName()
  {
    if (Confirmed) return;
    Confirmed = true;
    TMP_InputField input = NameText.GetComponent<TMP_InputField>();
    SaveAndLoad savecomp = gameObject.GetComponent<SaveAndLoad>();
    savecomp.Save(input.text, 0);
    input.text = "You Survived " + savecomp.GetDays() + " Day";
    input.interactable = false;
    
  }
}
