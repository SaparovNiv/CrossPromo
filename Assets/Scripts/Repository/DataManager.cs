using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class DataManager
{
    private readonly NetworkImpl Network;
    private List<Result> VideoClipsDataset;
    private const string VIDEO_CLIPS_URL = "https://run.mocky.io/v3/81fab340-9550-4ab4-8859-836b01ee48ff";
    private Action StartPlayingCallback;
    private static readonly string VideoDataFilename = "dataObject.fun";
    public DataManager(NetworkImpl network, Action startPlayingCallback)
    {
        Network = network;
        StartPlayingCallback = startPlayingCallback;
    }

    public IEnumerator FetchVideos() => Network.Get<Root>(VIDEO_CLIPS_URL, OnDataReceived);

    public Result GetCurrent(int index) => VideoClipsDataset[index];

    public int DatasetCount() => VideoClipsDataset.Count;
    private void OnDataReceived(Root videos)
    {
        /**
         * Once we receive data, we get the local data in order to compare and start downloading missing videos.
         * We download while streaming for a better user experience.       
         */
        var localDataVideos = LocalDataImpl.GetList<Result>(VideoDataFilename);
        var diffDataVideos = DiffVidedoData(localDataVideos,videos.results);
        this.VideoClipsDataset = MergeVideoList(localDataVideos, diffDataVideos);
        StartPlayingCallback();
        DownloadNewFiles();
    }

    public IEnumerator DownloadNewFiles()
    {
        foreach(var video in VideoClipsDataset)
        {
            if (!video.isCached)
            {
                var filename = video.id + Path.GetExtension(video.video_url);
                yield return Network.Download(video.video_url, filename, SaveFile);
            }
        }
        yield return null;
    }

    private void SaveFile(byte[] data, string filename)
    {
        Debug.Log("Save file" + filename);
        // Save file to local 
        string newPath = LocalDataImpl.SaveFile(data, filename);
        string fileId = Path.GetFileNameWithoutExtension(filename);
        // Update dataset
        Result item = VideoClipsDataset.Find(item => item.id == fileId);
        if(item != null)
        {
            item.video_url = newPath;
            item.isCached = true;
        }

        // Update localdb
        LocalDataImpl.SaveList(VideoClipsDataset, VideoDataFilename);


    }
    private List<Result> MergeVideoList(List<Result> local, List<Result> diff)
    {
        var list = new List<Result>();
        if(local != null)
        {
            list.AddRange(local);
        }
        if (diff != null)
        {
            list.AddRange(diff);
        }

        return list;
    }
    private List<Result> DiffVidedoData(List<Result> localData, List<Result> remoteData)
    {
        if (remoteData == null) return default;
        if (localData == null || localData.Count == remoteData.Count)
        {
            return default;
        }
        var diffData = new List<Result>();
        remoteData.ForEach(remoteItem =>
        {
            var isExist = false;
            localData.ForEach(localItem =>
            {
                if (remoteItem.id == localItem.id)
                {
                    isExist = true;
                    return;
                }
            });
            if (!isExist)
            {
                diffData.Add(remoteItem);
            }
        });
        return diffData;
    }

    public IEnumerator SendTracking(string trackingURL, string deafultPlayerID, string playerID) => Network.Get(TrackingURLWithPlayerID(trackingURL, deafultPlayerID, playerID));
    private string TrackingURLWithPlayerID(string trackingURL, string deafultPlayerID, string playerID) => trackingURL.Replace(deafultPlayerID, playerID);


}
