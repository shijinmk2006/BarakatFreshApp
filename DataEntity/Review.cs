using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataEntity
{
    public class Review
    {
        public int ItemId { get; set; }
        public string CustomerName { get; set; }
        public string ReviewComments { get; set; }
        public int Rating { get; set; }
        [IgnoreDataMember]
        public string ReviewDate { private get; set; }

    }
}
