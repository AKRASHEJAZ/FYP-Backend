
using Data_Layer.Entities;
using Data_Layer.filters;

namespace Data_Layer.Interfaces
{
    public interface IUserRepository
    {
        User? GetByEmail(string email);
        bool Add(User user);

        User? GetById(int id);
        List<User>? GetAll(userFilters filter);

        User? Delete(int id);

        User? Update(int id, User newuser);
        Role? AddRole(Role role);
    }
}
