using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using RealRelianceBanking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Infrastructure.Persistance
{
    internal class UserRepository : IUserRepository
    {
        Task IUserRepository.Add(User user)
        {
            throw new NotImplementedException();
        }

        Task<User?> IUserRepository.GetUserByEmail(string email)
        {
            throw new NotImplementedException();
        }
    }
}
