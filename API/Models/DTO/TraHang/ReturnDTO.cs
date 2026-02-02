using System;
using System.Collections.Generic;
using Data.Models;

namespace API.Models.DTO.TraHang
{
    public class ReturnDTO
    {
        public Guid HoaDonId { get; set; }
        public string TenCuaKhachHang { get; set; }
        public string SDTCuaKhachHang { get; set; }
        public DateTime NgayTao { get; set; }
        public float TongTienSauKhiGiam { get; set; }
        public string TrangThai { get; set; }
        public List<ReturnItemDTO> SanPhams { get; set; } = new List<ReturnItemDTO>();
        public string TenHinhThuc { get; set; }
    }
}