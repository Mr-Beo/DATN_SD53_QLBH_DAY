using System.Net.Http.Json;
using System.Text.Json;
using API.Models.DTO;
using BlazorKhachHang.Service.IService;
using Data.Models;

namespace BlazorKhachHang.Service
{
    public class TaiKhoanService : ITaiKhoanService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public TaiKhoanService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<TaiKhoan>> GetAllTaiKhoanAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<TaiKhoan>>(
                "/api/TaiKhoans", _jsonOptions
            ) ?? new List<TaiKhoan>();
        }

        public async Task<TaiKhoan?> GetByIdTaiKhoanAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"/api/TaiKhoans/{id}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return string.IsNullOrWhiteSpace(json)
                ? null
                : JsonSerializer.Deserialize<TaiKhoan>(json, _jsonOptions);
        }

        public async Task<TaiKhoan?> GetByUsernameAsync(string username)
        {
            var encoded = Uri.EscapeDataString(username);
            return await _httpClient.GetFromJsonAsync<TaiKhoan>(
                $"/api/TaiKhoans/username/{encoded}", _jsonOptions
            );
        }

        public async Task<TaiKhoan?> CreateTaiKhoanAsync(TaiKhoan tk)
        {
            var res = await _httpClient.PostAsJsonAsync("/api/TaiKhoans", tk);
            if (!res.IsSuccessStatusCode) return null;

            return await res.Content.ReadFromJsonAsync<TaiKhoan>();
        }

        public async Task UpdateTaiKhoanAsync(TaiKhoan tk)
        {
            var res = await _httpClient.PutAsJsonAsync(
                $"/api/TaiKhoans/{tk.TaikhoanId}", tk
            );
            res.EnsureSuccessStatusCode();
        }

        public async Task DeleteTaiKhoanAsync(Guid id)
        {
            var res = await _httpClient.DeleteAsync($"/api/TaiKhoans/{id}");
            res.EnsureSuccessStatusCode();
        }

        public async Task<LoginResponseDto> GetKhachHangAsync(string username, string password)
        {
            try
            {
                var encodedUser = Uri.EscapeDataString(username);
                var encodedPass = Uri.EscapeDataString(password);

                var response = await _httpClient.GetAsync(
                    $"/api/TaiKhoans/loginkh?username={encodedUser}&pass={encodedPass}"
                );

                if (!response.IsSuccessStatusCode)
                {
                    return new LoginResponseDto
                    {
                        IsSuccess = false,
                        Message = "Tên đăng nhập hoặc mật khẩu không đúng."
                    };
                }

                return await response.Content.ReadFromJsonAsync<LoginResponseDto>(_jsonOptions)
                       ?? new LoginResponseDto { IsSuccess = false, Message = "Phản hồi rỗng từ server." };
            }
            catch
            {
                return new LoginResponseDto
                {
                    IsSuccess = false,
                    Message = "Không thể kết nối đến máy chủ."
                };
            }
        }
    }
}
