using Data.Models;

namespace API.IRepository
{
    public interface IHoaDonRepo
    {
        Task<List<HoaDon>> GetAll();
        Task<HoaDon> GetById(Guid id);
        Task Create(HoaDon hoaDon);
        Task Update(HoaDon hoaDon);
        Task<bool> Delete(Guid id);
    }
}
