using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class HoaDon
    {
        [Key]
        public Guid HoaDonId { get; set; } = Guid.NewGuid();

        public Guid NhanVienId { get; set; }
        public Guid? KhachHangId { get; set; }
        public Guid? HinhThucThanhToanId { get; set; }
        public Guid? VoucherId { get; set; }

        public DateTime NgayTao { get; set; } = DateTime.Now;
        public DateTime NgayNhanHang { get; set; }

        public float TongTienSauKhiGiam { get; set; }//Tong tiền sau khi áp dụng voucher và giảm giá (nếu có),va phiship
        public float PhiShip { get; set; }
        public string TrangThai { get; set; }
    
        [MaxLength(200)]
        public string GhiChu { get; set; }

        public Voucher ? voucher { get; set; }
        public NhanVien? nhanVien { get; set; }
        public HinhThucThanhToan? hinhThucThanhToan {  get; set; }
        public KhachHang? khachHang { get; set; }
        public virtual ICollection<HoaDonChiTiet> HoaDonChiTiets { get; set; } = new List<HoaDonChiTiet>();


    }

}
