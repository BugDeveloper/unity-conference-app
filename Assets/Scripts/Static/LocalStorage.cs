using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEngine;

public static class LocalStorage
{
    public static void Initialize()
    {
        Debug.Log(Application.persistentDataPath);

        if (!DirExists(ImageDownloader.PicDir))
            CreateDir(ImageDownloader.PicDir);

        if (!DirExists(GoogleMap.MapDir))
            CreateDir(GoogleMap.MapDir);
    }

    public static void DeleteFilesInDir(string dir)
    {
        var files = Directory.GetFiles(Application.persistentDataPath + "/" + dir);

        foreach (var file in files)
        {
            File.Delete(file);
        }
    }

    public static void DeleteFilesInDir()
    {
        var files = Directory.GetFiles(Application.persistentDataPath);

        foreach (var file in files)
        {
            File.Delete(file);
        }
    }

    public static void ClearCache()
    {
        DeleteFilesInDir();
        DeleteFilesInDir("maps");
        DeleteFilesInDir("news");
    }

    public static void DeleteFile(string file)
    {
        File.Delete(Application.persistentDataPath + "/" + file);
    }

    public static bool FileExists(string file)
    {
        return File.Exists(Application.persistentDataPath + "/" + file);
    }

    public static bool DirExists(string dirname)
    {
        return Directory.Exists(Application.persistentDataPath + "/" + dirname);
    }

    public static void CreateDir(string dirname)
    {
        Directory.CreateDirectory(Application.persistentDataPath + "/" + dirname);
    }

    public static object Load(string filename)
    {
        var bf = new BinaryFormatter();
        var file = File.Open(Application.persistentDataPath + "/" + filename, FileMode.OpenOrCreate);
        var obj = bf.Deserialize(file);
        file.Close();
        return obj;
    }

    public static void Save(string filename, object obj)
    {
        var bf = new BinaryFormatter();
        var file = File.Create(Application.persistentDataPath + "/" + filename);

        bf.Serialize(file, obj);
        file.Close();
    }

}