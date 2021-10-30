using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
public static class LocalDataImpl
{
    private static readonly string Directory = "/CrossPromo/";

    public static string GetFilePath(string filename)
    {
        Debug.Log("Get file path" + filename);

        CreateDirIfNotExist();
        string path = Application.persistentDataPath + Directory + filename;
        if (File.Exists(path))
        {
            return path;
        }
        Debug.LogError($"File ${filename} not found");
        return default;
    }

    public static string SaveFile(byte[] raw, string filename)
    {
        Debug.Log("Saving file" + filename);

        CreateDirIfNotExist();
        var dir = CreateDirIfNotExist();
        string path = dir + filename;
        File.WriteAllBytes(path, raw);
        return path;
    }

    public static List<T> GetList<T>(string filename)
    {
        Debug.Log("Get object data");

        CreateDirIfNotExist();
        string path = Application.persistentDataPath + Directory + filename;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            List<T> data = formatter.Deserialize(stream) as List<T>;
            stream.Close();
            return data;
        }
        Debug.LogError($"Data not exist");
        return new List<T>();
    }

    public static void SaveList<T>(List<T> data, string filename)
    {
        Debug.Log("Saving list of data");

        CreateDirIfNotExist();
        string path = Application.persistentDataPath + Directory + filename;
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static string CreateDirIfNotExist()
    {
        string dir = Application.persistentDataPath + Directory;
        if (!System.IO.Directory.Exists(dir))
        {
            Debug.Log("Creating dir at " + dir);
            System.IO.Directory.CreateDirectory(dir);
        }

        return dir;
    }

    public static void Clear()
    {
        FileUtil.DeleteFileOrDirectory(Application.persistentDataPath + "/CrossPromo");
    }
}