namespace LionFire.Orleans_.AspNetCore_
{
    public class OrleansCheckHostedServiceOptions
    {
        public string PathString { get; set; } = "/health";
        public int Port { get; set; } = 8880;
    }
}
