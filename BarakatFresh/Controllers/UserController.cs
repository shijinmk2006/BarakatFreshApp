using BarakatFresh.WebSecurity;
using DataAccess;
using DataEntity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Security;
using WebMatrix.WebData;

namespace testcart.Controllers
{


    public class UserController : ApiController
    {
        DALUser dalUser = null;
        public UserController()
        {
            dalUser = new DALUser();
        }

        [InitializeSimpleMembership]
        [HttpGet]
        public async Task<HttpResponseMessage> LoginUser(string userName, string password)
        {
            IList<UserDetails> user = null;
            if (!string.IsNullOrEmpty(userName))
            {
                bool isValid = Membership.ValidateUser(userName, password);
                if (isValid)
                {
                    user = await Task.Run(() => dalUser.GetUser(userName));
                    if (user != null)
                    {
                        return new HttpResponseMessage()
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(new { data = user }), Encoding.UTF8, "application/json")
                        };
                    }
                    else
                        return new HttpResponseMessage()
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(new { data = user,error="Invalid UserName and Password" }), Encoding.UTF8, "application/json")
                        };
                }
                else
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(new { data = user, error = "Invalid UserName and Password" }), Encoding.UTF8, "application/json")
                    };



            }

            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { data = "" }), Encoding.UTF8, "application/json")
            };

        }

    }
}
