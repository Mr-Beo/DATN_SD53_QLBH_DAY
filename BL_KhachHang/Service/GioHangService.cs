using System.Net.Http.Json;
using API.Models.DTO;
using BlazorKhachHang.Service.IService;

namespace BlazorKhachHang.Service
{
    public class GioHangService : IGioHangService
    {
        private readonly HttpClient _http;
        public GioHangService(HttpClient http) => _http = http;

        public async Task AddToCartAsync(Guid gioHangId, Guid giayChiTietId, int soLuong)
        {
            var payload = new
            {
                GioHangId = gioHangId,
                GiayChiTietId = giayChiTietId,
                SoLuongSanPham = soLuong
            };

            var res = await _http.PostAsJsonAsync("api/GioHangChiTiet/add-to-cart", payload);
            res.EnsureSuccessStatusCode();
        }

        public async Task<GioHangDTO> LayTheoTaiKhoanAsync(Guid taiKhoanId)
            => await _http.GetFromJsonAsync<GioHangDTO>($"api/giohang/tai-khoan/{taiKhoanId}");

        public async Task CapNhatSoLuong(Guid chiTietId, int soLuong)
            => await _http.PutAsJsonAsync($"api/giohang/cap-nhat-so-luong/{chiTietId}", soLuong);

        public async Task XoaSanPham(Guid chiTietId)
            => await _http.DeleteAsync($"api/giohang/xoa/{chiTietId}");
    }
}
