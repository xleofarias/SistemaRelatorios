using Microsoft.EntityFrameworkCore;
using RelatoriosWorker.Models;

namespace RelatoriosWorker.Data
{
    public class WorkerDbContext(DbContextOptions<WorkerDbContext> options) : DbContext(options)
    {
        public DbSet<Relatorio> Relatorios { get; set; }
    }
}
