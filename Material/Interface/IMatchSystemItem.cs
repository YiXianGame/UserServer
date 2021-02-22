using System;
using System.Collections.Generic;
using System.Text;

namespace Material.Interface
{
    public interface IMatchSystemItem
    {
        public int Rank { get; set; }
        public int Count { get; set; }
        public long StartMatchTime { get; set; }
        public int AverageRank { get; set; }
    }
}
