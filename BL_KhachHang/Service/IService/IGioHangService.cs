using API.Models.DTO;

namespace BlazorKhachHang.Service.IService
{
    public interface IGioHangService
    {
        // ✅ Chỉnh lại tên tham số và bỏ chiTietGiayId nếu API không dùng
        Task AddToCartAsync(Guid gioHangId, Guid giayChiTietId, int soLuong);

        Task<GioHangDTO> LayTheoTaiKhoanAsync(Guid taiKhoanId);
        Task CapNhatSoLuong(Guid chiTietId, int soLuong);
        Task XoaSanPham(Guid chiTietId);
    }
}
