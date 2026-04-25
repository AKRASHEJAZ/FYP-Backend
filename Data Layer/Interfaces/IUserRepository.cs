
using Data_Layer.Entities;

namespace Data_Layer.Interfaces
{
    public interface IUserRepository
    {
        User? GetByEmail(string email);
        void Add(User user);
    }
}
