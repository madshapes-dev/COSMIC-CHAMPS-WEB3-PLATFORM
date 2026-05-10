using Amazon.DynamoDBv2.DataModel;

namespace CosmicChamps.Api.Model;

public class Device
{
    [DynamoDBProperty]
    public string Id { set; get; }

    [DynamoDBProperty]
    public string Model { set; get; }

    [DynamoDBProperty]
    public DeviceGraphics Graphics { set; get; }

    [DynamoDBProperty]
    public DeviceOperatingSystem OperatingSystem { set; get; }
}