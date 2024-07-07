using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private ScreenFlicker screenFlicker;
    [SerializeField] private ScreenOverlay screenOverlay;

    private float shakeAmount = 0;
    private Vector3 originalPosition;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        originalPosition = mainCamera.transform.position;
    }

    public void Shake (float amount, float length)
    {
        shakeAmount = amount;
        originalPosition = mainCamera.transform.localPosition;
        StartCoroutine(ShakeCoroutine(length));
        if (screenFlicker != null)
        {
            screenFlicker.StartFlicker();
        }
    }

    private IEnumerator ShakeCoroutine(float length)
    {
        float elapsed = 0f;

        while (elapsed < length)
        {
            Vector3 camPosition = originalPosition;

            float offsetX = Random.value * shakeAmount * 2 - shakeAmount;
            float offsetY = Random.value * shakeAmount * 2 - shakeAmount;

            camPosition.x += offsetX;
            camPosition.y += offsetY;

            mainCamera.transform.localPosition = camPosition;

            elapsed += Time.deltaTime;

            yield return null;
        }

        mainCamera.transform.localPosition = originalPosition;

        screenOverlay.ShowBlackScreen();

        yield return new WaitForSeconds(1f);

        TransitionToNextScene();
    }

    private void TransitionToNextScene()
    {
        GameObject player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            PlayerPositionManager.PlayerPosition = player.transform.position;
            Debug.Log("Player position stored: " + PlayerPositionManager.PlayerPosition);
        }

        SceneManager.LoadScene("Level_1-2");
    }
}
