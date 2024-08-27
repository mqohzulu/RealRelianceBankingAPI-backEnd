using RealRelianceBanking.Application.Common.Interfaces.Persistance;
using RealRelianceBanking.Domain.Aggregates;
using RealRelianceBanking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Infrastructure.Persistance
{
    public class PersonRepository : IPersonRepository
    {
        Task<Guid> IPersonRepository.Add(PersonModel person)
        {
            throw new NotImplementedException();
        }

        Task IPersonRepository.Deactivate(Guid personId)
        {
            throw new NotImplementedException();
        }

        Task<PersonModel> IPersonRepository.GetByIdNumber(int idNumber)
        {
            throw new NotImplementedException();
        }

        Task<PersonModel> IPersonRepository.GetByIdNumberAsync(int IdNumber)
        {
            throw new NotImplementedException();
        }

        Task<List<PersonModel?>> IPersonRepository.GetPersonByEmail(string email)
        {
            throw new NotImplementedException();
        }

        Task<PersonModel> IPersonRepository.GetPersonById(Guid id)
        {
            throw new NotImplementedException();
        }

        Task<PersonDto> IPersonRepository.GetPersonDtoByIdNumber(int idNumber)
        {
            throw new NotImplementedException();
        }

        Task<List<PersonModel>> IPersonRepository.GetPersons(bool activeOnly)
        {
            throw new NotImplementedException();
        }

        Task<bool> IPersonRepository.HasActiveAccounts(Guid personId)
        {
            throw new NotImplementedException();
        }

        Task<bool> IPersonRepository.Update(PersonModel person)
        {
            throw new NotImplementedException();
        }
    }
}
