using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class gameData
{
    [SerializeField] private float positionX;
    [SerializeField] private float positionY;
    [SerializeField] private float positionZ;
    [SerializeField] private string sceneName; // Add this line

    public float setPosX
    {
        get => positionX;
        set => positionX = value;
    }

    public float setPosY
    {
        get => positionY;
        set => positionY = value;
    }

    public float setPosZ
    {
        get => positionZ;
        set => positionZ = value;
    }

    public string setSceneName
    {
        get => sceneName;
        set => sceneName = value;
    }

    public gameData(Vector3 currentPlayerPos, string currentSceneName)
    {
        positionX = currentPlayerPos.x;
        positionY = currentPlayerPos.y;
        positionZ = currentPlayerPos.z;
        sceneName = currentSceneName;
    }
}
