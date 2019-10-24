using System;
using System.Drawing; 
using Newtonsoft; 
using Microsoft.AspNet.Identity; 
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework; 
using System.Web.Http.Cors;
using System.Threading.Tasks;
using webApi2;
using System.Net;
using System.Web.Http;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.IO;

namespace WebApi2.Controllers
{

    // [EnableCors(origins: "http://localhost:81", headers: "*", methods: "*")]
    //[EnableCors(origins: "http://localhost:81", headers: "*",methods: "*", SupportsCredentials = true)]
    public class ValuesController : ApiController
    {
        private Workinstruction_testEntities entity = new Workinstruction_testEntities(); 
        public HttpResponseMessage Options() {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }
        //  KPI_Datamart_1Entities1 db = new KPI_Datamart_1Entities1();
        // GET api/values
        [Authorize(Users = "ensco\\011311")]
        public IEnumerable<  aaWi> Get() {

            //return db.a0.Select(s => new { s.name }); 
            string s = User.Identity.Name;
            //UserManager = new UserManager<ApplicationUser, string>(new UserStore<ApplicationUser, CustomRole, string, CustomUserLogin, CustomUserRole, CustomUserClaim>(new myDbContext()));

            //UserManager<IdentityUser> userManager=new UserManager<IdentityUser>();
            // RoleManager<IdentityRole> rolesManager;
            //userManager.AddToRole("011311", "admin"); 
            return this.entity.aaWis; 
        }
        [Route("api/values/equipment/{id:int=0}")]
        public IHttpActionResult  GetEquipment(int? id ) {
            string sql = "select id, name from equipmentType where lang='e' ";
            if (id != 0)
                sql = "select id, name from equipmentMake where lang='e' and equipmentTypeId= " + id.ToString();
            sql = "select 0 as id, '' as name union " +sql ; 
            var data = this.entity.Database.SqlQuery<MyList>(sql); 
            return Ok(data);
        }
        //GET api/values/5
        public IHttpActionResult Get(int id) {

            var wi = this.entity.aaWis.Where(t => t.id == id).FirstOrDefault();
            var js  = this.entity.aaJobSteps.Where(t => t.wiid == id); 
            // var obj = new { "result":"fail"}; 
            return Ok( new {
                       header = wi, 
                       jobSteps = js 
                   }) ; 
        }
        //   ClaimsIdentityFactory
        // POST api/values
        public string  Post() {
            var httpRequest = HttpContext.Current.Request;
            return  this.UpdateFiles(httpRequest.Files);
        }

        //public void Post(MyModel m ) {

        //}
        string UpdateFiles(HttpFileCollection files) {
            string imgFolder = @"c:\dev\emoc\images\";
            try {
                foreach (string name  in files) {
                    HttpPostedFile file = files[name];
                    string guid = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    file.SaveAs(imgFolder +guid);
                    return guid; 
                }
            } catch (Exception ex) {
                return ex.Message;
            }
            return "OK";
        }
        void  UpdatePhoto( aaJobStep jobStep ) {
            string photo = jobStep.photo;
            string name = jobStep.note;
            if (name == "")
                return; 
            string imgFolder = @"c:\dev\emoc\images\"; 
            string guid = Guid.NewGuid().ToString() + Path.GetExtension(name);
            var myString = photo.Split(new char[] { ',' });
            byte[] bytes = Convert.FromBase64String(myString[1]);
            using (MemoryStream ms = new MemoryStream(bytes)) {
                Image image = Image.FromStream(ms);
                image.Save( imgFolder+  guid ) ;
            }
            jobStep.photo = guid;
            jobStep.note = "";             
        }
        // PUT api/values/5
        [Authorize(Users = "ensco\\011311")]
        public string Put(MyModel m) {
            return "put "; 
        }

        // DELETE api/values/5
        public void Delete(int id) {
        }
    }
    public class MyModel
    {
        public aaWi header;
        public aaJobStep [] jobSteps; 
    }
    public class Header
    {
        public string jobTitle { get; set; }
        public string facility { get; set; }
        public string generalPrecaution { get; set; }
        public string localPrecaution { get; set; }
        public string wiNo { get; set; }
        public string status { get; set; }
    }
    public class JobStep
    {
        public string description { get; set; }
        public string warning { get; set; }
        public string caution { get; set; }
        public string note { get; set; }
        public string photo { get; set; }
        public bool barrier { get; set; }
    }
    public class MyList 
    {
        public int id  { get; set; }
        public string name  { get; set; }
    }
    public class EquipmentMake 
    {
        public int id { get; set; }
        public int parentId { get; set; }
        public string name { get; set; }
    }
    public class ApplicationRole 
    {
        public  string Role { get; set; } // example, not necessary
    }
}
