using API.Models.DTO;
using Application.DTOs;
using Data.IRepository;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiaChiKhachHangController : ControllerBase
    {
        private readonly IDiaChiKhachHangRepository _repository;

        public DiaChiKhachHangController(IDiaChiKhachHangRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiaChiKhachHangDto>>> GetAll()
        {
            var data = await _repository.GetAllAsync();
            return Ok(data.Select(MapToDto));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DiaChiKhachHangDto>> GetById(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();
            return Ok(MapToDto(entity));
        }

        [HttpGet("khachhang/{khachHangId}")]
        public async Task<ActionResult<IEnumerable<DiaChiKhachHangDto>>> GetByKhachHangId(Guid khachHangId)
        {
            var list = await _repository.GetByKhachHangIdAsync(khachHangId);
            return Ok(list.Select(MapToDto));
        }

        [HttpGet("default/{khachHangId}")]
        public async Task<ActionResult<DiaChiKhachHangDto>> GetDefault(Guid khachHangId)
        {
            var entity = await _repository.GetDefaultByKhachHangIdAsync(khachHangId);
            if (entity == null) return NotFound();
            return Ok(MapToDto(entity));
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] DiaChiKhachHangDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = MapToEntity(dto);
            entity.DiaChiKhachHangId = Guid.NewGuid();

            var result = await _repository.CreateAsync(entity);
            return result
                ? Ok(new { message = "Thêm địa chỉ thành công", id = entity.DiaChiKhachHangId })
                : StatusCode(500, "Lỗi khi thêm địa chỉ");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] DiaChiKhachHangDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return NotFound();

            entity.DiaChiCuThe = dto.DiaChiCuThe;
            entity.PhuongXa = dto.PhuongXa;
            entity.QuanHuyen = dto.QuanHuyen;
            entity.ThanhPho = dto.ThanhPho;
            entity.TrangThai = dto.TrangThai;
            entity.IsDefault = dto.IsDefault;

            var result = await _repository.UpdateAsync(entity);
            return result ? Ok(new { message = "Cập nhật thành công" }) : StatusCode(500);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await _repository.DeleteAsync(id);
            return result ? Ok(new { message = "Xóa thành công" }) : NotFound();
        }

        [HttpPut("set-default/{id}")]
        public async Task<ActionResult> SetDefault(Guid id)
        {
            var result = await _repository.SetDefaultAsync(id);
            return result ? Ok(new { message = "Đã đặt làm địa chỉ mặc định" }) : NotFound();
        }

        private DiaChiKhachHangDto MapToDto(DiaChiKhachHang x) => new DiaChiKhachHangDto
        {
            DiaChiKhachHangId = x.DiaChiKhachHangId,
            DiaChiCuThe = x.DiaChiCuThe,
            PhuongXa = x.PhuongXa,
            QuanHuyen = x.QuanHuyen,
            ThanhPho = x.ThanhPho,
            KhachHangId = x.KhachHangId,
            TrangThai = x.TrangThai,
            IsDefault = x.IsDefault
        };

        private DiaChiKhachHang MapToEntity(DiaChiKhachHangDto dto) => new DiaChiKhachHang
        {
            DiaChiKhachHangId = dto.DiaChiKhachHangId,
            DiaChiCuThe = dto.DiaChiCuThe,
            PhuongXa = dto.PhuongXa,
            QuanHuyen = dto.QuanHuyen,
            ThanhPho = dto.ThanhPho,
            KhachHangId = dto.KhachHangId,
            TrangThai = dto.TrangThai,
            IsDefault = dto.IsDefault
        };
    }
}
