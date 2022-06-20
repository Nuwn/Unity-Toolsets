using Lazy.Utility;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using static System.Environment;

// Requires packages:
// https://github.com/Lazy-Solutions/Unity.CoroutineUtility
// Newtonsoft.json

public static class SaveSystem
{
    #region Settings
    private static SaveEventArgs saveEventArgs = new SaveEventArgs();
    
    const string global = "Global";
    const string Key = "SECRET_KEY";
    const string IV = "SECRET_IV";

    //Set the slot, so you don't have to specify it every time.
    public static int Slot { get; set; } = 0;
    #endregion

    #region Events
    public static event Action<SaveEventArgs> OnSaveRequest;
    #endregion

    #region Methods

    #endregion

    #region Save
    private static bool AutoSave() {
        OnSaveRequest?.Invoke(saveEventArgs);
        var list = SaveEventArgs.Data;

        foreach (var item in list)
            Save(item.Key, item.Value);

        SaveEventArgs.Flush();
        return true;
    }

    /// <summary>
    /// Saves Everything connected to OnSaveRequest event.
    /// </summary>
    /// <returns>True if successfull save</returns>
    public static bool QuickSave() => AutoSave();

    /// <summary>
    /// Saves to the current slot
    /// </summary>
    /// <param name="file"></param>
    /// <param name="saveObject"></param>
    public static void Save(string file, object saveObject) => 
        QueueSave(file, Slot.ToString(), saveObject);

    /// <summary>
    /// Use This to specify slot on save.
    /// </summary>
    /// <param name="file"></param>
    /// <param name="slot"></param>
    /// <param name="saveObject"></param>
    public static void SaveSlot(string file, int slot, object saveObject) => 
        QueueSave(file, slot.ToString(), saveObject);

    /// <summary>
    /// Saves to the global scope
    /// </summary>
    /// <param name="file"></param>
    /// <param name="saveObject"></param>
    public static void SaveGlobal(string file, object saveObject) => 
        QueueSave(file, global, saveObject);

    

    private static readonly Queue<(string file, string slot, object saveObject)> queued = new Queue<(string file, string slot, object saveObject)>();

    private static void Queue(string file, string slot, object saveObject) =>
        queued.Enqueue((file, slot, saveObject));

    private static void QueueSave(string file, string slot, object saveObject) =>
        Queue(file, slot, saveObject);

    private static void DoImmediateSave(string file, string slot, object saveObject)
    {
        try
        {
            var path = GetPath(slot, file.Split("/"));
            var json = JsonConvert.SerializeObject(saveObject);
            WriteFile(path, json);
        }
        catch (Exception e)
        {
            Debug.LogError("Save failed: " + e);
        }
    }

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize() => Run();

    private static GlobalCoroutine coroutine;
    public static bool isRunning => coroutine?.isRunning ?? false;

    public static void Run()
    {

        if (isRunning)
            return;

        //GlobalCoroutine.Stop() might not actually stop coroutine this frame, it'll probably run exactly one more,
        //causing onComplete to be called after already setting new value for new coroutine, if Run() is called again.
        //We would then lose reference to new coroutine, this check prevents that.
        GlobalCoroutine c = null;
        c = Coroutine().StartCoroutine(onComplete: () => { if (coroutine == c) coroutine = null; });
        coroutine = c;

        IEnumerator Coroutine()
        {
            while (true)
            {
                yield return null;
                if (queued.TryDequeue(out var item))
                {
                    DoImmediateSave(item.file, item.slot, item.saveObject);
                    if (queued.Count == 0)
                        Debug.Log("All items saved.");
                }
            }
        }

    }

    public static void Stop() => coroutine?.Stop();

    #endregion

    #region Load
    public static T Load<T>(string file) where T : class => DoLoad<T>(file, Slot.ToString());
    public static T LoadSlot<T>(string file, int slot) where T : class => DoLoad<T>(file, slot.ToString());
    public static T LoadGlobal<T>(string file) where T : class => DoLoad<T>(file, global);

    private static T DoLoad<T>(string file, string slot) where T : class
    {
        try
        {
            var path = GetPath(slot, file.Split("/"));
            string data = ReadFile(path);
            return JsonConvert.DeserializeObject<T>(data);
        }
        catch (Exception e)
        {
            Debug.LogError("Load failed: " + e);
            return null;
        }
    }
    #endregion

    #region File management
    private static string ReadFile(string Path)
    {
        using var cryptic = new DESCryptoServiceProvider();
        cryptic.Key = ASCIIEncoding.ASCII.GetBytes(Key);
        cryptic.IV = ASCIIEncoding.ASCII.GetBytes(IV);

        using var fs = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.Read);
#if !UNITY_EDITOR
        using var cr = new CryptoStream(fs, cryptic.CreateDecryptor(), CryptoStreamMode.Read);
        using var sr = new StreamReader(cr, Encoding.UTF8);
#else
        using var sr = new StreamReader(fs, Encoding.UTF8);
#endif

        string content = sr.ReadToEnd();
        return content;
    }

    // Writes an encrypted file.
    private static void WriteFile(string Path, string data)
    {
        using var cryptic = new DESCryptoServiceProvider();
        cryptic.Key = ASCIIEncoding.ASCII.GetBytes(Key);
        cryptic.IV = ASCIIEncoding.ASCII.GetBytes(IV);

        using var fs = new FileStream(Path, FileMode.Create, FileAccess.Write);

        //So we can see what is written.
#if !UNITY_EDITOR
        using var cr = new CryptoStream(fs, cryptic.CreateEncryptor(), CryptoStreamMode.Write);
        using var sr = new StreamWriter(cr, Encoding.UTF8);
#else
        using var sr = new StreamWriter(fs, Encoding.UTF8);
#endif
        sr.WriteLine(data);
    }

    /// <summary>
    /// Combines information to create a path to our save file, also generates a folder if it doesn't exist
    /// </summary>
    private static string GetPath(string slot = global, string[] category = null)
    {
        string folder = Path.Combine(GetFolderPath(SpecialFolder.MyDocuments),
                                     "My Games",
                                     Application.productName,
                                     slot,
                                     Path.Combine(category.Take(category.Length - 1).ToArray()));

        string path = Path.Combine(folder,
                                   Path.Combine(category.Last())) + ".txt";

        CheckOrCreateFolders(folder);

        return path;
    }

    private static void CheckOrCreateFolders(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

    }
    #endregion


}
public class SaveEventArgs : EventArgs
{
    public static Dictionary<string, object> Data { get; private set; } = new Dictionary<string, object>();
    public static void Flush() => Data.Clear(); 
    public void Save(string file, object saveObject) => Data.Add(file, saveObject); //todo handle multiple of same "file"
}
