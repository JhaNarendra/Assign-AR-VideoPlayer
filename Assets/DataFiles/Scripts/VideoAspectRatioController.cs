using UnityEngine;
using UnityEngine.Video;

public class VideoAspectRatioController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Size")]
    [SerializeField] private float targetHeight = 0.9f;

    [Header("Fallback Aspect Ratio")]
    [SerializeField] private float fallbackAspectRatio = 16f / 9f;

    private void Awake()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();
    }

    private void OnEnable()
    {
        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted += OnVideoPrepared;
        }
    }

    private void OnDisable()
    {
        if (videoPlayer != null)
        {
            videoPlayer.prepareCompleted -= OnVideoPrepared;
        }
    }

    private void Start()
    {
        ApplyAspectRatio(fallbackAspectRatio);
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        float width = source.width;
        float height = source.height;

        if (width <= 0 || height <= 0)
        {
            ApplyAspectRatio(fallbackAspectRatio);
            return;
        }

        float aspectRatio = width / height;
        ApplyAspectRatio(aspectRatio);
    }

    private void ApplyAspectRatio(float aspectRatio)
    {
        float targetWidth = targetHeight * aspectRatio;

        transform.localScale = new Vector3(
            targetWidth,
            targetHeight,
            1f
        );
    }
}