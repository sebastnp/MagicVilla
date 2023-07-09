using AutoMapper;
using MagicVilla_API.Models;
using MagicVilla_API.Models.Dto;

namespace MagicVilla_API
{
    public class MappingConfig : Profile
    {
        public MappingConfig() 
        { 
            // < Fuente, Destino >
            CreateMap<Villa, VillaDto>();
            // lo inverso ahora
            CreateMap<VillaDto, Villa>();

            // para los otros dto que tenemos
            // y de una forma mas corta pero es lo mismo que arriba
            CreateMap<Villa, VillaCreateDto>().ReverseMap();

            CreateMap<Villa, VillaUpdateDto>().ReverseMap();
        }
    }
}
