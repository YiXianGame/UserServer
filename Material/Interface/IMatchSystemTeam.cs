using System;
using System.Collections.Generic;
using System.Text;

namespace Material.Interface
{
    public interface IMatchSystemTeam<T> : IMatchSystemItem where T:IMatchSystemItem
    {
        public ICollection<T> Items { get; set; }

        public bool Add(T item);
        public bool Remove(T item);
    }
}
