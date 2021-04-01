// The following code has originally been published by Yaronzz under MIT License
// https://github.com/yaronzz/TidalLib
// File has been modified


using EnumsNET;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SimpleTidalApi.Helpers;
using SimpleTidalApi.Model;

namespace SimpleTidalApi
{
    public static class TidalClient
    {
        private const string BASE_URL = "https://api.tidalhifi.com/v1/";

        private static async Task<(string, string)> Request(LoginKey key, string path, Dictionary<string, string> queryParams)
        {
            if (queryParams == null)
                queryParams = new Dictionary<string, string>();

            queryParams["countryCode"] = key.CountryCode;
            var url = HttpHelper.BuildQueryString(new Uri(BASE_URL + path), queryParams);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Tidal-SessionId", key.SessionID.ToString());
                if (!string.IsNullOrWhiteSpace(key.AccessToken))
                    client.DefaultRequestHeaders.Add("authorization", $"Bearer { key.AccessToken}");

                try
                {
                    var response = await client.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        return (response.ReasonPhrase, null);
                    }

                    return (null, await response.Content.ReadAsStringAsync());
                } catch (HttpRequestException e)
                {
                    return ($"Exception while sending HTTP GET-Request: {e}", null);
                }
            }
        }

        public static (string, LoginKey) GetAccessTokenFromTidalDesktop()
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
                return ($"Can't read TIDAL desktop log file. Exception: {e}", null);
            }

            // find json representation of key
            var sliceStart = content.LastIndexOf("[info] - Session was changed");
            if (sliceStart == -1)
                return ("Could not find: '[info] - Session was changed'", null);
            content = content.Slice(sliceStart);

            sliceStart = content.IndexOf('{');
            var sliceEnd = content.IndexOf("}");
            if (sliceStart == -1 || sliceEnd == -1)
                return ("Could not find opening or closing bracket of the login key's JSON representation", null);
            sliceEnd += 1 + content.Slice(sliceEnd + 1).IndexOf("}"); // find second closing bracket, as this is the correct one
            if (sliceEnd == -1)
                return ("Could not find closing bracket of the login key's JSON representation", null);
            content = content[sliceStart..(sliceEnd + 1)];


            var json = JsonDocument.Parse(content.ToString()).RootElement;
            LoginKey key = new LoginKey();

            key.AccessToken = json.GetProperty("oAuthAccessToken").GetString();
            if (string.IsNullOrWhiteSpace(key.AccessToken))
                return ("oAuthAccessToken was blank", null);
            key.RefreshToken = json.GetProperty("oAuthRefreshToken").GetString();
            if (string.IsNullOrWhiteSpace(key.RefreshToken))
                return ("oAuthRefreshToken was blank", null);
            key.UserID = json.GetProperty("userId").GetInt64();
            key.CountryCode = json.GetProperty("countryCode").GetString();
            if (string.IsNullOrWhiteSpace(key.CountryCode))
                return ("countryCode was blank", null);

            return (null, key);
        }

        public static async Task<(string, SearchResult)> Search(LoginKey key, string searchText, int limit = 10, QueryFilter eType = QueryFilter.ALL)
        {
            string types = eType.AsString(EnumFormat.Description);

            Dictionary<string, string> queryParams = new Dictionary<string, string>()
            {
                { "query", searchText },
                { "offset", "0" },
                { "types", types },
                { "limit", limit.ToString()},
            };
            (string msg, string res) = await Request(key, "search", queryParams);
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

