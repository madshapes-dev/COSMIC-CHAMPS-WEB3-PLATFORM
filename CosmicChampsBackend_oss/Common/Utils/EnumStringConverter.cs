using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace CosmicChamps.Common.Utils;

public class EnumStringConverter<T> : IPropertyConverter where T : Enum
{
    public DynamoDBEntry ToEntry (object value)
    {
        return new Primitive (((T)value).ToString ());
    }

    public object FromEntry (DynamoDBEntry entry)
    {
        return Enum.Parse (typeof (T), entry.AsString ());
    }
}