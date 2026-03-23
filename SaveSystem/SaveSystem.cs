using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace LazySaveSystem
{
    public static partial class SaveSystem
    {
        #region Settings
        private static readonly SaveEventArgs saveEventArgs = new();

        const string global = "Global";

        private static (byte[] Key, byte[] IV) encryption =
            (Encoding.UTF8.GetBytes("your-32-byte-long-key-for-256b--"),
             Encoding.UTF8.GetBytes("your-16-byte-IV1"));

        public static void SetEncryption(string Key, string Iv) =>
            encryption = (Encoding.UTF8.GetBytes(Key), Encoding.UTF8.GetBytes(Iv));

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        private static string RootPath => Path.Combine(Application.persistentDataPath, Application.productName, "Development");
#else
        private static string RootPath => Path.Combine(Application.persistentDataPath, Application.productName);
#endif


        public static int Slot { get; set; } = 0;

        private static bool UseEncryption =>
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            false;
#else
            true;
#endif

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
                    Save(item.Key, item.Value);   // Now truly immediate

                SaveEventArgs.Flush();
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log($"AutoSave encountered an error: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region Save (NOW IMMEDIATE - no coroutine, no queue)
        public static bool QuickSave() => AutoSave();

        public static void Save(string file, object saveObject)
            => DoSave(file, Slot.ToString(), saveObject);

        public static void SaveSlot(string file, int slot, object saveObject)
            => DoSave(file, slot.ToString(), saveObject);

        public static void SaveGlobal(string file, object saveObject)
            => DoSave(file, global, saveObject);

        private static void DoSave(string file, string slot, object saveObject)
        {
            try
            {
                var path = GetOrCreateFile(slot, file.Split("/"));

                var type = saveObject.GetType();
                var converter = ConverterRegistry.GetConverter(type);
                var data = converter.Serialize(saveObject);

                WriteFile(path, data);
            }
            catch (Exception e)
            {
                Debug.LogError($"Save failed for file: {file}, slot: {slot}. Exception: {e}");
            }
        }

        internal static byte[] SerializeToBase64(object saveObject)
        {
            using MemoryStream memoryStream = new();
            BinaryFormatter formatter = new();
            formatter.Serialize(memoryStream, saveObject);
            return memoryStream.ToArray();
        }
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

                return (T)converter.Deserialize<T>(ReadFile(path));
            }
            catch (Exception e)
            {
                Debug.LogError("Load failed: " + e);
                return null;
            }
        }

        internal static object DeserializeFromBase64(string base64String)
        {
            if (base64String.Length == 0) return null;

            byte[] bytes = Convert.FromBase64String(base64String);
            using MemoryStream memoryStream = new(bytes);
            BinaryFormatter formatter = new();
            return formatter.Deserialize(memoryStream);
        }
        #endregion

        #region Reset
        public static void ResetCurrent() => DoReset(GetOrCreateFolder(Slot.ToString()));
        public static void ResetGlobal() => DoReset(GetOrCreateFolder(global));
        public static void ResetSlot(int slot) => DoReset(GetOrCreateFolder(slot.ToString()));
        public static void ResetAll() => DoReset(RootPath);

        // NEW: Reset a specific address
        public static void Reset(string address) => DoResetFile(address, Slot.ToString());
        public static void ResetSlot(string address, int slot) => DoResetFile(address, slot.ToString());
        public static void ResetGlobal(string address) => DoResetFile(address, global);

        private static void DoReset(string path)
        {
            if (!Directory.Exists(path))
                return;

            DirectoryInfo di = new DirectoryInfo(path);

            foreach (FileInfo file in di.GetFiles())
                file.Delete();

            foreach (DirectoryInfo dir in di.GetDirectories())
                dir.Delete(true);
        }

        private static void DoResetFile(string address, string slot)
        {
            try
            {
                var path = GetOrCreateFile(slot, address.Split("/"));

                if (File.Exists(path))
                    File.Delete(path);
            }
            catch (Exception e)
            {
                Debug.LogError($"Reset failed for address: {address}, slot: {slot}. Exception: {e}");
            }
        }
        #endregion

        #region File management
        private static string ReadFile(string path)
        {
            if (!File.Exists(path))
                return string.Empty;

            using var cryptic = new AesCryptoServiceProvider
            {
                Key = encryption.Key,
                IV = encryption.IV
            };

            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            if (UseEncryption)
            {
                using var cr = new CryptoStream(fs, cryptic.CreateDecryptor(), CryptoStreamMode.Read);
                using var sr = new StreamReader(cr, Encoding.UTF8);
                return sr.ReadToEnd();
            }
            else
            {
                using var sr = new StreamReader(fs, Encoding.UTF8);
                return sr.ReadToEnd();
            }
        }

        private static void WriteFile(string path, string data)
        {
            if (string.IsNullOrEmpty(data))
                return;


            using var cryptic = new AesCryptoServiceProvider
            {
                Key = encryption.Key,
                IV = encryption.IV
            };

            using var fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            if (UseEncryption)
            {
                using var cr = new CryptoStream(fs, cryptic.CreateEncryptor(), CryptoStreamMode.Write);
                using var sw = new StreamWriter(cr, Encoding.UTF8);
                sw.Write(data);
            }
            else
            {
                using var sw = new StreamWriter(fs, Encoding.UTF8);
                sw.Write(data);
            }

        }

        private static string GetOrCreateFile(string slot, string[] category)
        {
            string folder = GetOrCreateFolder(slot, category.Take(category.Length - 1).ToArray());
            return Path.Combine(folder, category.Last()) + ".txt";
        }

        private static string GetOrCreateFolder(string slot, string[] category = null)
        {
            string categoryPath = category != null ? Path.Combine(category) : "";
            string folder = Path.Combine(RootPath, slot, categoryPath);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            return folder;
        }
        #endregion
    }

    public class SaveEventArgs : EventArgs
    {
        public static Dictionary<string, object> Data { get; private set; } = new();
        public static void Flush() => Data.Clear();
        public void Save(string file, object saveObject) => Data.Add(file, saveObject);
    }

#if UNITY_EDITOR

    public static class LazySaveSystemEditor
    {
        [MenuItem("Tools/Lazy Save System/Open Save Folder")]
        public static void OpenSaveFolder()
        {
            string RootPath = Path.Combine(Application.persistentDataPath, Application.productName, "Development");

            // Ensure the folder exists
            if (!Directory.Exists(RootPath))
            {
                Directory.CreateDirectory(RootPath);
                UnityEngine.Debug.Log($"Created save folder at {RootPath}");
            }

            System.Diagnostics.Process.Start("explorer.exe", RootPath.Replace("/", "\\"));
        }
    }
#endif
}