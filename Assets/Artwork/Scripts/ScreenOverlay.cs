using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenOverlay : MonoBehaviour
{
    [SerializeField] private Image blackScreen;
    [SerializeField] private float fadeDuration = 1.0f;

    private void Awake()
    {
        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(false);
        }
    }

    public void ShowBlackScreen()
    {
        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
            StartCoroutine(FadeIn());
        }
    }

    public void HideBlackScreen()
    {
        if (blackScreen != null)
        {
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;
        Color color = blackScreen.color;

        while (elapsed < fadeDuration)
        {
            color.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            blackScreen.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }

        color.a = 1f;
        blackScreen.color = color;
    }

    public IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color color = blackScreen.color;

        while (elapsed < fadeDuration)
        {
            color.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            blackScreen.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }

        color.a = 0f;
        blackScreen.color = color;
        blackScreen.gameObject.SetActive(false);
    }
}