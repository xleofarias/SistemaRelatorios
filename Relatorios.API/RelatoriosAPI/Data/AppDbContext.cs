using Microsoft.EntityFrameworkCore;
using RelatoriosAPI.Models;

namespace RelatoriosAPI.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options): DbContext(options)
    {
        public DbSet<Relatorio> Relatorios { get; set; }
    }
}
