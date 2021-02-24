using EnumsNET;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TidalLib.Helpers;

namespace TidalLib
{
    public class TidalClient
    {
        private const string BASE_URL = "https://api.tidalhifi.com/v1/";

        private LoginKey LoginKey;

        public TidalClient()
        {
            RefreshAccessTokenFromTidalDesktop();
        }

        private async Task<(string, string)> Request(string path, Dictionary<string, string> queryParams, int retry = 3)
        {
            if (queryParams == null)
                queryParams = new Dictionary<string, string>();

            queryParams["countryCode"] = LoginKey.CountryCode;
            var url = HttpHelper.BuildQueryString(new Uri(BASE_URL + path), queryParams);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Tidal-SessionId", LoginKey.SessionID.ToString());
                if (!string.IsNullOrWhiteSpace(LoginKey.AccessToken))
                    client.DefaultRequestHeaders.Add("authorization", $"Bearer { LoginKey.AccessToken}");

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return (response.ReasonPhrase, null);
                }

                return (null, await response.Content.ReadAsStringAsync());
            }
        }

        public void RefreshAccessTokenFromTidalDesktop()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var path = appData + "\\TIDAL\\Logs\\app.log";

            ReadOnlySpan<char> content;
            try
            {
                content = Helper.ReadFile(path);
            }
            catch (Exception e)
            {
                throw new Exception($"Can't read TIDAL desktop log file. Exception: {e}");
            }

            // find json representation of key
            var sliceStart = content.LastIndexOf("[info] - Session was changed");
            if (sliceStart == -1)
                throw new Exception("Could not find: '[info] - Session was changed'");
            content = content.Slice(sliceStart);

            sliceStart = content.IndexOf('{');
            var sliceEnd = content.IndexOf("}");
            if (sliceStart == -1 || sliceEnd == -1)
                throw new Exception("Could not find opening or closing bracket of the login key's JSON representation");
            sliceEnd += 1 + content.Slice(sliceEnd + 1).IndexOf("}"); // find second closing bracket, as this is the correct one
            if (sliceEnd == -1)
                throw new Exception("Could not find closing bracket of the login key's JSON representation");
            content = content[sliceStart..(sliceEnd + 1)];


            var json = JsonDocument.Parse(content.ToString()).RootElement;
            LoginKey key = new LoginKey();

            key.AccessToken = json.GetProperty("oAuthAccessToken").GetString();
            if (string.IsNullOrWhiteSpace(key.AccessToken))
                throw new Exception("oAuthAccessToken was blank");
            key.RefreshToken = json.GetProperty("oAuthRefreshToken").GetString();
            if (string.IsNullOrWhiteSpace(key.RefreshToken))
                throw new Exception("oAuthRefreshToken was blank");
            key.UserID = json.GetProperty("userId").GetInt64();
            key.CountryCode = json.GetProperty("countryCode").GetString();
            if (string.IsNullOrWhiteSpace(key.CountryCode))
                throw new Exception("countryCode was blank");

            LoginKey = key;
        }

        public async Task<(string, SearchResult)> Search(string searchText, int limit = 10, QueryFilter eType = QueryFilter.ALL)
        {
            string types = eType.AsString(EnumFormat.Description);

            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                { "query", searchText },
                { "offset", "0" },
                { "types", types },
                { "limit", limit.ToString()},
            };
            (string msg, string res) = await Request("search", data);
            if (!string.IsNullOrWhiteSpace(msg) || string.IsNullOrWhiteSpace(res))
                return (msg, null);

            // parse result from JSON
            SearchResult result = new SearchResult();
            var serializationOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, IgnoreReadOnlyProperties = true };

            try
            {
                var json = JsonDocument.Parse(res);

                var artists = json.RootElement.GetProperty("artists").GetProperty("items").GetRawText();
                result.Artists = JsonSerializer.Deserialize<List<Artist>>(artists, serializationOptions);

                var albums = json.RootElement.GetProperty("albums").GetProperty("items").GetRawText();
                result.Albums = JsonSerializer.Deserialize<List<Album>>(albums, serializationOptions);

                var tracks = json.RootElement.GetProperty("tracks").GetProperty("items").GetRawText();
                result.Tracks = JsonSerializer.Deserialize<List<Track>>(tracks, serializationOptions);

                var videos = json.RootElement.GetProperty("videos").GetProperty("items").GetRawText();
                result.Videos = JsonSerializer.Deserialize<List<Video>>(videos, serializationOptions);

                var playlists = json.RootElement.GetProperty("playlists").GetProperty("items").GetRawText();
                result.Playlists = JsonSerializer.Deserialize<List<Playlist>>(playlists, serializationOptions);
            }
            catch (JsonException e)
            {
                return ($"Exception while parsing JSON: {e}", null);
            }

            return (null, result);
        }
    }
}

