using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Nuwn
{
    namespace Extras
    {
        public abstract class GameManager : MonoBehaviour
        {
            private void OnEnable()
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
            }

            protected virtual void OnSceneLoaded(Scene arg0, LoadSceneMode arg1) { }

            public bool gameIsPaused { get; set; }
            public void PauseGame()
            {
                Time.timeScale = 0;
                gameIsPaused = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            public void ResumeGame()
            {
                Time.timeScale = 1;
                gameIsPaused = false;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            public void RestartGame()
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
