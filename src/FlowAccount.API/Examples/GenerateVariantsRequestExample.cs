using FlowAccount.Application.DTOs.Product;
using Swashbuckle.AspNetCore.Filters;

namespace FlowAccount.API.Examples;

/// <summary>
/// Example request for generating product variants
/// </summary>
public class GenerateVariantsRequestExample : IExamplesProvider<GenerateVariantsRequest>
{
    public GenerateVariantsRequest GetExamples()
    {
        return new GenerateVariantsRequest
        {
            ProductMasterId = 10,
            SelectedOptions = new Dictionary<int, List<int>>
            {
                { 17, new List<int> { 82, 83, 84, 85, 86 } }, // Size: XS, S, M, L, XL
                { 18, new List<int> { 92, 93, 94 } },          // Color: Black, White, Red
                { 19, new List<int> { 97, 98 } }               // Material: Cotton, Polyester
            },
            PriceStrategy = PriceStrategy.Fixed,
            BasePrice = 299.00m,
            BaseCost = 150.00m
        };
    }
}

/// <summary>
/// Example request for maximum capacity (250 variants)
/// </summary>
public class GenerateVariantsMaxCapacityExample : IExamplesProvider<GenerateVariantsRequest>
{
    public GenerateVariantsRequest GetExamples()
    {
        return new GenerateVariantsRequest
        {
            ProductMasterId = 10,
            SelectedOptions = new Dictionary<int, List<int>>
            {
                // 10 sizes (XS to 6XL)
                { 17, new List<int> { 82, 83, 84, 85, 86, 87, 88, 89, 90, 91 } },
                // 5 colors
                { 18, new List<int> { 92, 93, 94, 95, 96 } },
                // 5 materials
                { 19, new List<int> { 97, 98, 99, 100, 101 } }
            },
            PriceStrategy = PriceStrategy.Fixed,
            BasePrice = 299.00m,
            BaseCost = 150.00m
        };
    }
}
