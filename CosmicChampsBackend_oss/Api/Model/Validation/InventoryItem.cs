namespace CosmicChamps.Api.Model.Validation;

public class InventoryItem
{
    private sealed class EqualityComparer : IEqualityComparer<InventoryItem>
    {
        public bool Equals (InventoryItem x, InventoryItem y)
        {
            if (ReferenceEquals (x, y)) return true;
            if (ReferenceEquals (x, null)) return false;
            if (ReferenceEquals (y, null)) return false;
            if (x.GetType () != y.GetType ()) return false;
            return x._id == y._id;
        }

        public int GetHashCode (InventoryItem obj)
        {
            return obj._id.GetHashCode ();
        }
    }

    public static IEqualityComparer<InventoryItem> Comparer { get; } = new EqualityComparer ();

    public string _id { set; get; }
    public int quantity { set; get; }
}