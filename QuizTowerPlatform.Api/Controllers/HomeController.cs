using Microsoft.AspNetCore.Mvc;
using QuizTowerPlatform.Api.Accessors;
using QuizTowerPlatform.Api.Models;
using System.Diagnostics;

namespace QuizTowerPlatform.Api.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IRequestAccessor request) : base(request)
        {

        }

        // GET: api/<ValuesController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
