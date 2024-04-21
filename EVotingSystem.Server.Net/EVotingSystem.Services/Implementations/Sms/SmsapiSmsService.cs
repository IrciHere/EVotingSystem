using System.Net.Http.Headers;
using EVotingSystem.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace EVotingSystem.Services.Implementations.Sms;

public class SmsapiSmsService : ISmsService
{
    private readonly string _url;
    private readonly string _bearer;

    public SmsapiSmsService(IConfiguration configuration)
    {
        _url = configuration["SmsapiSettings:Url"];
        _bearer = configuration["SmsapiSettings:Token"];
    }
    
    public async Task SendOtpCote(string phoneNumber, string otpCode)
    {
        HttpClient httpClient = new()
        {
            BaseAddress = new Uri(_url)
        };
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearer);

        string uriPhoneNumber = phoneNumber.Replace("+", "");
        var message = $"Kod weryfikacyjny głosu: {otpCode}";
        
        var uriWithParams = $"sms.do?to={uriPhoneNumber}&message={message}";
        HttpResponseMessage response = await httpClient.PostAsync(uriWithParams, null);
        
        string jsonResponse = await response.Content.ReadAsStringAsync();
        Console.WriteLine(jsonResponse);
    }
}