using System;
using UnityEngine;

[Serializable]
public class LevelEntry
{
    public LevelData data;
    public LevelPage page;

    [HideInInspector]
    public bool unlocked = true;

    [HideInInspector]
    public int stars;

    [HideInInspector]
    public bool completed;
}