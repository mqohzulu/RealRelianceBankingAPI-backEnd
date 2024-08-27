using RealRelianceBanking.Domain.Aggregates;
using RealRelianceBanking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Common.Interfaces.Persistance
{
    public interface IPersonRepository
    {
        Task<PersonModel> GetPersonById(Guid id);

        Task<Guid> Add(PersonModel person);

        Task<List<PersonModel>> GetPersons(bool activeOnly);

        Task<List<PersonModel?>> GetPersonByEmail(string email);
        Task Deactivate(Guid personId);
        Task<PersonModel> GetByIdNumberAsync(int IdNumber);
        Task<bool> HasActiveAccounts(Guid personId);

        Task<PersonDto> GetPersonDtoByIdNumber(int idNumber);

        Task<PersonModel> GetByIdNumber(int idNumber);

        Task<bool> Update(PersonModel person);

    }
}
