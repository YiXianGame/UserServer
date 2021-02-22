using System;
using System.Collections.Generic;
using System.Text;

namespace Material.Interface
{
    public interface IMatchSystemItem
    {
        public long StartMatchTime { get; set; }
        public int Count { get; set; }

        public int SumRank { get; set; }
        public int AverageRank { get; set; }
    }
}
