using Api_Agenda.Models;
using Microsoft.EntityFrameworkCore;

namespace Api_Agenda.Data
{
    public class DataContext : DbContext
    {
       
        //Contexto 
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }


        //Configuracion de los dbsets
        public DbSet<User> user { get; set; }

    }
}
