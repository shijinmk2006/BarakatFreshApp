using DataAccess;
using DataEntity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace BarakatFresh.Controllers
{
    public class OrderController : ApiController
    {
        DALOrder dalOrder = null;

        public OrderController()
        {
            dalOrder = new DALOrder();

        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetZone()
        {
            IList<DeliveryZone> zones = await Task.Run(() => dalOrder.GetZones());
            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { data = zones }), Encoding.UTF8, "application/json")
            };

        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetZoneArea(int zoneId)
        {
            IList<DeliveryArea> deliveryArea = await Task.Run(() => dalOrder.GetZoneArea(zoneId));
            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { data = deliveryArea }), Encoding.UTF8, "application/json")
            };

        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetSlotByArea(int AreaId)
        {
            IList<Slot> slotList = await Task.Run(() => dalOrder.GetSlotsByArea(AreaId));
            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { data = slotList }), Encoding.UTF8, "application/json")
            };

        }
        [HttpGet]
        public async Task<HttpResponseMessage> GetAllArea()
        {
            IList<DeliveryArea> areaList = await Task.Run(() => dalOrder.GetAllArea());
            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { data = areaList }), Encoding.UTF8, "application/json")
            };

        }
        [HttpPost]
        public async Task<HttpResponseMessage> AddOrder([FromBody] CheckOut checkout)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int orderId = await Task.Run(() => dalOrder.AddOrder(checkout));
                    string orderref = string.Empty;

                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(new { data = orderId, data1 = orderref, status = 1 }), Encoding.UTF8, "application/json")
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
            catch (Exception)
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = 0, error = "An Error Occured", status = 0 }), Encoding.UTF8, "application/json")
                };
            }

        }

        [HttpPost]
        public async Task<HttpResponseMessage> AddSubOrder(OrderItem orderItem)
        {
            try
            {
                int returnId = await Task.Run(() => dalOrder.AddSubOrder(orderItem));

                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = returnId }), Encoding.UTF8, "application/json")
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
        [HttpGet]
        public async Task<HttpResponseMessage> CheckValidCouponDiscount(string couponCode, DateTime deliveryDate)
        {
            decimal discount = await Task.Run(() => dalOrder.CheckCouponDiscount(couponCode, deliveryDate));
            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { data = discount }), Encoding.UTF8, "application/json")
            };

        }

        [HttpGet]
        public async Task<HttpResponseMessage> CheckValidVoucherDiscount(string voucherCode, DateTime deliveryDate)
        {
            decimal discount = await Task.Run(() => dalOrder.CheckVoucherValidity(voucherCode, deliveryDate));
            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { data = discount }), Encoding.UTF8, "application/json")
            };

        }

        [HttpGet]
        public async Task<HttpResponseMessage> CheckVAT(int ItemId)
        {
            bool vat = await Task.Run(() => dalOrder.CheckVAT(ItemId));
            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { data = vat }), Encoding.UTF8, "application/json")
            };

        }

        [HttpGet]
        public async Task<HttpResponseMessage> CheckSlotAvailability(int slotId, DateTime deliveryDate)
        {
            bool slotAvailable = await Task.Run(() => dalOrder.CheckSlotAvailability(slotId, deliveryDate));
            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { data = slotAvailable }), Encoding.UTF8, "application/json")
            };

        }
        [HttpGet]
        public async Task<HttpResponseMessage> CheckRewardPointsExists(string rewardPointName)
        {
            bool rewardExists = false;
            if (!string.IsNullOrEmpty(rewardPointName))
            {
                rewardExists = await Task.Run(() => dalOrder.ValidateReward(rewardPointName));
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = rewardExists }), Encoding.UTF8, "application/json")
                };
            }
            else
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = rewardExists, error = "Invalid Reward" }), Encoding.UTF8, "application/json")
                };
            }
        }
        [HttpPost]
        public async Task<HttpResponseMessage> EditOrderStatus(int OrderId)
        {
            int rows = await Task.Run(() => dalOrder.EditOrderStatus(OrderId));
            if (rows > 0)
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = rows }), Encoding.UTF8, "application/json")
                };
            else
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = rows, error = "Editing order failed" }), Encoding.UTF8, "application/json")
                };

        }
        [HttpPost]
        public async Task<HttpResponseMessage> DeleteOrder([FromBody] Order order)
        {
            int rows = await Task.Run(() => dalOrder.DeleteOrder(order.OrderId));
            if (rows > 0)
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = rows }), Encoding.UTF8, "application/json")
                };
            else
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = rows, error = "Cancelling order failed" }), Encoding.UTF8, "application/json")
                };

        }

        [HttpPost]
        public async Task<HttpResponseMessage> EditDeliveryStatus(int OrderId)
        {
            int rows = await Task.Run(() => dalOrder.EditDeliveryStatus(OrderId));
            if (rows > 0)
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = rows }), Encoding.UTF8, "application/json")
                };
            else
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = rows, error = "Editing Delivery failed" }), Encoding.UTF8, "application/json")
                };

        }

        [HttpPost]
        public async Task<HttpResponseMessage> InsertTransaction(string orderRef, string transactionNumber)
        {
            int rows = await Task.Run(() => dalOrder.InsertTransaction(orderRef, transactionNumber));
            if (rows > 0)
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = rows }), Encoding.UTF8, "application/json")
                };
            else
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = rows, error = "Transaction failed" }), Encoding.UTF8, "application/json")
                };

        }

        [HttpPost]
        public async Task<HttpResponseMessage> PaymentRegistration([FromBody] Payment order)
        {
            string returnUrl = await Task.Run(() => dalOrder.PaymentRegistration(order.Amount, order.OrderId, order.OrderRef));
            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { data = returnUrl }), Encoding.UTF8, "application/json")
            };

        }

        [HttpPost]
        public async Task<HttpResponseMessage> UpdateTransaction([FromBody] Transaction transaction)
        {
            string returnUrl = await Task.Run(() => dalOrder.PaymentResponseResult(transaction.TransactionId, transaction.OrderId));
            try
            {
                if (Convert.ToInt16(returnUrl) >= 0)
                {
                    send_order_confirmation(Convert.ToInt32(transaction.OrderId), transaction.Email);
                    string orderref = dalOrder.get_order_ref(Convert.ToInt32(transaction.OrderId));
                    Utility.Common.send_sms_notification(transaction.Mobile, transaction.CustomerName, orderref);
                }
                else
                {
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(new { data = returnUrl, error = "An error occured.Incorrect credentials" }), Encoding.UTF8, "application/json")
                    };
                }
            }
            catch (Exception)
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = returnUrl, error = "An error occured.Please try again" }), Encoding.UTF8, "application/json")
                };
            }
            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { data = returnUrl, error = "An error occured.Please try again" }), Encoding.UTF8, "application/json")
            };

        }


        public void send_order_confirmation(int id, string _email)
        {

            DataTable _dtmain = dalOrder.LoadDataDetails(id, "load_delivery_info");
            DataTable _dtitem = dalOrder.LoadDataDetails(id, "load_order_details");
            SmtpClient _myclient = new SmtpClient("mail.webaapsmanager.com");
            _myclient.EnableSsl = false;
            _myclient.Port = 587;
            _myclient.Credentials = new System.Net.NetworkCredential("barakatonline@webaapsmanager.com", "Mail#cc0028");
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("\"BARAKAT FRESH\"<no-reply@barakatfresh.ae>");
            msg.To.Add(new MailAddress(_email));
            msg.Priority = MailPriority.High;
            msg.IsBodyHtml = true;
            msg.Subject = "Your Order Ref is #" + _dtmain.Rows[0]["order_ref"].ToString();
            decimal _cart_total = Convert.ToDecimal(_dtmain.Rows[0]["cart_total"].ToString());
            decimal _vat = decimal.Round((_cart_total / 105) * 5, 2);
            string _items = "";
            foreach (DataRow _drow in _dtitem.Rows)
            {
                decimal _total = decimal.Round(Convert.ToDecimal(_drow["Total"].ToString()), 2);
                string _qty = _drow["item_qty"].ToString() + " X " + _drow["Eq_Qty"].ToString() + _drow["item_unit"].ToString();
                //_items = _items + "<tr><td>" + _drow["item_title"].ToString() + "</td><td>" + _drow["item_price"].ToString() + "</td><td>" + _qty + "</td><td class='alignright'>AED " + _total.ToString() + "</td></tr>";
                _items = _items + "<tr style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;'><td style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;vertical-align:top;padding-top:5px;padding-bottom:5px;padding-right:0;padding-left:0;border-top-width:1px;border-top-style:solid;border-top-color:#eee;width:55%!important;text-align:left;'>" + _drow["item_title"].ToString() + "</td><td style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;vertical-align:top;padding-top:5px;padding-bottom:5px;padding-right:0;padding-left:0;border-top-width:1px;border-top-style:solid;border-top-color:#eee;width:15%;text-align:center!important;' class='aligncenter' >AED " + _drow["item_price"].ToString() + "</td><td style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;vertical-align:top;padding-top:5px;padding-bottom:5px;padding-right:0;padding-left:0;border-top-width:1px;border-top-style:solid;border-top-color:#eee;width:15%;text-align:center!important;' class='aligncenter' >" + _qty + "</td><td class='alignright' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;vertical-align:top;padding-top:5px;padding-bottom:5px;padding-right:0;padding-left:0;border-top-width:1px;border-top-style:solid;border-top-color:#eee;width:15%;text-align:right!important;'>AED " + _total.ToString() + "</td></tr>";
            }
            //_items = _items + "<p>Above invoice inclusive VAT @5% " + _vat.ToString() + "</p>";
            //string _user_confirmation_content = "<table style='width:800px;font-family:Verdana;font-size:11px;' border='0' cellpadding='0' cellspacing='0'><tr ><td style='border-bottom:1px solid #ddd;'><img src='http://bq.webaaps.com/Content/image/barakat-fresh-logo.png' alt='' /></td></tr><tr><td><p style='font-size:18px;color:#5ebe29;margin-bottom:0px;'>Thank you for Shopping on Barakat Fresh!</p><p style='margin:3px'>Your order has been placed and expected delivery on " + _dtmain.Rows[0]["delivery_date"].ToString() + ", " + _dtmain.Rows[0]["delivery_schedule"].ToString() + ". You will be notified when the courier picks it up!</p></td></tr><tr><td style='border:1px solid #ddd;padding:10px;background-color:#f5f5f5'><p style='margin:3px'><b>Here is Your Order Information</b></p><p style='margin:3px'>Order Number/Ref.:<b>" + _dtmain.Rows[0]["order_ref"].ToString() + "</b></p><p style='margin:3px'>Payment Method: " + _dtmain.Rows[0]["payment_method"].ToString() + "</p><p style='margin:3px'>Total Amount: " + _cart_total.ToString() + " AED</p></td></tr><tr><td></td></tr><tr><td style='border-top:1px solid #ddd;padding:10px;background-color:#f5f5f5'><b> Your Order Details</b></td></tr><tr> <td style='padding-top:10px;'><table style='width:800px;border-collapse:collapse;border:1px solid #ddd;' ><tr style='border:1px solid #ddd;background-color:#f5f5f5'><td style='border:1px solid #ddd;padding:10px;width:420px; font-weight:bold'>Item Description</td><td style='border:1px solid #ddd;padding:10px;text-align:right;width:100px;font-weight:bold'>Unit Price</td><td style='border:1px solid #ddd;padding:10px;text-align:center;width:100px;font-weight:bold'>Qty</td><td style='border:1px solid #ddd;padding:10px;text-align:right;width:100px;font-weight:bold'>Total</td></tr>" + _items + "<tr style='border:1px solid #ddd;background-color:#fff'><td style='border:1px solid #ddd;padding:10px;text-align:right;font-weight:bold' colspan='3'>Sub Total</td><td style='border:1px solid #ddd;padding:10px;text-align:right;font-weight:bold'>" + _dtmain.Rows[0]["sub_total"].ToString() + "</td></tr><tr style='border:1px solid #ddd;background-color:#fff'><td style='border:1px solid #ddd;padding:10px;text-align:right;font-weight:bold' colspan='3'>Discount</td><td style='border:1px solid #ddd;padding:10px;text-align:right;font-weight:bold'>" + _dtmain.Rows[0]["discount_amount"].ToString() + "</td></tr><tr style='border:1px solid #ddd;background-color:#fff'><td style='border:1px solid #ddd;padding:10px;text-align:right;font-weight:bold' colspan='3'>Total Incl. VAT</td><td style='border:1px solid #ddd;padding:10px;text-align:right;font-weight:bold'>" + _cart_total.ToString() + "</td></tr><tr style='border:1px solid #ddd;background-color:#fff'><td style='border:1px solid #ddd;padding:10px;text-align:right;' colspan='4'>The above invoice incl. VAT @5% = " + _vat.ToString() + "AED</td></tr></table><br/></td></tr><tr><td style='border-top:1px solid #ddd;padding:10px;background-color:#f5f5f5'><b> Your Order will be Delivered to</b> </td></tr><tr><td style='padding-top:10px;'><p style='margin:3px'><b>Delivery Address</b></p><p style='margin:3px'><b>" + _dtmain.Rows[0]["customer_name"].ToString() + "</b></p><p style='margin:3px'>" + _dtmain.Rows[0]["delivery_address"].ToString() + "</p><p style='margin:3px'>" + _dtmain.Rows[0]["delivery_location"].ToString() + "</p><p style='margin:3px'>" + _dtmain.Rows[0]["delivery_emirate"].ToString() + "</p><p style='margin:3px'>" + _dtmain.Rows[0]["customer_mobile"].ToString() + "<br/>" + _dtmain.Rows[0]["customer_email"].ToString() + "</p></td></tr></table>";


            //string _htmlmessage = "<!DOCTYPE HTML PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'><html xmlns='http://www.w3.org/1999/xhtml'><head><meta name='viewport' content='width=device-width' /><meta http-equiv='Content-Type' content='text/html; charset=UTF-8' /><title>Order Confirmation - Barakat Fresh</title><link href='https://bq.webaaps.com/Content/emailtemplate.css' media='all' rel='stylesheet' type='text/css' /></head><body><table class='body-wrap'><tr><td></td><td class='container' width='800'><div class='content'><table class='main' width='100%' cellpadding='0' cellspacing='0'><tr><td class='content-wrap'><table cellpadding='0' cellspacing='0'><tr><td class='aligncenter'><img class='img-responsive' src='https://bq.webaaps.com/Content/image/barakat-fresh-logo-email.jpg' /></td></tr><tr><td class='content-block'><h3 style='color:#009d3f'>Thank you for Shopping on Barakat Fresh!</h3><p>Your order has been accepted and delivery scheduled for <strong> " + _dtmain.Rows[0]["delivery_date"].ToString() + ", " + _dtmain.Rows[0]["delivery_schedule"].ToString() + "</strong>. You will be notified when the courier picks it up!</p></td></tr><tr><td class='content-block'>Order Number/Ref.: <strong>" + _dtmain.Rows[0]["order_ref"].ToString() + " </strong><br />Payment Method: <strong>" + _dtmain.Rows[0]["payment_method"].ToString() + "</strong><br />Total Amount: <strong>AED " + _cart_total.ToString() + "</strong></td></tr><tr><td class='content-block'><table class='invoice'><tr><td><table class='invoice-items' cellpadding='0' cellspacing='0'><tr class='heading'><td>Item Description</td><td>Price</td><td>Quantity</td><td>Total</td></tr>" + _items + "<tr class='sub-total'><td class='alignright' width='80%' colspan='3'>Sub Total</td><td class='alignright'>AED " + _dtmain.Rows[0]["sub_total"].ToString() + "</td></tr><tr class='sub-total'><td class='alignright' width='80%' colspan='3'>Discount</td><td class='alignright'>AED " + _dtmain.Rows[0]["discount_amount"].ToString() + "</td></tr><tr class='total'><td class='alignright' width='80%' colspan='3'>Total</td><td class='alignright'>AED " + _cart_total.ToString() + "</td></tr></table></td></tr></table></td></tr><tr><td class='content-block'><strong>Delivery Address</strong><br />" + _dtmain.Rows[0]["customer_name"].ToString() + "<br />" + _dtmain.Rows[0]["delivery_address"].ToString() + "<br />" + _dtmain.Rows[0]["delivery_location"].ToString() + "<br />" + _dtmain.Rows[0]["delivery_emirate"].ToString() + "<br />" + _dtmain.Rows[0]["customer_mobile"].ToString() + "<br />" + _dtmain.Rows[0]["customer_email"].ToString() + "</td></tr><tr><td class='content-block' style='color:#555555;'><i>NEED HELP?</i> Call <strong>800 BARAKAT (2272528)</strong> | Email info@barakatfresh.ae</td></tr></table></td></tr></table><div class='footer'> <table width='100%'><tr><td class='aligncenter content-block'>Copyright © 2018. Barakat Vegetables &amp; Fruits Co. (L.L.C.). All Rights Reserved.</td></tr></table></div></div></td><td></td></tr></table></body></html>";

            string _htmlmessage = "<!DOCTYPE HTML PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'><html xmlns='http://www.w3.org/1999/xhtml' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;'><head style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;'><meta name='viewport' content='width=device-width' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;' /><meta http-equiv='Content-Type' content='text/html; charset=UTF-8' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;' /><title style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;'>Order Confirmation - Barakat Fresh</title><style type='text/css' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;'> * { margin: 0;padding: 0;font-family: 'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing: border-box; font-size: 13px; } img { max-width: 100%;} body {-webkit-font-smoothing: antialiased;-webkit-text-size-adjust: none; width: 100% !important; height: 100%; line-height: 1.6;} table td { vertical-align: top; }body {background-color: #f6f6f6;}.body-wrap {background-color: #f6f6f6;            width: 100%;}.container {display: block !important; max-width: 700px !important; margin: 0 auto !important; clear: both !important;}.content { max-width: 700px; margin: 0 auto;display: block;            padding: 20px;}.main {background: #fff;border: 1px solid #e9e9e9;border-radius: 3px;}.content-wrap {padding: 20px;}.content-block {padding: 0 0 20px; }.header { width: 100%; margin-bottom: 20px; } .footer { width: 100%; clear: both; color: #999;padding: 20px; }.footer a {color: #999; }.footer p, .footer a, .footer unsubscribe, .footer td { font-size: 12px;} h1, h2, h3 { font-family: 'Helvetica Neue', Helvetica, Arial, 'Lucida Grande', sans-serif; color: #009d3f;margin: 40px 0 0; line-height: 1.2;font-weight: 400; }  h1 { font-size: 32px; font-weight: 500;} h2 { font-size: 24px; }h3 { font-size: 18px; } h4 {font-size: 14px; font-weight: 600; }p, ul, ol { margin-bottom: 10px; font-weight: normal; } p li, ul li, ol li {margin-left: 5px; list-style-position: inside; } a {color: #1ab394;            text-decoration: underline; } .btn-primary {text-decoration: none;color: #FFF;background-color: #1ab394; border: solid #1ab394; border-width: 5px 10px; line-height: 2; font-weight: bold; text-align: center;            cursor: pointer; display: inline-block; border-radius: 5px; text-transform: capitalize;} .last {margin-bottom: 0;}.first {margin-top: 0; } .aligncenter {text-align: center;}.alignright {text-align: right !important;}.alignleft {text-align: left;}.clear {clear: both;}.alert { font-size: 16px; color: #fff; font-weight: 500; padding: 20px; text-align: center; border-radius: 3px 3px 0 0; }.alert a { color: #fff; text-decoration: none;  font-weight: 500; font-size: 16px; }.alert.alert-warning { background: #f8ac59; }.alert.alert-bad { background: #ed5565;}.alert.alert-good {background: #1ab394;} .invoice { margin: 0px auto; text-align: left; width: 100%; } .invoice td { padding: 5px 0; } .invoice .invoice-items { width: 100%; }.invoice .invoice-items tr.heading {font-weight: 600;} .invoice .invoice-items td { border-top: #eee 1px solid; } .invoice .invoice-items td:first-child {  width: 55%; text-align: left;  }.invoice .invoice-items td:last-child { width: 55%;  text-align: right;  } .invoice .invoice-items .total td { border-top: 2px solid #333; border-bottom: 2px solid #333;  font-weight: 700;  }.invoice .invoice-items .sub-total td { font-weight: 600; } @media only screen and (max-width: 640px) {  h1, h2, h3, h4 { font-weight: 600 !important; margin: 20px 0 5px !important; } h1 { font-size: 22px !important;} h2 {font-size: 18px !important;  }   h3 { font-size: 16px !important; } .container { width: 100% !important;  } .content, .content-wrap {padding: 10px !important; } .invoice { width: 100% !important;  } }</style></head><body style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;-webkit-font-smoothing:antialiased;-webkit-text-size-adjust:none;width:100% !important;height:100%;line-height:1.6;background-color:#f6f6f6;'><table class='body-wrap' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;background-color:#f6f6f6;width:100%;'><tr style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;'><td style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;vertical-align:top;'></td> <td class='container' width='800' style='padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;vertical-align:top;display:block !important;max-width:700px !important;margin-top:0 !important;margin-bottom:0 !important;margin-right:auto !important;margin-left:auto !important;clear:both !important;'><div class='content' style='font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;max-width:700px;margin-top:0;margin-bottom:0;margin-right:auto;margin-left:auto;display:block;padding-top:20px;padding-bottom:20px;padding-right:20px;padding-left:20px;'> <table class='main' width='100%' cellpadding='0' cellspacing='0' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;background-color:#fff;background-image:none;background-repeat:repeat;background-position:top left;background-attachment:scroll;border-width:1px;border-style:solid;border-color:#e9e9e9;border-radius:3px;'>  <tr style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;'> <td class='content-wrap' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;vertical-align:top;padding-top:20px;padding-bottom:20px;padding-right:20px;padding-left:20px;'>  <table cellpadding='0' cellspacing='0' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;'><tr style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;'> <td class='aligncenter' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;vertical-align:top;text-align:center;'><img class='img-responsive' src='https://barakatfresh.ae/Content/image/barakat-fresh-logo-email.jpg' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;max-width:100%;' /> </td> </tr><tr style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;'><td class='content-block' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;vertical-align:top;padding-top:0;padding-bottom:20px;padding-right:0;padding-left:0;'> <h3 style='padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;box-sizing:border-box;color:#009d3f!important;font-family:'Helvetica Neue', Helvetica, Arial, 'Lucida Grande', sans-serif;margin-top:40px;margin-bottom:0;margin-right:0;margin-left:0;line-height:1.2;font-weight:400;font-size:18px;'>Thank you for Shopping on Barakat Fresh!</h3><p style='margin-top:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;margin-bottom:10px;font-weight:normal;'>Your order has been accepted and delivery scheduled for <strong style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;'> " + _dtmain.Rows[0]["delivery_date"].ToString() + ", " + _dtmain.Rows[0]["delivery_schedule"].ToString() + " </strong>. You will be notified when your order is out for delivery!</p></td> </tr><tr style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;'> <td class='content-block' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;vertical-align:top;padding-top:0;padding-bottom:20px;padding-right:0;padding-left:0;'>Order Number/Ref.: <strong style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;'> " + _dtmain.Rows[0]["order_ref"].ToString() + " </strong><br style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;' />Payment Method: <strong style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;'>" + _dtmain.Rows[0]["payment_method"].ToString() + "</strong><br style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;' />Total Amount: <strong style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;'>AED " + _cart_total.ToString() + "</strong></td></tr> <tr style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;'><td class='content-block' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;vertical-align:top;padding-top:0;padding-bottom:20px;padding-right:0;padding-left:0;'><table class='invoice' style='padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;margin-top:0px;margin-bottom:0px;margin-right:auto;margin-left:auto;text-align:left;width:100%;'><tr style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;'><td style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;vertical-align:top;padding-top:5px;padding-bottom:5px;padding-right:0;padding-left:0;'><table class='invoice-items' cellpadding='0' cellspacing='0' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;width:100%;'><tr class='heading' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;font-weight:600;'><td style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;vertical-align:top;padding-top:5px;padding-bottom:5px;padding-right:0;padding-left:0;border-top-width:1px;border-top-style:solid;border-top-color:#eee;width:55%!important;text-align:left!important;'>Item Description</td><td style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;vertical-align:top;padding-top:5px;padding-bottom:5px;padding-right:0;padding-left:0;border-top-width:1px;border-top-style:solid;border-top-color:#eee;width:15%!important;text-align:center!important;' class='aligncenter'>Price</td> <td style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;vertical-align:top;padding-top:5px;padding-bottom:5px;padding-right:0;padding-left:0;border-top-width:1px;border-top-style:solid;border-top-color:#eee;width:15%;text-align:center!important;' class='aligncenter'>Quantity</td> <td style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;vertical-align:top;padding-top:5px;padding-bottom:5px;padding-right:0;padding-left:0;border-top-width:1px;border-top-style:solid;border-top-color:#eee;width:15%!important;text-align:right!important;' class='alignright' >Total</td> </tr>" + _items + "<tr class='sub-total' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;'> <td class='alignright' width='80%' colspan='3' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;vertical-align:top;padding-top:5px;padding-bottom:5px;padding-right:0;padding-left:0;border-top-width:1px;border-top-style:solid;border-top-color:#eee;width:15%;text-align:right!important;font-weight:600;'>Sub Total</td>  <td class='alignright' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;vertical-align:top;padding-top:5px;padding-bottom:5px;padding-right:0;padding-left:0;border-top-width:1px;border-top-style:solid;border-top-color:#eee;width:15%;text-align:right!important;font-weight:600;'>AED " + _dtmain.Rows[0]["sub_total"].ToString() + "</td> </tr> <tr class='sub-total' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;'> <td class='alignright' width='80%' colspan='3' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;vertical-align:top;padding-top:5px;padding-bottom:5px;padding-right:0;padding-left:0;border-top-width:1px;border-top-style:solid;border-top-color:#eee;width:15%;text-align:right!important;font-weight:600;'>Discount</td> <td class='alignright' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;vertical-align:top;padding-top:5px;padding-bottom:5px;padding-right:0;padding-left:0;border-top-width:1px;border-top-style:solid;border-top-color:#eee;width:15%;text-align:right!important;font-weight:600;'>AED " + _dtmain.Rows[0]["discount_amount"].ToString() + "</td></tr> <tr class='total' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;'> <td class='alignright' width='80%' colspan='3' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;vertical-align:top;padding-top:5px;padding-bottom:5px;padding-right:0;padding-left:0;width:15%;text-align:right!important;border-top-width:2px;border-top-style:solid;border-top-color:#333;border-bottom-width:2px;border-bottom-style:solid;border-bottom-color:#333;font-weight:700;'>Total</td><td class='alignright' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;vertical-align:top;padding-top:5px;padding-bottom:5px;padding-right:0;padding-left:0;width:15%;text-align:right!important;border-top-width:2px;border-top-style:solid;border-top-color:#333;border-bottom-width:2px;border-bottom-style:solid;border-bottom-color:#333;font-weight:700;'>AED " + _cart_total.ToString() + "</td></tr> </table><p style='text-align:right!important;'>Above invoice inclusive VAT @5% " + _vat.ToString() + "</p></td>  </tr> </table></td></tr> <tr style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;'> <td class='content-block' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;vertical-align:top;padding-top:0;padding-bottom:20px;padding-right:0;padding-left:0;'> <strong style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;'>Delivery Address</strong><br style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;' />" + _dtmain.Rows[0]["customer_name"].ToString() + "<br style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;' />" + _dtmain.Rows[0]["delivery_zone"].ToString() + "<br style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;' />" + _dtmain.Rows[0]["delivery_address"].ToString() + "<br style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;' />" + _dtmain.Rows[0]["delivery_location"].ToString() + "<br style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;' />" + _dtmain.Rows[0]["delivery_emirate"].ToString() + "<br style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;' />" + _dtmain.Rows[0]["customer_mobile"].ToString() + "<br style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;' />" + _dtmain.Rows[0]["customer_email"].ToString() + "</td> </tr> <tr style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;'> <td class='content-block aligncenter' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;vertical-align:top;padding-top:0;padding-bottom:20px;padding-right:0;padding-left:0;text-align:center;'></td> </tr> <tr style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;'><td class='content-block' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;color:#555555;vertical-align:top;padding-top:0;padding-bottom:20px;padding-right:0;padding-left:0;'><i style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;'>NEED HELP?</i> Call <strong style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;'>04 320 3060</strong> | Email info@barakatfresh.ae </td></tr></table> </td></tr></table><div class='footer' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;width:100%;clear:both;color:#999;padding-top:20px;padding-bottom:20px;padding-right:20px;padding-left:20px;'><table width='100%' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;'><tr style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;'><td class='aligncenter content-block' style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;vertical-align:top;padding-top:0;padding-bottom:20px;padding-right:0;padding-left:0;text-align:center;font-size:12px;'>Copyright © 2018. Barakat Vegetables &amp; Fruits Co. (L.L.C.). All Rights Reserved.</td> </tr> </table> </div> </div></td><td style='margin-top:0;margin-bottom:0;margin-right:0;margin-left:0;padding-top:0;padding-bottom:0;padding-right:0;padding-left:0;font-family:'Helvetica Neue', 'Helvetica', Helvetica, Arial, sans-serif;box-sizing:border-box;font-size:13px;vertical-align:top;'></td></tr></table></body></html>";
            msg.Body = _htmlmessage;
            _myclient.Send(msg);

        }

    }
}