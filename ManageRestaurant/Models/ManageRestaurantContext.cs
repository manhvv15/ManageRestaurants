using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ManageRestaurant.Models
{
    public partial class ManageRestaurantContext : DbContext
    {
        public ManageRestaurantContext()
        {
        }

        public ManageRestaurantContext(DbContextOptions<ManageRestaurantContext> options)
            : base(options)
        {
        }

        public virtual DbSet<BookingRequest> BookingRequests { get; set; } = null!;
        public virtual DbSet<Menu> Menus { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<Table> Tables { get; set; } = null!;
        public virtual DbSet<Transaction> Transactions { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .Build();
            var strConn = config["ConnectionStrings:MyDatabase"];
            optionsBuilder.UseSqlServer(strConn);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookingRequest>(entity =>
            {
                entity.HasKey(e => e.BookingId)
                    .HasName("PK__BookingR__5DE3A5B103D8C0BD");

                entity.ToTable("BookingRequest");

                entity.Property(e => e.BookingId).HasColumnName("booking_id");

                entity.Property(e => e.Note)
                    .HasMaxLength(100)
                    .HasColumnName("note");

                entity.Property(e => e.NumberOfGuests).HasColumnName("number_of_guests");

                entity.Property(e => e.ReservationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("reservation_date");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");

                entity.Property(e => e.TableId).HasColumnName("table_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Table)
                    .WithMany(p => p.BookingRequests)
                    .HasForeignKey(d => d.TableId)
                    .HasConstraintName("FK__BookingRe__table__3E52440B");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.BookingRequests)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__BookingRe__user___3D5E1FD2");
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.Property(e => e.MenuId).HasColumnName("menu_id");

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(255)
                    .HasColumnName("image_url");

                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("price");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.CheckoutTime)
                    .HasColumnType("datetime")
                    .HasColumnName("checkout_time");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("createdAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreatedBy).HasColumnName("createdBy");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deletedAt");

                entity.Property(e => e.DeletedBy)
                    .HasMaxLength(100)
                    .HasColumnName("deletedBy");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");

                entity.Property(e => e.TableId).HasColumnName("table_id");

                entity.HasOne(d => d.CreatedByNavigation)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CreatedBy)
                    .HasConstraintName("FK__Orders__createdB__4316F928");

                entity.HasOne(d => d.Table)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.TableId)
                    .HasConstraintName("FK__Orders__table_id__4222D4EF");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("OrderDetail");

                entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetail_id");

                entity.Property(e => e.MenuId).HasColumnName("menu_id");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("price");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.TotalPrice)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("totalPrice");

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.MenuId)
                    .HasConstraintName("FK__OrderDeta__menu___48CFD27E");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__OrderDeta__order__47DBAE45");
            });

            modelBuilder.Entity<Table>(entity =>
            {
                entity.Property(e => e.TableId).HasColumnName("table_id");

                entity.Property(e => e.Capacity).HasColumnName("capacity");

                entity.Property(e => e.Description)
                    .HasMaxLength(1000)
                    .HasColumnName("description");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");

                entity.Property(e => e.TableNumber).HasColumnName("table_number");
            });

            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(e => e.TransactionId).HasColumnName("transaction_id");

                entity.Property(e => e.Description)
                    .HasMaxLength(1000)
                    .HasColumnName("description");

                entity.Property(e => e.OrderId).HasColumnName("order_id");

                entity.Property(e => e.PaymentDate)
                    .HasColumnType("datetime")
                    .HasColumnName("payment_date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.PaymentMethod)
                    .HasMaxLength(50)
                    .HasColumnName("payment_method");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("price");

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasColumnName("status");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Transactions)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__Transacti__order__4BAC3F29");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email, "UQ__Users__AB6E616450CD563E")
                    .IsUnique();

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.Balance).HasColumnName("balance");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("createdAt")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .HasColumnName("createdBy");

                entity.Property(e => e.DeletedAt)
                    .HasColumnType("datetime")
                    .HasColumnName("deletedAt");

                entity.Property(e => e.DeletedBy)
                    .HasMaxLength(100)
                    .HasColumnName("deletedBy");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .HasColumnName("password");

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .HasColumnName("phone");

                entity.Property(e => e.Role)
                    .HasMaxLength(50)
                    .HasColumnName("role");

                entity.Property(e => e.UpdateAt)
                    .HasColumnType("datetime")
                    .HasColumnName("updateAt");

                entity.Property(e => e.UpdateBy)
                    .HasMaxLength(100)
                    .HasColumnName("updateBy");

                entity.Property(e => e.UserName)
                    .HasMaxLength(100)
                    .HasColumnName("userName");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
