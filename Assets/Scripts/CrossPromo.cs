using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CrossPromo : MonoBehaviour
{
    private VideoPlayer mVideoPlayer;
    private DataManager mDataManager;
    private int ClipCurrentIndex;
    public string PlayerID;
    const string DefaultPlayerID = "[PLAYER_ID]";

    public void Next()
    {
        if (!mVideoPlayer.isPrepared) return;

        ClipCurrentIndex = ClipCurrentIndex + 1 >= mDataManager.DatasetCount() ? 0 : ClipCurrentIndex + 1;
        PlayVideo();
    }

    public void Previous()
    {
        if (!mVideoPlayer.isPrepared) return;

        ClipCurrentIndex = ClipCurrentIndex - 1 < 0 ? mDataManager.DatasetCount() - 1 : ClipCurrentIndex - 1;
        PlayVideo();
    }

    public void Pause()
    {
        if (mVideoPlayer.isPlaying)
        {
            mVideoPlayer.Pause();
        }
    }

    public void Resume()
    {
        if (mVideoPlayer.isPaused)
        {
            mVideoPlayer.Play();
        }
    }

    private void PlayVideo()
    {
        Debug.Log($"Playing video {CurrentItem.id}, from cached? {CurrentItem.isCached}");
        mVideoPlayer.url = CurrentItem.video_url;
        mVideoPlayer.Play();

    }
    private void VideoFinishEvent(VideoPlayer vp)
    {
        Next();
    }

    void Start()
    {
        Debug.Log("CrossPromo Starting");

        // Init variables
        mVideoPlayer = GetComponent<VideoPlayer>();
        mDataManager = new DataManager(new NetworkImpl(new JsonSerializationOption()), PlayVideoAndStartDownload);

        // Register to finish event 
        mVideoPlayer.loopPointReached += VideoFinishEvent;
        StartCoroutine(mDataManager.FetchVideos());
    }

    public void PlayVideoAndStartDownload()
    {
        PlayVideo();
        StartCoroutine(mDataManager.DownloadNewFiles());
    }

    public void OnVideoClicked()
    {
        if (!CurrentItem.clicked)
        {
            Debug.Log("Send request of tracking url");
            CurrentItem.clicked = true;
            StartCoroutine(mDataManager.SendTracking(CurrentItem.tracking_url, DefaultPlayerID, PlayerID));
        }
        Application.OpenURL(CurrentItem.click_url);
    }
    private Result CurrentItem => mDataManager.GetCurrent(ClipCurrentIndex);

}