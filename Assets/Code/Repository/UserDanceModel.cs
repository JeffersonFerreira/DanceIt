using System;
using UnityEngine;

[Serializable]
public class UserDanceModel
{
    [Header("User")] public string UserName;
    public string UserDanceName;

    [Header("Song")] public string SongTitle;
    public string SongArtistName;
}