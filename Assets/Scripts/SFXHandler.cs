using UnityEngine;
using UnityEngine.UI;

public class SFXHandler : MonoBehaviour
{
    [SerializeField] private AudioSource buttonClick;

    private Button button;

    private void Awake() => button = GetComponent<Button>();

    private void Start() => button.onClick.AddListener(delegate { buttonClick.Play(); });
}