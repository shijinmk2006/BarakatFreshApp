using DataAccess;
using DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace testcart.Controllers
{
    public class UserController : ApiController
    {
        DALUser dalUser = null;
        public UserController()
        {
            dalUser = new DALUser();
        }
        
        public async Task<HttpResponseMessage> GetUserDetails()
        {

            IList<UserDetails> listUser = new List<UserDetails>();
            var Users = await Task.Run(() => dalUser.GetUser());
            foreach (var user in Users)
            {
                UserDetails usrDtl = new UserDetails();
                usrDtl.userName = user.userName;
                listUser.Add(usrDtl);

            }
            return Request.CreateResponse(HttpStatusCode.OK, listUser);

        }

        [Route("ValidateUser")]
        [HttpGet]
        public async Task<HttpResponseMessage> ValidateUser(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                var isValid = await Task.Run(() => dalUser.ValidateUser(userName.Trim()));
                return Request.CreateResponse(HttpStatusCode.OK, isValid);
            }
            else
                return Request.CreateResponse(HttpStatusCode.OK, false);

        }
        
    }
}
