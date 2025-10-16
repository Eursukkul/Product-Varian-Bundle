using AutoMapper;
using FlowAccount.Application.DTOs.Product;
using FlowAccount.Application.DTOs.Bundle;
using FlowAccount.Domain.Entities;

namespace FlowAccount.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product Mappings
        CreateMap<ProductMaster, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null));

        CreateMap<VariantOption, VariantOptionDto>();
        CreateMap<VariantOptionValue, VariantOptionValueDto>();

        CreateMap<ProductVariant, ProductVariantDto>()
            .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.Attributes));

        CreateMap<ProductVariantAttribute, VariantAttributeDto>()
            .ForMember(dest => dest.OptionName, opt => opt.MapFrom(src => src.VariantOptionValue.VariantOption.Name))
            .ForMember(dest => dest.OptionValue, opt => opt.MapFrom(src => src.VariantOptionValue.Value));

        // Bundle Mappings
        CreateMap<Bundle, BundleDto>();
        CreateMap<BundleItem, BundleItemDto>()
            .ForMember(dest => dest.ItemName, opt => opt.Ignore())
            .ForMember(dest => dest.ItemSKU, opt => opt.Ignore());

        // Category Mapping
        CreateMap<Category, CategoryDto>();
    }
}

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
