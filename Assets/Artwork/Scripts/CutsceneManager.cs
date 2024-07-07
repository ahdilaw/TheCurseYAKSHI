using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{   
    private bool isCutsceneActive = false;
    public event Action OnStartCutscene;
    public event Action OnEndCutscene;

    public static CutsceneManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public bool IsCutsceneActive()
    {
        return isCutsceneActive;
    }

    public void StartCutscene()
    {
        isCutsceneActive = true;
        OnStartCutscene?.Invoke();
    }

    public void EndCutscene()
    {
        isCutsceneActive = false;
        OnEndCutscene?.Invoke();
    }
}
