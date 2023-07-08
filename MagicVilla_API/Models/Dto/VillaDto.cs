using System.ComponentModel.DataAnnotations;

namespace MagicVilla_API.Models.Dto
{
    // los dto (Data Transfer Object) proveen
    // una envoltura entre la entidad o modelo 
    // y lo que sera expuesto desde la API
    public class VillaDto
    {
        // solo escribo las propiedades 
        // que voy a querer mostrar 
        // e.g la fecha no esta aca 
        public int Id { get; set; }

        // Data notations 
        [Required]
        [MaxLength(30)]
        public string Nombre { get; set; }
        public string Detalle { get; set; }
        [Required]
        public double Tarifa { get; set; }
        public int Ocupantes { get; set; }
        public int MetrosCuadrados { get; set; }
        public string ImagenUrl { get; set; }
        public string Amenidad { get; set; }
    }
}
