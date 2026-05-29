using UnityEngine;

public class VideoSwitchButton : MonoBehaviour
{
    public void SwitchVideo()
    {
        VideoAddressablesLoader loader = FindFirstObjectByType<VideoAddressablesLoader>();

        if (loader == null)
        {
            Debug.LogWarning("No VideoAddressablesLoader found. Place the video first.");
            return;
        }

        loader.LoadNextVideo();
    }
}