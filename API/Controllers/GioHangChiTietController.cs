using API.IRepository;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GioHangChiTietController : ControllerBase
    {
        private readonly IGioHangChiTietRepository _repoGioHangChiTiet;
        private readonly IGiayChiTietRepository _repoGiayChiTiet;
        private readonly IGioHangRepository _repoGioHang;

        public GioHangChiTietController(
            IGioHangChiTietRepository repoGioHangChiTiet,
            IGiayChiTietRepository repoGiayChiTiet,
            IGioHangRepository repoGioHang)
        {
            _repoGioHangChiTiet = repoGioHangChiTiet;
            _repoGiayChiTiet = repoGiayChiTiet;
            _repoGioHang = repoGioHang;
        }

        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                if (request.SoLuongSanPham <= 0)
                    return BadRequest("Số lượng sản phẩm phải lớn hơn 0");

                // ✅ Kiểm tra giỏ hàng tồn tại
                var gioHang = await _repoGioHang.GetGioHang(request.GioHangId);
                if (gioHang == null)
                    return BadRequest("Giỏ hàng không tồn tại.");

                // ✅ Lấy chi tiết giày
                var giayChiTiet = await _repoGiayChiTiet.GetByIdAsync(request.GiayChiTietId);
                if (giayChiTiet == null)
                    return NotFound("Chi tiết giày không tồn tại.");

                // ✅ Kiểm tra giá hợp lệ
                if (float.IsNaN(giayChiTiet.Gia) || float.IsInfinity(giayChiTiet.Gia))
                    return BadRequest("Giá sản phẩm không hợp lệ.");

                // ✅ Kiểm tra sản phẩm đã có trong giỏ chưa
                var existingItem = await _repoGioHangChiTiet
                    .GetByGioHangVaGiayChiTietAsync(request.GioHangId, request.GiayChiTietId);

                if (existingItem != null)
                {
                    existingItem.SoLuongSanPham += request.SoLuongSanPham;
                    existingItem.NgayCapNhat = DateTime.Now;
                    await _repoGioHangChiTiet.UpdateAsync(existingItem);
                    return Ok(existingItem);
                }

                // ✅ Tạo mới sản phẩm trong giỏ
                var newItem = new GioHangChiTiet
                {
                    GioHangChiTietId = Guid.NewGuid(),
                    GioHangId = request.GioHangId,
                    GiayId = giayChiTiet.GiayId,
                    Gia = Convert.ToDecimal(giayChiTiet.Gia), // convert float -> decimal
                    SoLuongSanPham = request.SoLuongSanPham,
                    NgayTao = DateTime.Now,
                    NgayCapNhat = DateTime.Now,
                    TrangThai = true
                };

                await _repoGioHangChiTiet.AddAsync(newItem);
                return CreatedAtAction(nameof(GetById), new { id = newItem.GioHangChiTietId }, newItem);
            }
            catch (Exception ex)
            {
                // ✅ Trả lỗi chi tiết để debug
                return StatusCode(500, $"Lỗi khi thêm sản phẩm vào giỏ hàng: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _repoGioHangChiTiet.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }
    }

    public class AddToCartRequest
    {
        public Guid GioHangId { get; set; }
        public Guid GiayChiTietId { get; set; }
        public int SoLuongSanPham { get; set; }
    }
}
