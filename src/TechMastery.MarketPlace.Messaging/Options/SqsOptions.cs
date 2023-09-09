namespace TechMastery.Messaging.Consumers
{
    public class SqsOptions
    {
        public string? Region { get; set; }
        public string? AccessKey { get; set; }      
        public string? SecretKey { get; set; }
        public string? ServiceUrl { get; set; }
    }
}