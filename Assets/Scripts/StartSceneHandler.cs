using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    /// <summary>
    /// Handle start scene UI.
    /// </summary>
    public class StartSceneHandler : MonoBehaviour
    {
        [SerializeField] private RectTransform introScreenRect;
        [SerializeField] private RectTransform selectionScreenRect;
        [SerializeField] private Button startButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button quitButton;

        private void Start()
        {
            SwitchScreen(true);
            startButton.onClick.AddListener(StartButtonOmClick);
            loadButton.onClick.AddListener(LoadButtonClick);
            quitButton.onClick.AddListener(QuitButtonOmClick);
        }

        private void SwitchScreen(bool isIntroSelected)
        {
            introScreenRect.gameObject.SetActive(isIntroSelected);
            selectionScreenRect.gameObject.SetActive(!isIntroSelected);
        }

        private void StartButtonOmClick() => SwitchScreen(false);

        private void LoadButtonClick()
        {
            var data = SaveStateHandler.LoadData();
            PlayerSettings.Data = data;
            PlayerSettings.CubeSize = data.cubeSize;
            PlayerSettings.IsLoaded = true;
            SceneLoader.LoadSceneWithTargetSize(PlayerSettings.CubeSize);
        }

        private void QuitButtonOmClick() => Application.Quit();
    }
}