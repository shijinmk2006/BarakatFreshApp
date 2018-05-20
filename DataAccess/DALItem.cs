using DataEntity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Utility;

namespace DataAccess
{
    public class DALItem
    {
        private readonly string conString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

        public IList<QuikKartItems> GetQuikItems()
        {
            IList<QuikKartItems> cartItem = new List<QuikKartItems>();
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("load_quickcart_item", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var discountItem = new QuikKartItems();
                            discountItem.CartId = Convert.ToInt32(dr["quick_cart_id"].ToString());
                            discountItem.CartImage = ConfigurationManager.AppSettings["QuickCart"] + dr["quick_cart_image"].ToString();
                            discountItem.GroupId = Convert.ToInt32(dr["group_id"].ToString());
                            cartItem.Add(discountItem);
                        }
                    }
                }
            }
            return cartItem;
        }
        public IList<RecommendedItems> GetRecommendedItems(int groupId)
        {
            IList<RecommendedItems> recommended = new List<RecommendedItems>();
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("load_recommended", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", groupId);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var recommendedItem = new RecommendedItems();
                            recommendedItem.ItemId = Convert.ToInt32(dr["item_id"].ToString());
                            recommendedItem.Title = !Utility.Common.IsDBNull(dr["ecom_item_name_en"]) ? dr["ecom_item_name_en"].ToString() : string.Empty;
                            recommendedItem.ImagePath = ConfigurationManager.AppSettings["ProductImage"] + dr["item_image"].ToString();
                            recommendedItem.Description = Convert.ToString(dr["item_description_en"]);
                            recommendedItem.Origin = Convert.ToString(dr["item_origin"]);
                            recommendedItem.NoOfVisit = Convert.ToString(dr["no_of_visit"]);
                            recommended.Add(recommendedItem);
                        }
                    }
                }
            }
            return recommended;
        }
        public IList<Item> LoadItemGroupBasedList(int groupId, int level)
        {
            IList<Item> Item = new List<Item>();
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("load_master_item_category_based_list", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", groupId);
                    cmd.Parameters.AddWithValue("@level", level);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var objItem = new Item();
                            objItem.Id = Convert.ToInt32(dr["item_id"].ToString());
                            string _Title = !Utility.Common.IsDBNull(dr["ecom_item_name_en"]) ? dr["ecom_item_name_en"].ToString() : string.Empty;
                            if (_Title.Length > 23)
                                objItem.Title = _Title.Substring(0, 20) + "...";
                            else
                                objItem.Title = _Title;
                            objItem.Price = !Utility.Common.IsDBNull(dr["item_price"]) ? Convert.ToDecimal(dr["item_price"].ToString()) : decimal.Zero;
                            objItem.Image = !Utility.Common.IsDBNull(dr["item_image"]) ? ConfigurationManager.AppSettings["ProductImage"] + dr["item_image"].ToString() : ConfigurationManager.AppSettings["NoImage"];
                            objItem.Description = !Utility.Common.IsDBNull(dr["item_description_en"]) ? dr["item_description_en"].ToString() : string.Empty;
                            objItem.Origin = !Utility.Common.IsDBNull(dr["item_origin"]) ? dr["item_origin"].ToString() : string.Empty;
                            objItem.Group = Convert.ToInt32(dr["main_group_id"].ToString());
                            objItem.Unit = !Utility.Common.IsDBNull(dr["item_unit"]) ? dr["item_unit"].ToString() : string.Empty;
                            objItem.Discount = !Utility.Common.IsDBNull(dr["item_discount"]) ? dr["item_discount"].ToString() : string.Empty;
                            if (dr["item_weight_info"] != null && dr["item_weight_info"].ToString() != "")
                                objItem.WeightInfo = "(" + dr["item_weight_info"].ToString() + ")";
                            if (dr["item_discount"].ToString() == "True")
                            {
                                decimal discountper = !Utility.Common.IsDBNull(dr["discount_percentage"]) ? Convert.ToDecimal(dr["discount_percentage"]) : decimal.Zero;
                                objItem.PriceNew = decimal.Round(Convert.ToDecimal(objItem.Price.ToString()) - (Convert.ToDecimal(objItem.Price.ToString()) * (Convert.ToDecimal(discountper.ToString()) / 100)), 2);
                                objItem.DiscountPercentage = decimal.Round(discountper);
                            }

                            Item.Add(objItem);

                        }
                    }
                }
            }
            return Item;
        }


        public IList<ItemWidget> LoadNavigationCategory()
        {
            IList<ItemWidget> itemMainGroup = new List<ItemWidget>();
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("ussp_LoadNavigation", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var ItemCat = new ItemWidget();
                            ItemCat.itemMainGroup.MainGroupId = Convert.ToInt32(dr["main_group_id"].ToString().Trim());
                            ItemCat.itemMainGroup.MainGroupName = dr["main_group_name_en"].ToString().Trim();
                            ItemCat.itemCategory.CategoryId = Convert.ToInt32(dr["main_cate_id"].ToString().Trim());
                            ItemCat.itemCategory.CategoryName = dr["main_cate_name_en"].ToString().Trim();
                            ItemCat.itemCategory.MainGroupId = Convert.ToInt32(dr["main_group_id"].ToString().Trim());
                            ItemCat.itemSubCategory.SubCategoryName = dr["sub_cate_name_en"].ToString().Trim();
                            ItemCat.itemSubCategory.SubCategoryId = Convert.ToInt32(dr["sub_cate_id"].ToString().Trim());
                            ItemCat.itemSubCategory.CategoryId = Convert.ToInt32(dr["main_cate_id"].ToString().Trim());
                            itemMainGroup.Add(ItemCat);

                        }
                    }
                }
            }
            return itemMainGroup;
        }

        public List<Item> GetSearchResult(string searchValue)
        {
            List<Item> cartItem = new List<Item>();
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("usp_commonsearch", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@search_string", searchValue.Trim());
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var item = new Item();
                            item.Id = Convert.ToInt32(dr["item_id"].ToString());
                            item.Title = !Utility.Common.IsDBNull(dr["ecom_item_name_en"]) ? dr["ecom_item_name_en"].ToString() : string.Empty;
                            item.WeightInfo = !Utility.Common.IsDBNull(dr["item_weight_info"]) ? dr["item_weight_info"].ToString() : string.Empty;
                            item.Image = !Utility.Common.IsDBNull(dr["item_image"]) ? ConfigurationManager.AppSettings["ProductImage"] + dr["item_image"].ToString() : ConfigurationManager.AppSettings["NoImage"].ToString();
                            item.Price = !Utility.Common.IsDBNull(dr["item_price"]) ? Convert.ToDecimal(dr["item_price"].ToString()) : decimal.Zero;
                            item.DiscountPercentage = !Utility.Common.IsDBNull(dr["discount_percentage"]) ? Convert.ToDecimal(dr["discount_percentage"].ToString()) : decimal.Zero;
                            cartItem.Add(item);
                        }
                    }
                }
            }
            return cartItem;

        }
        public List<Item> GetWhishListItem(string userId)
        {
            List<Item> whishlistItem = new List<Item>();
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("load_wish_list", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", userId);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var item = new Item();
                            item.Id = !Common.IsDBNull(dr["item_id"]) ? Convert.ToInt32(dr["item_id"].ToString()) : 0;
                            item.Title = !Common.IsDBNull(dr["item_name"]) ? dr["item_name"].ToString() : string.Empty;
                            item.Unit = !Common.IsDBNull(dr["item_unit"]) ? dr["item_unit"].ToString() : string.Empty;
                            item.Image = !Common.IsDBNull(dr["item_image"]) ? ConfigurationManager.AppSettings["ProductImage"] + dr["item_image"].ToString() : ConfigurationManager.AppSettings["NoImage"];
                            item.Price = !Common.IsDBNull(dr["item_price"]) ? Convert.ToDecimal(dr["item_price"].ToString()) : decimal.Zero;
                            whishlistItem.Add(item);
                        }
                    }
                }
            }
            return whishlistItem;

        }
        public List<ItemDetail> GetItemDetails(int ItemId)
        {
            List<ItemDetail> itemDetails = new List<ItemDetail>();
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("usp_GetItemDetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", ItemId);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var itemdet = new ItemDetail();
                            itemdet.ItemId = Convert.ToInt32(dr["item_id"].ToString());
                            itemdet.Title = Common.IsDBNull(dr["ecom_item_name_en"]) ? dr["ecom_item_name_en"].ToString() : string.Empty;
                            itemdet.Price = Common.IsDBNull(dr["item_price"]) ? Convert.ToDecimal(dr["item_price"].ToString()) : decimal.Zero;
                            itemdet.Unit = Common.IsDBNull(dr["item_unit"]) ? dr["item_unit"].ToString() : string.Empty;
                            itemdet.Image = Common.IsDBNull(dr["item_image"]) ? ConfigurationManager.AppSettings["ProductImage"] + dr["item_image"].ToString() :
                                ConfigurationManager.AppSettings["NoImage"];
                            itemdet.Description = Common.IsDBNull(dr["item_description_en"]) ? dr["item_description_en"].ToString() : string.Empty;
                            itemdet.Bebefits = Common.IsDBNull(dr["item_benefit_en"]) ? dr["item_benefit_en"].ToString() : string.Empty;
                            itemdet.Usage =Common.IsDBNull(dr["item_usage_en"]) ? dr["item_usage_en"].ToString() : string.Empty;
                            itemdet.Category = Common.IsDBNull(dr["sub_cate_name_en"]) ? dr["sub_cate_name_en"].ToString() + " " + dr["main_cate_name_en"].ToString():string.Empty;
                            itemdet.origin = Common.IsDBNull(dr["country_name"]) ? dr["country_name"].ToString() : string.Empty;
                            itemdet.Code =Common.IsDBNull(dr["focus_item_code"]) ? dr["focus_item_code"].ToString() : string.Empty;
                            itemdet.OrganisationId = Common.IsDBNull(dr["item_organisation_id"]) ? Convert.ToInt32(dr["item_organisation_id"].ToString()) : 0;
                            itemDetails.Add(itemdet);
                        }
                    }
                }
            }
            return itemDetails;

        }


        public List<DiscountItem> GetDiscountItemDetails(int discountId)
        {
            List<DiscountItem> discountItem = new List<DiscountItem>();
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("load_product_discount_det", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", discountId);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var discountitem = new DiscountItem();
                            discountitem.DiscountName = !Common.IsDBNull(dr["discount_name"]) ? (dr["discount_name"].ToString()) : string.Empty;
                            discountitem.DiscountPrice = !Common.IsDBNull(dr["discount_price"]) ? dr["discount_price"].ToString() : string.Empty;
                            discountitem.DiscountId = discountId;
                            discountitem.DiscountDetails = !Common.IsDBNull(dr["discount_details"]) ? dr["discount_details"].ToString() : string.Empty;
                            discountitem.DiscountImage = !Common.IsDBNull(dr["discount_cover_image"]) ? ConfigurationManager.AppSettings["DiscountImages"] + dr["discount_cover_image"].ToString() : ConfigurationManager.AppSettings["NoImage"];
                            discountitem.EndDate = !Common.IsDBNull(dr["end_date"]) ? dr["end_date"].ToString() : string.Empty;
                            discountItem.Add(discountitem);
                        }
                    }
                }
            }
            return discountItem;

        }


        public int AddVisitedItem(string itemId)
        {
            int rows = 0;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("insert_item_visit", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@item_id", itemId);
                    rows = cmd.ExecuteNonQuery();
                }
            }
            return rows;
        }

        public int AddWhishListItem(string itemId, string userId)
        {
            int rows = 0;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("insert_wish_list", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@item_id", itemId);
                    cmd.Parameters.AddWithValue("@user_id", userId);
                    rows = cmd.ExecuteNonQuery();
                }
            }
            return rows;
        }
        public int AddItemsToCart(CartInsertItems itemCart)
        {

            int rows = 0;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("dml_cart_temp", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@cart_id", 0);
                    cmd.Parameters.AddWithValue("@item_id", itemCart.ItemId);
                    cmd.Parameters.AddWithValue("@item_image", itemCart.ItemImage);
                    cmd.Parameters.AddWithValue("@item_name", itemCart.ItemTitle);
                    cmd.Parameters.AddWithValue("@qty", itemCart.ItemQty);
                    cmd.Parameters.AddWithValue("@price", itemCart.ItemPrice);
                    cmd.Parameters.AddWithValue("@item_type", itemCart.ItemTypeId);//hardcoded
                    cmd.Parameters.AddWithValue("@user_session_id", itemCart.CustomerId);
                    rows = cmd.ExecuteNonQuery();
                }
            }
            return rows;

        }

        public List<ItemCart> GetMyCartItems(string userId)
        {
            List<ItemCart> cartItem = new List<ItemCart>();
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("load_my_cart", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@user_session", userId);
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            var item = new ItemCart();
                            item.CartId = !Common.IsDBNull(dr["cart_id"]) ? Convert.ToInt32(dr["cart_id"].ToString()) : 0;
                            item.ItemTitle = !Common.IsDBNull(dr["item_name"]) ? dr["item_name"].ToString() : string.Empty;
                            item.ItemImage = !Common.IsDBNull(dr["[item_image]"]) ? ConfigurationManager.AppSettings["ProductImage"] + dr["[item_image]"].ToString() : ConfigurationManager.AppSettings["NoImage"];
                            item.ItemPrice = !Common.IsDBNull(dr["price"]) ? Convert.ToDecimal(dr["price"].ToString()) : decimal.Zero;
                            item.ItemQty = !Common.IsDBNull(dr["qty"]) ? Convert.ToDecimal(dr["qty"]) : decimal.Zero;
                            decimal _Total = item.ItemPrice * item.ItemQty;
                            item.Total = decimal.Round(_Total, 2);
                            cartItem.Add(item);
                        }
                    }
                }
            }
            return cartItem;

        }

        public int UpdateCart(int cartId, decimal qty)
        {
            int rows = 0;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("update_cart", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@cart_id", cartId);
                    cmd.Parameters.AddWithValue("@qty", qty);
                    rows = cmd.ExecuteNonQuery();
                }
            }
            return rows;

        }
        public int DeleteCart(int cartId)
        {
            int rows = 0;
            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("delete_cart", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@cart_id", cartId);
                    rows = cmd.ExecuteNonQuery();
                }
            }
            return rows;

        }

        public int GetCartCount(int customerId)
        {

            using (SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("get_cart_count", con))
                {
                    SqlParameter outputIdParam = new SqlParameter("@count", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputIdParam);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@user_session", customerId);
                    cmd.ExecuteScalar();
                    return (int)outputIdParam.Value;
                }
            }

        }


    }
}
