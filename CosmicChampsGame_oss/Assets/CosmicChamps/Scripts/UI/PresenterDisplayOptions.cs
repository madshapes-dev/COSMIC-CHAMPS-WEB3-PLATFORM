using System;

namespace CosmicChamps.UI
{
    [Flags]
    public enum PresenterDisplayOptions
    {
        None = 0,
        Immediate = 1,
        Notify = 2,
        Default = Notify
    }
}