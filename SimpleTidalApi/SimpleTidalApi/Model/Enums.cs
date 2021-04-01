// The following code has originally been published by Yaronzz under MIT License
// https://github.com/yaronzz/TidalLib
// File has been modified

using System.ComponentModel;

namespace SimpleTidalApi.Model
{
    public enum AudioQuality
    {
        Normal,
        High,
        HiFi,
        Master
    }

    public enum VideoQuality
    {
        P240 = 240,
        P360 = 360,
        P480 = 480,
        P720 = 720,
        P1080 = 1080
    }

    public enum QueryFilter
    {
        [Description("ALBUMS")]
        ALBUM,
        [Description("ARTISTS")]
        ARTIST,
        [Description("PLAYLISTS")]
        PLAYLIST,
        [Description("TRACKS")]
        TRACK,
        [Description("VIDEOS")]
        VIDEO,
        SEARCH,
        [Description("ARTISTS,ALBUMS,TRACKS,VIDEOS,PLAYLISTS")]
        ALL
    }

    public enum ContributorRole
    {
        PRODUCER,
        COMPOSER,
        LYRICIST,
        ASSOCIATED_PERFORMER,
        BACKGROUND_VOCAL,
        BASS,
        DRUMS,
        GUITAR,
        MASTERING_ENGINEER,
        MIX_ENGINEER,
        PERCUSSION,
        SYNTHESIZER,
        VOCAL,
        PERFORMER,
        REMIXER,
        ENSEMBLE_ORCHESTRA,
        CHOIR,
        CONDUCTOR,
        ELSE,
    }
}
