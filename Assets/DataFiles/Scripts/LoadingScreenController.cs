using System.Collections;
using UnityEngine;

public class LoadingScreenController : MonoBehaviour
{
    [Header("UI Reference")]
    [Tooltip("The parent GameObject of the loading panel to disable.")]
    [SerializeField] private GameObject loadingPanel;

    [Header("Loading Settings")]
    [Tooltip("How long (in seconds) the loading screen should stay active.")]
    [SerializeField] private float loadingDuration = 2.5f;

    private void Start()
    {
        // If not assigned, assume this script is attached directly to the LoadingPanel
        if (loadingPanel == null)
        {
            loadingPanel = gameObject;
        }

        StartCoroutine(DeactivateLoadingScreenRoutine());
    }

    private IEnumerator DeactivateLoadingScreenRoutine()
    {
        Debug.Log($"[LoadingScreenController] Starting loading screen. Waiting for {loadingDuration} seconds.");

        // Wait for the specified duration (giving time for other scripts to initialize)
        yield return new WaitForSeconds(loadingDuration);

        if (loadingPanel != null)
        {
            loadingPanel.SetActive(false);
            Debug.Log("[LoadingScreenController] Loading screen deactivated successfully.");
        }
        else
        {
            Debug.LogWarning("[LoadingScreenController] Failed to deactivate loading screen: loadingPanel reference is null.");
        }
    }
}
