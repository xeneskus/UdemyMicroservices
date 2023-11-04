using FreeCourse.Services.Order.Application.Dtos;
using FreeCourse.Shared.Dtos;
using MediatR;

namespace FreeCourse.Services.Order.Application.Queries
{
    public class GetOrdersByUserIdQuery :IRequest<Response<List<OrderDto>>>
    {
        public string UserId { get; set; }
    }
}
