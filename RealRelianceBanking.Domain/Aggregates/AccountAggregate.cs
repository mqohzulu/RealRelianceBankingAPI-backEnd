using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Domain.Aggregates
{
    public class AccountAggregate
    {
        public Guid AccountId { get; set; }
        public Guid PersonId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public decimal Balance { get; set; }
        public bool IsClosed { get; set; }
        public bool ActiveInd { get; set; }

    }
}
