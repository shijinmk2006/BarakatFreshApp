using DataEntity;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Utility;

namespace DataAccess
{
    public class DALCustomer
    {
        private readonly string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
        private readonly object _obj_class;

        public int RegisterUser(Customer customer)
        {

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("customer_registration", con))
                {
                    SqlParameter outputIdParam = new SqlParameter("@customer_id", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputIdParam);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@first_name", customer.FirstName.Trim());
                    cmd.Parameters.AddWithValue("@last_name", customer.LastName.Trim());
                    cmd.Parameters.AddWithValue("@email_id", customer.Email.Trim());
                    cmd.Parameters.AddWithValue("@mobile_no", customer.MobileNumber.Trim());
                    cmd.Parameters.AddWithValue("@google_map_link", customer.GoogleMapLink.Trim());
                    cmd.ExecuteNonQuery();
                    return (int)outputIdParam.Value;
                }
            }

        }
        public int UpdateCustomerInformation(CustomerAddress customeraddress)
        {
            int rows;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("update_customer_information", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@user_id", customeraddress.customerId);
                    cmd.Parameters.AddWithValue("@first_name", customeraddress.FirstName.Trim());
                    cmd.Parameters.AddWithValue("@last_name", customeraddress.LastName.Trim());
                    cmd.Parameters.AddWithValue("@email_id", customeraddress.Email.Trim());
                    cmd.Parameters.AddWithValue("@mobile_no", customeraddress.MobileNumber.Trim());
                    cmd.Parameters.AddWithValue("@billing_address", customeraddress.BillingAddress.Trim());
                    cmd.Parameters.AddWithValue("@billing_location", customeraddress.BillingLocation.Trim());
                    cmd.Parameters.AddWithValue("@billing_emirate", customeraddress.BillingEmirate.Trim());
                    cmd.Parameters.AddWithValue("@shipping_address", customeraddress.ShippingAddress.Trim());
                    cmd.Parameters.AddWithValue("@shipping_location", customeraddress.ShippingLocation.Trim());
                    cmd.Parameters.AddWithValue("@shipping_emirate", customeraddress.ShippingEmirate.Trim());
                    rows = cmd.ExecuteNonQuery();
                }
            }
            return rows;
        }

        public bool CompleteRegisterUser(int customerId)
        {

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("complete_registration", con))
                {
                    SqlParameter outputIdParam = new SqlParameter("@account_status", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputIdParam);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@customer_id", customerId);
                    cmd.ExecuteNonQuery();
                    return (bool)outputIdParam.Value;
                }
            }

        }
        public bool ValidateUsername(string userName)
        {

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("validate_user_account", con))
                {
                    SqlParameter outputIdParam = new SqlParameter("@user_name_exists", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputIdParam);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@user_name", userName);
                    cmd.ExecuteScalar();
                    return (bool)outputIdParam.Value;
                }
            }

        }
        public IList<CustomerBillingDetails> GetCustomerBillingDetails(int userId)
        {
            IList<CustomerBillingDetails> billingDetails = new List<CustomerBillingDetails>();
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("load_customer_billing_details", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", userId);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var bilingCustomer = new CustomerBillingDetails();
                            bilingCustomer.Name = !Common.IsDBNull(dr["customer_name"]) ? dr["customer_name"].ToString() : string.Empty;
                            bilingCustomer.BillingAddress = !Common.IsDBNull(dr["billing_address"]) ? dr["billing_address"].ToString() : string.Empty;
                            bilingCustomer.BillingCity = !Common.IsDBNull(dr["billing_location"]) ? dr["billing_location"].ToString() : string.Empty;
                            bilingCustomer.BillingEmirate = !Common.IsDBNull(dr["billing_emirate"]) ? dr["billing_emirate"].ToString() : string.Empty;
                            bilingCustomer.ShippingAddress = !Common.IsDBNull(dr["shipping_address"]) ? dr["shipping_address"].ToString() : string.Empty;
                            bilingCustomer.ShippingCity = !Common.IsDBNull(dr["shipping_location"]) ? dr["shipping_location"].ToString() : string.Empty;
                            bilingCustomer.ShippingEmirate = !Common.IsDBNull(dr["shipping_emirate"]) ? dr["shipping_emirate"].ToString() : string.Empty;
                            bilingCustomer.Email = !Common.IsDBNull(dr["email_id"]) ? dr["email_id"].ToString() : string.Empty;
                            bilingCustomer.Mobile = !Common.IsDBNull(dr["mobile_no"]) ? dr["mobile_no"].ToString() : string.Empty;
                            billingDetails.Add(bilingCustomer);
                        }
                    }

                }
            }
            return billingDetails;
        }
        public int AddCustomerReviews(Review review)
        {
            int rows;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("insert_review", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@item_id", review.ItemId);
                    cmd.Parameters.AddWithValue("@customer_name", review.CustomerName);
                    cmd.Parameters.AddWithValue("@review", review.ReviewComments);
                    cmd.Parameters.AddWithValue("@rating", review.Rating);
                    rows = cmd.ExecuteNonQuery();
                }
            }
            return rows;
        }
        public IList<Review> GetCustomerReviews(int ItemId)
        {
            IList<Review> reviews = new List<Review>();
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("load_reviews", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", ItemId);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var review = new Review();

                            review.CustomerName = !Common.IsDBNull(dr["customer_name"]) ? dr["customer_name"].ToString() : string.Empty;
                            review.ReviewComments = !Common.IsDBNull(dr["review"]) ? dr["review"].ToString() : string.Empty;
                            review.Rating = !Common.IsDBNull(dr["rating"]) ? int.Parse(dr["rating"].ToString()) : 0;
                            review.ReviewDate = !Common.IsDBNull(dr["review_date"]) ? dr["review_date"].ToString() : string.Empty;
                            reviews.Add(review);
                        }
                    }
                }
            }
            return reviews;

        }
        public int AddCustomerFeedback(feedbackModel feedback)
        {
            int rows;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("insert_feedback", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@feedback_type", feedback.FeedbackType);
                    cmd.Parameters.AddWithValue("@feedback", feedback.Feedback);
                    cmd.Parameters.AddWithValue("@mobile_no", feedback.Mobile);
                    cmd.Parameters.AddWithValue("@email", feedback.Email);
                    cmd.Parameters.AddWithValue("@customer_name", feedback.CustomerName);
                    rows = cmd.ExecuteNonQuery();
                }
            }
            return rows;
        }

    }
}
