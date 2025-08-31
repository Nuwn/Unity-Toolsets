using Lazy.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

// With ASM
//using AdvancedSceneManager.Utility;


namespace LazySaveSystem
{

    // Requires packages:
    // https://github.com/Lazy-Solutions/Unity.CoroutineUtility
    // *included in ASM

    public static partial class SaveSystem
    {
        #region Settings
        private static readonly SaveEventArgs saveEventArgs = new();

        const string global = "Global";

        private static (byte[] Key, byte[] IV) encryption =
            (Encoding.UTF8.GetBytes("your-32-byte-long-key-for-256b--"),
             Encoding.UTF8.GetBytes("your-16-byte-IV1"));

        // To set new key, do it before saving or yeee... 
        public static void SetEncryption(string Key, string Iv) =>
            encryption = (Encoding.UTF8.GetBytes(Key), Encoding.UTF8.GetBytes(Iv));

        private static string RootPath => Path.Combine(Application.persistentDataPath, Application.productName);

        //Set the slot, so you don't have to specify it every time.
        public static int Slot { get; set; } = 0;
        #endregion

        #region Events
        public static event Action<SaveEventArgs> OnSaveRequest;

        #endregion

        #region Autosave
        private static bool AutoSave()
        {
            try
            {
                OnSaveRequest?.Invoke(saveEventArgs);
                var list = SaveEventArgs.Data;

                foreach (var item in list)
                {
                    Save(item.Key, item.Value);
                }

                SaveEventArgs.Flush();

                // Return true indicating the save operation was successful
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log($"AutoSave encountered an error: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region Save

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
        public static void Save(string file, object saveObject) => QueueSave(file, Slot.ToString(), saveObject);
        /// <summary>
        /// Use This to specify slot on save.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="slot"></param>
        /// <param name="saveObject"></param>
        public static void SaveSlot(string file, int slot, object saveObject) => QueueSave(file, slot.ToString(), saveObject);
        /// <summary>
        /// Saves to the global scope
        /// </summary>
        /// <param name="file"></param>
        /// <param name="saveObject"></param>
        public static void SaveGlobal(string file, object saveObject) => QueueSave(file, global, saveObject);


        private static readonly Queue<(string file, string slot, object saveObject)> queue = new();
        private static void Queue(string file, string slot, object saveObject) =>
            queue.Enqueue((file, slot, saveObject));
        private static void QueueSave(string file, string slot, object saveObject) =>
            Queue(file, slot, saveObject);
        private static void DoImmediateSave(string file, string slot, object saveObject)
        {
            try
            {
                var path = GetOrCreateFile(slot, file.Split("/"));

                var type = saveObject.GetType();
                var converter = ConverterRegistry.GetConverter(type);

                var data = converter != null ? converter.Serialize(saveObject) : saveObject;
                string serializedData = Convert.ToBase64String(SerializeToBase64(data));
                WriteFile(path, serializedData);
            }
            catch (Exception e)
            {
                Debug.LogError($"Save failed for file: {file}, slot: {slot}. Exception: {e}");
            }
        }
        private static byte[] SerializeToBase64(object saveObject)
        {
            using MemoryStream memoryStream = new();
            BinaryFormatter formatter = new();
            formatter.Serialize(memoryStream, saveObject);
            return memoryStream.ToArray();
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Initialize() => Run();

        private static GlobalCoroutine coroutine;
        public static bool IsRunning => coroutine?.isRunning ?? false;

        public static void Run()
        {
            if (IsRunning)
                return;

            if (coroutine != null && coroutine.isRunning)
            {
                coroutine.Stop();
            }

            coroutine = Coroutine().StartCoroutine();

            static IEnumerator Coroutine()
            {
                yield return null; // waiting a frame, just to make sure any prev coroutine is stopped.
                while (true)
                {
                    yield return null;
                    if (queue.TryDequeue(out var item))
                    {
                        DoImmediateSave(item.file, item.slot, item.saveObject);
                    }
                }
            }

        }

        public static void Stop() =>
            coroutine?.Stop();

        #endregion

        #region Load
        public static T Load<T>(string address) where T : class => DoLoad<T>(address, Slot.ToString());
        public static T LoadSlot<T>(string address, int slot) where T : class => DoLoad<T>(address, slot.ToString());
        public static T LoadGlobal<T>(string address) where T : class => DoLoad<T>(address, global);
        private static T DoLoad<T>(string address, string slot) where T : class
        {
            try
            {
                var path = GetOrCreateFile(slot, address.Split("/"));

                var converter = ConverterRegistry.GetConverter(typeof(T));

                return (T)converter.Deserialize(DeserializeFromBase64(ReadFile(path)));
                
            }
            catch (Exception e)
            {
                Debug.LogError("Load failed: " + e);
                return null;
            }
        }

        private static object DeserializeFromBase64(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);
            using MemoryStream memoryStream = new(bytes);
            BinaryFormatter formatter = new();
            return formatter.Deserialize(memoryStream);
        }

        #endregion

        #region Reset
        /// <summary>
        /// Resets/removes all saved data fom the current slot
        /// </summary>
        public static void ResetCurrent() => DoReset(GetOrCreateFolder(Slot.ToString()));

        /// <summary>
        /// Resets/removes all saved data from the Global save slot.
        /// </summary>
        public static void ResetGlobal() => DoReset(GetOrCreateFolder(global));

        /// <summary>
        /// Resets/removes all saved data from given slot.
        /// </summary>
        /// <param name="slot"></param>
        public static void ResetSlot(int slot) => DoReset(GetOrCreateFolder(slot.ToString()));

        /// <summary>
        /// Resets/removes ALL saved data.
        /// </summary>
        public static void ResetAll() => DoReset(RootPath);

        private static void DoReset(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }
        #endregion

        #region File management
        private static string ReadFile(string Path)
        {
            using var cryptic = new AesCryptoServiceProvider();
            cryptic.Key = encryption.Key;
            cryptic.IV = encryption.IV;

            using var fs = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.Read);
            if (fs.Length == 0) return string.Empty;

            using var cr = new CryptoStream(fs, cryptic.CreateDecryptor(), CryptoStreamMode.Read);
            using var sr = new StreamReader(cr, Encoding.UTF8);

            return sr.ReadToEnd();
        }

        private static void WriteFile(string Path, string data)
        {
            if (data == string.Empty) return;

            using var cryptic = new AesCryptoServiceProvider();
            cryptic.Key = encryption.Key;
            cryptic.IV = encryption.IV;

            using var fs = new FileStream(Path, FileMode.Create, FileAccess.Write);

            using var cr = new CryptoStream(fs, cryptic.CreateEncryptor(), CryptoStreamMode.Write);
            using var sr = new StreamWriter(cr, Encoding.UTF8);

            sr.WriteLine(data);
        }

        private static string GetOrCreateFile(string slot = global, string[] category = null)
        {
            string folder = GetOrCreateFolder(slot, category.Take(category.Length - 1).ToArray());

            string path = Path.Combine(folder, Path.Combine(category.Last())) + ".txt";

            return path;
        }

        private static string GetOrCreateFolder(string slot = global, string[] category = null)
        {
            string categoryPath = category != null ? Path.Combine(category) : "";
            string folder = Path.Combine(RootPath, slot, categoryPath);
            CheckOrCreateFolders(folder);
            return folder;
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
}