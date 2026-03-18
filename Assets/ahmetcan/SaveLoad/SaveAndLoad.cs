using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class PlayerData
{
  public string username;
  public float score;
}

[System.Serializable]
public class PlayerList
{
  public List<PlayerData> players = new List<PlayerData>();
}

public class SaveAndLoad : MonoBehaviour
{
  string key = "PlayerList";

  public void Save(string username, float score)
  {
    PlayerList playerList = Load();

    bool found = false;

    foreach (var player in playerList.players)
    {
      if (player.username == username)
      {
        player.score = score;
        found = true;
        break;
      }
    }

    if (!found)
    {
      PlayerData newPlayer = new PlayerData();
      newPlayer.username = username;
      newPlayer.score = score;
      playerList.players.Add(newPlayer);
    }

    string json = JsonUtility.ToJson(playerList);
    PlayerPrefs.SetString(key, json);
    PlayerPrefs.Save();
  }
  
  public PlayerList Load()
  {
    if (PlayerPrefs.HasKey(key))
    {
      string json = PlayerPrefs.GetString(key);
      return JsonUtility.FromJson<PlayerList>(json);
    }

    return new PlayerList();
  }
  
  public GameObject textPrefab;   
  public Transform parentObjectScore;  

  public void ShowPlayers()
  {
    PlayerList list = Load();

    foreach (var player in list.players)
    {
      GameObject obj = Instantiate(textPrefab, parentObjectScore);

      TMP_Text text = obj.GetComponent<TMP_Text>();
      text.text = player.username + " : " + player.score;
    }
  }
}
