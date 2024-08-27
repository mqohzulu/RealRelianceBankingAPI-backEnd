using RealRelianceBanking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Common.Interfaces.Persistance
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmail(string email);
        Task Add(User user);
    }
}
