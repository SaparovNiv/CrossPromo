using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;

public class NetworkImpl 
{
    private readonly ISerializationOption serializationOption;
    public NetworkImpl(ISerializationOption serializationOption)
    {
        this.serializationOption = serializationOption;
    }

    public IEnumerator Get(string endpoint) => Get<string>(endpoint, null);
    public IEnumerator Get<T>(string endpoint, Action<T> callback)
    {
        Debug.Log($"Send request to {endpoint}");

        UnityWebRequest request = UnityWebRequest.Get(endpoint);
        request.SetRequestHeader("Content-Type", serializationOption.ContentType);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
        }

        var jsonResponse = request.downloadHandler.text;
        var result = serializationOption.Deserialize<T>(jsonResponse);

        if (result != null)
        {
            callback(result);
        }
    }

    public IEnumerator Download(string endpoint, string filename, Action<byte[], string> callback)
    {
        Debug.Log($"Send request to {endpoint}");

        UnityWebRequest request = UnityWebRequest.Get(endpoint);
        request.SetRequestHeader("Content-Type", serializationOption.ContentType);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
        }

        var data = request.downloadHandler.data;
        if (data != null)
        {
            callback(data, filename);
        }
    }

}
