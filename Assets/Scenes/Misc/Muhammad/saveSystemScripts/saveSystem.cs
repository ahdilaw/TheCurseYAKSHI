using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class saveSystem : MonoBehaviour
{
    private static string dataPath = Application.dataPath;
    private static string jsonPath = "/Scenes/Muhammad/savedata.json";
    private static string newSavedPos = dataPath + jsonPath;

    public static void saveGame(Vector3 currentPlayerPos)
    {
        string json;
        gameData newData;

        newData = new gameData(currentPlayerPos);
        json = JsonUtility.ToJson(newData);
        File.WriteAllText(newSavedPos, json);
        Debug.Log("Game Saved to: " + Application.dataPath + jsonPath);
    }

    public static Vector3 loadGame()
    {
        if (!File.Exists(newSavedPos))
        {
            return Vector3.zero;
        }

        string json;
        gameData newData;

        json = File.ReadAllText(newSavedPos);
        newData = JsonUtility.FromJson<gameData>(json);

        //return new Vector3(newData.positionX newData.positionY, newData.positionZ);
        return new Vector3(newData.setPosX, newData.setPosY, newData.setPosZ);

        //if (File.Exists(newSavedPos))
        //{
        //    string json = File.ReadAllText(newSavedPos);
        //    gameData data = JsonUtility.FromJson<gameData>(json);

        //    return new Vector3(data.positionX, data.positionY, data.positionZ);
        //}

        //else
        //{
        //    return Vector3.zero;
        //}
    }
}
