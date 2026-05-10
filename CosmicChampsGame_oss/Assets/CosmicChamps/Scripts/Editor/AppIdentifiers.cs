namespace CosmicChamps.Editor
{
    public class AppIdentifier
    {
        /*public static readonly AppIdentifier Server = new("com.madshapes.cosmicchampsserver", "Cosmic Champs Server");
        public static readonly AppIdentifier Android = new("com.MadShapes.CosmicChamps", "Cosmic Champs");
        public static readonly AppIdentifier IOS = new("com.madshapes.cosmicchamps", "Cosmic Champs");*/
        public static readonly AppIdentifier StandalonePrimary = new("com.madshapes.cosmicchamps", "Cosmic Champs");
        public static readonly AppIdentifier StandaloneSecondary = new("com.madshapes.cosmicchampssecondary", "Cosmic Champs Secondary");

        public readonly string Identifier;
        public readonly string ProductName;

        private AppIdentifier (string identifier, string productName)
        {
            Identifier = identifier;
            ProductName = productName;
        }
    }
}