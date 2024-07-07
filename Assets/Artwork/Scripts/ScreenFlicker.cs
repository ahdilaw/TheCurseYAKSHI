using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFlicker : MonoBehaviour
{
    [SerializeField] private Image blackScreen;
    [SerializeField] private float flickerDuration = 1.0f;
    [SerializeField] private float flickerFrequency = 0.1f;
    private Coroutine flickerCoroutine;

    private void Awake()
    {
        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(false);
        }
    }

    public void StartFlicker()
    {
        if (flickerCoroutine != null)
        {
            StopCoroutine(flickerCoroutine);
        }

        flickerCoroutine = StartCoroutine(FlickerCoroutine());
    }

    private IEnumerator FlickerCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < flickerDuration)
        {
            if (blackScreen != null)
            {
                blackScreen.gameObject.SetActive(!blackScreen.gameObject.activeSelf);
            }

            elapsed += flickerFrequency;
            yield return new WaitForSeconds(flickerFrequency);
        }

        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(false);
        }
    }
}
