namespace CosmicChamps
{
    public static class Layers
    {
        public const int Default = 0;
		public const int TransparentFX = 1;
		public const int IgnoreRaycast = 2;
		public const int Water = 4;
		public const int UI = 5;
		public const int Ground = 6;
		public const int Base = 7;
		public const int Obstacle = 8;
		public const int Unit = 9;
		public const int Projectile = 10;
		public const int Air = 11;
		public const int GroundHolesCover = 12;

        public static class Masks
        {
            public const int Default = 1 << Layers.Default;
			public const int TransparentFX = 1 << Layers.TransparentFX;
			public const int IgnoreRaycast = 1 << Layers.IgnoreRaycast;
			public const int Water = 1 << Layers.Water;
			public const int UI = 1 << Layers.UI;
			public const int Ground = 1 << Layers.Ground;
			public const int Base = 1 << Layers.Base;
			public const int Obstacle = 1 << Layers.Obstacle;
			public const int Unit = 1 << Layers.Unit;
			public const int Projectile = 1 << Layers.Projectile;
			public const int Air = 1 << Layers.Air;
			public const int GroundHolesCover = 1 << Layers.GroundHolesCover;
        }
    }
}