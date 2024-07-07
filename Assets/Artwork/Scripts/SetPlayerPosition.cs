using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPlayerPosition : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(SetPositionAfterFrame());
    }

    private IEnumerator SetPositionAfterFrame()
    {
        yield return null;

        Debug.Log("Setting player position to: " + PlayerPositionManager.PlayerPosition);
        transform.position = PlayerPositionManager.PlayerPosition;

        ScreenOverlay screenOverlay = FindObjectOfType<ScreenOverlay>();
        if (screenOverlay != null)
        {
            screenOverlay.StartCoroutine(screenOverlay.FadeOut());
        }
    }
}
