using RealRelianceBanking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Domain.Aggregates
{
    public class NewPersonAccountAggregate
    {

        public Guid PersonId { get; set; }

        public Guid AccountId { get; set; }

        public Guid UserId { get; set; }

        public virtual Account Account { get; set; } = null!;

        public virtual PersonModel Person { get; set; } = null!;

    }
}
