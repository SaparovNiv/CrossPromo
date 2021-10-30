using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CrossPromo : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private DataManager dataManager;
    private int videoClipCurrentIndex;
    public string PlayerID;
    private
    const string DefaultPlayerID = "[PLAYER_ID]";

    public void Next()
    {
        if (!videoPlayer.isPrepared) return;

        videoClipCurrentIndex = videoClipCurrentIndex + 1 >= dataManager.DatasetCount() ? 0 : videoClipCurrentIndex + 1;
        StartPlay();
    }

    public void Previous()
    {
        if (!videoPlayer.isPrepared) return;

        videoClipCurrentIndex = videoClipCurrentIndex - 1 < 0 ? dataManager.DatasetCount() - 1 : videoClipCurrentIndex - 1;
        StartPlay();
    }

    public void Pause()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();
        }
    }

    public void Resume()
    {
        if (videoPlayer.isPaused)
        {
            videoPlayer.Play();
        }
    }

    private void StartPlay()
    {
        Debug.Log($"Playing video {CurrentItem.id}, from cached? {CurrentItem.isCached}");
        videoPlayer.url = CurrentItem.video_url;
        videoPlayer.Play();
        StartCoroutine(dataManager.DownloadNewFiles());
    }
    private void VideoFinishEvent(VideoPlayer vp)
    {
        Next();
    }

    void Start()
    {
        Debug.Log("CrossPromo Starting");

        // Init variables
        videoPlayer = GetComponent<VideoPlayer>();
        dataManager = new DataManager(new NetworkImpl(new JsonSerializationOption()), StartPlay);

        // Register to finish event 
        videoPlayer.loopPointReached += VideoFinishEvent;
        StartCoroutine(dataManager.FetchVideos());
    }

    public void OnVideoClicked()
    {
        if (!CurrentItem.clicked)
        {
            Debug.Log("Send request of tracking url");
            CurrentItem.clicked = true;
            StartCoroutine(dataManager.SendTracking(CurrentItem.tracking_url, DefaultPlayerID, PlayerID));
        }
        Application.OpenURL(CurrentItem.click_url);
    }
    private Result CurrentItem => dataManager.GetCurrent(videoClipCurrentIndex);

}