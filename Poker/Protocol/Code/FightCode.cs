using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.Code
{
    public class FightCode
    {
        public const int StartFight_BRO = 0;

        public const int Leave_CREQ = 1;
        public const int Leave_BRO = 2;

        public const int StartBet_BRO = 3;

        public const int Call_CREQ = 6;

        public const int Call_BRO = 7;

        public const int RAISE_CREQ = 8;

        public const int FOLD_CREQ = 9;
        public const int FOLD_BRO = 10;

        public const int GameOver_BRO = 13;

        public const int Flop_BRO = 14;

        public const int Turn_BRO = 15;

        public const int River_BRO = 16;

        public const int Raise_BRO = 17;

        public const int Flush_BRO = 18;





    }
}
