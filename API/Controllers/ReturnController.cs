using Microsoft.AspNetCore.Mvc;
using API.Models.DTO.TraHang;
using Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.IRepository;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReturnController : ControllerBase
    {
        private readonly IReturnRepository _returnRepository;

        public ReturnController(IReturnRepository returnRepository)
        {
            _returnRepository = returnRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReturnDTO>>> SearchHoaDon(
            [FromQuery] string maHoaDon = null,
            [FromQuery] string tenKhachHang = null,
            [FromQuery] string sdt = null,
            [FromQuery] DateTime? ngayTao = null,
            [FromQuery] string trangThai = null)
        {
            try
            {
                var hoaDons = await _returnRepository.GetHoaDonWithChiTietAsync(maHoaDon, tenKhachHang, sdt, ngayTao, trangThai);
                var result = new List<ReturnDTO>();

                foreach (var hoaDon in hoaDons)
                {
                    var soLuongConLai = await _returnRepository.GetSoLuongConLaiChoTraAsync(hoaDon.HoaDonId);
                    var returnItems = new List<ReturnItemDTO>();

                    foreach (var chiTiet in hoaDon.HoaDonChiTiets.Where(hdc => !hdc.TrangThai))
                    {
                        var soLuongDaTra = hoaDon.HoaDonChiTiets
                            .Where(hdc => hdc.GiayChiTietId == chiTiet.GiayChiTietId && hdc.TrangThai)
                            .Sum(hdc => hdc.SoLuongSanPham);

                        returnItems.Add(new ReturnItemDTO
                        {
                            GiayChiTietId = chiTiet.GiayChiTietId,
                            TenSanPham = chiTiet.GiayChiTiet.Giay.TenGiay ?? "Không xác định",
                            SoLuongMua = chiTiet.SoLuongSanPham,
                            SoLuongDaTra = soLuongDaTra,
                            SoLuongConLai = soLuongConLai.ContainsKey(chiTiet.GiayChiTietId) ? soLuongConLai[chiTiet.GiayChiTietId] : 0,
                            Gia = chiTiet.Gia
                        });
                    }

                    result.Add(new ReturnDTO
                    {
                        HoaDonId = hoaDon.HoaDonId,
                        
                        NgayTao = hoaDon.NgayTao,
                        TongTienSauKhiGiam = hoaDon.TongTienSauKhiGiam,
                        TrangThai = hoaDon.TrangThai,
                        SanPhams = returnItems,
                        TenHinhThuc = hoaDon.hinhThucThanhToan.TenHinhThuc,
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API] Lỗi SearchHoaDon: {ex.Message}");
                return StatusCode(500, "Lỗi hệ thống khi xử lý dữ liệu hóa đơn");
            }
        }

        [HttpGet("{hoaDonId}")]
        public async Task<ActionResult<ReturnDTO>> GetReturnInfo(Guid hoaDonId)
        {
            var hoaDon = await _returnRepository.GetHoaDonByIdAsync(hoaDonId);
            if (hoaDon == null) return NotFound();

            var soLuongConLai = await _returnRepository.GetSoLuongConLaiChoTraAsync(hoaDonId);
            var returnItems = new List<ReturnItemDTO>();

            foreach (var chiTiet in hoaDon.HoaDonChiTiets.Where(hdc => !hdc.TrangThai))
            {
                var soLuongDaTra = hoaDon.HoaDonChiTiets
                    .Where(hdc => hdc.GiayChiTietId == chiTiet.GiayChiTietId && hdc.TrangThai)
                    .Sum(hdc => hdc.SoLuongSanPham);

                returnItems.Add(new ReturnItemDTO
                {
                    GiayChiTietId = chiTiet.GiayChiTietId,
                    TenSanPham = chiTiet.GiayChiTiet.Giay.TenGiay,
                    SoLuongMua = chiTiet.SoLuongSanPham,
                    SoLuongDaTra = soLuongDaTra,
                    SoLuongConLai = soLuongConLai.ContainsKey(chiTiet.GiayChiTietId) ? soLuongConLai[chiTiet.GiayChiTietId] : 0,
                    Gia = chiTiet.Gia
                });
            }

            return Ok(new ReturnDTO
            {
                HoaDonId = hoaDon.HoaDonId,
         
                NgayTao = hoaDon.NgayTao,
                TongTienSauKhiGiam = hoaDon.TongTienSauKhiGiam,
                TrangThai = hoaDon.TrangThai,
                SanPhams = returnItems,
                TenHinhThuc = hoaDon.hinhThucThanhToan.TenHinhThuc,
            
            });
        }

        [HttpGet("history/{hoaDonId}")]
        public async Task<ActionResult<List<ReturnHistoryDTO>>> GetReturnHistory(Guid hoaDonId)
        {
            var lichSu = await _returnRepository.GetLichSuTraHangAsync(hoaDonId);
            var result = lichSu.Select(hdc => new ReturnHistoryDTO
            {
                HoaDonChiTietId = hdc.HoaDonChiTietId,
                GiayId = hdc.GiayChiTietId,
                TenSanPham = hdc.GiayChiTiet.Giay.TenGiay,
                SoLuong = hdc.SoLuongSanPham,
                Gia = hdc.Gia,
                NgayTraHang = hdc.NgayTraHang,
                GhiChu = hdc.GhiChu
            }).ToList();

            return Ok(result);
        }
        [HttpPost("validate")]
        public async Task<ActionResult<List<ValidateReturnResultDTO>>> ValidateReturn([FromBody] CreateReturnDTO request)
        {
            try
            {
                if (request == null || request.Items == null)
                {
                    return BadRequest(new { error = "Dữ liệu yêu cầu không hợp lệ" });
                }

                var results = await ValidateReturnInternal(request);
                return Ok(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API] Lỗi ValidateReturn: {ex.Message}");
                return StatusCode(500, new { error = $"Lỗi hệ thống: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<bool>> ProcessReturn([FromBody] CreateReturnDTO request)
        {
            try
            {
                if (request == null || request.Items == null || !request.Items.Any(i => i.SoLuong > 0))
                {
                    return BadRequest(new { error = "Dữ liệu yêu cầu trả hàng không hợp lệ hoặc không có sản phẩm để trả" });
                }

                var idClaim = User.FindFirst("id")?.Value;
                if (!Guid.TryParse(idClaim, out var taiKhoanId))
                {
                    taiKhoanId = Guid.Empty;
                }

                var validationResults = await ValidateReturnInternal(request);
                if (validationResults.Any(r => !r.CanReturn))
                {
                    return BadRequest(new { error = "Kiểm tra trả hàng thất bại", details = validationResults });
                    //return Ok(validationResults); //=> đây là cách 1 phù hợp resful api
                }

                var result = await _returnRepository.TraHangAsync(request.HoaDonId, request.Items, taiKhoanId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[API] Lỗi ProcessReturn: {ex.Message}");
                return StatusCode(500, new { error = $"Lỗi hệ thống: {ex.Message}" });
            }
        }


        [HttpDelete("{hoaDonChiTietId}")]
        public async Task<ActionResult<bool>> DeleteReturn(Guid hoaDonChiTietId)
        {
            var result = await _returnRepository.HuyTraHangAsync(hoaDonChiTietId);
            return Ok(result);
        }
        private async Task<List<ValidateReturnResultDTO>> ValidateReturnInternal(CreateReturnDTO request)
        {
            var results = new List<ValidateReturnResultDTO>();
            var canReturn = await _returnRepository.KiemTraConHanTraHangAsync(request.HoaDonId);
            var soLuongConLai = await _returnRepository.GetSoLuongConLaiChoTraAsync(request.HoaDonId);

            foreach (var item in request.Items)
            {
                var tenSanPham = await _returnRepository.GetTenGiayAsync(item.GiayChiTietId);

                if (!canReturn)
                {
                    results.Add(new ValidateReturnResultDTO
                    {
                        GiayId = item.GiayChiTietId,
                        TenSanPham = tenSanPham,
                        CanReturn = false,
                        SoLuongConLai = 0,
                        LyDoTuChoi = "Hóa đơn đã quá hạn trả hàng (7 ngày)"
                    });
                }
                else
                {
                    var canReturnItem = await _returnRepository.KiemTraDuocPhepTra(request.HoaDonId, item.GiayChiTietId, item.SoLuong);
                    results.Add(new ValidateReturnResultDTO
                    {
                        GiayId = item.GiayChiTietId,
                        TenSanPham = tenSanPham,
                        CanReturn = canReturnItem,
                        SoLuongConLai = soLuongConLai.ContainsKey(item.GiayChiTietId) ? soLuongConLai[item.GiayChiTietId] : 0,
                        LyDoTuChoi = canReturnItem ? "" : "Số lượng trả vượt quá số lượng còn lại"
                    });
                }
            }

            return results;
        }

    }
}