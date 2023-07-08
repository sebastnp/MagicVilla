using MagicVilla_API.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Datos
{
    public class AplicationDbContext :DbContext // para que herede 
    {

        // aplicando inyeccion de dependencia
        public AplicationDbContext(DbContextOptions<AplicationDbContext> options) :base(options) 
        {
            
        }

        // aqui pondremos los modelos que queremos
        // que se creen como una tabla en la db

        // este modelo de tipo Villa se creara como una tabla en la bd
        public DbSet<Villa> Villas { get; set; }

        // Sobrescribimos un metodo que viene en DbContext para 
        // crear ya datos cuando se cree la base de datos
        // ALIMENTADOR
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Villa>().HasData(
                new Villa()
                {
                    Id = 1,
                    Nombre = "Villa Real",
                    Detalle = "Detalle de la villa...",
                    ImagenUrl = "",
                    Ocupantes = 5,
                    Tarifa = 200,
                    MetrosCuadrados = 50,
                    Amenidad = "",
                    FechaCreacion = DateTime.Now,
                    FechaActualizacion = DateTime.Now,
                },
                new Villa()
                {
                    Id = 2,
                    Nombre = "Villa Almar",
                    Detalle = "Detalle de la villa...",
                    ImagenUrl = "",
                    Ocupantes = 4,
                    Tarifa = 400,
                    MetrosCuadrados = 80,
                    Amenidad = "",
                    FechaCreacion = DateTime.Now,
                    FechaActualizacion = DateTime.Now,
                }

            );
        }
    }
}
