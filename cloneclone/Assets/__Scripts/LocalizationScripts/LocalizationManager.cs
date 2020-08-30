﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LocalizationManager : MonoBehaviour
{

    public static LocalizationManager instance;

    private Dictionary<string, string> localizedText;
    private bool isReady = false;
    private string missingTextString = "Localized text not found";

    public TextAsset masterText_EN;
    public TextAsset masterText_ES;

    // Use this for initialization
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            LoadLocalizedText("EN"); // COLIN DONT PUSH!
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void LoadLocalizedText(string languageKey)
    {
        localizedText = new Dictionary<string, string>();

        // if english
        if (masterText_EN != null && languageKey == "EN")
        {
            string dataAsJson = masterText_EN.text;
            //Debug.Log(dataAsJson);
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            for (int i = 0; i < loadedData.items.Length; i++)
            {
                //Debug.Log("Adding key " + loadedData.items[i].key);
                localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
            }
        }
        else if (masterText_ES != null && languageKey == "ES") {
            string dataAsJson = masterText_ES.text;
            //Debug.Log(dataAsJson);
            LocalizationData loadedData = JsonUtility.FromJson<LocalizationData>(dataAsJson);

            for (int i = 0; i < loadedData.items.Length; i++)
            {
                //Debug.Log("Adding key " + loadedData.items[i].key);
                localizedText.Add(loadedData.items[i].key, loadedData.items[i].value);
            }
        }
        else
        {
            Debug.LogError("Cannot find file!");
        }

        isReady = true;
    }

    public string GetLocalizedValue(string key, bool displayOnFail = false)
    {
        string result = missingTextString;
        if (key == ""){
            result = "";
        }
        if (localizedText.ContainsKey(key))
        {
            result = localizedText[key];
        }else if (displayOnFail){
            result = key;
        }else{
            Debug.Log("Could not find key: " + key);
        }

        return result;

    }

    public bool GetIsReady()
    {
        return isReady;
    }

}