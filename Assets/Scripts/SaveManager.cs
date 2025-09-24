using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public LiberationData liberationData = new LiberationData();
}

[System.Serializable]
public class LiberationData
{
    public BartenderAbilityData bartenderAbilityData = new BartenderAbilityData();

    [SerializeField] private int steadfite = 0;
    public int Steadfite { get { return steadfite; } set { steadfite = value; } }
}

[System.Serializable]
public class BartenderAbilityData
{
    public bool[] bartenderAbility = new bool[6];
}

public static class SaveSystem
{
    private static string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);
    }

    public static SaveData Load()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            return JsonUtility.FromJson<SaveData>(json);
        }
        return new SaveData();
    }
}

public class SaveManager : MonoBehaviour
{
    private static SaveManager instance;

    public static SaveManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SaveManager>();
                if (instance == null)
                {
                    GameObject saveMangerOb = new GameObject("SaveManager");
                    instance = saveMangerOb.AddComponent<SaveManager>();
                    DontDestroyOnLoad(saveMangerOb);
                }
            }
            return instance;
        }
    }

    private SaveData saveData = new SaveData();

    private void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else if (instance != this) Destroy(gameObject);

        saveData = SaveSystem.Load();
        SyncFromLiberationData();
    }

    public void SaveAbility(string characterName, int point, bool enable)
    {
        switch (characterName)
        {
            case "bartender":
                saveData.liberationData.bartenderAbilityData.bartenderAbility[point] = enable;
                SaveSystem.Save(saveData);
                break;
            default:
                break;
        }
    }

    public void SetSteadfite(int value)
    {
        saveData.liberationData.Steadfite = value;
    }


    public void SyncFromLiberationData()
    {
        for(int i = 0; i <= 5; i++)
        {
            AbilityManager.Instance.SyncAbility(i, saveData.liberationData.bartenderAbilityData.bartenderAbility[i]);
        }
        GameManager.Instance.CurrentPlayer.GetComponent<PlayerStatus>().Steadfite = saveData.liberationData.Steadfite;
    }
}
