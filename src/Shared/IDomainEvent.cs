using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public interface IDomainEvent { }

    public interface IDomainEventHandler<T> where T : IDomainEvent
    {
        Task Handle(T domainEvent);
    }

}
