using DesafioDesktop.Models;
using Microsoft.EntityFrameworkCore;

namespace DesafioDesktop.Data
{
    public class DesafioHiperContext : DbContext
    {
        private const string ConnectionString = "Data Source= .\\SQLEXPRESS;Initial Catalog=DesafioDb;Integrated Security=true";


        public DbSet<Produto> Produtos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(ConnectionString);
        }
    }
}
