using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataEntity
{
    public class Transaction
    {
        public string TransactionId { get; set; }
        public string OrderId { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string CustomerName { get; set; }
    }
}
