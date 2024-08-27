using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Domain.Aggregates
{
    public class TransferAggregate
    {
        public Guid? AccountId { get; set; }

        public Guid TransactionId { get; set; }

        public DateTime Date { get; set; }

        public string Type { get; set; } = null!;

        public string Operation { get; set; } = null!;

        public decimal Amount { get; set; }

        public decimal Balance { get; set; }

        public Guid? AccountFrom { get; set; }
    }
}
