using Google.Protobuf.WellKnownTypes;
using Sms.Test;

namespace Client.Services.SmsTest;

public class SmsTestService : ISmsTestService
{
    private readonly Sms.Test.SmsTestService.SmsTestServiceClient _smsTestServiceClient;

    public SmsTestService(Sms.Test.SmsTestService.SmsTestServiceClient smsTestServiceClient)
    {
        _smsTestServiceClient = smsTestServiceClient;
    }

    public async Task<GetMenuResponse> GetMenuAsync(bool withPrice)
    {
        return await _smsTestServiceClient.GetMenuAsync(new BoolValue { Value = withPrice });
    }

    public async Task<SendOrderResponse> SendOrderAsync(Guid orderId, ICollection<OrderItem> orderItems)
    {
        return await _smsTestServiceClient.SendOrderAsync(new Order { Id = orderId.ToString(), OrderItems = { orderItems } });
    }
}