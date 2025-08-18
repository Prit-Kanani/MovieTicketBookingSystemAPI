using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Movie_Management_API.Models;

public partial class MovieManagementContext : DbContext
{
    public MovieManagementContext()
    {
    }

    public MovieManagementContext(DbContextOptions<MovieManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }
    public virtual DbSet<Genre> Genres { get; set; }
    public virtual DbSet<Movie> Movies { get; set; }
    public virtual DbSet<Screen> Screens { get; set; }
    public virtual DbSet<SeatsBooked> SeatsBookeds { get; set; }
    public virtual DbSet<ShowTime> ShowTimes { get; set; }
    public virtual DbSet<Theatre> Theatres { get; set; }
    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("server=LAPTOP-9S570SUD\\SQLEXPRESS; database=MovieManagement; trusted_connection=true; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // BOOKINGS
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Bookings__73951ACD2D60E46C");

            entity.Property(e => e.BookingId).HasColumnName("BookingID");
            entity.Property(e => e.BookingType).HasMaxLength(10);
            entity.Property(e => e.DateTime).HasDefaultValueSql("(getdate())").HasColumnType("datetime");
            entity.Property(e => e.PaymentStatus).HasMaxLength(20);
            entity.Property(e => e.ShowId).HasColumnName("ShowID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Show).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ShowId)
                .OnDelete(DeleteBehavior.Cascade) // deleting a Show cascades to its Bookings
                .HasConstraintName("FK__Bookings__ShowID__5CD6CB2B");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade) // deleting a user will delete bookings
                .HasConstraintName("FK__Bookings__UserID__5BE2A6F2");
        });

        // GENRES
        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.GenreId);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.GenreId).HasColumnName("GenreID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        // MOVIES
        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.MovieId);
            entity.Property(e => e.MovieId).HasColumnName("MovieID");
            entity.Property(e => e.Language).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Poster).HasMaxLength(255);

            entity.HasMany(d => d.Genres).WithMany(p => p.Movies)
                .UsingEntity<Dictionary<string, object>>(
                    "MovieGenre",
                    r => r.HasOne<Genre>().WithMany()
                          .HasForeignKey("GenreId")
                          .OnDelete(DeleteBehavior.Cascade),
                    l => l.HasOne<Movie>().WithMany()
                          .HasForeignKey("MovieId")
                          .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("MovieId", "GenreId");
                        j.ToTable("MovieGenres");
                        j.IndexerProperty<int>("MovieId").HasColumnName("MovieID");
                        j.IndexerProperty<int>("GenreId").HasColumnName("GenreID");
                    });
        });

        // SCREENS
        modelBuilder.Entity<Screen>(entity =>
        {
            entity.HasKey(e => e.ScreenId);
            entity.Property(e => e.ScreenId).HasColumnName("ScreenID");
            entity.Property(e => e.TheatreId).HasColumnName("TheatreID");

            entity.HasOne(d => d.Theatre).WithMany(p => p.Screens)
                .HasForeignKey(d => d.TheatreId)
                .OnDelete(DeleteBehavior.Cascade) // deleting a Theatre deletes its Screens
                .HasConstraintName("FK__Screens__Theatre__52593CB8");
        });

        // SEATS BOOKED
        modelBuilder.Entity<SeatsBooked>(entity =>
        {
            entity.HasKey(e => new { e.BookingId, e.SeatNo });
            entity.ToTable("SeatsBooked");
            entity.Property(e => e.BookingId).HasColumnName("BookingID");
            entity.Property(e => e.SeatNo).HasMaxLength(10);

            entity.HasOne(d => d.Booking).WithMany(p => p.SeatsBookeds)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.Cascade); // deleting Booking deletes SeatsBooked
        });

        // SHOWTIMES
        modelBuilder.Entity<ShowTime>(entity =>
        {
            entity.HasKey(e => e.ShowId);
            entity.Property(e => e.ShowId).HasColumnName("ShowID");
            entity.Property(e => e.MovieId).HasColumnName("MovieID");
            entity.Property(e => e.ScreenId).HasColumnName("ScreenID");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Movie).WithMany(p => p.ShowTimes)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.Cascade); // deleting Movie deletes ShowTimes

            entity.HasOne(d => d.Screen).WithMany(p => p.ShowTimes)
                .HasForeignKey(d => d.ScreenId)
                .OnDelete(DeleteBehavior.Cascade); // deleting Screen deletes ShowTimes
        });

        // THEATRES
        modelBuilder.Entity<Theatre>(entity =>
        {
            entity.HasKey(e => e.TheatreId);
            entity.Property(e => e.TheatreId).HasColumnName("TheatreID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Theatres)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade); // deleting user will delete Theatre
        });

        // USERS
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Role).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
