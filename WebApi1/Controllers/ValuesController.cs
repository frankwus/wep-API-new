using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity;
using System.Threading.Tasks; 
namespace WebApi1.Controllers
{
    public class ValuesController : ApiController
    {

        KPI_Datamart_1Entities1 db = new KPI_Datamart_1Entities1();
        // GET api/values
        public IEnumerable<string> Get() {
            //return db.a0.Select(s => new { s.name }); 
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id) {
           // return db.a0.FirstOrDefault().name; 
            return db.a0.Where(s => s.ID == id).FirstOrDefault().name;
            return "value";
        }

        // POST api/values
        public string  Post( MyModel m) {
            return "post " + m.firstName;
        }

        // PUT api/values/5
        public string  Put(MyModel m) {
            return "put "+m.firstName; 
        }

        // DELETE api/values/5
        public void Delete(int id) {
        }
    }
    public class MyModel
    {
        public string firstName { get; set; }
    }
}
