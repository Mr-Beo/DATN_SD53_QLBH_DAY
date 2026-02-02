using Microsoft.AspNetCore.Mvc;
using API.IRepository;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoaDonController : ControllerBase
    {
        private readonly IHoaDonRepo _hoaDonRepo;
        private readonly IThongBaoRepository _thongBaoRepo; // ✅ thêm repo thông báo

        public HoaDonController(IHoaDonRepo hoaDonRepo, IThongBaoRepository thongBaoRepo)
        {
            _hoaDonRepo = hoaDonRepo;
            _thongBaoRepo = thongBaoRepo;
        }

        // GET: api/HoaDon
        [HttpGet]
        public async Task<ActionResult<IEnumerable<HoaDon>>> GetAllHoaDons()
        {
            var hoaDons = await _hoaDonRepo.GetAll();
            return Ok(hoaDons);
        }

        // GET: api/HoaDon/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<HoaDon>> GetHoaDonById(Guid id)
        {
            var hoaDon = await _hoaDonRepo.GetById(id);
            if (hoaDon == null)
                return NotFound();

            return Ok(hoaDon);
        }

        // POST: api/HoaDon
        [HttpPost]
        public async Task<ActionResult<HoaDon>> CreateHoaDon(HoaDon hoaDon)
        {
            await _hoaDonRepo.Create(hoaDon);

            // ✅ Ghi thông báo tạo hoá đơn
            await _thongBaoRepo.ThemThongBaoAsync($"🧾 Đã tạo hoá đơn mới với mã: {hoaDon.HoaDonId}");

            return CreatedAtAction(nameof(GetHoaDonById), new { id = hoaDon.HoaDonId }, hoaDon);
        }

        // PUT: api/HoaDon/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHoaDon(Guid id, HoaDon hoaDon)
        {
            if (id != hoaDon.HoaDonId)
                return BadRequest("ID mismatch");

            await _hoaDonRepo.Update(hoaDon);

            // ✅ Ghi thông báo cập nhật hoá đơn
            await _thongBaoRepo.ThemThongBaoAsync($"✏️ Đã cập nhật hoá đơn: {hoaDon.HoaDonId}");

            return NoContent();
        }

        // DELETE: api/HoaDon/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHoaDon(Guid id)
        {
            var hoaDon = await _hoaDonRepo.GetById(id);
            if (hoaDon == null) return NotFound();

            var result = await _hoaDonRepo.Delete(id);
            if (!result) return NotFound();

            // ✅ Ghi thông báo xoá hoá đơn
            await _thongBaoRepo.ThemThongBaoAsync($"🗑️ Đã xoá hoá đơn: {hoaDon.HoaDonId}");

            return NoContent();
        }
    }
}
