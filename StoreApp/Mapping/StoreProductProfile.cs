using AutoMapper;
using Shared.DTOs;
using StoreApp.Database.Models;
using StoreApp.Models;

namespace StoreApp.Mapping;

/// <summary>
/// AutoMapper profile for store products.
/// </summary>
public class StoreProductProfile : Profile
{
    /// <summary>
    /// Creates a store product profile.
    /// </summary>
    public StoreProductProfile()
    {
        CreateMap<Product, ProductDTO>();
        CreateMap<ProductCreateRequest, Product>();
        CreateMap<ProductUpdateRequest, Product>()
            .ForAllMembers(opt =>
                opt.Condition(
                    (_, _, sourceMember, _) =>
                        sourceMember != null));
    }
}