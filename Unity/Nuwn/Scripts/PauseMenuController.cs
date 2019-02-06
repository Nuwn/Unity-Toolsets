using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private GameObject PauseMenu = default;
    void Start()
    {
        PauseMenu.SetActive(false);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!GameManager.Instance.gameIsPaused)
            {
                Pause();
            }
            else
            {
                Resume();
            }
        }
    }
    public void Pause()
    {
        GameManager.Instance.PauseGame();
        PauseMenu.SetActive(true);
    }
    public void Resume()
    {
        GameManager.Instance.ResumeGame();
        PauseMenu.SetActive(false);
    }
}
