using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetCoreWithVue.DataHelp;
using DotnetCoreWithVue.Model;
using Microsoft.AspNetCore.Mvc;

namespace DotnetCoreWithVue.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "dotnet core with vue", "value2" };
        }
        // POST api/values/LoginCheck
        [HttpPost("LoginCheck")]
        public bool LoginCheck(string LogName,string pwd)
        {
           bool IsSucess=false;
           var inputpwd = PwdModel.GetKeyPwd(pwd);
           var data = ConnectToData.InstConnectToData.SQL_ExecuteScalar($"select [pwd] From [ur_user] where [login_name]={LogName}", "data source=.;user id=sa;password=sa;initial catalog=SOAP");
            if (data.ToString()==inputpwd)
            {
              IsSucess = true;
            }
           return IsSucess;
        }
        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
