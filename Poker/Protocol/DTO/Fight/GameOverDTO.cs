using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocol.DTO.Fight
{
    /// <summary>
    /// game over DTO
    /// </summary>
    [Serializable]
    public class GameOverDTO
    {
        /// <summary>
        /// decide what to do after this game
        /// </summary>
        public enum ActionType
        {
            Continue,
            Over,
        }

        public Dictionary<PlayerDTO, int> winnerCoinDict;

        public List<PlayerDTO> loserList;

        public ActionType actionType;

        public GameOverDTO()
        {
            winnerCoinDict = new Dictionary<PlayerDTO, int>();
            loserList = new List<PlayerDTO>();
            actionType = ActionType.Continue;
        }

        public void Change(Dictionary<PlayerDTO, int> winnerCoinDict, List<PlayerDTO> loserList, ActionType actionType)
        {
            this.winnerCoinDict = winnerCoinDict;
            this.loserList = loserList;
            this.actionType = actionType;
        }
    }
}
