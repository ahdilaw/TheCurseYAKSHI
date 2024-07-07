using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        if (saveSystem.SaveDataExists())
        {
            saveSystem.loadGame();

            //if (!string.IsNullOrEmpty(sceneName))
            //{
            //    StartCoroutine(LoadSceneAndSetPosition(sceneName, playerPos));
            //}
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    private IEnumerator LoadSceneAndSetPosition(string sceneName, Vector3 playerPos)
    {
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

      
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = playerPos;
        }
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}



