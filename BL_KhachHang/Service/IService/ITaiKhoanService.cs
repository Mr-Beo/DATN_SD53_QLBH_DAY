using API.Models.DTO;
using Data.Models;

namespace BlazorKhachHang.Service.IService
{
    public interface ITaiKhoanService
    {
        Task<List<TaiKhoan>> GetAllTaiKhoanAsync();
        Task<TaiKhoan?> GetByIdTaiKhoanAsync(Guid id);
        Task<TaiKhoan?> GetByUsernameAsync(string username);

        Task<TaiKhoan?> CreateTaiKhoanAsync(TaiKhoan tk);
        Task UpdateTaiKhoanAsync(TaiKhoan tk);
        Task DeleteTaiKhoanAsync(Guid id);

        // Đăng nhập khách hàng
        Task<LoginResponseDto> GetKhachHangAsync(string username, string password);
    }
}
