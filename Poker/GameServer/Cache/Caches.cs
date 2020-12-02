using GameServer.Cache.Fight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Cache
{
    /// <summary>
    /// Caches manage matchCache list and fightCache IN CASE any copy of cache exists
    /// </summary>
    public class Caches
    {
        /// <summary>
        /// only matchCache list
        /// </summary>
        public  static List<MatchCache> matchCacheList { get; set; }

        /// <summary>
        /// only fightCache
        /// </summary>
        public static FightCache fightCache { get; set; }
        static Caches()
        {
            matchCacheList = new List<MatchCache>();
            for (int i = 0; i < 3; i++)
            {
                matchCacheList.Add(new MatchCache());
            }

            fightCache = new FightCache();
        }
    }
}
