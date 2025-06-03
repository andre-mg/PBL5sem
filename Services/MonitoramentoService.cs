using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PBL5sem.Services
{
    public class MonitoramentoService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://107.22.87.223:1026/v2/entities/";

        public MonitoramentoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("fiware-service", "smart");
            _httpClient.DefaultRequestHeaders.Add("fiware-servicepath", "/");
        }

        public async Task<DadosSensor> ObterDadosSensorAsync(string sensorId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}{sensorId}/attrs");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<DadosSensor>(content);
            }
            catch (Exception ex)
            {
                // Tratar erros adequadamente
                throw;
            }
        }
    }

    public class DadosSensor
    {
        public decimal Temperatura { get; set; }
        public decimal Umidade { get; set; }
        public decimal Luminosidade { get; set; }
    }
}