using DataAccess;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PaymentClass
{
    public class Payment
    {
        DALOrder dalOrder = null;

        public  Payment()
        {
            dalOrder = new DALOrder();
        }
   

    }
}
