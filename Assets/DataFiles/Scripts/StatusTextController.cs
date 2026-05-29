using UnityEngine;
using TMPro;

public class StatusTextController : MonoBehaviour
{
    public static StatusTextController Instance { get; private set; }

    [Header("UI Reference")]
    [Tooltip("The TextMeshPro component used to display the status message.")]
    [SerializeField] private TextMeshProUGUI statusText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (statusText == null)
        {
            statusText = GetComponentInChildren<TextMeshProUGUI>();
        }
    }

    private void Start()
    {
        SetStatus("Scan the wall slowly");
    }

    public void SetStatus(string text)
    {
        if (statusText != null)
        {
            statusText.text = text;
            Debug.Log($"[StatusTextController] Status updated: {text}");
        }
    }
}
