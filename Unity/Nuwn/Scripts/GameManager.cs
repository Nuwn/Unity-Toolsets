using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; set; }
    public bool gameIsPaused { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    [Serializable]
    public class JumpscareGroups
    {
        public int ActiveCount;
        public List<GameObject> jumpscares;
    }
    public List<JumpscareGroups> jumpscareGroups;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        
    }




    private void Start()
    {
        LoadJumpScares();
        ResumeGame();
    }

    // INVENTORY -------------------------
    public enum Pussletypes
    {
        BUNNY
    }
    public static class Pussles
    {
        public static bool Bunny = false;

        public static bool Check(Pussletypes type)
        {
            switch (type)
            {
                case Pussletypes.BUNNY:
                    return Bunny;
                default:
                    return false;
            }
                
        }
        public static void Collect(Pussletypes type)
        {
            switch (type)
            {
                case Pussletypes.BUNNY:
                    Bunny = true;
                    break;
            }
        }
    }
    // INVENTORY--------------------------

    public Behaviour[] ScriptsToDisableOnPause;
    private void DisableScripts(bool v)
    {
        foreach (var s in ScriptsToDisableOnPause)
        {
            s.enabled = !v;
        }
    }

    

    public void PauseGame()
    {
        Time.timeScale = 0;
        DisableScripts(true);
        gameIsPaused = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
        DisableScripts(false);
        gameIsPaused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    void LoadJumpScares()
    {
        foreach(var z in jumpscareGroups)
        {
            var ran = StaticGameManager.GetRandomElements<GameObject>(z.jumpscares, z.ActiveCount);
            foreach(var r in ran)
            {
                r.GetComponent<JumpscareRandomizer>().Active = true;
            }
        }
    }
    

}
public static class StaticGameManager
{
    public static List<T> GetRandomElements<T>(this IEnumerable<T> list, int elementsCount)
    {
        return list.OrderBy(arg => Guid.NewGuid()).Take(elementsCount).ToList();
    }
}