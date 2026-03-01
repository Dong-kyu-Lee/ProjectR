using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StoryData
{
    public string name;
    public bool value;
}

[Serializable]
public class SingleUseStory
{
    public List<StoryData> stories = new List<StoryData>();
}
