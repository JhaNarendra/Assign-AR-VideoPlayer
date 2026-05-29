using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Video;

public class VideoAddressablesLoader : MonoBehaviour
{
    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Remote Addressable Video Keys")]
    [SerializeField] private List<string> videoAddresses = new List<string>
    {
        "video_01",
        "video_02"
    };

    [Header("Startup")]
    [SerializeField] private bool loadFirstVideoOnStart = true;
    [SerializeField] private int defaultVideoIndex = 0;

    private int currentVideoIndex = -1;
    private AsyncOperationHandle<VideoClip>? currentVideoHandle;
    private bool isLoading;

    private void Awake()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();
    }

    private void Start()
    {
        if (loadFirstVideoOnStart)
        {
            LoadVideo(defaultVideoIndex);
        }
    }

    public void LoadVideo(int index)
    {
        if (isLoading)
            return;

        if (videoAddresses == null || videoAddresses.Count == 0)
        {
            Debug.LogError("No Addressable video addresses assigned.");
            return;
        }

        if (index < 0 || index >= videoAddresses.Count)
        {
            Debug.LogError($"Video index {index} is out of range.");
            return;
        }

        StartCoroutine(LoadVideoRoutine(index));
    }

    public void LoadNextVideo()
    {
        if (videoAddresses == null || videoAddresses.Count == 0)
            return;

        int nextIndex = currentVideoIndex + 1;

        if (nextIndex >= videoAddresses.Count)
            nextIndex = 0;

        LoadVideo(nextIndex);
    }

    private IEnumerator LoadVideoRoutine(int index)
    {
        isLoading = true;

        string address = videoAddresses[index];

        Debug.Log($"Loading remote video: {address}");
        if (StatusTextController.Instance != null)
        {
            StatusTextController.Instance.SetStatus($"loading video_{index + 1}");
        }

        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            videoPlayer.clip = null;
        }

        ReleaseCurrentVideo();

        AsyncOperationHandle<VideoClip> loadHandle = Addressables.LoadAssetAsync<VideoClip>(address);
        yield return loadHandle;

        if (loadHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"Failed to load Addressable video: {address}");
            if (StatusTextController.Instance != null)
            {
                StatusTextController.Instance.SetStatus($"Failed to load video_{index + 1}");
            }
            isLoading = false;
            yield break;
        }

        currentVideoHandle = loadHandle;
        currentVideoIndex = index;

        VideoClip loadedClip = loadHandle.Result;

        videoPlayer.clip = loadedClip;
        videoPlayer.isLooping = true;

        videoPlayer.Prepare();

        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        videoPlayer.Play();

        Debug.Log($"Playing remote video: {address}");
        if (StatusTextController.Instance != null)
        {
            StatusTextController.Instance.SetStatus($"playing video_{index + 1}");
        }

        isLoading = false;
    }

    private void ReleaseCurrentVideo()
    {
        if (currentVideoHandle.HasValue && currentVideoHandle.Value.IsValid())
        {
            Addressables.Release(currentVideoHandle.Value);
            currentVideoHandle = null;
        }
    }

    private void OnDestroy()
    {
        ReleaseCurrentVideo();
    }
}