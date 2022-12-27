using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    /// <summary>
    /// Handle starting the game based on the selected magic cube size.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class LevelSelector: MonoBehaviour
    {
        [SerializeField] private ECubeSize cubeSize;
        
        private Button actionBtn;
        
        private void Awake() =>  actionBtn = GetComponent<Button>();
        
        private void Start() =>  actionBtn.onClick.AddListener(OnLevelSelect);
        
        private void OnLevelSelect() => SceneLoader.LoadSceneWithTargetSize(cubeSize);
        
    }
}