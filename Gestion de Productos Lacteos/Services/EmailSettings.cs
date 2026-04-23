namespace SistemaInventarioLacteos.Services
{
    public class EmailSettings
    {
        public required string From { get; set; }
        public required string Password { get; set; }
        public required string Host { get; set; }
        public int Port { get; set; }
    }
}