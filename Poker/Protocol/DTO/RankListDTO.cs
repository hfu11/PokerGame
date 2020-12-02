using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.DTO
{
    [Serializable]
    public class RankListDTO
    {
        public List<RankItemDTO> rankList;

        public RankListDTO()
        {
            rankList = new List<RankItemDTO>();
        }

        public void Clear()
        {
            rankList.Clear();
        }

        public void Add(RankItemDTO item)
        {
            rankList.Add(item);
        }
    }
}
