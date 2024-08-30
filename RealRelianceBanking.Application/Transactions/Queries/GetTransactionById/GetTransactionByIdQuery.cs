﻿using MediatR;
using RealRelianceBanking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace RealRelianceBanking.Application.Transactions.Queries.GetTransactionById
{
    public record GetTransactionByIdQuery(Guid TransactionId) : IRequest<TransactionsModel>;

}
