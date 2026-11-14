using APCapstoneProject.Service;
using APCapstoneProject.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

public class CaptchaService : ICaptchaService
{
    private readonly HttpClient _httpClient;
    private readonly string _secretKey;

    public CaptchaService(IOptions<CaptchaSettings> config)
    {
        _httpClient = new HttpClient();
        _secretKey = config.Value.SecretKey;
    }

    public async Task<bool> VerifyCaptchaAsync(string token)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        var content = new FormUrlEncodedContent(new[]
        {
                new KeyValuePair<string, string>("secret", _secretKey),
                new KeyValuePair<string, string>("response", token)
            });

        var response = await _httpClient.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
        if (!response.IsSuccessStatusCode)
            return false;

        var json = await response.Content.ReadAsStringAsync();
        var captchaResult = JsonConvert.DeserializeObject<CaptchaVerifyResponse>(json);
        return captchaResult?.Success ?? false;
    }
}