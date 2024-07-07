using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    // Start is called before the first frame update


    [SerializeField] private GameObject menu;
    void Start()
    {
        if (menu != null )
        {
            menu.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (menu != null)
            {
                Time.timeScale = 0;
                menu.SetActive(true);
            }
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        menu.SetActive(false);
    }

    public void ExitGame()
    {
        if (menu != null)
        {
            Application.Quit();
            Debug.Log("The game has quited.");
        }
    }
}
