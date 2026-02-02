using System.Net.Http.Json;
using Data.Models;
using BlazorKhachHang.Service.IService;

namespace BlazorKhachHang.Service
{

    public class KhachHangService : IKhachHangService
    {
        private readonly HttpClient _httpClient;

        public KhachHangService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<KhachHang>> GetAllAsync()
        {
            // GET: api/KhachHang
            return await _httpClient.GetFromJsonAsync<List<KhachHang>>("api/KhachHang")
                   ?? new List<KhachHang>();
        }

        public async Task<KhachHang?> GetByIdAsync(Guid id)
        {
            // GET: api/KhachHang/{id}
            return await _httpClient.GetFromJsonAsync<KhachHang>($"api/KhachHang/{id}");
        }

        public async Task<bool> CreateAsync(KhachHang khachHang)
        {
            // POST: api/KhachHang
            var response = await _httpClient.PostAsJsonAsync("api/KhachHang", khachHang);
            return response.IsSuccessStatusCode;
        }

        public async Task UpdateAsync(KhachHang khachHang)
        {
            // PUT: api/KhachHang/{id}
            var response = await _httpClient.PutAsJsonAsync(
                $"api/KhachHang/{khachHang.KhachHangId}", khachHang);
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<KhachHang>> SearchAsync(string keyword)
        {
            // GET: api/KhachHang/search?keyword=...
            var result = await _httpClient.GetFromJsonAsync<List<KhachHang>>(
                $"api/KhachHang/search?keyword={Uri.EscapeDataString(keyword ?? string.Empty)}");
            return result ?? new List<KhachHang>();
        }
    }
}
