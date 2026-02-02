using API.IRepository;
using API.Models.DTO.TraHang;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace API.IRepository.Repository
{
    public class ReturnRepository : IReturnRepository
    {
        private readonly DbContextApp _context;

        public ReturnRepository(DbContextApp context)
        {
            _context = context;
        }
        public async Task<List<HoaDon>> GetHoaDonWithChiTietAsync(string maHoaDon = null, string tenKhachHang = null, string sdt = null, DateTime? ngayTao = null, string trangThai = null)
        {
            var query = _context.HoaDons
                .Include(h => h.HoaDonChiTiets)
                .ThenInclude(hdc => hdc.GiayChiTiet)
                   .Include(h => h.nhanVien)
                 .Include(h => h.khachHang)
                .Include(h => h.hinhThucThanhToan)
                .AsQueryable();

            if (!string.IsNullOrEmpty(maHoaDon))
                query = query.Where(h => h.HoaDonId.ToString().Contains(maHoaDon));

            if (ngayTao.HasValue)
                query = query.Where(h => h.NgayTao.Date == ngayTao.Value.Date);
            if (!string.IsNullOrEmpty(trangThai))
                query = query.Where(h => h.TrangThai.ToLower().Contains(trangThai.ToLower()));

            return await query.OrderByDescending(h => h.NgayTao).ToListAsync();
        }

        public async Task<HoaDon?> GetHoaDonByIdAsync(Guid hoaDonId)
        {
            return await _context.HoaDons
                .Include(h => h.HoaDonChiTiets)
                .ThenInclude(hdc => hdc.GiayChiTiet)
                .Include(h => h.nhanVien)
                 .Include(h => h.khachHang)
                .Include(h => h.hinhThucThanhToan)
                .FirstOrDefaultAsync(h => h.HoaDonId == hoaDonId);
        }

        public async Task<List<HoaDonChiTiet>> GetLichSuTraHangAsync(Guid hoaDonId)
        {
            return await _context.HoaDonChiTiets
                .Where(hdc => hdc.HoaDonId == hoaDonId && hdc.TrangThai == true)
                .Include(hdc => hdc.GiayChiTiet)
                .OrderByDescending(hdc => hdc.NgayTraHang)
                .ToListAsync();
        }

        public async Task<Dictionary<Guid, int>> GetSoLuongConLaiChoTraAsync(Guid hoaDonId)
        {
            var chiTiets = await _context.HoaDonChiTiets
                .Where(hdc => hdc.HoaDonId == hoaDonId)
                .ToListAsync();

            var result = new Dictionary<Guid, int>();
            foreach (var group in chiTiets.GroupBy(hdc => hdc.GiayChiTietId))
            {
                var soLuongMua = group.Where(hdc => !hdc.TrangThai).Sum(hdc => hdc.SoLuongSanPham);
                var soLuongDaTra = group.Where(hdc => hdc.TrangThai).Sum(hdc => hdc.SoLuongSanPham);
                result[group.Key] = soLuongMua - soLuongDaTra;
            }
            return result;
        }

        public async Task<bool> KiemTraConHanTraHangAsync(Guid hoaDonId)
        {
            var hoaDon = await _context.HoaDons.FirstOrDefaultAsync(h => h.HoaDonId == hoaDonId);
            if (hoaDon == null) return false;
            return (DateTime.UtcNow - hoaDon.NgayTao).TotalDays <= 7;
        }

        public async Task<bool> SanPhamNamTrongHoaDon(Guid hoaDonId, Guid giayId)
        {
            return await _context.HoaDonChiTiets
                .AnyAsync(hdc => hdc.HoaDonId == hoaDonId && hdc.GiayChiTietId == giayId);
        }

        public async Task<bool> KiemTraDuocPhepTra(Guid hoaDonId, Guid giayId, int soLuongMuonTra)
        {
            var soLuongConLai = await GetSoLuongConLaiChoTraAsync(hoaDonId);
            return soLuongConLai.ContainsKey(giayId) && soLuongConLai[giayId] >= soLuongMuonTra;
        }

        public async Task<bool> TraHangAsync(Guid hoaDonId, List<ChiTietTraHangDTO> items, Guid taiKhoanId)
        {
            if (!await KiemTraConHanTraHangAsync(hoaDonId)) return false;

            foreach (var item in items)
            {
                if (!await KiemTraDuocPhepTra(hoaDonId, item.GiayChiTietId, item.SoLuong)) return false;

                var chiTiet = new HoaDonChiTiet
                {
                    HoaDonId = hoaDonId,
                    GiayChiTietId = item.GiayChiTietId,
                    SoLuongSanPham = item.SoLuong,
                    Gia = (await _context.HoaDonChiTiets
                        .FirstAsync(hdc => hdc.HoaDonId == hoaDonId && hdc.GiayChiTietId == item.GiayChiTietId && !hdc.TrangThai)).Gia,
                    TrangThai = true,
                    NgayTraHang = DateTime.UtcNow,
                    GhiChu = item.GhiChu
                };
                _context.HoaDonChiTiets.Add(chiTiet);
            }

            await _context.SaveChangesAsync();
            await CapNhatTrangThaiHoaDonNeuTraHet(hoaDonId);
            return true;
        }

        public async Task CapNhatTrangThaiHoaDonNeuTraHet(Guid hoaDonId)
        {
            var soLuongConLai = await GetSoLuongConLaiChoTraAsync(hoaDonId);
            if (soLuongConLai.Values.All(sl => sl == 0))
            {
                var hoaDon = await _context.HoaDons.FirstAsync(h => h.HoaDonId == hoaDonId);
                hoaDon.TrangThai = "DaTraHet";
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<HoaDonChiTiet>> GetDanhSachSanPhamTrongHoaDon(Guid hoaDonId)
        {
            return await _context.HoaDonChiTiets
                .Where(hdc => hdc.HoaDonId == hoaDonId)
                .Include(hdc => hdc.GiayChiTiet.Giay.TenGiay)
                .OrderByDescending(hdc => hdc.NgayTraHang ?? DateTime.MaxValue)
                .ToListAsync();
        }

        public async Task<bool> HuyTraHangAsync(Guid hoaDonChiTietId)
        {
            var chiTiet = await _context.HoaDonChiTiets
                .FirstOrDefaultAsync(hdc => hdc.HoaDonChiTietId == hoaDonChiTietId && hdc.TrangThai == true);
            if (chiTiet == null) return false;

            _context.HoaDonChiTiets.Remove(chiTiet);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CapNhatGhiChuTraHang(Guid hoaDonChiTietId, string ghiChuMoi)
        {
            var chiTiet = await _context.HoaDonChiTiets
                .FirstOrDefaultAsync(hdc => hdc.HoaDonChiTietId == hoaDonChiTietId && hdc.TrangThai == true);
            if (chiTiet == null) return false;

            chiTiet.GhiChu = ghiChuMoi;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> GetTenGiayAsync(Guid giayId)
        {
            var giay = await _context.Giays.FirstOrDefaultAsync(g => g.GiayId == giayId);
            return giay?.TenGiay ?? "Unknown";
        }
    }
}