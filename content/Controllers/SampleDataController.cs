using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace aspnetcore_vue.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SampleDataController : ControllerBase
    {
        [HttpGet("numbers")]
        public IEnumerable<int> GetNumbers()
        {
            return Enumerable.Range(1, 10);
        }
    }
}