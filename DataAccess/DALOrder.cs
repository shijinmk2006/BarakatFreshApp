using DataEntity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Utility;


namespace DataAccess
{
    public class DALOrder
    {
        private readonly string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
        public IList<DeliveryZone> GetZones()
        {
            IList<DeliveryZone> zones = new List<DeliveryZone>();
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("load_delivery_zone", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var zone = new DeliveryZone();
                            zone.ZoneId = Convert.ToInt32(dr["zone_id"].ToString());
                            zone.ZoneName = !Common.IsDBNull(dr["zone_name"]) ? dr["zone_name"].ToString() : string.Empty;
                            zones.Add(zone);
                        }
                    }
                }
            }
            return zones;
        }
        public IList<DeliveryArea> GetZoneArea(int zoneId)
        {
            IList<DeliveryArea> zoneArea = new List<DeliveryArea>();
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("load_delivery_zone_area", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", zoneId);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var zonearea = new DeliveryArea();
                            zonearea.AreaId = Convert.ToInt32(dr["area_id"].ToString());
                            zonearea.AreaName = !Common.IsDBNull(dr["area_name"]) ? dr["area_name"].ToString() : string.Empty;
                            zoneArea.Add(zonearea);
                        }
                    }
                }
            }
            return zoneArea;
        }


        public int AddOrder(CheckOut checkOut)
        {
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("insert_order", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@user_id", checkOut.CustomerId);
                    cmd.Parameters.AddWithValue("@checkout_type", checkOut.CheckoutType);
                    cmd.Parameters.AddWithValue("@customer_name", checkOut.CustomerName);
                    cmd.Parameters.AddWithValue("@customer_mobile", checkOut.Mobile);
                    cmd.Parameters.AddWithValue("@customer_email", checkOut.Email);
                    cmd.Parameters.AddWithValue("@sub_total", checkOut.SubTotal);
                    cmd.Parameters.AddWithValue("@delivery_charge", checkOut.DeliveryCharge);
                    cmd.Parameters.AddWithValue("@cart_total", checkOut.CartTotal);
                    cmd.Parameters.AddWithValue("@payment_type", checkOut.PaymentType);
                    cmd.Parameters.AddWithValue("@delivery_date", checkOut.DeliveryDate);
                    cmd.Parameters.AddWithValue("@delivery_schedule", checkOut.DeliveryTime);
                    cmd.Parameters.AddWithValue("@delivery_address", checkOut.DeliveryAddress);
                    cmd.Parameters.AddWithValue("@delivery_location", checkOut.DeliveryLocation);
                    cmd.Parameters.AddWithValue("@delivery_emirate", checkOut.DeliveryEmirate);
                    cmd.Parameters.AddWithValue("@delivery_note", checkOut.DeliveryNote);
                    cmd.Parameters.AddWithValue("@billing_address", checkOut.BillingAddress);
                    cmd.Parameters.AddWithValue("@billing_location", checkOut.BillingLocation);
                    cmd.Parameters.AddWithValue("@billing_emirate", checkOut.BillingEmirate);
                    cmd.Parameters.AddWithValue("@session_ip", string.Empty);
                    cmd.Parameters.AddWithValue("@discount_amount", checkOut.DiscountApplied);
                    cmd.Parameters.AddWithValue("@coupon_applied", checkOut.CouponApplied);
                    cmd.Parameters.AddWithValue("@coupon_code", checkOut.Coupon);
                    cmd.Parameters.AddWithValue("@voucher_applied", checkOut.VoucherApplied);
                    cmd.Parameters.AddWithValue("@voucher_code", checkOut.VoucherCode);
                    cmd.Parameters.AddWithValue("@latitude", checkOut.Latitude);
                    cmd.Parameters.AddWithValue("@longitude", checkOut.Longitude);
                    cmd.Parameters.AddWithValue("@delivery_zone_area", checkOut.DeliveryArea);
                    cmd.Parameters.Add("@return_id", SqlDbType.Int);
                    cmd.Parameters["@return_id"].Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();
                    return (int)cmd.Parameters["@return_id"].Value;
                }
            }

        }

        public int AddSubOrder(OrderItem orderitem)
        {
            int rows = 0;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("insert_order_sub", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@order_id", orderitem.OrderId);
                    cmd.Parameters.AddWithValue("@item_id", orderitem.ItemId);
                    cmd.Parameters.AddWithValue("@item_price", orderitem.Price);
                    cmd.Parameters.AddWithValue("@item_qty", orderitem.Qty);
                    cmd.Parameters.AddWithValue("@item_title", orderitem.ItemName);
                    cmd.Parameters.AddWithValue("@item_price_id", orderitem.PriceId);
                    rows = cmd.ExecuteNonQuery();

                }
            }
            return rows;
        }

        public decimal CheckCouponDiscount(string couponCode, DateTime deliveryDate)
        {
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("check_coupon_validity", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@coupon_code", couponCode);
                    cmd.Parameters.AddWithValue("@delivery_date", deliveryDate);
                    cmd.Parameters.Add("@discount", SqlDbType.Decimal);
                    cmd.Parameters["@discount"].Direction = ParameterDirection.Output;
                    cmd.Parameters["@discount"].Precision = 18;
                    cmd.Parameters["@discount"].Scale = 2;
                    cmd.ExecuteNonQuery();
                    return (decimal)cmd.Parameters["@discount"].Value;
                }
            }

        }

        public decimal CheckVoucherValidity(string voucherCode, DateTime deliveryDate)
        {
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("check_voucher_validity", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@voucher_code", voucherCode);
                    cmd.Parameters.AddWithValue("@delivery_date", deliveryDate);
                    cmd.Parameters.Add("@discount", SqlDbType.Decimal);
                    cmd.Parameters["@discount"].Direction = ParameterDirection.Output;
                    cmd.Parameters["@discount"].Precision = 18;
                    cmd.Parameters["@discount"].Scale = 2;
                    cmd.ExecuteNonQuery();
                    return (decimal)cmd.Parameters["@discount"].Value;
                }
            }

        }

        public bool CheckVAT(int ItemId)
        {
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("CheckVAT", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", ItemId);
                    cmd.Parameters.Add("@VAT", SqlDbType.Bit);
                    cmd.Parameters["@VAT"].Direction = ParameterDirection.Output;
                    cmd.ExecuteScalar();
                    return (bool)cmd.Parameters["@VAT"].Value;
                }
            }

        }
        public bool ValidateReward(string rewardPointName)
        {
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("validate_reward", con))
                {
                    SqlParameter outputParam = new SqlParameter("@reward_exists", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputParam);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@reward_point_name", rewardPointName.Trim());
                    cmd.ExecuteScalar();
                    return (bool)outputParam.Value;
                }
            }

        }
        public int EditOrderStatus(int orderId)
        {
            int rows = 0;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("edit_order_status", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", orderId);
                    rows = cmd.ExecuteNonQuery();
                }
            }
            return rows;
        }
        public int DeleteOrder(int orderId)
        {
            int rows = 0;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("delete_order", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@order_id", orderId);
                    rows = cmd.ExecuteNonQuery();
                }
            }
            return rows;
        }

        public int EditDeliveryStatus(int orderId)
        {
            int rows = 0;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("edit_delivery_status", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", orderId);
                    rows = cmd.ExecuteNonQuery();
                }
            }
            return rows;
        }

        public IList<PurchaseList> GetPurchaseListHistory(int userId)
        {
            IList<PurchaseList> purchaseList = new List<PurchaseList>();
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("usp_GetPurchaseHistory", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var purchase = new PurchaseList();
                            purchase.ItemId = Convert.ToInt32(dr["item_id"].ToString());
                            purchase.UnitPrice = !Common.IsDBNull(dr["item_price"]) ? Convert.ToDecimal(dr["item_price"].ToString()) :
                                decimal.Zero;
                            purchase.ItemQty = !Common.IsDBNull(dr["item_qty"]) ? Convert.ToDecimal(dr["item_qty"].ToString()) :
                                decimal.Zero;
                            purchase.ItemTitle = !Common.IsDBNull(dr["item_title"]) ? dr["item_title"].ToString() :
                                string.Empty;
                            purchase.ItemImage = !Common.IsDBNull(dr["item_image_file"]) ?
                                ConfigurationManager.AppSettings["ProductImage"] + dr["item_image_file"].ToString() :
                                ConfigurationManager.AppSettings["NoImage"];
                            purchase.OrderDate = !Common.IsDBNull(dr["order_date"]) ? dr["order_date"].ToString() : string.Empty;
                            purchase.OrderNo = !Common.IsDBNull(dr["order_no"]) ? dr["order_no"].ToString() : string.Empty;
                            purchase.DeliveryDate = !Common.IsDBNull(dr["delivery_date"]) ? dr["delivery_date"].ToString() : string.Empty;
                            purchaseList.Add(purchase);
                        }
                    }
                }
            }
            return purchaseList;
        }

        public IList<Slot> GetSlotsByArea(int AreaId)
        {
            IList<Slot> slotList = new List<Slot>();
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("load_delivery_area_slots", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", AreaId);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var slot = new Slot();
                            slot.slotId = Convert.ToInt32(dr["slot_id"].ToString());
                            slot.slotTime = !Common.IsDBNull(dr["slot_timing"]) ? dr["slot_timing"].ToString() : string.Empty;
                            slotList.Add(slot);
                        }
                    }
                }
            }
            return slotList;
        }

        public bool CheckSlotAvailability(int slotId, DateTime deliveryDate)
        {
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("check_slot_availability", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@deliverydate", deliveryDate);
                    cmd.Parameters.AddWithValue("@deliveryslot", slotId);
                    cmd.Parameters.Add("@availability", SqlDbType.Bit);
                    cmd.Parameters["@availability"].Direction = ParameterDirection.Output;
                    cmd.ExecuteScalar();
                    return (bool)cmd.Parameters["@availability"].Value;
                }
            }

        }


        public DataTable LoadDataDetails(int id, string spName)
        {

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                SqlDataAdapter _dta = null;
                using (SqlCommand cmd = new SqlCommand(spName, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@id", id);
                    _dta = new SqlDataAdapter(cmd);
                    DataTable _dtable = new DataTable();
                    _dta.Fill(_dtable);
                    return _dtable;
                }
            }


        }

        public string get_order_ref(int orderId)
        {
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("get_order_ref", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@order_id", orderId);
                    cmd.Parameters.Add("@order_ref", SqlDbType.VarChar, 15);
                    cmd.Parameters["@order_ref"].Direction = ParameterDirection.Output;
                    cmd.ExecuteScalar();
                    return (string)cmd.Parameters["@order_ref"].Value;
                }
            }

        }

        public int InsertTransaction(string orderRef, string TransactionNumber)
        {
            int rows = 0;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("insert_transaction", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@order_ref", orderRef);
                    cmd.Parameters.AddWithValue("@transaction_no", TransactionNumber);
                    rows = cmd.ExecuteNonQuery();
                }
            }
            return rows;
        }
        public int UpdateTransaction(string orderRef, string transactionid, int responsecode, string uniqueid, string approvalcode, decimal amount, decimal balance)
        {

            int rows = 0;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("update_transaction", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@order_ref", orderRef);
                    cmd.Parameters.AddWithValue("@transaction_no", transactionid);
                    cmd.Parameters.AddWithValue("@response_code", responsecode);
                    cmd.Parameters.AddWithValue("@unique_id", uniqueid);
                    cmd.Parameters.AddWithValue("@approval_code", approvalcode);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@balance", balance);
                    rows = cmd.ExecuteNonQuery();
                }
            }
            return rows;


        }
        public IList<DeliveryArea> GetAllArea()
        {
            IList<DeliveryArea> areaList = new List<DeliveryArea>();
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("load_delivery_zone_area_all", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var area = new DeliveryArea();
                            area.AreaId = Convert.ToInt32(dr["area_id"].ToString());
                            area.AreaName = !Common.IsDBNull(dr["area_name"]) ? dr["area_name"].ToString() : string.Empty;
                            areaList.Add(area);
                        }
                    }
                }
            }
            return areaList;
        }

        public string PaymentRegistration(string _amount, string OrderId, string OrderInfo)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string URL = ConfigurationManager.AppSettings["PaymentGateway"];
            string InputJSON = "{\"Registration\":{\"Customer\":\"Demo Merchant\",\"Channel\":\"Web\",\"Amount\":\"" + _amount + "\",\"Currency\":\"AED\",\"OrderID\":" + OrderId + ",\"OrderName\":\"BARAKATFRESH\",\"OrderInfo\":" + OrderInfo + ",\"TransactionHint\":\"CPT:Y;VCC:Y;\",\"ReturnPath\":\"" + ConfigurationManager.AppSettings["ReturnURLPayment"] + "" + OrderId + "\"}}";
            try
            {


                X509Certificate2 certificate = new X509Certificate2(System.Web.Hosting.HostingEnvironment.MapPath("~/Certificates/Demo Merchant.pfx"), "Comtrust");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(URL));
                byte[] lbPostBuffer = ASCIIEncoding.ASCII.GetBytes(InputJSON);
                request.ClientCertificates.Add(certificate);
                request.UserAgent = ".NET Framework Test Client";
                request.Accept = "text/xml-standard-api";
                request.Method = "POST";
                request.ContentLength = lbPostBuffer.Length;
                request.ContentType = "application/json";
                request.Timeout = 600000;
                request.ClientCertificates.Add(certificate);
                HttpWebResponse response;
                Stream loPostData = request.GetRequestStream();
                loPostData.Write(lbPostBuffer, 0, lbPostBuffer.Length);
                loPostData.Close();
                response = (HttpWebResponse)request.GetResponse();
                Encoding enc = Encoding.GetEncoding(1252);
                StreamReader loResponseStream = new StreamReader(response.GetResponseStream(), enc);
                string result = loResponseStream.ReadToEnd();
                dynamic jsonObj = JObject.Parse(result);
                string Id = jsonObj.Transaction.TransactionID;
                InsertTransaction(OrderInfo, Id);
                response.Close();
                loResponseStream.Close();
                return Id;
            }
            catch (Exception e)
            {

                return e.InnerException.ToString();
            }


        }
        public string PaymentResponseResult(string _transactionid, string _orderId)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string URL = ConfigurationManager.AppSettings["PaymentGateway"];
            string InputJSON = "{\"Finalization\":{\"Customer\":\"BARAKAT VEGETABLE FRUITS\",\"TransactionID\":\"" + _transactionid + "\"}}";
            try
            {
                X509Certificate2 certificate = new X509Certificate2(System.Web.Hosting.HostingEnvironment.MapPath("~/Certificates/Demo Merchant.pfx"), "Comtrust");
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(new Uri(URL));
                byte[] lbPostBuffer = ASCIIEncoding.ASCII.GetBytes(InputJSON);
                request.ClientCertificates.Add(certificate);
                request.UserAgent = ".NET Framework Test Client";
                request.Accept = "text/xml-standard-api";
                request.Method = "POST";
                request.ContentLength = lbPostBuffer.Length;
                request.ContentType = "application/json";
                request.Timeout = 600000;
                request.ClientCertificates.Add(certificate);
                HttpWebResponse response;
                Stream loPostData = request.GetRequestStream();
                loPostData.Write(lbPostBuffer, 0, lbPostBuffer.Length);
                loPostData.Close();
                response = (HttpWebResponse)request.GetResponse();
                Encoding enc = Encoding.GetEncoding(1252);
                StreamReader loResponseStream = new StreamReader(response.GetResponseStream(), enc);
                string result = loResponseStream.ReadToEnd();
                dynamic jsonObj = JObject.Parse(result);
                string Id = jsonObj.Transaction.ResponseCode;
                response.Close();
                loResponseStream.Close();
                string _Uid = jsonObj.Transaction.UniqueID;
                string _ApprovalCode = jsonObj.Transaction.ApprovalCode;
                string _Amount = jsonObj.Transaction.Amount.Value;
                string _Balance = jsonObj.Transaction.Balance.Value;
                UpdateTransaction(_orderId, _transactionid, Convert.ToInt32(Id), _Uid ?? "", _ApprovalCode ?? "", Convert.ToDecimal(_Amount), Convert.ToDecimal(_Balance));

                return Id;
            }
            catch (Exception e)
            {
                return e.ToString();
            }


        }


    }
}
