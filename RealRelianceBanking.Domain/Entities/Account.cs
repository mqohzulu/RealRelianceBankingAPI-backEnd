using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Domain.Entities
{
    public class Account
    {
        public Guid AccountID { get; set; }
        public Guid PersonID { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public decimal Balance { get; set; }
        public bool Status { get; set; }
        public bool ActiveInd { get; set; }

        public List<TransactionsModel> Transactions = new List<TransactionsModel>();
    }
}
