using PulseFlow.Interfaces;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows; // MessageBox를 위해 추가

namespace PulseFlow.Services
{
    public class MachineControlService : IMachineControlService
    {
      
        private static readonly HttpClient _httpClient = new HttpClient();


        private readonly string _baseUrl = "http://localhost:5000/api/control";

        public async Task<bool> StopMachineAsync(string machineName)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{_baseUrl}/{machineName}/stop", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
            
                MessageBox.Show($"[{machineName} 정지 통신 에러]\n{ex.Message}", "에러 발생");
                return false;
            }
        }

        public async Task<bool> StartMachineAsync(string machineName)
        {
            try
            {
                var response = await _httpClient.PostAsync($"{_baseUrl}/{machineName}/start", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"[{machineName} 가동 통신 에러]\n{ex.Message}", "에러 발생");
                return false;
            }
        }
    }
}