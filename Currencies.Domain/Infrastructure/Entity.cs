using System;

namespace Currencies.Domain.Infrastructure
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
    }
}
