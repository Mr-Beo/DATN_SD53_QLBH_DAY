using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace API.IRepository.Repository
{
    public class GioHangRepository : IGioHangRepository
    {
        private readonly DbContextApp _dbcontext;

        public GioHangRepository(DbContextApp dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task CreateGioHang(GioHang gh)
        {
            try
            {
                _dbcontext.Add<GioHang>(gh);
                await _dbcontext.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task DeleteGioHang(Guid id)
        {
            try
            {
                var ghdel = await _dbcontext.GioHangs.FindAsync(id);
                if (ghdel != null) { 
                    _dbcontext.Remove<GioHang>(ghdel);
                    await _dbcontext.SaveChangesAsync();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<GioHang>> GetAllGioHang()
        {
            return await _dbcontext.GioHangs.ToListAsync();
        }

        public async Task<GioHang> GetGioHang(Guid id)
        {
            return await _dbcontext.GioHangs.FindAsync(id);
        }

        public async Task UpdateGioHang(GioHang gh)
        {
            try
            {
                _dbcontext.Update<GioHang>(gh);
                await _dbcontext.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
