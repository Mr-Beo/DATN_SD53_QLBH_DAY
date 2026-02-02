using Data.Models;

namespace BlazorKhachHang.Service.IService
{
    public interface IKhachHangService
    {

        Task<List<KhachHang>> GetAllAsync();


        Task<KhachHang?> GetByIdAsync(Guid id);

        Task<bool> CreateAsync(KhachHang khachHang);

        Task UpdateAsync(KhachHang khachHang);

        Task<List<KhachHang>> SearchAsync(string keyword);
    }
}
