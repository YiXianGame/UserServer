using System;
using System.Collections.Generic;
using System.Text;

namespace Material.Model.MatchSystem.Interface
{
    public interface IMatchSystemItem
    {
        public int SumRank { get; set; }
        public int Count { get; set; }
        public long StartMatchTime { get; set; }
        public int AverageRank { get; set; }
    }
}
