using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DemoTest.Models;

public partial class TrenirovkaContext : DbContext
{
    public TrenirovkaContext()
    {
    }

    public TrenirovkaContext(DbContextOptions<TrenirovkaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Manufacturer> Manufacturers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<PickUpPoint> PickUpPoints { get; set; }

    public virtual DbSet<Provider> Providers { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Tovar> Tovars { get; set; }

    public virtual DbSet<TovarInOrder> TovarInOrders { get; set; }

    public virtual DbSet<TovarType> TovarTypes { get; set; }

    public virtual DbSet<Unit> Units { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5438;Database=trenirovka;Username=nastya;Password=123");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("pk_category");

            entity.ToTable("category");

            entity.Property(e => e.CategoryId)
                .ValueGeneratedNever()
                .HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .HasColumnType("character varying")
                .HasColumnName("category_name");
        });

        modelBuilder.Entity<Manufacturer>(entity =>
        {
            entity.HasKey(e => e.ManufacturerId).HasName("pk_manufacturer");

            entity.ToTable("manufacturer");

            entity.Property(e => e.ManufacturerId)
                .ValueGeneratedNever()
                .HasColumnName("manufacturer_id");
            entity.Property(e => e.ManufacturerName)
                .HasColumnType("character varying")
                .HasColumnName("manufacturer_name");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Orderid).HasName("order_pkey");

            entity.ToTable("order");

            entity.Property(e => e.Orderid).HasColumnName("orderid");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.DateDeliv).HasColumnName("date_deliv");
            entity.Property(e => e.DateOrder).HasColumnName("date_order");
            entity.Property(e => e.FullName).HasColumnName("full_name");
            entity.Property(e => e.OrderStatusId).HasColumnName("order_status_id");
            entity.Property(e => e.PickUpPointId).HasColumnName("pick_up_point_id");

            entity.HasOne(d => d.FullNameNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.FullName)
                .HasConstraintName("order_full_name_fkey");

            entity.HasOne(d => d.OrderStatus).WithMany(p => p.Orders)
                .HasForeignKey(d => d.OrderStatusId)
                .HasConstraintName("order_order_status_id_fkey");

            entity.HasOne(d => d.PickUpPoint).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PickUpPointId)
                .HasConstraintName("order_pick_up_point_id_fkey");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.OrderStatusId).HasName("order_status_pkey");

            entity.ToTable("order_status");

            entity.Property(e => e.OrderStatusId)
                .ValueGeneratedNever()
                .HasColumnName("order_status_id");
            entity.Property(e => e.OrderStatusName)
                .HasColumnType("character varying")
                .HasColumnName("order_status_name");
        });

        modelBuilder.Entity<PickUpPoint>(entity =>
        {
            entity.HasKey(e => e.PickUpPointId).HasName("pick_up_point_pkey");

            entity.ToTable("pick_up_point");

            entity.Property(e => e.PickUpPointId).HasColumnName("pick_up_point_id");
            entity.Property(e => e.PickUpPointName)
                .HasColumnType("character varying")
                .HasColumnName("pick_up_point_name");
        });

        modelBuilder.Entity<Provider>(entity =>
        {
            entity.HasKey(e => e.ProviderId).HasName("pk_provider");

            entity.ToTable("provider");

            entity.Property(e => e.ProviderId)
                .ValueGeneratedNever()
                .HasColumnName("provider_id");
            entity.Property(e => e.ProviderName)
                .HasColumnType("character varying")
                .HasColumnName("provider_name");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("pk_role");

            entity.ToTable("role");

            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasColumnType("character varying")
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<Tovar>(entity =>
        {
            entity.HasKey(e => e.TovarId).HasName("tovar_pkey");

            entity.ToTable("tovar");

            entity.Property(e => e.TovarId).HasColumnName("tovar_id");
            entity.Property(e => e.Art)
                .HasColumnType("character varying")
                .HasColumnName("art");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.DiscountNow).HasColumnName("discount_now");
            entity.Property(e => e.ManufacturerId).HasColumnName("manufacturer_id");
            entity.Property(e => e.Photo)
                .HasColumnType("character varying")
                .HasColumnName("photo");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.ProviderId).HasColumnName("provider_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.TovarTypeId).HasColumnName("tovar_type_id");
            entity.Property(e => e.Unit).HasColumnName("unit");

            entity.HasOne(d => d.Category).WithMany(p => p.Tovars)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("tovar_category_id_fkey");

            entity.HasOne(d => d.Manufacturer).WithMany(p => p.Tovars)
                .HasForeignKey(d => d.ManufacturerId)
                .HasConstraintName("tovar_manufacturer_id_fkey");

            entity.HasOne(d => d.Provider).WithMany(p => p.Tovars)
                .HasForeignKey(d => d.ProviderId)
                .HasConstraintName("tovar_provider_id_fkey");

            entity.HasOne(d => d.TovarType).WithMany(p => p.Tovars)
                .HasForeignKey(d => d.TovarTypeId)
                .HasConstraintName("tovar_tovar_type_id_fkey");

            entity.HasOne(d => d.UnitNavigation).WithMany(p => p.Tovars)
                .HasForeignKey(d => d.Unit)
                .HasConstraintName("tovar_unit_fkey");
        });

        modelBuilder.Entity<TovarInOrder>(entity =>
        {
            entity.HasKey(e => e.TovarInOrderId).HasName("tovar_in_order_pkey");

            entity.ToTable("tovar_in_order");

            entity.Property(e => e.TovarInOrderId).HasColumnName("tovar_in_order_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.TovarId).HasColumnName("tovar_id");

            entity.HasOne(d => d.Order).WithMany(p => p.TovarInOrders)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("tovar_in_order_order_id_fkey");

            entity.HasOne(d => d.Tovar).WithMany(p => p.TovarInOrders)
                .HasForeignKey(d => d.TovarId)
                .HasConstraintName("tovar_in_order_tovar_id_fkey");
        });

        modelBuilder.Entity<TovarType>(entity =>
        {
            entity.HasKey(e => e.TovarTypeId).HasName("tovar_type_pkey");

            entity.ToTable("tovar_type");

            entity.Property(e => e.TovarTypeId)
                .ValueGeneratedNever()
                .HasColumnName("tovar_type_id");
            entity.Property(e => e.TovarTypeName)
                .HasColumnType("character varying")
                .HasColumnName("tovar_type_name");
        });

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.HasKey(e => e.UnitId).HasName("unit_pkey");

            entity.ToTable("unit");

            entity.Property(e => e.UnitId)
                .ValueGeneratedNever()
                .HasColumnName("unit_id");
            entity.Property(e => e.UnitName)
                .HasColumnType("character varying")
                .HasColumnName("unit_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("user_pkey");

            entity.ToTable("user");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.FullName)
                .HasColumnType("character varying")
                .HasColumnName("full_name");
            entity.Property(e => e.Login)
                .HasColumnType("character varying")
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasColumnType("character varying")
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("user_role_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
