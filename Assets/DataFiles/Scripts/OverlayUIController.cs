using UnityEngine;

public class OverlayUIController : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private RectTransform topPanel;
    [SerializeField] private RectTransform bottomPanel;

    [Header("Animation Settings")]
    [SerializeField] private float slideDuration = 0.3f;

    private Vector2 topPanelShownPos;
    private Vector2 topPanelHiddenPos;
    private Vector2 bottomPanelShownPos;
    private Vector2 bottomPanelHiddenPos;

    private Coroutine topPanelCoroutine;
    private Coroutine bottomPanelCoroutine;
    private bool isTopPanelOpen = false;
    private bool isInitialized = false;

    private WallOverlayTransformController CurrentOverlay
    {
        get
        {
            return FindFirstObjectByType<WallOverlayTransformController>();
        }
    }

    private void Start()
    {
        Debug.Log("[OverlayUIController] Start: Scheduling UI initialization.");
        StartCoroutine(InitializeUIRoutine());
    }

    private System.Collections.IEnumerator InitializeUIRoutine()
    {
        // Wait 1 frame to allow Unity's UI Canvas layout and anchors to resolve their sizes
        yield return null;

        Debug.Log("[OverlayUIController] InitializeUIRoutine: Resolving panel positions.");

        // Cache positions based on their design positions in the inspector
        if (topPanel != null)
        {
            // Ensure panel is active so we can read rect values correctly
            topPanel.gameObject.SetActive(true);
            
            topPanelShownPos = topPanel.anchoredPosition;
            
            float height = topPanel.rect.height;
            if (height <= 0f)
            {
                height = Mathf.Abs(topPanel.sizeDelta.y);
                if (height <= 0f) height = 300f; // Safe fallback
            }

            // Slide up by its height to hide it
            topPanelHiddenPos = topPanelShownPos + new Vector2(0f, height);
            topPanel.anchoredPosition = topPanelHiddenPos; // Start hidden
            topPanel.gameObject.SetActive(false); // Start deactivated (hidden)
            
            Debug.Log($"[OverlayUIController] Top Panel Initialized - Shown: {topPanelShownPos}, Hidden: {topPanelHiddenPos}, Resolved Height: {height}");
        }
        else
        {
            Debug.LogWarning("[OverlayUIController] Top Panel reference is missing in the Inspector!");
        }

        if (bottomPanel != null)
        {
            // Ensure panel is active so we can read rect values correctly
            bottomPanel.gameObject.SetActive(true);

            bottomPanelShownPos = bottomPanel.anchoredPosition;

            float height = bottomPanel.rect.height;
            if (height <= 0f)
            {
                height = Mathf.Abs(bottomPanel.sizeDelta.y);
                if (height <= 0f) height = 300f; // Safe fallback
            }

            // Slide down by its height to hide it
            bottomPanelHiddenPos = bottomPanelShownPos - new Vector2(0f, height);
            bottomPanel.anchoredPosition = bottomPanelHiddenPos; // Start hidden
            bottomPanel.gameObject.SetActive(false); // Start deactivated (hidden)
            
            Debug.Log($"[OverlayUIController] Bottom Panel Initialized - Shown: {bottomPanelShownPos}, Hidden: {bottomPanelHiddenPos}, Resolved Height: {height}");
        }
        else
        {
            Debug.LogWarning("[OverlayUIController] Bottom Panel reference is missing in the Inspector!");
        }

        isInitialized = true;
        Debug.Log("[OverlayUIController] UI Initialization complete.");
    }

    // --- UI Flow Actions ---

    public void OnMenuButtonClicked()
    {
        if (!isInitialized)
        {
            Debug.LogWarning("[OverlayUIController] OnMenuButtonClicked ignored: UI not fully initialized.");
            return;
        }

        Debug.Log($"[OverlayUIController] OnMenuButtonClicked: isTopPanelOpen = {isTopPanelOpen}");
        if (isTopPanelOpen)
        {
            HideTopPanel();
        }
        else
        {
            ShowTopPanel();
        }
    }

    public void OnNextVideoButtonClicked()
    {
        if (!isInitialized) return;

        Debug.Log("[OverlayUIController] OnNextVideoButtonClicked.");
        HideTopPanel();

        // Automatically switch to the next video
        VideoSwitchButton switchButton = FindFirstObjectByType<VideoSwitchButton>();
        if (switchButton != null)
        {
            Debug.Log("[OverlayUIController] Found VideoSwitchButton, switching video.");
            switchButton.SwitchVideo();
        }
        else
        {
            Debug.LogWarning("[OverlayUIController] No VideoSwitchButton found in scene.");
        }
    }

    public void OnScreenControlsButtonClicked()
    {
        if (!isInitialized) return;

        Debug.Log("[OverlayUIController] OnScreenControlsButtonClicked.");
        HideTopPanel();
        ShowBottomPanel();
    }

    public void OnCloseScreenControlsButtonClicked()
    {
        if (!isInitialized) return;

        Debug.Log("[OverlayUIController] OnCloseScreenControlsButtonClicked.");
        HideBottomPanel();
    }

    // --- Panel Slide Animations ---

    public void ShowTopPanel()
    {
        Debug.Log("[OverlayUIController] ShowTopPanel called.");
        isTopPanelOpen = true;
        if (topPanel != null)
        {
            topPanel.gameObject.SetActive(true); // Activate panel before starting transition
        }
        SlidePanel(ref topPanelCoroutine, topPanel, topPanelShownPos);
    }

    public void HideTopPanel()
    {
        Debug.Log("[OverlayUIController] HideTopPanel called.");
        isTopPanelOpen = false;
        SlidePanel(ref topPanelCoroutine, topPanel, topPanelHiddenPos);
    }

    public void ShowBottomPanel()
    {
        Debug.Log("[OverlayUIController] ShowBottomPanel called.");
        if (bottomPanel != null)
        {
            bottomPanel.gameObject.SetActive(true); // Activate panel before starting transition
        }
        SlidePanel(ref bottomPanelCoroutine, bottomPanel, bottomPanelShownPos);
    }

    public void HideBottomPanel()
    {
        Debug.Log("[OverlayUIController] HideBottomPanel called.");
        SlidePanel(ref bottomPanelCoroutine, bottomPanel, bottomPanelHiddenPos);
    }

    private void SlidePanel(ref Coroutine activeCoroutine, RectTransform panel, Vector2 targetPosition)
    {
        if (panel == null)
        {
            Debug.LogError("[OverlayUIController] SlidePanel failed: panel RectTransform is null.");
            return;
        }

        Debug.Log($"[OverlayUIController] Sliding {panel.name} to {targetPosition}");

        if (activeCoroutine != null)
        {
            Debug.Log($"[OverlayUIController] Stopping active coroutine for {panel.name}");
            StopCoroutine(activeCoroutine);
        }

        activeCoroutine = StartCoroutine(SlideRoutine(panel, targetPosition));
    }

    private System.Collections.IEnumerator SlideRoutine(RectTransform panel, Vector2 targetPosition)
    {
        Vector2 startPosition = panel.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / slideDuration;
            // Smooth step interpolation for a premium/smooth feel
            t = Mathf.SmoothStep(0f, 1f, t);
            panel.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        panel.anchoredPosition = targetPosition;
        Debug.Log($"[OverlayUIController] Slide completed for {panel.name} at {targetPosition}");

        // Disable panel game object once it completes hiding
        if (panel == topPanel && targetPosition == topPanelHiddenPos)
        {
            panel.gameObject.SetActive(false);
            Debug.Log("[OverlayUIController] Top Panel deactivated off-screen.");
        }
        else if (panel == bottomPanel && targetPosition == bottomPanelHiddenPos)
        {
            panel.gameObject.SetActive(false);
            Debug.Log("[OverlayUIController] Bottom Panel deactivated off-screen.");
        }
    }

    // --- Overlay Transform Controls ---

    public void ScaleUp()
    {
        Debug.Log("[OverlayUIController] ScaleUp clicked.");
        if (CurrentOverlay == null)
        {
            Debug.LogWarning("[OverlayUIController] No placed overlay found. Place the video first.");
            return;
        }

        CurrentOverlay.ScaleUp();
    }

    public void ScaleDown()
    {
        Debug.Log("[OverlayUIController] ScaleDown clicked.");
        if (CurrentOverlay == null)
        {
            Debug.LogWarning("[OverlayUIController] No placed overlay found. Place the video first.");
            return;
        }

        CurrentOverlay.ScaleDown();
    }

    public void RotateLeft()
    {
        Debug.Log("[OverlayUIController] RotateLeft clicked.");
        if (CurrentOverlay == null)
        {
            Debug.LogWarning("[OverlayUIController] No placed overlay found. Place the video first.");
            return;
        }

        CurrentOverlay.RotateLeft();
    }

    public void RotateRight()
    {
        Debug.Log("[OverlayUIController] RotateRight clicked.");
        if (CurrentOverlay == null)
        {
            Debug.LogWarning("[OverlayUIController] No placed overlay found. Place the video first.");
            return;
        }

        CurrentOverlay.RotateRight();
    }
}