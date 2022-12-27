using UnityEngine.SceneManagement;

namespace Scripts
{
    /// <summary>
    /// Handle loading scenes.
    /// </summary>
    public static class SceneLoader
    {
        public static void LoadSceneWithTargetSize(ECubeSize size)
        {
            PlayerSettings.CubeSize = (int)size;
            SceneManager.LoadScene(1);
        }
        
        public static void LoadSceneWithTargetSize(int size)
        {
            PlayerSettings.CubeSize = size;
            SceneManager.LoadScene(1);
        }
        
        public static void LoadMenuScene()
        {
          SceneManager.LoadScene(0);
        }
    }
}