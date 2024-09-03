using Dapper;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using RealRelianceBanking.Domain.Entities;
using RealRelianceBanking.Infrastructure.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Infrastructure.Persistance
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;

        public UserRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task Add(User user)
        {
            using (var db = _context.CreateConnection())
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@UserId", user.Id);
                parameters.Add("@Email", user.Email);
                parameters.Add("@FirstName", user.FirstName);
                parameters.Add("@LastName", user.LastName);
                parameters.Add("@Password", user.password);
                parameters.Add("@Role", user.Role);

                string sql = @"INSERT INTO Users (UserId,FirstName, LastName, Email, Password, Role,ActiveInd)
                   VALUES (@UserId,@FirstName,@LastName, @Email, @Password, @Role,1);";

               await db.ExecuteAsync(sql, parameters);
            }
        }

        public async Task<User?> GetUserByEmail(string email)
        {
            using (var db = _context.CreateConnection())
            {
                try
                {
                    var sql = @"
                        select * from dbo.Users
                        WHERE Email = @Email";

                    var parameters = new { Email = email };
                    var user = await db.QuerySingleOrDefaultAsync<User>(sql, parameters);
                    return user;
                }
                catch (Exception)
                {
                    return new User(); ;
                }
            }
        }
    }
}