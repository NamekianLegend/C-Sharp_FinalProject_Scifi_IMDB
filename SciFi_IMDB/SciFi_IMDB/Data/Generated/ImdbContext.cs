using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SciFi_IMDB;

public partial class ImdbContext : DbContext
{
    public ImdbContext(DbContextOptions<ImdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Name> Names { get; set; }

    public virtual DbSet<Principal> Principals { get; set; }

    public virtual DbSet<Rating> Ratings { get; set; }

    public virtual DbSet<Title> Titles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.GenreId).HasName("PK_Title_Genres");

            entity.Property(e => e.GenreId).HasColumnName("GenreID");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Name>(entity =>
        {
            entity.Property(e => e.NameId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("nameID");
            entity.Property(e => e.BirthYear).HasColumnName("birthYear");
            entity.Property(e => e.DeathYear).HasColumnName("deathYear");
            entity.Property(e => e.PrimaryName)
                .HasMaxLength(125)
                .IsUnicode(false)
                .HasColumnName("primaryName");
            entity.Property(e => e.PrimaryProfession)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("primaryProfession");
        });

        modelBuilder.Entity<Principal>(entity =>
        {
            entity.HasKey(e => new { e.TitleId, e.Ordering });

            entity.Property(e => e.TitleId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("titleID");
            entity.Property(e => e.Ordering).HasColumnName("ordering");
            entity.Property(e => e.Characters)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("characters");
            entity.Property(e => e.Job)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("job");
            entity.Property(e => e.JobCategory)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("job_category");
            entity.Property(e => e.NameId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("nameID");

            entity.HasOne(d => d.Name).WithMany(p => p.Principals)
                .HasForeignKey(d => d.NameId)
                .HasConstraintName("FK_Principals_Names");
        });

        modelBuilder.Entity<Rating>(entity =>
        {
            entity.HasKey(e => e.TitleId);

            entity.Property(e => e.TitleId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("titleID");
            entity.Property(e => e.AverageRating)
                .HasColumnType("numeric(4, 2)")
                .HasColumnName("averageRating");
            entity.Property(e => e.NumVotes).HasColumnName("numVotes");

            entity.HasOne(d => d.Title).WithOne(p => p.Rating)
                .HasForeignKey<Rating>(d => d.TitleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Ratings_Titles");
        });

        modelBuilder.Entity<Title>(entity =>
        {
            entity.Property(e => e.TitleId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("titleID");
            entity.Property(e => e.EndYear).HasColumnName("endYear");
            entity.Property(e => e.IsAdult).HasColumnName("isAdult");
            entity.Property(e => e.OriginalTitle)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("originalTitle");
            entity.Property(e => e.PrimaryTitle)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("primaryTitle");
            entity.Property(e => e.RuntimeMinutes).HasColumnName("runtimeMinutes");
            entity.Property(e => e.StartYear).HasColumnName("startYear");
            entity.Property(e => e.TitleType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("titleType");

            entity.HasMany(d => d.Genres).WithMany(p => p.Titles)
                .UsingEntity<Dictionary<string, object>>(
                    "TitleGenre",
                    r => r.HasOne<Genre>().WithMany()
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Title_Genres_Titles"),
                    l => l.HasOne<Title>().WithMany()
                        .HasForeignKey("TitleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Title_Genres_Titles1"),
                    j =>
                    {
                        j.HasKey("TitleId", "GenreId").HasName("PK_Title_Genres_1");
                        j.ToTable("Title_Genres");
                        j.IndexerProperty<string>("TitleId")
                            .HasMaxLength(10)
                            .IsUnicode(false)
                            .HasColumnName("titleID");
                        j.IndexerProperty<int>("GenreId").HasColumnName("genreID");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
