using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
   public class GameManager : MonoBehaviour
   {
      [SerializeField] private InputHandler inputHandler;
      [SerializeField] private int shuffleTimes;
      [SerializeField] private float shuffleTimer;
      [SerializeField] private MagicCube magicCubePrefab;
      [SerializeField] private GameObject winMessage;
      [SerializeField] private GameObject settings;
      [SerializeField] private TMP_Text timer;
      [SerializeField] private Animator settingsPanelAnimator;
      [SerializeField] private AudioSource shuffleAudioSource;

      private MagicCube magicCubeInstance;
      private bool lockCam;
      private float timerValue;
      private int seconds;
      private int minutes;
      private string TimerTextStr;

      public static GameManager Instance { get; set; }

      // Use this for initialization
      private void Awake ()
      {
         Instance ??= this;
         PlayerSettings.TimerOn = true;
         lockCam = false;
         StartGame();
      }

      // Rest game configuration.
      private void StartGame()
      {
         PlayerSettings.SettingsOn = false;
         PlayerSettings.IsGameEnd = false;
         PlayerSettings.FaceRotation = false;
         PlayerSettings.CubeRotation = false;
         PlayerSettings.IsShuffling = false;
         magicCubeInstance = Instantiate(magicCubePrefab);
         inputHandler.SetMagicCube(magicCubeInstance);
         magicCubeInstance.transform.position = transform.position;
         magicCubeInstance.GenerateCube();
         if (!PlayerSettings.IsLoaded)
            Invoke(nameof(ShuffleCube), 0.5f);
         else
            magicCubeInstance.SetMagicCubeInfo(PlayerSettings.Data);
      }

      // Check timer.
      private void Update()
      {
         if (!PlayerSettings.SettingsOn && !PlayerSettings.IsGameEnd && !PlayerSettings.IsShuffling)
            timerValue += Time.deltaTime;

         minutes = Mathf.FloorToInt(timerValue / 60F);
         seconds = Mathf.FloorToInt(timerValue - minutes * 60);
         TimerTextStr = $"Time: {minutes:0}:{seconds:00}";
         timer.text = TimerTextStr;
      }

      // Save data to Json.
      public void SaveGame()
      {
         var info = magicCubeInstance.GetMagicCubeInfo();
         info.cubeSize = PlayerSettings.CubeSize;
         SaveStateHandler.SaveData(info);
      }

      // Handle showing and hiding timer in UI.
      public bool ToggleTimerState()
      {
         timer.gameObject.SetActive(!timer.gameObject.activeSelf);
         return timer.gameObject.activeSelf;
      }
      
      public void GameEnd() {
         winMessage.gameObject.SetActive(true);
         PlayerSettings.IsGameEnd = true;
      }

      public void ToggleSettings()
      {
         if (PlayerSettings.IsGameEnd || PlayerSettings.IsShuffling) return;
         PlayerSettings.SettingsOn = !PlayerSettings.SettingsOn;
         lockCam = PlayerSettings.CameraDisable;
         PlayerSettings.CameraDisable = PlayerSettings.SettingsOn;
         settings.SetActive(PlayerSettings.SettingsOn);
         settingsPanelAnimator.SetBool("IsOpen",PlayerSettings.SettingsOn);
         if (PlayerSettings.SettingsOn) return;
         PlayerSettings.FaceRotation = false;
         PlayerSettings.CameraDisable = lockCam;
      }

      public void ShuffleCube()
      {
         if (PlayerSettings.SettingsOn)
            ToggleSettings();

         StartCoroutine(magicCubeInstance.ShuffleCube(shuffleTimes, shuffleTimer));
         timerValue = 0.0f;
      }

      public void RestartGame() 
      {
         StopAllCoroutines();
         winMessage.gameObject.SetActive(false);
         settings.SetActive(false);
         Destroy(magicCubeInstance.gameObject);
         SceneLoader.LoadSceneWithTargetSize(PlayerSettings.CubeSize);
      }

      public void ReturnToMenu()
      {
         if (PlayerSettings.SettingsOn)
            ToggleSettings();

         PlayerSettings.IsGameEnd = false;
         StopAllCoroutines();
         Destroy(magicCubeInstance.gameObject);
         SceneLoader.LoadMenuScene();
      }

      public void Undo() => inputHandler.Undo();

      public void PlayShuffleAudio() => shuffleAudioSource.Play();
   }
}
