using Microsoft.EntityFrameworkCore;
using WebApiINMO.Entities;

namespace WebApiINMO
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext( DbContextOptions options ): base( options )
        {
               
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PropertyAmenity>().HasKey(pa => new { pa.PropertyId, pa.AmenityId });
        }

        // Crear una tabla a partir del schema que creamos en nuestra clase
        public DbSet<Property> Properties { get; set; }
        public DbSet<Adviser> Advisers { get; set; }
        public DbSet<Amenity> Amenities { get; set; }

        public DbSet<PropertyAmenity> PropertiesAmenities { get; set; }



    }
}
