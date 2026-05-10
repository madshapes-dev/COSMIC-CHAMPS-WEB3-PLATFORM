using Amazon.DynamoDBv2.DataModel;

namespace CosmicChamps.Api.Model;

public class DeviceGraphics
{
    [DynamoDBProperty]
    public int Id { set; get; }

    [DynamoDBProperty]
    public string Name { set; get; }

    [DynamoDBProperty]
    public string Type { set; get; }

    [DynamoDBProperty]
    public string Vendor { set; get; }

    [DynamoDBProperty]
    public int VendorID { set; get; }

    [DynamoDBProperty]
    public string Version { set; get; }

    [DynamoDBProperty]
    public int MemorySize { set; get; }
}