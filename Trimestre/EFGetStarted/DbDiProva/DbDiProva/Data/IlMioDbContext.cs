using DbDiProva.Model;
using Microsoft.EntityFrameworkCore;

namespace DbDiProva.Data
{
    public class IlMioDbContext : DbContext
    {
        public DbSet<Studente> studenti { get; set; } = null!;
        public DbSet<Scuola> scuole  { get; set; } = null!;
        public DbSet<Sport> sport { get; set; } = null!;
        public DbSet<Videogioco> videogiochi { get; set; } = null!;
        public string DbPath { get; }

        public IlMioDbContext()
        {
            var appDir = AppContext.BaseDirectory;
            var path = Path.Combine(appDir, "../../../IlMioDb.db");
            DbPath = path;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source = {DbPath}");
        }
}
}
