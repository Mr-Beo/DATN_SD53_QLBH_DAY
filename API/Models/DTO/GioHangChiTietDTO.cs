namespace API.Models.DTO
{
    public class GioHangChiTietDTO
    {
        public Guid GioHangChiTietId { get; set; }
        public string TenGiay { get; set; }
        public string? HinhAnh { get; set; }
        public int SoLuong { get; set; }
        public decimal Gia { get; set; }
        public decimal ThanhTien => Gia * SoLuong;
    }
}
