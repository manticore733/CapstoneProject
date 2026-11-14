using APCapstoneProject.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace APCapstoneProject.Service
{
    public interface ICaptchaService
    {
        Task<bool> VerifyCaptchaAsync(string token);
    }
}
