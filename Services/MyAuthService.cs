using Api_Agenda.Data;
using Api_Agenda.Models;
using Microsoft.EntityFrameworkCore;

namespace Api_Agenda.Services
{
    public interface IAuthService
    {
        Task SignUp(User user);
        User GetUserForSignIn(string name);
    }

    public class MyAuthServices: IAuthService
    {
        readonly DbSet<User> _dbSet;
        readonly DataContext _context;

        public MyAuthServices(DataContext context)
        {
            _context = context;
            _dbSet = _context.Set<User>();
        }
        public async Task SignUp(User user)
        {
            _dbSet.Add(user);
            await _context.SaveChangesAsync();
        }


        public User GetUserForSignIn(string name)
        {
            var userResult = _context.user.Where(x => x.Name == name).FirstOrDefault();
            return userResult;
        }


    }

}




