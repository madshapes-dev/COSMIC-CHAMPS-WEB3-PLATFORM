namespace ClientFilterInterceptor;

public class BuildVersions
{
    public BuildVersion Default { set; get; }
    public Dictionary<string, BuildVersion>? Platforms { set; get; }
}