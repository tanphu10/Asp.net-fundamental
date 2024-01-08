using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DemoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public string Greeting()
        { return "Hello world"; }

        [HttpPost]
        public string Greeting(string greeting)
        {
            return "aaaa";
        }
    }
}
