using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour, Interactable
{
    [SerializeField] private string sceneName;
    [SerializeField] private ScreenFade screenFade;

    private void Awake()
    {
        if (screenFade == null)
        {
            screenFade = FindObjectOfType<ScreenFade>();
        }
    }
    public void Interact()
    {
        StartCoroutine(FadeOutAndTransition());
    }

    private IEnumerator FadeOutAndTransition()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            PlayerPositionManager.PlayerPosition = player.transform.position;
        }

        if (screenFade != null)
        {
            yield return screenFade.FadeOutAndLoadScene(sceneName);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }
}