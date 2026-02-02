using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Data.Models;
using API.IRepository;
using API.Models.DTO;
using Data.Repositories;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaiKhoansController : ControllerBase
    {
        private readonly ITaiKhoanRepository _context;
        private readonly INhanVienRepository _nhanVienRepository;
        private readonly IKhachHangRepository _khachHangRepository;
        private readonly IChucVuRepository _chucVuRepository;
        private readonly IThongBaoRepository _thongBaoRepository; // ✅ thêm

        public TaiKhoansController(
            ITaiKhoanRepository context,
            INhanVienRepository nhanVienRepository,
            IChucVuRepository chucVuRepository,
            IThongBaoRepository thongBaoRepository,
            IKhachHangRepository khachHangRepository
            ) // ✅ inject
        {
            _context = context;
            _nhanVienRepository = nhanVienRepository;
            _chucVuRepository = chucVuRepository;
            _thongBaoRepository = thongBaoRepository;
            _khachHangRepository = khachHangRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaiKhoan>>> GetTaiKhoans()
        {
            return Ok(await _context.GetAllTaiKhoanAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaiKhoan>> GetTaiKhoan(Guid id)
        {
            return Ok(await _context.GetByIdTaiKhoanAsync(id));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaiKhoan(Guid id, TaiKhoan taiKhoan)
        {
            await _context.UpdateTaiKhoanAsync(taiKhoan);

            // ✅ Ghi thông báo
            await _thongBaoRepository.ThemThongBaoAsync($"✏️ Đã cập nhật tài khoản: {taiKhoan.Username}");

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult<TaiKhoan>> PostTaiKhoan(TaiKhoan taiKhoan)
        {
            await _context.CreateTaiKhoanAsync(taiKhoan);

            // ✅ Ghi thông báo
            await _thongBaoRepository.ThemThongBaoAsync($"👤 Tạo tài khoản mới: {taiKhoan.Username}");

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaiKhoan(Guid id)
        {
            var taiKhoan = await _context.GetByIdTaiKhoanAsync(id);
            if (taiKhoan == null) return NotFound();

            await _context.DeleteTaiKhoanAsync(id);

            // ✅ Ghi thông báo
            await _thongBaoRepository.ThemThongBaoAsync($"🗑️ Đã xoá tài khoản: {taiKhoan.Username}");

            return Ok();
        }

        [HttpGet("username/{username}")]
        public async Task<ActionResult<TaiKhoan>> GetTaiKhoanByUsername(string username)
        {
            var taiKhoan = await _context.GetByUsernameAsync(username);
            Console.WriteLine($"Username received: {username}, Found: {taiKhoan != null}");

            if (taiKhoan == null)
            {
                return NotFound();
            }
            return Ok(taiKhoan);
        }

        [HttpGet("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(string username, string pass)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new LoginResponseDto
                {
                    IsSuccess = false,
                    Message = "Dữ liệu đăng nhập không hợp lệ: " + string.Join("; ", errors)
                });
            }

            var userChucVu = await _context.GetByIdChucVuAsync(username, pass);

            if (userChucVu != null)
            {
                string role = _chucVuRepository
                    .GetByIdChucVuAsync(
                        _nhanVienRepository.GetIdNhanVienTaiKhoan(userChucVu.TaikhoanId).Result.ChucVuId.Value
                    ).Result.TenChucVu;

                return Ok(new LoginResponseDto
                {
                    Id = userChucVu.NhanVien.NhanVienId,
                    IsSuccess = true,
                    Username = username,
                    Role = role,
                    Message = "Đăng nhập thành công!"
                });
            }
            else
            {
                return Unauthorized(new LoginResponseDto
                {
                    IsSuccess = false,
                    Username = username,
                    Role = null,
                    Message = "Tên đăng nhập hoặc mật khẩu không đúng."
                });
            }
        }
        [HttpGet("loginkh")]
        public async Task<ActionResult<LoginResponseDto>> LoginKH(string username, string pass)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new LoginResponseDto
                {
                    IsSuccess = false,
                    Message = "Dữ liệu đăng nhập không hợp lệ: " + string.Join("; ", errors)
                });
            }

            var userChucVu = await _context.GetKhachHang(username, pass);

            if (userChucVu != null)
            {
                return Ok(new LoginResponseDto
                {
                    Id = userChucVu.KhachHang.KhachHangId,
                    IsSuccess = true,
                    Username = username,
                    Message = "Đăng nhập thành công!"
                });
            }
            else
            {
                return Unauthorized(new LoginResponseDto
                {
                    IsSuccess = false,
                    Username = username,
                    Role = null,
                    Message = "Tên đăng nhập hoặc mật khẩu không đúng."
                });
            }
        }
    }
}
