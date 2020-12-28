using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Make.MODEL.EventArgsModel
{
    public class DeathEventArgs : EventArgs
    {
        public Player Killer;
        public Player Killed;

        public DeathEventArgs(Player killer, Player killed)
        {
            Killer = killer;
            Killed = killed;
        }
    }
}
