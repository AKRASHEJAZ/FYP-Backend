using System;
using System.Collections.Generic;
using Data_Layer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data_Layer.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAuditLog> UserAuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07F06666D7");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC07B8BB44B8");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        modelBuilder.Entity<UserAuditLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserAudi__3214EC077EFDA346");

            entity.Property(e => e.PerformedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.PerformedByNavigation).WithMany(p => p.UserAuditLogPerformedByNavigations).HasConstraintName("FK_UserAuditLogs_PerformedBy");

            entity.HasOne(d => d.User).WithMany(p => p.UserAuditLogUsers).HasConstraintName("FK_UserAuditLogs_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
