using System.Text.Json.Serialization;

namespace GithubAPIClient
{
    public class Repository
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("description")]
        public string Description { get; set; } = null!;

        [JsonPropertyName("html_url")]
        public Uri GitHubHomeUrl { get; set; } = null!;

        [JsonPropertyName("homepage")]
        public Uri Homepage { get; set; } = null!;

        [JsonPropertyName("watchers")]
        public int Watchers { get; set; }

        [JsonPropertyName("pushed_at")]
        public DateTime LastPushUtc { get; set; }

        public DateTime LastPush => LastPushUtc.ToLocalTime();

    }
}
