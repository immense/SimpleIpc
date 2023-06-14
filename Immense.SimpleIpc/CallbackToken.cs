using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleIpc
{
    public readonly struct CallbackToken : IEquatable<CallbackToken>
    {
        public CallbackToken()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public bool Equals(CallbackToken other)
        {
            return Id == other.Id;
        }
    }
}
