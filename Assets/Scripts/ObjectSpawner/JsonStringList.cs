using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class JsonStringList : ScriptableObject
{
    public List<string> strings;

    internal string GetString(int idInList)
    {
        if(idInList > strings.Count) { return ""; }
        return strings[idInList];
    }

    internal void SetString(string jsonString, int idInList)
    {
        if(strings.Count <= idInList) 
        {
            int count = strings.Count - idInList + 1;
            while(count > 0)
            {
                strings.Add("");
                count--;
            }
        }
        strings[idInList] = jsonString;
    }
}
