using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FitnessApp.Models
{
    public partial class FitnessDBContext : DbContext
    {
        public FitnessDBContext()
        {
        }

        public FitnessDBContext(DbContextOptions<FitnessDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<FoodData> FoodData { get; set; }
        public virtual DbSet<RefreshToken> RefreshToken { get; set; }
        public virtual DbSet<SportData> SportData { get; set; }
        public virtual DbSet<UserData> UserData { get; set; }
        public virtual DbSet<UserFood> UserFood { get; set; }
        public virtual DbSet<UserFoodTotal> UserFoodTotal { get; set; }
        public virtual DbSet<UserSleep> UserSleep { get; set; }
        public virtual DbSet<UserSport> UserSport { get; set; }
        public virtual DbSet<UserSportTotal> UserSportTotal { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FoodData>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("PK__FoodData__737584F773C30330");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.TokenId);

                entity.Property(e => e.TokenId).HasColumnName("token_id");

                entity.Property(e => e.Alias)
                    .IsRequired()
                    .HasColumnName("alias")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ExpiryDate)
                    .HasColumnName("expiry_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasColumnName("token")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.AliasNavigation)
                    .WithMany(p => p.RefreshToken)
                    .HasForeignKey(d => d.Alias)
                    .HasConstraintName("FK__RefreshTo__alias__73BA3083");
            });

            modelBuilder.Entity<SportData>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("PK__SportDat__737584F7A466388F");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserData>(entity =>
            {
                entity.HasKey(e => e.Alias)
                    .HasName("PK__UserData__70F4A9E1724F60A4");

                entity.Property(e => e.Alias)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Birthday).HasColumnType("date");

                entity.Property(e => e.Gender)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.Region)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserFood>(entity =>
            {
                entity.HasKey(e => new { e.FoodName, e.UserAlias })
                    .HasName("PK__UserFood__9010C56BCD69ED5A");

                entity.Property(e => e.FoodName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserAlias)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.FoodNameNavigation)
                    .WithMany(p => p.UserFood)
                    .HasForeignKey(d => d.FoodName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserFood_FoodData");

                entity.HasOne(d => d.UserAliasNavigation)
                    .WithMany(p => p.UserFood)
                    .HasForeignKey(d => d.UserAlias)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserFood_UserData");
            });

            modelBuilder.Entity<UserFoodTotal>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.UserAlias)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.UserAliasNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.UserAlias)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserFoodTotal_UserData");
            });

            modelBuilder.Entity<UserSleep>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.UserAlias)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.UserAliasNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.UserAlias)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserSleep_UserData");
            });

            modelBuilder.Entity<UserSport>(entity =>
            {
                entity.HasKey(e => new { e.SportName, e.UserAlias });

                entity.Property(e => e.SportName)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UserAlias)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.SportNameNavigation)
                    .WithMany(p => p.UserSport)
                    .HasForeignKey(d => d.SportName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserSport_SportData");

                entity.HasOne(d => d.UserAliasNavigation)
                    .WithMany(p => p.UserSport)
                    .HasForeignKey(d => d.UserAlias)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserSport_UserData");
            });

            modelBuilder.Entity<UserSportTotal>(entity =>
            {
                entity.HasKey(e => e.UserAlias)
                    .HasName("PK__UserSpor__1A4394F093EAA553");

                entity.Property(e => e.UserAlias)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.UserAliasNavigation)
                    .WithOne(p => p.UserSportTotal)
                    .HasForeignKey<UserSportTotal>(d => d.UserAlias)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserSportTotal_UserData");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
