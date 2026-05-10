using Amazon.DynamoDBv2.DataModel;

namespace CosmicChamps.Api.Model;

public class DeviceOperatingSystem
{
    [DynamoDBProperty]
    public string Name { set; get; }

    [DynamoDBProperty]
    public string Family { set; get; }
}