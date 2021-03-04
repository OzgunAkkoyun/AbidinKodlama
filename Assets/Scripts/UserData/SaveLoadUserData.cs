using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class SaveLoadUserData : MonoBehaviour
{
    public static SaveLoadUserData instance;
    public List<UserData> userDataList = new List<UserData>();

    void Awake()
    {
        instance = this;
        GetUserData();
    }

    public List<float> GetSpecificUserData(int senarioIndex, int levelIndex, int subLevelIndex)
    {
        var getSpesificList = userDataList
            .FindAll(v =>  (v.senarioIndex == senarioIndex && v.levelIndex == levelIndex && v.subLevelIndex == subLevelIndex))
            .Select(v => v.userTime).ToList();
        return getSpesificList;
    }

    public void SaveUserData(int senarioIndex, int levelIndex, int subLevelIndex, float duration)
    {
        userDataList.Add(new UserData(senarioIndex, levelIndex, subLevelIndex, duration));

        string userDataJson = JsonConvert.SerializeObject(userDataList, Formatting.Indented, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
        PlayerPrefs.SetString("userData", userDataJson);
    }

    public void GetUserData()
    {
        var userDataString = PlayerPrefs.GetString("userData");
        if (userDataString != "")
        {
            userDataList = JsonConvert.DeserializeObject<List<UserData>>(userDataString, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        }
    }
}
