using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Attribute = Domain.Entities.Attribute;

#nullable disable

namespace EfCore.Persistence.Contexts
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> context)
            : base(context)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=DefaultConnection");
            }
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<TokenRefresh> TokenRefreshes { get; set; }
        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<Attribute> Attributes { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<EmailConfiguration> EmailConfigurations { get; set; }
        public virtual DbSet<EmailRegistration> EmailRegistrations { get; set; }
        public virtual DbSet<EmailSignUp> EmailSignUps { get; set; }
        public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }
        public virtual DbSet<Gallery> Galleries { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductAttribute> ProductAttributes { get; set; }
        public virtual DbSet<ProductImage> ProductImages { get; set; }
        public virtual DbSet<ProductRelated> ProductRelateds { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Website> Websites { get; set; }
        public virtual DbSet<UserVerification> UserVerifications { get; set; }
        public virtual DbSet<ProductDisplay> ProductDisplays { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<UserClaim> UserClaims { get; set; }
        public virtual DbSet<UserDeviceToken> UserDeviceTokens { get; set; }
        public virtual DbSet<UserNotification> UserNotifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");
                entity.Property(f => f.Id)
                    .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role");
                entity.Property(f => f.Id)
                    .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.ToTable("UserRole");
                entity.HasKey(c => new { c.RoleId, c.UserId });
                entity.HasOne(p => p.User);
                entity.HasOne(p => p.Role);
            });

            modelBuilder.Entity<UserClaim>(entity =>
            {
                entity.ToTable("UserClaim");
                entity.HasKey(c => c.Id);
                entity.Property(f => f.Id)
                    .ValueGeneratedOnAdd();

                entity.HasOne(p => p.User);
            });

            modelBuilder.Entity<ProductDisplay>(entity =>
            {
                entity.ToTable("ProductDisplays");
            });

            modelBuilder.Entity<TokenRefresh>(entity =>
            {
                entity.ToTable("TokenRefresh");
                entity.Property(f => f.Id)
                    .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Article>()
                .ToTable("Article")
                .HasOne(b => b.Menu)
                .WithMany(p => p.Articles)
                .HasForeignKey(p => p.MenuId)
                .HasPrincipalKey(p => p.Id);

            modelBuilder.Entity<Attribute>()
                .ToTable("Attribute")
                .HasMany(b => b.ProductAttributes);

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customers");
                entity.Property(f => f.Id)
                    .ValueGeneratedOnAdd();

                entity.HasIndex(x => x.Code).IsUnique();

                entity.HasMany(p => p.Orders)
                    .WithOne(p => p.Customer);
            });

            modelBuilder.Entity<EmailConfiguration>(entity =>
            {
                entity.ToTable("EmailConfiguration");
            });

            modelBuilder.Entity<EmailRegistration>(entity =>
            {
                entity.ToTable("EmailRegistration");
            });
            modelBuilder.Entity<EmailSignUp>(entity =>
            {
                entity.ToTable("EmailSignUp");
            });

            modelBuilder.Entity<EmailTemplate>(entity =>
            {
                entity.ToTable("EmailTemplate");
            });

            modelBuilder.Entity<Gallery>(entity =>
            {
                entity.ToTable("Gallery");
                entity.Property(f => f.Id)
                    .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.ToTable("Menu");
                entity.Property(f => f.Id)
                    .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(f => f.Id)
                    .ValueGeneratedOnAdd();

                entity.HasIndex(x => x.Code).IsUnique();

                entity.HasOne(p => p.Customer)
                    .WithMany(b => b.Orders)
                    .HasForeignKey(p => p.CustomerId);

                entity.HasMany(b => b.OrderDetails)
                    .WithOne(p => p.Order)
                    .HasForeignKey(p => p.OrderId);
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.ToTable("OrderDetail");

                entity.Property(f => f.Id)
                    .ValueGeneratedOnAdd();

                entity.HasOne(p => p.Order)
                    .WithMany(b => b.OrderDetails)
                    .HasForeignKey(p => p.OrderId);
            });

            modelBuilder.Entity<ProductImage>()
                .ToTable("ProductImage");

            modelBuilder.Entity<ProductRelated>()
                .ToTable("ProductRelated");

            modelBuilder.Entity<ProductRelated>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Review>()
                .ToTable("Review");

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.HasIndex(x => x.Code).IsUnique();
                entity.HasIndex(x => x.Alias).IsUnique();

                entity.HasOne(p => p.Menu)
                    .WithMany(b => b.Products)
                    .HasForeignKey(p => p.MenuId);

                entity.HasMany(b => b.ProductImages)
                    .WithOne(p => p.Product)
                    .HasForeignKey(p => p.ProductId);

                entity.HasMany(b => b.ProductRelateds)
                    .WithOne(p => p.Product)
                    .HasForeignKey(p => p.ProductId)
                    .HasPrincipalKey(p => p.Id);

                entity.HasMany(b => b.Reviews)
                    .WithOne(p => p.Product)
                    .HasForeignKey(p => p.ProductId)
                    .HasPrincipalKey(p => p.Id);

                entity.HasMany(p => p.Attributes)
                    .WithMany(p => p.Products)
                    .UsingEntity<ProductAttribute>(
                        j => j
                            .HasOne(pt => pt.Attribute)
                            .WithMany(t => t.ProductAttributes)
                            .HasForeignKey(pt => pt.AttributeId),
                        j => j
                            .HasOne(pt => pt.Product)
                            .WithMany(p => p.ProductAttributes)
                            .HasForeignKey(pt => pt.ProductId),
                        j =>
                        {
                            j.HasKey(t => new { t.ProductId, t.AttributeId });
                        });
            });

            modelBuilder.Entity<ProductAttribute>(entity =>
            {
                entity.ToTable("ProductAttribute");
                entity.HasKey(t => new { t.Id });

                entity.Property(f => f.Id)
                    .ValueGeneratedOnAdd();

                entity.HasOne(pt => pt.Product)
                    .WithMany(p => p.ProductAttributes)
                    .HasForeignKey(pt => pt.ProductId);

                entity.HasOne(pt => pt.Attribute)
                    .WithMany(t => t.ProductAttributes)
                    .HasForeignKey(pt => pt.AttributeId);
            });

            modelBuilder.Entity<Website>()
                .ToTable("Website");

            modelBuilder.Entity<UserNotification>(entity =>
            {
                entity.ToTable("UserNotification");
                entity.Property(f => f.Id)
                    .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<UserDeviceToken>(entity =>
            {
                entity.ToTable("UserDeviceToken");
                entity.Property(f => f.Id)
                    .ValueGeneratedOnAdd();
            });
        }
    }
}