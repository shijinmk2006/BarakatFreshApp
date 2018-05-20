using DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Utility;

namespace DataAccess
{
    public class DALUser
    {

        private readonly string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
        public IList<UserDetails> GetUser(string userName)
        {
            IList<UserDetails> user = new List<UserDetails>();
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("usp_GetUserProfile", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@user_name", userName);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var objUser = new UserDetails();
                            objUser.userName =!Common.IsDBNull(dr["UserName"])? dr["UserName"].ToString():string.Empty;
                            objUser.customerId = !Common.IsDBNull(dr["UserId"])? dr["UserId"].ToString():string.Empty;
                            objUser.Mobile = !Common.IsDBNull(dr["mobile_no"])? dr["mobile_no"].ToString():string.Empty;
                            objUser.FirstName = !Common.IsDBNull(dr["first_name"])? dr["first_name"].ToString():string.Empty;
                            user.Add(objUser);
                        }
                    }
                }
            }
            return user;
        }

        public bool ValidateUser(string userName)
        {

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("validate_username", con))
                {
                    SqlParameter outputIdParam = new SqlParameter("@verified", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@user_name", userName.Trim());
                    cmd.Parameters.Add(outputIdParam);
                    cmd.ExecuteNonQuery();
                    return (bool)outputIdParam.Value;
                }
            }

        }
    }
}
