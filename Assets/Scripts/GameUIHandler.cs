using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts
{
    /// <summary>
    /// This class handles the UI inside the game.
    /// </summary>
    public class GameUIHandler : MonoBehaviour
    {
        [SerializeField] private Button menuToggleButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button timerToggleButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button undoButton;
        [SerializeField] private List<Button> restartButtons;

        private void Start()
        {
            menuToggleButton.onClick.AddListener(MenuToggleButtonClick);
            saveButton.onClick.AddListener(SaveButtonClick);
            
            foreach (var restartButton in restartButtons)
                restartButton.onClick.AddListener(RestartButtonClick);
            
            timerToggleButton.onClick.AddListener(TimerToggleButtonClick);
            exitButton.onClick.AddListener(ExitButtonClick);
            undoButton.onClick.AddListener(UndoButtonClick);
        }

        private void MenuToggleButtonClick() => GameManager.Instance.ToggleSettings();

        private void SaveButtonClick() => GameManager.Instance.SaveGame();

        private void RestartButtonClick() => GameManager.Instance.RestartGame();

        private void ExitButtonClick() => GameManager.Instance.ReturnToMenu();
        
        private void UndoButtonClick() => GameManager.Instance.Undo();

        private void TimerToggleButtonClick()
        {
            var state = GameManager.Instance.ToggleTimerState();
            timerToggleButton.GetComponentInChildren<TMP_Text>().text = state ? "Timer: ON" : "Timer: OFF";
        }
    }
}