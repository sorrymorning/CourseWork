using Microsoft.EntityFrameworkCore;
using MPICoursework.GenerateDb.GenerateEntities;

namespace MPICoursework
{
    class AppDbContext : DbContext
    {
        // Таблица заявок
        public DbSet<Movie> Movies { get; set; }
        // Таблица менеджеров
        public DbSet<Director> Directors { get; set; }
        // Таблица статусов
        public DbSet<Genre> Genres { get; set; }
        // Таблица пользователей
        public DbSet<Actor> Actors { get; set; }

        public DbSet<MovieActor> MovieActors { get; set; }

        public AppDbContext()
        {
            // Проверка базы данных на существование
            Database.EnsureCreated();
        }
        // Fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Отключение автоматического заполнения Id у таблицы Status
            modelBuilder.Entity<Genre>().Property(e => e.Id).ValueGeneratedNever();
            base.OnModelCreating(modelBuilder);

            // Установка составного первичного ключа для MovieActor
            modelBuilder.Entity<MovieActor>()
                .HasKey(ma => new { ma.MovieId, ma.ActorId });

            // Настройка связей
            modelBuilder.Entity<MovieActor>()
                .HasOne(ma => ma.Movie)
                .WithMany(m => m.MovieActors)
                .HasForeignKey(ma => ma.MovieId);

            modelBuilder.Entity<MovieActor>()
                .HasOne(ma => ma.Actor)
                .WithMany(a => a.MovieActors)
                .HasForeignKey(ma => ma.ActorId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Строка подключения к базе данных
            optionsBuilder.UseSqlServer("Server=(localdb)\\localDB;Database=AmirSuperKrutoi;Trusted_Connection=True;");
        } 
    }
}
