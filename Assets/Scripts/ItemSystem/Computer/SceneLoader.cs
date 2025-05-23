using UnityEngine;
using UnityEngine.SceneManagement;

namespace ItemSystem.Computer
{

    public class SceneLoader : MonoBehaviour
    {
        // Загрузи сцену по её имени
        public void LoadSceneByName(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        // Загрузи сцену по индексу
        public void LoadSceneByIndex(int sceneIndex)
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }
}