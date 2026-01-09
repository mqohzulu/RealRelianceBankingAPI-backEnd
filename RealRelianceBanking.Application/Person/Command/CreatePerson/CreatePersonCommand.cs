using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Person.Command.CreatePerson
{
    public record CreatePersonCommand(
       Guid personId,
       int IdNumber,
       string FirstName,
       string LastName,
       string Email,
       string PhoneNumber,
       bool ActiveInd,
       DateTime DateOfBirth) : IRequest<CreatePersonResponse>;

    public record CreatePersonResponse(
        bool Success,
        string? ErrorMessage = null,
        Guid? PersonId = null);
}
