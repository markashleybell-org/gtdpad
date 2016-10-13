using Nancy;

namespace gtdpad
{
    public class Home : NancyModule
    {
        public Home()
        {
            Get("/", args => "Hello World");
        }
    }
}