// The following code has originally been published by Yaronzz under MIT License
// https://github.com/yaronzz/TidalLib
// File has been modified



using System.Collections.Generic;

namespace SimpleTidalApi.Model
{
    internal static class Tools
    {
        public static string GetCoverUrl(string sID, string iWidth = "320", string iHeight = "320")
        {
            if (sID == null)
                return null;
            return string.Format("https://resources.tidal.com/images/{0}/{1}x{2}.jpg", sID.Replace('-', '/'), iWidth, iHeight);
        }

        public static string[] GetArtistsList(List<Artist> Artists)
        {
            if (Artists == null)
                return null;
            List<string> names = new List<string>();
            foreach (var item in Artists)
                names.Add(item.Name);
            return names.ToArray();
        }

        public static string GetArtists(List<Artist> Artists)
        {
            if (Artists == null)
                return null;
            string[] names = GetArtistsList(Artists);
            string ret = string.Join(" / ", names);
            return ret;
        }

        public static string GetFlag(object data, QueryFilter type, bool bShort = true, string separator = " / ")
        {
            bool bMaster = false;
            bool bExplicit = false;

            if (type == QueryFilter.ALBUM)
            {
                Album album = (Album)data;
                if (album.AudioQuality == "HI_RES")
                    bMaster = true;
                if (album.Explicit)
                    bExplicit = true;
            }
            else if (type == QueryFilter.TRACK)
            {
                Track track = (Track)data;
                if (track.AudioQuality == "HI_RES")
                    bMaster = true;
                if (track.Explicit)
                    bExplicit = true;
            }
            else if (type == QueryFilter.VIDEO)
            {
                Video video = (Video)data;
                if (video.Explicit)
                    bExplicit = true;
            }

            if (bMaster == false && bExplicit == false)
                return "";

            List<string> flags = new List<string>();
            if (bMaster)
                flags.Add(bShort ? "M" : "Master");
            if (bExplicit)
                flags.Add(bShort ? "E" : "Explicit");
            return string.Join(separator, flags.ToArray());
        }

        public static string GetDisplayTitle(Track track)
        {
            if (track.Version != null && !string.IsNullOrWhiteSpace(track.Version))
                return $"{track.Title} ({track.Version})";
            return track.Title;
        }
    }

    public class LoginKey
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public long UserID { get; set; }
        public string CountryCode { get; set; }
        public long SessionID { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string ExpirationDate { get; set; }
    }

    public class TidalDeviceCode
    {
        public string DeviceCode { get; set; }
        public string UserCode { get; set; }
        public string VerificationUri { get; set; }
        public int ExpiresIn { get; set; }
        public int Interval { get; set; }
    }

    public class Album
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public int Duration { get; set; }
        public bool StreamReady { get; set; }
        public string StreamStartDate { get; set; }
        public bool AllowStreaming { get; set; }
        public bool PremiumStreamingOnly { get; set; }
        public int NumberOfTracks { get; set; }
        public int NumberOfVideos { get; set; }
        public int NumberOfVolumes { get; set; }
        public string ReleaseDate { get; set; }
        public string Copyright { get; set; }
        public string Type { get; set; }
        public string Version { get; set; }
        public string Url { get; set; }
        public string Cover { get; set; }
        public string VideoCover { get; set; }
        public bool Explicit { get; set; }
        public string Upc { get; set; }
        public int Popularity { get; set; }
        public string AudioQuality { get; set; }
        public Artist Artist { get; set; }
        public string[] AudioModes { get; set; }

        public string CoverUrl { get { return Tools.GetCoverUrl(Cover); } }
        public string CoverHighUrl { get { return Tools.GetCoverUrl(Cover, "1280", "1280"); } }
        public string ArtistsName { get { return Tools.GetArtists(Artists); } }
        public string Flag { get { return Tools.GetFlag(this, QueryFilter.ALBUM, false); } }
        public string FlagShort { get { return Tools.GetFlag(this, QueryFilter.ALBUM, true); } }

