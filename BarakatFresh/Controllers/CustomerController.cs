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
using WebMatrix.WebData;

namespace DataCartServiceApp.Controllers
{
    public class CustomerController : ApiController
    {
        DALCustomer dalCustomer = null;
        public CustomerController()
        {
            dalCustomer = new DALCustomer();
        }
        [Route("CustomerRegistration")]
        [HttpPost]
        public async Task<HttpResponseMessage> RegisterCustomer(Customer customer)
        {
            if (ModelState.IsValid)
            {
                if (dalCustomer.ValidateUsername(customer.Email.Trim()))
                {
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(new { data = "", error = "UserName already exists", status = 0 }), Encoding.UTF8, "application/json")
                    };
                }
                var customerId = await Task.Run(() => dalCustomer.RegisterUser(customer));
                if (customerId > 0)
                {
                    WebSecurity.CreateUserAndAccount(customer.Email, customer.Password, propertyValues: new { CustomerId = customerId, Verified = 0, UserType = 1 });
                    Utility.Common.send_registration_acknowledgement(customer.Email, customer.FirstName, customerId);
                    Utility.Common.SendSMS(customer.MobileNumber, customer.FirstName, customer.Email);

                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(new { data = customerId, status = customerId > 0 ? 1 : 0, error = customerId <= 0 ? "Registration Failed" : "", success = customerId > 0 ? "Registration successful" : "" }), Encoding.UTF8, "application/json")
                    };
                }
                else
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(new { data = "", error = "Registration Failure", status = 0 }), Encoding.UTF8, "application/json")
                    };
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = false, error = errors, status = -1 }), Encoding.UTF8, "application/json")
                };
            }


        }

        [Route("CompleteCustomerRegistration")]
        [HttpGet]
        public async Task<HttpResponseMessage> CompleteCustomerRegistration(string customerId)
        {
            PassProtection.Protection obj = new PassProtection.Protection();
            customerId = obj.Decoding(customerId);
            if (!string.IsNullOrEmpty(customerId))
            {
                bool completed = await Task.Run(() => dalCustomer.CompleteRegisterUser(Convert.ToInt32(customerId)));
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = completed, customerId = customerId }), Encoding.UTF8, "application/json")
                };

            }
            else
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = false, error = "Invalid CustomerId" }), Encoding.UTF8, "application/json")
                };
        }

        [Route("CheckUserNameExists")]
        [HttpGet]
        public async Task<HttpResponseMessage> CheckUserNameExists(string userName)
        {

            if (!string.IsNullOrEmpty(userName))
            {
                bool validated = await Task.Run(() => dalCustomer.ValidateUsername(userName));
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = validated }), Encoding.UTF8, "application/json")
                };

            }
            else
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = false, error = "Invalid UserName" }), Encoding.UTF8, "application/json")
                };
        }

        [Route("UpdateCustomerInformation")]
        [HttpPost]
        public async Task<HttpResponseMessage> UpdateCustomerInformation(CustomerAddress customerAddress)
        {
            if (ModelState.IsValid)
            {
                int rows = await Task.Run(() => dalCustomer.UpdateCustomerInformation(customerAddress));
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = rows }), Encoding.UTF8, "application/json")
                };
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList();
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = false, error = errors }), Encoding.UTF8, "application/json")
                };
            }

        }
        public async Task<HttpResponseMessage> ForgotPassword([FromBody] ForgotPassword ForgotPassword)
        {

            if (!string.IsNullOrEmpty(ForgotPassword.UserName))
            {
                bool isValid = dalCustomer.ValidateUsername(ForgotPassword.UserName);
                if (isValid)
                {
                    string _token = WebSecurity.GeneratePasswordResetToken(ForgotPassword.UserName, 1440);
                    Utility.Common.SendPasswordResetTocken(ForgotPassword.UserName, WebSecurity.GetUserId(ForgotPassword.UserName), _token);
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(new { data = true }), Encoding.UTF8, "application/json")
                    };

                }
                else
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(new { data = "", error = " Invalid UserName" }), Encoding.UTF8, "application/json")
                    };


            }
            else
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = "" }), Encoding.UTF8, "application/json")
                };

        }


        [HttpGet]
        public async Task<HttpResponseMessage> GetCustomerBillingDetails(int customerId)
        {
            IList<CustomerBillingDetails> customerBillingDetails = await Task.Run(() => dalCustomer.GetCustomerBillingDetails(customerId));
            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { data = customerBillingDetails }), Encoding.UTF8, "application/json")
            };

        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetReviews(int ItemId)
        {
            IList<Review> reviews = await Task.Run(() => dalCustomer.GetCustomerReviews(ItemId));
            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { data = reviews }), Encoding.UTF8, "application/json")
            };

        }

        [HttpPost]
        public async Task<HttpResponseMessage> AddReviews([FromBody] Review review)
        {
            try
            {
                int added = await Task.Run(() => dalCustomer.AddCustomerReviews(review));

                if (added > 0)
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(new { data = added }), Encoding.UTF8, "application/json")
                    };
                else
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(new { data = added, error = "Failure in adding review" }), Encoding.UTF8, "application/json")
                    };
            }
            catch (Exception)
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = 0, error = "An Error Occured" }), Encoding.UTF8, "application/json")
                };
            }

        }
        [HttpPost]
        public async Task<HttpResponseMessage> AddFeedback([FromBody] feedbackModel feedback)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int added = await Task.Run(() => dalCustomer.AddCustomerFeedback(feedback));

                    if (added > 0)
                        return new HttpResponseMessage()
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(new { data = added }), Encoding.UTF8, "application/json")
                        };
                    else
                        return new HttpResponseMessage()
                        {
                            Content = new StringContent(JsonConvert.SerializeObject(new { data = added, error = "Failure in adding feedback" }), Encoding.UTF8, "application/json")
                        };
                } 
                else
                {
                    var errors = ModelState.Select(x => x.Value.Errors)
                          .Where(y => y.Count > 0)
                          .ToList();
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(new { data = false, error = errors }), Encoding.UTF8, "application/json")
                    };
                }
            }
            catch (Exception)
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = 0, error = "An Error Occured" }), Encoding.UTF8, "application/json")
                };
            }

        }



    }
}