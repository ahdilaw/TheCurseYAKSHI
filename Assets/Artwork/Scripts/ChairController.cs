using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChairController : MonoBehaviour, Interactable
{
    private CameraShake camShake;
    [SerializeField] private float camShakeAmt = 0.5f;
    [SerializeField] private float camShakeLength = 1f;

    private void Awake()
    {
        camShake = FindObjectOfType<CameraShake>();
    }

    public void Interact()
    {
        StartCoroutine(StartChairCutscene());
    }

    private IEnumerator StartChairCutscene()
    {
        CutsceneManager.Instance.StartCutscene();

        camShake.Shake(camShakeAmt, camShakeLength);

        yield return new WaitForSeconds(camShakeLength);

        CutsceneManager.Instance.EndCutscene();
    }
}
