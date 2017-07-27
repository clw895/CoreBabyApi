using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;

public class DropboxHelper
{
    private string AccessToken { get; } = "UPDATE_YOUR_ACCESS_TOKEN_HERE";
    protected static HttpClient HttpClient { get; set; }
    public DropboxHelper()
	{
        HttpClient = new HttpClient()
        {
            BaseAddress = new Uri("https://api.dropboxapi.com/2/files/")
        };
        HttpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {AccessToken}");
        HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<List<string>> GetFilePaths()
    {
        const string urlPath = "list_folder";

        var postData = new
        {
            path = string.Empty,
            recursive = false,
            include_media_info = false,
            include_deleted = false,
            include_has_explicit_shared_members = false
        };
        var response = await HttpClient.PostAsync(urlPath, new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json"));
        var files = JObject.Parse(await response.Content.ReadAsStringAsync());
        var uris = files["entries"]
            .OrderByDescending(x => DateTime.Parse(x["server_modified"].ToString()))
            .Select(y => y["path_lower"].ToString()).ToList();

        return uris;
    }

    public async Task<string> GetFileUri(string filePath)
    {
        const string urlPath = "get_temporary_link";

        var postData = new
        {
            path = filePath
        };

        var response = await HttpClient.PostAsync(urlPath, new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json"));
        var files = JObject.Parse(await response.Content.ReadAsStringAsync());
        var link = files["link"].ToString();
        return link;
    }
}