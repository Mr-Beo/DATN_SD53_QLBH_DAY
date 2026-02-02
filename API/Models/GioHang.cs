using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models
{
    public class GioHang
    {
        [Key]
        public Guid GioHangId { get; set; }
        public DateTime NgayTaoGioHang { get; set; }
        public DateTime NgayCapNhatCuoiCung { get; set; }
        public bool TrangThai { get; set; }
        public Guid KhachHangId { get; set; }
        public virtual KhachHang KhachHang { get; set; }
        public virtual ICollection<GioHangChiTiet> GioHangChiTiets { get; set; } = new List<GioHangChiTiet>();

    }
}
