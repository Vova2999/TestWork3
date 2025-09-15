using Client.Domain.Dtos.CommandResultData;
using Client.Domain.Dtos;
using Sms.Test;

namespace Client.Services.SmsTest;

public interface ISmsTestService
{
    Task<Sms.Test.GetMenuResponse> GetMenuAsync(bool withPrice);
    Task<SendOrderResponse> SendOrderAsync(Guid orderId, ICollection<OrderItem> orderItems);
}