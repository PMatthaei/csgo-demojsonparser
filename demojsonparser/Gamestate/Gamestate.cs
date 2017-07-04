using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JSON.Entities;
using JSON.Events;

namespace JSON
{
    public class ReplayGamestate
    {
        /// <summary>
        /// Meta data about the game/match
        /// </summary>
        public GamestateMeta meta { get; set; }

        /// <summary>
        /// The raw match data with events and playerdata
        /// </summary>
        public Match match { get; set; }
    }

    public class GamestateMeta
    {
        /// <summary>
        /// ID of the gametstate if needed for DB applications
        /// </summary>
        public int gamestate_id { get; set; }

        /// <summary>
        /// Name of the map played in this game
        /// </summary>
        public string mapname { get; set; }

        /// <summary>
        /// Tickrate of the game in hz
        /// </summary>
        public float tickrate { get; set; }

        /// <summary>
        /// All players participating in the game
        /// </summary>
        public List<PlayerMeta> players { get; set; }
    }

    public class Match
    {
        /// <summary>
        /// All rounds of this match
        /// </summary>
        public List<Round> rounds { get; set; }
    }

    public class Round
    {
        internal string winner_team;

        /// <summary>
        /// ID of the round i.e. the number of the round
        /// </summary>
        public int round_id { get; set; }

        /// <summary>
        /// Winner of this round
        /// </summary>
        public string winner { get; set; }

        /// <summary>
        /// Ticks occuring in this round
        /// </summary>
        public List<Tick> ticks { get; set; }

    }

    public class Tick
    {
        /// <summary>
        /// ID of the tick. TickIDs may have gaps !
        /// </summary>
        public int tick_id { get; set; }

        /// <summary>
        /// All events occuring in this tick
        /// </summary>
        public List<Gameevent> tickevents { get; set; }
    }
}
