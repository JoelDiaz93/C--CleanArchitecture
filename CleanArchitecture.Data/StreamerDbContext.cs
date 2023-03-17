using CleanArchitecture.Domain;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Data
{
    public class StreamerDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=JOELPC\SQLEXPRESS;Database=Streamer;Integrated Security=SSPI;TrustServerCertificate=true;")
                .LogTo(Console.WriteLine, new[] {DbLoggerCategory.Database.Command.Name}, Microsoft.Extensions.Logging.LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
            base.OnConfiguring(optionsBuilder);
        }
        //Relacion uno Streamer a varios videos
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Streamer>()
                .HasMany(m => m.Videos)
                .WithOne(m => m.Streamer)
                .HasForeignKey(m => m.StreamerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Video>()
                .HasMany(p => p.Actores)
                .WithMany(t => t.Videos)
                .UsingEntity<VideosActores>(
                    pt => pt.HasKey(e => new { e.ActorId, e.VideoId })
                );
        }
        public DbSet<Streamer>? Streamers { get; set; }

        public DbSet<Video>? Videos { get; set; }

        public DbSet<Actor>? Actores { get; set; }

        public DbSet<Director>? Directores { get; set; }

    }
}
