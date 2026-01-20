using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using APIServerNFC.Model;
using System.Threading.Tasks;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APIServerNFC.API_Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        // GET: api/<UsersController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "BBBBBBB";
        }

        // POST api/<UsersController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
           
        }
        [HttpPost]
        [Route("CheckUser")]
        public IActionResult CheckUser([FromBody] Users users)
        {
            using (NFCDBContext nFCDBContext=new NFCDBContext())
            {
                var query = nFCDBContext.Users.Where(p => p.UsersName.Equals(users.UsersName) && p.Password.Equals(users.Password)).FirstOrDefault();
               if(query!=null)
                {
                    query.DateAccess = DateTime.Now;
                   
                    nFCDBContext.SaveChanges();
                  
                }
                string jsonoutput = JsonConvert.SerializeObject(query);
                //object ob = new JsonResult(lstsp);
                return new JsonResult(jsonoutput);
            }
        }
        [HttpPost]
        [Route("CheckUserClass")]
        public Users CheckUserClass([FromBody] Users users)
        {
            using (NFCDBContext nFCDBContext = new NFCDBContext())
            {
                var query = nFCDBContext.Users.Where(p => p.UsersName.Equals(users.UsersName) && p.Password.Equals(users.Password)).FirstOrDefault();
                if (query != null)
                {
                    query.DateAccess = DateTime.Now;

                    nFCDBContext.SaveChanges();

                }
                //Users users1 = new Users();
                //users1.UsersName = "HuynhIT";
                //users1.Password = "Passsssss";
                //users1.KhuVuc = "Khuvực";
                //users1.Stt = 1;
                //users1.TenUser = "huynh";
                //return users1;
                return query;
                ////string jsonoutput = JsonConvert.SerializeObject(query);
                //////object ob = new JsonResult(lstsp);
                ////return new JsonResult(jsonoutput);
            }
        }

        [HttpPut]
        [Route("SaveFireBase")]
        public bool SaveFireBase(DeviceIdandroid deviceIdandroid)
        {
            using (NFCDBContext nFCDBContext = new NFCDBContext())
            {
                var query = nFCDBContext.DeviceIdandroid.Where(p => p.DeviceId.Equals(deviceIdandroid.DeviceId)).FirstOrDefault();
                if (query == null)
                {
                    nFCDBContext.DeviceIdandroid.Add(deviceIdandroid);
                    nFCDBContext.SaveChanges();
                }
                return true;
            }
        }
      


      

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
