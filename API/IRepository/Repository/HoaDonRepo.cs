using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace API.IRepository.Repository
{
    public class HoaDonRepo : IHoaDonRepo
    {
        private readonly DbContextApp _context;

        public HoaDonRepo(DbContextApp context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<HoaDon>> GetAll()
        {
            return await _context.HoaDons
                .Include(h => h.voucher)
                .Include(h => h.nhanVien)
                .Include(h => h.hinhThucThanhToan)
                .Include(h => h.khachHang)
                .Include(h => h.HoaDonChiTiets)
                .ToListAsync();
        }

        public async Task<HoaDon> GetById(Guid id)
        {
            var hoaDon = await _context.HoaDons
                .Include(h => h.voucher)
                .Include(h => h.nhanVien) // Đã thay vì .taiKhoan
                .Include(h => h.hinhThucThanhToan)
                .Include(h => h.khachHang)
                .Include(h => h.HoaDonChiTiets)
                .FirstOrDefaultAsync(h => h.HoaDonId == id);

            return hoaDon ?? throw new KeyNotFoundException("Hóa đơn không tồn tại với ID đã cho.");
        }

        public async Task Create(HoaDon hoaDon)
        {
            _context.HoaDons.Add(hoaDon);
            await _context.SaveChangesAsync();
        }

        public async Task Update(HoaDon hoaDon)
        {
            _context.HoaDons.Update(hoaDon);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> Delete(Guid id)
        {
            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon == null)
                return false;

            _context.HoaDons.Remove(hoaDon);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
