using Newtonsoft.Json;

public class CaptchaVerifyResponse
{
    [JsonProperty("success")]
    public bool Success { get; set; }

    [JsonProperty("challenge_ts")]
    public DateTime ChallengeTimestamp { get; set; }

    [JsonProperty("hostname")]
    public string Hostname { get; set; }
}