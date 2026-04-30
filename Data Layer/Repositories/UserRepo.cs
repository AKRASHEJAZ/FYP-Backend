using Data_Layer.Data;
using Data_Layer.Entities;
using Data_Layer.Interfaces;
using Microsoft.EntityFrameworkCore;
using Data_Layer.filters;

namespace Data_Layer.Repositories
{
    public class UserRepo : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepo(AppDbContext context)
        {
            _context = context;
        }

        public User? GetByEmail(string email)
        {
            try
            {
                return _context.Users.Include(u => u.Role).FirstOrDefault(x => x.Email == email);
            }
            catch
            {
                return null;
            }
        }

        User? IUserRepository.GetById(int id)
        {
            try
            {
                return _context.Users.Include(u => u.Role).FirstOrDefault(x => x.Id == id);
            }
            catch
            {
                return null;
            }
        }

        List<User>? IUserRepository.GetAll(UserFilters filter)
        {
            try
            {
                var query = _context.Users.Include(u => u.Role).AsQueryable();

                if (filter.ID?.Count > 0)
                {
                    query = query.Where(u => filter.ID.Contains(u.Id));
                }

                if (!string.IsNullOrEmpty(filter.Name))
                {
                    query = query.Where(u => u.Name.StartsWith(filter.Name));
                }

                if (filter.isActive.HasValue)
                {
                    query = query.Where(u => u.IsActive == filter.isActive.Value);
                }

                if (filter.Roles?.Count > 0)
                {
                    query = query.Where(u => filter.Roles.Contains(u.Role.Name));
                }

                return query.ToList();
            }
            catch
            {
                return null;
            }
        }

        bool IUserRepository.Add(User user)
        {
            try
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        User? IUserRepository.Delete(int id)
        {
            try
            {
                var user = _context.Users.Include(u => u.Role).FirstOrDefault(x => x.Id == id);
                if (user == null) return null;
                _context.Users.Remove(user);
                _context.SaveChanges();
                return user;
            }
            catch
            {
                return null;
            }
        }

        public User? Update(int id, User newuser)
        {
            try
            {
                var user = _context.Users.Include(u => u.Role).FirstOrDefault(x => x.Id == id);

                if (user == null)
                    return null;

                if (!string.IsNullOrEmpty(newuser.Name))
                    user.Name = newuser.Name;

                if (!string.IsNullOrEmpty(newuser.Email))
                    user.Email = newuser.Email;

                if (newuser.RoleId != 0)
                    user.RoleId = newuser.RoleId;

                if (newuser.IsActive != user.IsActive)
                    user.IsActive = newuser.IsActive;

                _context.SaveChanges();

                return user;
            }
            catch
            {
                return null;
            }
        }
    }
}
