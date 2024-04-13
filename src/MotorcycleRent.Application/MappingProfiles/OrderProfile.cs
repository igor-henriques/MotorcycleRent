namespace MotorcycleRent.Application.MappingProfiles;

public sealed class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderDto>();
    }
}
