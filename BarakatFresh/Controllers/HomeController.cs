using DataAccess;
using DataEntity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data;
using Utility;

namespace BarakatFresh.Controllers
{
    public class HomeController : ApiController
    {

        DALItem dalItem = null;
        DALOrder dalOrder = null;
        public HomeController()
        {
            dalItem = new DALItem();
            dalOrder = new DALOrder();
        }
        [ActionName("QuikCartItems")]
        [HttpGet]
        public async Task<HttpResponseMessage> QuikCartItems()
        {
            IList<QuikKartItems> items = await Task.Run(() => dalItem.GetQuikItems());
            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { data = items }), Encoding.UTF8, "application/json")
            };
        }
        [ActionName("LoadItemGroupBasedList")]
        [HttpGet]
        public async Task<HttpResponseMessage> LoadItemGroupBasedList(int groupId, int LevelId)
        {
            IList<Item> items = await Task.Run(() => dalItem.LoadItemGroupBasedList(groupId, LevelId));
            if (items != null && items.Count > 0)
            {
                items = items.Take(10).ToList();
            }
            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { data = items }), Encoding.UTF8, "application/json")
            };
        }
        [ActionName("LoadNavigationCategory")]
        [HttpGet]
        public async Task<HttpResponseMessage> LoadNavigationCategory()
        {
            IList<ItemWidget> itemCategoryList = await Task.Run(() => dalItem.LoadNavigationCategory());
            IList<ItemWidget> itemCategoryNew = new List<ItemWidget>();
            if (itemCategoryList != null && itemCategoryList.Count > 0)
            {
                var uniqueGroups = (from row in itemCategoryList
                                    select row.itemMainGroup.MainGroupId).Distinct();

                if (uniqueGroups != null && uniqueGroups.Count() > 0)
                {
                    foreach (int groupId in uniqueGroups.ToList())
                    {
                        ItemWidget obj = new ItemWidget();

                        obj.groupId = groupId;
                        obj.groupName = itemCategoryList.Where(x => x.itemMainGroup.MainGroupId == groupId).FirstOrDefault()?.itemMainGroup.MainGroupName;

                        var Categories = (from row in itemCategoryList
                                          where row.itemCategory.MainGroupId == groupId
                                          select row.itemCategory.CategoryId).Distinct();
                        if (Categories != null && Categories.Count() > 0)
                        {
                            foreach (var Category in Categories)
                            {
                                var itemCat = new ItemCategory();
                                itemCat.CategoryId = Category;
                                itemCat.CategoryName = itemCategoryList.Where(x => x.itemCategory.CategoryId == Category).FirstOrDefault()?.itemCategory.CategoryName;

                                obj.itemCategories.Add(itemCat);

                                var SubCategories = (from row in itemCategoryList
                                                     where row.itemSubCategory.CategoryId == Category
                                                     select row.itemSubCategory.SubCategoryId).Distinct();
                                if (SubCategories != null && SubCategories.Count() > 0)
                                {
                                    foreach (var subCategory in SubCategories)
                                    {
                                        var itemSub = new ItemSubCategory();
                                        itemSub.SubCategoryId = subCategory;
                                        itemSub.SubCategoryName = itemCategoryList.Where(x => x.itemSubCategory.SubCategoryId == subCategory).FirstOrDefault()?.itemSubCategory.SubCategoryName;

                                        var regionObject = obj.itemCategories.Find(row => row.CategoryId == Category);
                                        regionObject.subCategories.Add(itemSub);
                                    }
                                }

                            }
                        }
                        itemCategoryNew.Add(obj);
                    }
                }
            }

            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { data = itemCategoryNew }), Encoding.UTF8, "application/json")
            };
        }
        [ActionName("GetSearchItem")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetSearchItem(string searchValue)
        {
            List<Item> items = null;
            if (!string.IsNullOrEmpty(searchValue))
                items = await Task.Run(() => dalItem.GetSearchResult(searchValue));

            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { data = items }), Encoding.UTF8, "application/json")
            };
        }

        [ActionName("AddVisitedItem")]
        [HttpGet]
        public async Task<HttpResponseMessage> AddVisitedItem(string itemId)
        {
            if (!string.IsNullOrEmpty(itemId))
            {
                int addResult = await Task.Run(() => dalItem.AddVisitedItem(itemId));
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = addResult > 0 ? true : false }), Encoding.UTF8, "application/json")
                };
            }
            else
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = false }), Encoding.UTF8, "application/json")
                };
            }
        }
        [ActionName("AddWhishListItem")]
        [HttpGet]
        public async Task<HttpResponseMessage> AddWhishListItem(string itemId, string userId)
        {
            if (!string.IsNullOrEmpty(itemId) && !string.IsNullOrEmpty(userId))
            {
                int addResult = await Task.Run(() => dalItem.AddWhishListItem(itemId, userId));
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = addResult > 0 ? true : false }), Encoding.UTF8, "application/json")
                };
            }
            else
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = false, error = "Invalid ItemId/UserId" }), Encoding.UTF8, "application/json")
                };
            }
        }
        [ActionName("GetWhishListItems")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetWhishListItems(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                List<Item> items = await Task.Run(() => dalItem.GetWhishListItem(userId));
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = items }), Encoding.UTF8, "application/json")
                };
            }
            else
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = string.Empty, error = "Invalid UserId" }), Encoding.UTF8, "application/json")
                };
            }
        }
        [ActionName("GetRecommendedItems")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetRecommendedItems(int groupId)
        {

            IList<RecommendedItems> items = await Task.Run(() => dalItem.GetRecommendedItems(groupId));
            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { data = items }), Encoding.UTF8, "application/json")
            };

        }

        [ActionName("GetItemDetails")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetItemDetails(int ItemId)
        {

            List<ItemDetail> items = await Task.Run(() => dalItem.GetItemDetails(ItemId));
            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { data = items }), Encoding.UTF8, "application/json")
            };

        }
        [ActionName("GetDiscountItemDetails")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetDiscountItemDetails(string discountId)
        {
            if (!string.IsNullOrEmpty(discountId) && Common.isNummeric(discountId))
            {
                List<DiscountItem> discountitems = await Task.Run(() => dalItem.GetDiscountItemDetails(Convert.ToInt32(discountId)));
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = discountitems }), Encoding.UTF8, "application/json")
                };
            }
            else
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = string.Empty, error = "Invalid DiscountId" }), Encoding.UTF8, "application/json")
                };
            }
        }
        [ActionName("AddItemsToCart")]
        [HttpPost]
        public async Task<HttpResponseMessage> AddItemsToCart(CartInsertItems cartitem)
        {

            int addResult = await Task.Run(() => dalItem.AddItemsToCart(cartitem));
            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { data = addResult > 0 ? true : false }), Encoding.UTF8, "application/json")
            };

        }
        [HttpGet]
        public async Task<HttpResponseMessage> GetMyCartItems(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                List<ItemCart> cartitems = await Task.Run(() => dalItem.GetMyCartItems(userId));
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = cartitems }), Encoding.UTF8, "application/json")
                };
            }
            else
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = string.Empty, error = "Invalid UserId" }), Encoding.UTF8, "application/json")
                };
            }
        }
        [ActionName("UpdateCart")]
        [HttpPost]
        public async Task<HttpResponseMessage> UpdateCart(int cartId, decimal Qty)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(cartId)))
            {
                int updated = await Task.Run(() => dalItem.UpdateCart(cartId, Qty));
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = updated > 0 ? true : false }), Encoding.UTF8, "application/json")
                };
            }
            else
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = false, error = "Invalid CartId" }), Encoding.UTF8, "application/json")
                };
            }

        }
        [ActionName("DeleteCart")]
        [HttpPost]
        public async Task<HttpResponseMessage> DeleteCart(int cartId)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(cartId)))
            {
                int deleted = await Task.Run(() => dalItem.DeleteCart(cartId));
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = deleted > 0 ? true : false }), Encoding.UTF8, "application/json")
                };
            }
            else
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = false, error = "Invalid CartId" }), Encoding.UTF8, "application/json")
                };
            }

        }

        [ActionName("GetCartCount")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetCartCount(int customerId)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(customerId)))
            {
                int cartcount = await Task.Run(() => dalItem.GetCartCount(customerId));
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = cartcount }), Encoding.UTF8, "application/json")
                };
            }
            else
            {
                return new HttpResponseMessage()
                {
                    Content = new StringContent(JsonConvert.SerializeObject(new { data = "", error = "Invalid customerId" }), Encoding.UTF8, "application/json")
                };
            }

        }

        [ActionName("GetPurchaseHistory")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetPurchaseHistory(int userId)
        {
            IList<PurchaseList> purchaseList = null;
            purchaseList = await Task.Run(() => dalOrder.GetPurchaseListHistory(userId));

            return new HttpResponseMessage()
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { data = purchaseList }), Encoding.UTF8, "application/json")
            };
        }

    }
}