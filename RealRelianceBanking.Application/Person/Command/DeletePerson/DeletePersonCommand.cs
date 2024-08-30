using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealRelianceBanking.Application.Person.Command.DeletePerson
{
    public record DeletePersonCommand(Guid PersonId) : IRequest;

}
