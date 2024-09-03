using MediatR;
using RealRelianceBanking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Person.Command.EditPerson
{
    public record EditPersonCommand(Guid personId, int IdNumber, string FirstName,
                                   string LastName, string Email,
                                   string PhoneNumber, bool ActiveInd,
                                   DateTime DateOfBirth) : IRequest<bool>;
}
