using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; set; }
    public bool GameIsLoaded { get; set; }

    void Awake()
    {
        //if we don't have an [_instance] set yet
        if (!Instance)
            Instance = this;
        //otherwise, if we do, kill this thing
        else
            Destroy(gameObject);


        DontDestroyOnLoad(gameObject);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
        
    }
    
    public void ResumeGame()
    {
    }

    void PauseGame()
    {
        
    }

}
