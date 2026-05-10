namespace CosmicChamps.Data
{
    public class PlayerGameSession
    {
        public string Id { set; get; }
        public string PlayerSessionId { set; get; }
        public string DnsName { set; get; }
        public string IpAddress { set; get; }
        public int Port { set; get; }
        public int WebGLPort { set; get; }
    }
}