using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlueCoinApi.Models
{
    public class AddTransactionViewModel
    {
        public int Sender { get; set; }
        public int Receiver { get; set; }
        public decimal Amount { get; set; }
    }
}
