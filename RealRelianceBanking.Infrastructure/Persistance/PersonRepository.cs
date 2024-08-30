using Dapper;
using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using RealRelianceBanking.Domain.Aggregates;
using RealRelianceBanking.Domain.Entities;
using RealRelianceBanking.Infrastructure.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Infrastructure.Persistance
{
    public class PersonRepository : IPersonRepository
    {
        private readonly DapperContext _context;
        public PersonRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Guid> Add(PersonModel person)
        {
            using (var db = _context.CreateConnection())
            {
                var sql = @"
                    INSERT INTO Person (
                        PersonId, IdNumber,FirstName, LastName,  Email, ActiveInd, 
                        PhoneNumber,DateOfBirth
                    ) VALUES (
                        @PersonId, @IdNumber,@FirstName, @LastName,@Email, @ActiveInd, 
                         @PhoneNumber,@DateOfBirth
                    )";

                var parameters = new
                {
                    PersonId = Guid.NewGuid(),
                    person.FirstName,
                    person.LastName,
                    person.IdNumber,
                    person.ActiveInd,
                    person.Email,
                    person.PhoneNumber,
                    person.DateOfBirth,
                };

                await db.ExecuteAsync(sql, parameters);

                return person.PersonID;

            }
        }
        public async Task<PersonModel> GetByIdNumber(int idNumber)
        {
            using (var db = _context.CreateConnection())
            {
                var sql = "SELECT * FROM Person WHERE IdNumber = @IdNumber";
                return await db.QuerySingleOrDefaultAsync<PersonModel>(sql, new { IdNumber = idNumber });
            }
        }
        public async Task<PersonModel> GetPersonById(Guid id)
        {
            using (var db = _context.CreateConnection())
            {
                try
                {
                    var sql = "SELECT * FROM Person WHERE PersonId = @Id";
                    return await db.QuerySingleOrDefaultAsync<PersonModel>(sql, new { Id = id });
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }
        public async Task<List<PersonModel>> GetPersons(bool activeOnly)
        {
            using (var db = _context.CreateConnection())
            {
                try
                {
                    var query = "SELECT * FROM Person WHERE  (@activeOnly = 0 OR ActiveInd = @activeOnly) ";
                    var parameters = new DynamicParameters();
                    parameters.Add("@activeOnly", activeOnly);

                    var result = await db.QueryAsync<PersonModel>(query, parameters);
                    return result.ToList();
                }
                catch (Exception ex)
                {
                    return new List<PersonModel>();
                }
            }
        }
        public async Task<List<PersonModel?>> GetPersonByEmail(string email)
        {
            using (var db = _context.CreateConnection())
            {
                try
                {
                    var query = "SELECT * FROM Person WHERE Email = @email";
                    var parameters = new DynamicParameters();
                    parameters.Add("@email", email);

                    var result = await db.QueryAsync<PersonModel>(query, parameters);
                    return result.ToList();
                }
                catch (Exception ex)
                {
                    return new List<PersonModel?>();
                }
            }
        }

        public async Task Deactivate(Guid personId)
        {

            using (var db = _context.CreateConnection())
            {
                try
                {
                    var sql = "UPDATE Person SET ActiveInd = 0 WHERE PersonId = @PersonId";
                    await db.ExecuteAsync(sql, new { PersonId = personId });
                }
                catch (Exception ex)
                {
                    new Exception();
                }
            }

        }


        public async Task<PersonModel> GetByIdNumberAsync(int IdNumber)
        {
            using (var db = _context.CreateConnection())
            {
                try
                {

                    var sql = "SELECT * FROM Person WHERE IdNumber = @IdNumber";
                    var ret = await db.QueryFirstOrDefaultAsync<PersonModel>(sql, new { IdNumber = IdNumber });

                    return ret;
                }
                catch (Exception ex)
                {
                    throw new Exception();
                }
            }
        }

        public async Task<bool> HasActiveAccounts(Guid personId)
        {


            using (var db = _context.CreateConnection())
            {
                try
                {

                    return await db.ExecuteScalarAsync<bool>(
                    "SELECT COUNT(*) > 0 FROM Account WHERE PersonId = @PersonId AND IsClosed = 0",
                    new { PersonId = personId });
                }
                catch (Exception ex)
                {
                    throw new Exception();
                }
            }
        }

        public async Task<PersonDto> GetPersonDtoByIdNumber(int idNumber)
        {
            using (var db = _context.CreateConnection())
            {
                try
                {
                    var sql = @"
                    SELECT p.*, COUNT(a.AccountId) AS AccountCount
                    FROM Person p
                    LEFT JOIN Account a ON p.PersonId = a.PersonId
                    WHERE p.IdNumber = @IdNumber
                    GROUP BY p.PersonId, p.IdNumber, p.FirstName, p.LastName, p.Email, p.PhoneNumber, p.Address, p.DateOfBirth, p.ActiveInd";

                    return await db.QueryFirstOrDefaultAsync<PersonDto>(sql, new { IdNumber = idNumber });
                }
                catch (Exception ex)
                {
                    throw new Exception();
                }
            }
        }
        public async Task<bool> Update(PersonModel person)
        {
            using (var db = _context.CreateConnection())
            {
                var sql = @"
                UPDATE Person SET 
                    IdNumber = @IdNumber,
                    FirstName = @FirstName, 
                    LastName = @LastName,  
                    Email = @Email, 
                    ActiveInd = @ActiveInd, 
                    PhoneNumber = @PhoneNumber,
                    DateOfBirth = @DateOfBirth
                WHERE PersonId = @PersonId";

                var parameters = new
                {
                    person.PersonID,
                    person.FirstName,
                    person.LastName,
                    person.IdNumber,
                    person.ActiveInd,
                    person.Email,
                    person.PhoneNumber,
                    person.DateOfBirth,
                };

                var affectedRows = await db.ExecuteAsync(sql, parameters);
                return affectedRows > 0;
            }
        }
    }
}
