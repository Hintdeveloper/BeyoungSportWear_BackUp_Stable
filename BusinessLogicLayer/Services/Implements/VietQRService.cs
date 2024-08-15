using BusinessLogicLayer.Services.Interface;
using BusinessLogicLayer.Viewmodels.VietQR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.Implements
{
    public class VietQRService : IVietQRService
    {
        private readonly HttpClient _httpClient;
        private readonly VietQRSettings _settings;

        public VietQRService(HttpClient httpClient, IOptions<VietQRSettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }
        public async Task<VietQRResponse> GenerateQR(VietQRRequest request)
        {
            var jsonRequest = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-client-id", _settings.ClientId);
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _settings.ApiKey);

            var response = await _httpClient.PostAsync("https://api.vietqr.io/v2/generate", httpContent);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<VietQRResponse>(jsonResponse);
        }
    }
}
