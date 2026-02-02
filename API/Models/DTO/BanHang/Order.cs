using Data.Models;

namespace API.Models.DTO.BanHang
{
    public class Order
    {
        public List<GiayChiTietDTO> Products { get; set; } = new();
        public KhachHang? Customer { get; set; }
        public DiaChiKhachHang? Address { get; set; }
        public ThongTinHoaDon ThongTinHoaDon { get; set; } = new();
        public DateTime ThoiGianTao { get; set; } = DateTime.Now;
    }
}
