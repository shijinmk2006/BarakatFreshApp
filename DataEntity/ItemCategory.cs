using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataEntity
{

    public class ItemWidget
    {
        public int groupId { get;set; }
        public string groupName { get; set; }

        public ItemSubCategory itemSubCategory { get; set; }
        public ItemCategory itemCategory { get; set; }

        public List<ItemCategory> itemCategories { get; set; }
        public ItemMainGroup itemMainGroup { get; set; }

        public ItemWidget()
        {
            itemCategory = new ItemCategory();
            itemSubCategory = new ItemSubCategory();
            itemMainGroup = new ItemMainGroup();
            itemCategories = new List<ItemCategory>();

        }

    }
    public class NameChildObject
    {
        public int name { get; set; }
        public int size { get; set; }
        public List<NameChildObject> children { get; set; }

        public NameChildObject()
        {
            children = new List<NameChildObject>();
        }
    }
    public class ItemCategory
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public int MainGroupId { get; set; }

        public string MainGroupname { get; set; }
        public List<ItemSubCategory> subCategories { get; set; }

        public ItemCategory()
        {
            subCategories = new List<ItemSubCategory>();
            
        }
    }
    public class ItemSubCategory
    {
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }

        public int CategoryId { get; set; }
    }

    public class ItemMainGroup
    {
        public int MainGroupId { get; set; }
        public string MainGroupName { get; set; }

        public List<ItemCategory> Categories { get; set; }

        public ItemMainGroup()
        {
            Categories = new List<ItemCategory>();

        }
    }

}