        public List<Artist> Artists { get; set; }
        public List<Track> Tracks { get; set; }
        public List<Video> Videos { get; set; }
    }

    public class Artist
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string Picture { get; set; }
        public int Popularity { get; set; }
        public string[] ArtistTypes { get; set; }

        public string CoverUrl { get { return Tools.GetCoverUrl(Picture); } }
        public string CoverHighUrl { get { return Tools.GetCoverUrl(Picture, "750", "750"); } }

        public List<Album> Albums { get; set; }
    }

    public class Track
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public int Duration { get; set; }
        public double ReplayGain { get; set; }
        public double Peak { get; set; }
        public bool AllowStreaming { get; set; }
        public bool StreamReady { get; set; }
        public string StreamStartDate { get; set; }
        public bool PremiumStreamingOnly { get; set; }
        public int TrackNumber { get; set; }
        public int VolumeNumber { get; set; }
        public string Version { get; set; }
        public int Popularity { get; set; }
        public string Copyright { get; set; }
        public string Url { get; set; }
        public string Isrc { get; set; }
        public bool Editable { get; set; }
        public bool Explicit { get; set; }
        public string AudioQuality { get; set; }
        public string[] AudioModes { get; set; }

        public string DisplayTitle { get { return Tools.GetDisplayTitle(this); } }
        public string ArtistsName { get { return Tools.GetArtists(Artists); } }
        public string Flag { get { return Tools.GetFlag(this, QueryFilter.TRACK, false); } }
        public string FlagShort { get { return Tools.GetFlag(this, QueryFilter.TRACK, true); } }

        public Album Album { get; set; }
        public List<Artist> Artists { get; set; }
    }

    public class StreamUrl
    {
        public string TrackID { get; set; }
        public string Url { get; set; }
        public string Codec { get; set; }
        public string EncryptionKey { get; set; }
        public int PlayTimeLeftInMinutes { get; set; }
        public string SoundQuality { get; set; }
    }

    public class VideoStreamUrl
    {
        public string Codec { get; set; }
        public string Resolution { get; set; }
        public string[] ResolutionArray { get; set; }
        public string M3u8Url { get; set; }
    }

    public class Video
    {
        public long ID { get; set; }
        public string Title { get; set; }
        public int Duration { get; set; }
        public string ImageID { get; set; }
        public int TrackNumber { get; set; }
        public string ReleaseDate { get; set; }
        public string Version { get; set; }
        public string Copyright { get; set; }
        public string Quality { get; set; }
        public bool Explicit { get; set; }
        public Artist Artist { get; set; }
        public Album Album { get; set; }

        public string CoverUrl { get { return Tools.GetCoverUrl(ImageID); } }
        public string CoverHighUrl { get { return Tools.GetCoverUrl(ImageID, "1280", "1280"); } }
        public string ArtistsName { get { return Tools.GetArtists(Artists); } }
        public string Flag { get { return Tools.GetFlag(this, QueryFilter.VIDEO, false); } }
        public string FlagShort { get { return Tools.GetFlag(this, QueryFilter.VIDEO, true); } }

        public List<Artist> Artists { get; set; }
    }

    public class Playlist
    {
        public string UUID { get; set; }
        public string Title { get; set; }
        public int NumberOfTracks { get; set; }
        public int NumberOfVideos { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public string LastUpdated { get; set; }
        public string Created { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public string Image { get; set; }
        public string SquareImage { get; set; }
        public bool PublicPlaylist { get; set; }
        public int Popularity { get; set; }

        public string CoverUrl { get { return Tools.GetCoverUrl(SquareImage); } }
        public string CoverHighUrl { get { return Tools.GetCoverUrl(SquareImage, "1080", "1080"); } }

        public List<Track> Tracks { get; set; }
        public List<Video> Videos { get; set; }
    }

    public class SearchResult
    {
        public List<Artist> Artists { get; set; }
        public List<Album> Albums { get; set; }
        public List<Track> Tracks { get; set; }
        public List<Video> Videos { get; set; }
        public List<Playlist> Playlists { get; set; }
    }
}
