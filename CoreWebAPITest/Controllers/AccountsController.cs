using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CoreWebAPITest.Filters;
using CoreWebAPITest.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CoreWebAPITest.Controllers
{
    //[ApiVersion("1.0")]
    //[ApiVersion("2.0")]
    //[Route("api/v{version:apiVersion}/[controller]")]
     [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : Controller //ControllerBase
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum AccType
        {
            Primary, Secondary
        }

        public class Letter
        {
            [Required]
            public string Content { get; set; }

            [Required]
            [EnumDataType(typeof(AccType))]
            [JsonConverter(typeof(StringEnumConverter))]
            public AccType Priority { get; set; }
        }


  //   //   [MapToApiVersion("1.0")]
  //   //   [HttpGet, Route("TestVersion/{accountId}")]
  //      public string TestVersion(string accountId)
  //      {
  //          string str = Startup.DataProtectionHelper.Protect(accountId);
  //          return "Version1";//  str.ToString();
  //      }

  // //     [HttpGet, Route("TestVersion/{accountId}")]
  ////      [MapToApiVersion("2.0")]
  //      public string TestVersionV2(string accountId)
  //      {
  //          string str = Startup.DataProtectionHelper.Protect(accountId);
  //          return "Version2";// str.ToString();
  //      }

        
        [ApiKeyAuthAttribute]
        [HttpGet, Route("TestApiKeyAuthFilter")]
        public IActionResult TestApiKeyAuthFilter(string str)
        {

            return Ok("Success");
        }
        [Authorize]

        [HttpGet, Route("TestAuthorization/{accountId}")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public string TestAuthorization(string accountId)
        {
          //  string str = Startup.DataProtectionHelper.Protect(accountId);
            //  var str = User.FindFirst("sub")?.Value;
            var currentuserName = User.Identity.Name;
            return currentuserName;
        }

        [HttpGet, Route("TestDataProtect/{accountId}")]
        public string TestDataProtect(string accountId)
        {
            string str = Startup.DataProtectionHelper.Protect(accountId);
            return str.ToString();
        }

        [HttpGet, Route("TestDataUnProtect/{accountId}")]
        public string TestDataUnProtect(string accountId)
        {
            string str = Startup.DataProtectionHelper.Unprotect(accountId);
            return str.ToString();
        }
        // GET: api/Accounts
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }


        [Route("TestForwardProxy")]
        [AcceptVerbs("Get")]
        public IEnumerable<string> TestForwardProxy()
        {

            string uri = Url.Link("GetById", new { accountId = 55 });
            return new string[] { uri,
            Request.GetSelfReferenceBaseUrl().ToString(),
            Request.RebaseUrlForClient(new Uri(uri) ).ToString()
            };
        }


        [HttpGet, Route("TestEnum/{accType:enum(CoreWebAPITest.Controllers.AccountsController+AccType)}")]

        public string Get(AccType accType)
        {
            return accType.ToString();
        }



       // [HttpGet, Route("TestCustomValidation/{accountId:validAccount}")]
        [HttpGet, Route("TestCustomValidation/{accountId:validIP}", Name = "GetById")]

        public string Get(string accountId)
        {
            Thread.Sleep(1000);

            return accountId.ToString();
        }


        [ Route("TestInt/{id:int:range(50,100)}")]
        [AcceptVerbs("Get")]
        public string Get(int id)
        {
            return id.ToString();
        }

        [HttpGet, Route("TestClass/{letter}")]

        public string Get(Letter letter)
        {
            return letter.Content;
        }

        // POST: api/Accounts
        [HttpPost, Route("{prodId:int:range(50,100)}")]
        public System.Net.Http.HttpResponseMessage CreateAccount([System.Web.Http.FromUri] int  prodId)
        {
            var response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Created);

            string uri = Url.Link("GetById", new { accountId = prodId });
            response.Headers.Location =new Uri( uri);
            return response;
        }


        // POST: api/Accounts
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Accounts/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
