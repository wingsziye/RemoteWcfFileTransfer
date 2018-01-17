using System.Collections.Generic;
using Remote.Infrastructure.DataContracts;

namespace Remote.Infrastructure.Interfaces
{
    public interface IClientGroupCollection
    {
        ClientGroup this[string index] { get; }
        ClientGroup this[int index] { get; }
        int Count { get; }
        void Add(ClientGroup group);
        void Remove(ClientGroup group);
        void Remove(string groupName);
        void AddRange(IEnumerable<ClientGroup> groups);
        bool Contains(string groupName);
    }
}
