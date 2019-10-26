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
    [RoutePrefix("api/Values")]
    public class ValuesController : ApiController
    {
        private Workinstruction_testEntities entity = new Workinstruction_testEntities(); 
        public HttpResponseMessage Options() {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }
        //  KPI_Datamart_1Entities1 db = new KPI_Datamart_1Entities1();
        // GET api/values
        [Authorize(Users = "ensco\\011311")]
        [Route("")]
        public IEnumerable<  aaWi> Get() {

            //return db.a0.Select(s => new { s.name }); 
            string s = User.Identity.Name;
            //UserManager = new UserManager<ApplicationUser, string>(new UserStore<ApplicationUser, CustomRole, string, CustomUserLogin, CustomUserRole, CustomUserClaim>(new myDbContext()));

            //UserManager<IdentityUser> userManager=new UserManager<IdentityUser>();
            // RoleManager<IdentityRole> rolesManager;
            //userManager.AddToRole("011311", "admin"); 
            return this.entity.aaWis; 
        }
        [Route("equipment/{id:int=0}")]
        public IHttpActionResult  GetEquipment(int? id ) {
            string sql = "select id, name from equipmentType where lang='e' ";
            if (id != 0)
                sql = "select id, name from equipmentMake where lang='e' and equipmentTypeId= " + id.ToString();
            sql = "select 0 as id, '' as name union " +sql ; 
            var data = this.entity.Database.SqlQuery<MyList>(sql); 
            return Ok(data);
        }
        //GET api/values/5
        [Route("{id:int}")]
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
        public string  wPost() {


            var httpRequest = HttpContext.Current.Request;

            return  this.UpdateFiles(httpRequest.Files);
        }
        [HttpPost]
        [Route("")]
        public int  Post( MyModel m   ) {
            using (var tran = this.entity.Database.BeginTransaction()) {
                try {
                    //this.entity.Entry(m.header).State = System.Data.Entity.EntityState.Modified ;
                    this.entity.SaveChanges();
                    if (m.header.id == 0) {
                        aaWi header= this.entity.aaWis.Add(m.header);
                        this.entity.SaveChanges();
                        m.header.id = header.id; 
                    } else {
                        this.entity.Entry(m.header).State = System.Data.Entity.EntityState.Modified;
                    }
                    int wiid = m.header.id;

                    foreach (aaJobStep js in m.jobSteps)
                        js.wiid = wiid; 
                    List<int> list = m.jobSteps.Select(j => j.id).ToList();
                    var deleted = this.entity.aaJobSteps.Where(j => j.wiid == wiid && !list.Contains(j.id));
                    this.entity.aaJobSteps.RemoveRange(deleted);
                    var added = m.jobSteps.Where(t => t.id == 0);

                    this.entity.aaJobSteps.AddRange(added); 
                    var updated= this.entity.aaJobSteps.Where(j => j.wiid == wiid && list.Contains(j.id));
                    foreach(aaJobStep js in m.jobSteps) {
                        if (!list.Contains(js.id) || js.id==0)
                            continue;
                        this.entity.Entry(js).State = System.Data.Entity.EntityState.Modified; 
                    }
                    //this.entity.Entry()
                    this.entity.SaveChanges();
                    tran.Commit();
                    return wiid; 

                } catch (Exception e) {
                    tran.Rollback();
                    throw;
                }
            }
            return 0; 
        }
        [HttpPost]
        [Route("file")]
        public string  Post() {
            var httpRequest = HttpContext.Current.Request;
            return this.UpdateFiles(httpRequest.Files);
        }
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
        [HttpGet]
        [Route("Search/{text}")]
        public IHttpActionResult Search(string text ) {
            string sql = " exec usp_WiTools '"  + text + "'"; 
            var data = this.entity.Database.SqlQuery<MyList>(sql);
            return Ok(data);
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
