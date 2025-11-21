using Ambev.DeveloperEvaluation.Application.Sales;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Unit.TestHelpers;

public static class AutoMapperTestHelper
{
    private static IMapper? _mapper;

    public static IMapper GetMapper()
    {
        if (_mapper != null)
            return _mapper;

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<SaleProfile>();
        });

        config.AssertConfigurationIsValid();
        _mapper = config.CreateMapper();

        return _mapper;
    }

    public static IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<SaleProfile>();
        });

        config.AssertConfigurationIsValid();
        return config.CreateMapper();
    }
}
