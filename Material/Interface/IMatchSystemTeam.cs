using System;
using System.Collections.Generic;
using System.Text;

namespace Material.Interface
{
    public interface IMatchSystemTeam<T> : IMatchSystemItem where T:IMatchSystemItem
    {
        public ICollection<T> Items { get; set; }

        public void Add(T item);
        public void Remove(T item);
    }
}
