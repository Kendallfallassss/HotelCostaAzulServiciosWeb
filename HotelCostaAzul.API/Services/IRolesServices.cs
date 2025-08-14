using HotelCostaAzul.API.Models.Entities;

namespace HotelCostaAzul.API.Services
{
    public interface IRolesService
    {
        IEnumerable<Roles> GetAll();
        Roles GetById(int id);
        Task<Roles> Create(Roles role);
        Task<Roles> Update(int id, Roles role);
        Task<bool> Delete(int id);
    }
}
