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

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Damage> Damages { get; set; }

    public virtual DbSet<InventoryAction> InventoryActions { get; set; }

    public virtual DbSet<InventoryBatch> InventoryBatches { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Return> Returns { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAuditLog> UserAuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC07DE3A77DD");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customer__3214EC07CB3DEDB2");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Damage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Damages__3214EC0713991079");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Damages).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<InventoryAction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Inventor__3214EC07148B1D43");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.InventoryActions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Creat__1CBC4616");

            entity.HasOne(d => d.InventoryBatch).WithMany(p => p.InventoryActions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Inventory__Inven__1BC821DD");
        });

        modelBuilder.Entity<InventoryBatch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Inventor__3214EC079B02ED2D");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Product).WithMany(p => p.InventoryBatches)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InventoryBatches_Product");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Products__3214EC07F9F2BC5C");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsPurchasable).HasDefaultValue(true);
            entity.Property(e => e.IsSellable).HasDefaultValue(true);

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Category");

            entity.HasOne(d => d.Unit).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Unit");
        });

        modelBuilder.Entity<Return>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Returns__3214EC07B0713014");

            entity.Property(e => e.ReturnDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Returns)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Returns_Users");

            entity.HasOne(d => d.Customer).WithMany(p => p.Returns)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Returns_Customers");

            entity.HasOne(d => d.Sale).WithMany(p => p.Returns)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Returns_Sales");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07F06666D7");
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Sales__3214EC072472C1EC");

            entity.Property(e => e.SaleDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Sales)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sales__CreatedBy__17F790F9");

            entity.HasOne(d => d.Customer).WithMany(p => p.Sales).HasConstraintName("FK__Sales__CustomerI__17036CC0");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Units__3214EC07BFAFBE72");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
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
