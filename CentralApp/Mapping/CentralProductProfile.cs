using AutoMapper;
using CentralApp.Database.Models;
using CentralApp.Models;
using Shared.DTOs;
using StoreApp.Models;

namespace CentralApp.Mapping
{
    /// <summary>
    /// AutoMapper profile for central products.
    /// </summary>
    public class CentralProductProfile : Profile
    {
        /// <summary>
        /// Creates a central product profile.
        /// </summary>
        public CentralProductProfile()
        {
            CreateMap<CentralProduct, ProductDTO>().ReverseMap();
            CreateMap<Config, ConfigDTO>().ReverseMap();
            CreateMap<ProductCreateRequest, CentralProduct>()
                .ForMember(dest => dest.SourceStoreId, opt => opt.Ignore());

            CreateMap<ProductUpdateRequest, CentralProduct>()
                .ForMember(dest => dest.SourceStoreId, opt => opt.Ignore());
        }
    }
}