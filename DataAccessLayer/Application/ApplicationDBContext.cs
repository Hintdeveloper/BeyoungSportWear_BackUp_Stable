using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DataAccessLayer.Entity;
using DataAccessLayer.Configurations;

namespace DataAccessLayer.Application
{
    public class ApplicationDBContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDBContext()
        {
        }
        public ApplicationDBContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Data Source=.;Initial Catalog=BeyoungSportWear;Integrated Security=True;TrustServerCertificate=True"
                );
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            CreateRoles(modelBuilder);
            CreateColor(modelBuilder);
            CreateSizes(modelBuilder);
            CreateUsers(modelBuilder);

            modelBuilder.Entity<IdentityUser>()
             .ToTable("AspNetUsers")
             .HasIndex(u => u.Email)
             .IsUnique();
            modelBuilder.ApplyConfiguration(new AddressConfiguration());
            modelBuilder.ApplyConfiguration(new OptionsConfiguration());
            modelBuilder.ApplyConfiguration(new BrandConfiguration());
            modelBuilder.ApplyConfiguration(new CartConfiguration());
            modelBuilder.ApplyConfiguration(new CartOptionsConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ColorsConfiguration());
            modelBuilder.ApplyConfiguration(new ImagesConfiguration());
            modelBuilder.ApplyConfiguration(new ManufacturerConfiguration());
            modelBuilder.ApplyConfiguration(new MaterialConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderDetailsConfiguration());
            modelBuilder.ApplyConfiguration(new OrderHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new ProductDetailsConfiguration());
            modelBuilder.ApplyConfiguration(new VoucherConfiguration());
            modelBuilder.ApplyConfiguration(new VoucherUserConfiguration());


            base.OnModelCreating(modelBuilder);
        }

        private void CreateRoles(ModelBuilder builder)
        {
            var roles = new List<IdentityRole>
            {
            new IdentityRole() { Id = Convert.ToString(Guid.NewGuid), Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole() { Id = Convert.ToString(Guid.NewGuid),Name = "Client", NormalizedName = "CLIENT" },
            new IdentityRole() { Id = Convert.ToString(Guid.NewGuid),Name = "Staff", NormalizedName = "STAFF" }
            };

        }
     

        private void CreateColor(ModelBuilder builder)
        {
            builder.Entity<Colors>().HasData(
                    new Colors() { ID = Guid.NewGuid(), Name = "White", Description = "", CreateBy = "", CreateDate = DateTime.Now, Status = 1 },
                    new Colors() { ID = Guid.NewGuid(), Name = "Black", Description = "", CreateBy = "", CreateDate = DateTime.Now, Status = 1 },
                    new Colors() { ID = Guid.NewGuid(), Name = "Red", Description = "", CreateBy = "", CreateDate = DateTime.Now, Status = 1 },
                    new Colors() { ID = Guid.NewGuid(), Name = "Blue", Description = "", CreateBy = "", CreateDate = DateTime.Now, Status = 1 },
                    new Colors() { ID = Guid.NewGuid(), Name = "Green", Description = "", CreateBy = "", CreateDate = DateTime.Now, Status = 1 }
                );
        }
        private void CreateSizes(ModelBuilder builder)
        {
            builder.Entity<Sizes>().HasData(
                    new Sizes() { ID = Guid.NewGuid(), Name = "XS", Description = "", CreateBy = "", CreateDate = DateTime.Now, Status = 1 },
                    new Sizes() { ID = Guid.NewGuid(), Name = "S", Description = "", CreateBy = "", CreateDate = DateTime.Now, Status = 1 },
                    new Sizes() { ID = Guid.NewGuid(), Name = "M", Description = "", CreateBy = "", CreateDate = DateTime.Now, Status = 1 },
                    new Sizes() { ID = Guid.NewGuid(), Name = "L", Description = "", CreateBy = "", CreateDate = DateTime.Now, Status = 1 },
                    new Sizes() { ID = Guid.NewGuid(), Name = "XL", Description = "", CreateBy = "", CreateDate = DateTime.Now, Status = 1 }
                );
        }
        private void CreateUsers(ModelBuilder builder)
        {
            var adminUser = new IdentityUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                EmailConfirmed = true
            };

            adminUser.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(adminUser, "Admin@123");

            builder.Entity<IdentityUser>().HasData(adminUser);
        }
        public virtual DbSet<ApplicationUser> ApplicationUser { get; set; } = null!;
        public virtual DbSet<Brand> Brand { get; set; } 
        public virtual DbSet<Options> Options { get; set; } = null!;
        public virtual DbSet<Cart> Cart { get; set; } = null!;
        public virtual DbSet<Address> Address { get; set; } = null!;
        public virtual DbSet<CartOptions> CartOptions { get; set; } = null!;
        public virtual DbSet<Category> Category { get; set; } = null!;
        public virtual DbSet<Colors> Colors { get; set; } 
        public virtual DbSet<Images> Images { get; set; } = null!;
        public virtual DbSet<Manufacturer> Manufacturer { get; set; } 
        public virtual DbSet<Material> Material { get; set; } 
        public virtual DbSet<Order> Order { get; set; } = null!;
        public virtual DbSet<OrderDetails> OrderDetails { get; set; } = null!;
        public virtual DbSet<OrderHistory> OrderHistory { get; set; } = null!;
        public virtual DbSet<Product> Product { get; set; } = null!;
        public virtual DbSet<ProductDetails> ProductDetails { get; set; } = null!;
        public virtual DbSet<Sizes> Sizes { get; set; } 
        public virtual DbSet<Voucher> Voucher { get; set; } = null!;
        public virtual DbSet<VoucherUser> VoucherUser { get; set; } = null!;
    }
}
