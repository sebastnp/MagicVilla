using AutoMapper;
using MagicVilla_API.Datos;
using MagicVilla_API.Models;
using MagicVilla_API.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {

        // Inyectamos nuestro DbContext para poder usar la base 
        private readonly AplicationDbContext _db;
        // Se lo inyectamos abajo en en constructor de logger (no es necesario que sea asi)

        // Inyectando el servicio de Logger

        private readonly ILogger<VillaController> _logger;

        // Inyectamos el servicio de Mapper
        private readonly IMapper _mapper;
        public VillaController(ILogger<VillaController> logger, AplicationDbContext db, IMapper mapper)
        {
            _logger = logger;
            _db = db;
            _mapper = mapper;
        }



        /*
            Aca creamos los endpoints, que son 
            metodos de c#
         */

        // IEnumbrable pq nos retorna una lista de tipo Villa
        // todos nuestros endpoints deben tener un verbo 
        [HttpGet]
        // Atributos para documentar los status code
        [ProducesResponseType(StatusCodes.Status200OK)]

        // definimos nuestro tipo de return con el ActionResult
        public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillas()
        {
            IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();
            // mapeo
            return Ok(_mapper.Map<IEnumerable<VillaDto>>(villaList));
        }


        // creamos un nuevo endpoint que solo devuelve
        // 1 de las villas de nuestro store
        // No se pueden tener dos enpoint de HttpGet o verbo sin diferenciar
        // por eso ponemos ("id")

        // Con ese name podemos dirigirnos a esa ruta
        [HttpGet("id", Name = "GetVilla")]
        // Atributo para documentar los status code
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDto>> GetVilla(int id)
        {
            // para encontrar el id 
            // var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            var villa = await _db.Villas.FirstOrDefaultAsync(x => x.Id == id);

            if (villa == null || id == 0)
            {
                return NotFound();
            }
            else
            {
                return Ok(_mapper.Map<VillaDto>(villa));
            }
        }


        // Nuevo endpoint para crear nuevas villas
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // el aributo FromBody indica que vamos a recibir datos
        public async Task<ActionResult<VillaDto>> CrearVilla([FromBody] VillaCreateDto villa)
        {
            // si una de las propiedades no esta valida 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // es decir que encontro algun registro con el mismo nombre
            if (await _db.Villas.FirstOrDefaultAsync(v => v.Nombre.ToLower() == villa.Nombre.ToLower()) != null)
            {
                // Agregando modelstates personalizados
                // el primer parametro es el nombre de la validacion
                // y el segundo es el mensaje que quiero que se muestre 
                ModelState.AddModelError("NombreExiste", "La villa ya existe");
                return BadRequest(ModelState);
            }

            if (villa == null)
            {
                return BadRequest();
            }

            //villa.Id = VillaStore.villaList.OrderByDescending(x => x.Id).FirstOrDefault().Id + 1;
            //VillaStore.villaList.Add(villa);

            //Villa modelo = new()
            //{
            //    Nombre = villa.Nombre,
            //    Detalle = villa.Detalle,
            //    ImagenUrl = villa.ImagenUrl,
            //    Ocupantes = villa.Ocupantes,
            //    MetrosCuadrados = villa.MetrosCuadrados,
            //    Tarifa = villa.Tarifa,
            //    Amenidad = villa.Amenidad,
            //};

            Villa modelo = _mapper.Map<Villa>(villa);

            await _db.Villas.AddAsync(modelo); // hacemos un insert
            await _db.SaveChangesAsync(); // para que los datos se vean reflejados en la bd

            // CreatedAtRoute llama esa ruta para que retorne
            // el id que le estamos pasando
            return CreatedAtRoute("GetVilla", new { id = modelo.Id }, modelo);
        }

        // endpoint para eliminar un registro
        [HttpDelete("id")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // Con la interfazAction no necesita el modelo
        // ya que retorna un nocontent
        public async Task<IActionResult> DeleteVilla(int id)
        {

            if (id == 0)
            {
                return BadRequest();
            }
            //var villa = VillaStore.villaList.FirstOrDefault(x => x.Id == id);
            var villa =await _db.Villas.FirstOrDefaultAsync(x => x.Id == id);
            if (villa == null)
            {
                return NotFound();
            }

            _db.Villas.Remove(villa);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        // Endpoint para actualizar el registro completo (put)
        [HttpPut("id")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto)
        {
            {
                // si no existe 
                if (updateDto == null || id != updateDto.Id)
                {
                    return BadRequest();
                }

                //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
                //villa.Nombre = villaDto.Nombre;
                //villa.Ocupantes = villaDto.Ocupantes;
                //villa.MetrosCuadrados = villaDto.MetrosCuadrados;

                //Villa modelo = new() 
                //{
                //    Id = villaDto.Id,
                //    Nombre = villaDto.Nombre,
                //    Detalle = villaDto.Detalle,
                //    ImagenUrl = villaDto.ImagenUrl,
                //    Ocupantes = villaDto.Ocupantes,
                //    Tarifa = villaDto.Tarifa,
                //    MetrosCuadrados = villaDto.MetrosCuadrados,
                //    Amenidad = villaDto.Amenidad,
                //};

                Villa modelo = _mapper.Map<Villa>(updateDto);

                _db.Villas.Update(modelo);
                await _db.SaveChangesAsync();
                return NoContent();
            }

        }

        [HttpPatch("id")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> PatchVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
        {
            if(patchDto == null || id == 0)
            {
                return BadRequest();
            }
            // capturamos el registro
            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            var villa =await _db.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);

            VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa);

            // antes que se actualice lo ponemos tenmporalmente en este modelo
            //VillaUpdateDto villaDto = new()
            //{
            //    Id = villa.Id,
            //    Nombre = villa.Nombre,
            //    Detalle = villa.Detalle,
            //    Amenidad = villa.Amenidad,
            //    Ocupantes = villa.Ocupantes,
            //    Tarifa = villa.Tarifa,
            //    MetrosCuadrados = villa.MetrosCuadrados,
            //    ImagenUrl = villa.ImagenUrl
            //};

            if(villa == null) return BadRequest();

            // le enviamos el registro acutal y capturamos el ModelState
            patchDto.ApplyTo(villaDto, ModelState);

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Villa modelo = _mapper.Map<Villa>(villaDto);

            //Villa modelo = new()
            //{
            //    Id = villaDto.Id,
            //    Nombre = villaDto.Nombre,
            //    Detalle = villaDto.Detalle,
            //    Amenidad = villaDto.Amenidad,
            //    Ocupantes = villaDto.Ocupantes,
            //    Tarifa = villaDto.Tarifa,
            //    MetrosCuadrados = villaDto.MetrosCuadrados,
            //    ImagenUrl = villaDto.ImagenUrl
            //};

            _db.Villas.Update(modelo);
            await _db.SaveChangesAsync();  

            return NoContent();
        }

    }
}
