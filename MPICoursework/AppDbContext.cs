using Microsoft.EntityFrameworkCore;
using MPICoursework.GenerateDb.GenerateEntities;

namespace MPICoursework
{
    class AppDbContext : DbContext
    {
        // Таблица заявок
        public DbSet<Album> Albums { get; set; }
        // Таблица менеджеров
        public DbSet<Artist> Artists { get; set; }
        // Таблица статусов
        public DbSet<Playlist> Playlists { get; set; }
        // Таблица пользователей
        public DbSet<Track> Tracks { get; set; }

        public AppDbContext()
        {
            // Проверка базы данных на существование
            Database.EnsureCreated();
        }
        // Fluent API
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    // Отключение автоматического заполнения Id у таблицы Status
        //    modelBuilder.Entity<Status>().Property(e => e.Id).ValueGeneratedNever();
        //}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Строка подключения к базе данных
            optionsBuilder.UseSqlServer("Server=(localdb)\\localDB;Database=AmirKrutoi;Trusted_Connection=True;");
        } 
    }
}
