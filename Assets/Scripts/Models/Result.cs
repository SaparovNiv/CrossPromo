using System.Collections.Generic;
using System;

[Serializable]
public class Result
{
    public string id;
    public string video_url;
    public string click_url;
    public string tracking_url;
    public bool clicked = false;
    public bool isCached = false;
}
[Serializable]
public class Root
{
    public List<Result> results;
}