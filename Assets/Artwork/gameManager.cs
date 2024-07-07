using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class jsonRW : MonoBehaviour
{

    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private GameObject player;

    private void Start()
    {
        if (saveButton != null)
        {
            saveButton.onClick.AddListener(saveGame);
        }

        if (loadButton != null)
        {
            loadButton.onClick.AddListener(loadGame);
        }

        //saveButton.onClick.AddListener(saveGame);
        //loadButton.onClick.AddListener(loadGame);
    }

    private void saveGame()
    {
        if (player != null)
        {
            saveSystem.saveGame(player.transform.position);
        }
        //saveSystem.saveGame(player.transform.position);
    }

    private void loadGame()
    {
        //Vector3 currentPos = saveSystem.loadGame();

        //if (currentPos != Vector3.zero)
        //{
        //    player.transform.position = currentPos;
        //}

        if (player != null)
        {
            Vector3 position = saveSystem.loadGame();
            if (position != Vector3.zero)
            {
                player.transform.position = position;
            }
        }
    }
}
