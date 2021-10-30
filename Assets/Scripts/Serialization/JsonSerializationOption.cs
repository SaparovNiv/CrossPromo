using UnityEngine;
using System;

public class JsonSerializationOption : ISerializationOption
{
    string ISerializationOption.ContentType => "application/json";

    public T Deserialize<T>(string text)
    {
        try
        {
            var result = JsonUtility.FromJson<T>(text);
            return result;
        }
        catch (Exception e)
        {
            Debug.Log($"could not parse json {text}, {e.Message}");
            return default;
        }
    }
}
