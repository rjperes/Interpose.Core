namespace Sample
{
    public class HelloWorldService : IHelloWorldService
    {
        [Trace]
        public string SayHello()
        {
            return "Hello, World!";
        }
    }
}