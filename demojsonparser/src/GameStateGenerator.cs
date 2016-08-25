using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using demojsonparser.src.JSON;
using DemoInfo;
using Newtonsoft.Json;
using demojsonparser.src.JSON.objects;
using demojsonparser.src.JSON.events;
using demojsonparser.src.logging;

namespace demojsonparser.src
{
    public class GameStateGenerator
    {
        //public static Logger logger = new Logger("Logging started");

        private const int positioninterval = 8;

        private static StartView sv;
        //
        //
        // TODO:    1) use de-/serialization and streams for less GC and memory consumption?
        //          2) build json object-oriented with markers (#round1) to paste corresponding string
        //          3) Why are DemoInfo objects kept after using statement? (in StartView.cs)
        //



        public static void GenerateJSONFile(DemoParser parser, string path)
        {

            //Measure time to roughly check performance
            var watch = System.Diagnostics.Stopwatch.StartNew();

            //TODO: Check why/how GC is not grabbing the gamestate if these are within GenerateJSONFile()
            JSONMatch match = new JSONMatch();
            JSONRound round = new JSONRound();
            JSONTick tick = new JSONTick();
            JSONParser jsonparser;
            JSONGamestate gs;

            //JSONTick nulltick = new JSONTick(); //Just for testing or if empty ticks are wanted

            bool hasMatchStarted = false;
            bool hasRoundStarted = false;
            bool hasFreeezEnded = false;

            int tick_id = 0;
            int round_id = 0;



            //JSON holding the whole gamestate
            gs = new JSONGamestate();

            //Parser to transform DemoParser events to JSON format
            jsonparser = new JSONParser(parser, path);

            //Init lists
            match.rounds = new List<JSONRound>();
            round.ticks = new List<JSONTick>();
            tick.tickevents = new List<JSONGameevent>();


            //Obligatory to use this parser
            parser.ParseHeader();

            #region Main Gameevents
            //Start writing the gamestate object
            parser.MatchStarted += (sender, e) =>
            {
                hasMatchStarted = true;
                //Assign Gamemetadata
                gs.meta = jsonparser.assembleGamemeta();
            };

            //Assign match object
            parser.WinPanelMatch += (sender, e) =>
            {
                if (hasMatchStarted)
                    gs.match = match;
                hasMatchStarted = false;

            };

            //Start writing a round object
            parser.RoundStart += (sender, e) =>
            {
                if (hasMatchStarted)
                {
                    hasRoundStarted = true;
                    round_id++;
                    round.round_id = round_id;
                }

            };

            //Add round object to match object
            parser.RoundEnd += (sender, e) =>
            {
                if (hasMatchStarted)
                {
                    if (hasRoundStarted) //TODO: maybe round fires false -> do in tickdone event (see github issues of DemoInfo)
                    {
                        round.winner = e.Winner.ToString();
                        match.rounds.Add(round);
                        round = new JSONRound();
                        round.ticks = new List<JSONTick>();
                    }

                    hasRoundStarted = false;

                }

            };



            parser.WeaponFired += (object sender, WeaponFiredEventArgs we) =>
            {
                if (hasMatchStarted)
                    tick.tickevents.Add(jsonparser.assembleWeaponFire(we));
            };


            parser.PlayerKilled += (object sender, PlayerKilledEventArgs e) =>
            {
                if (hasMatchStarted)
                {
                    //the killer is null if vicitm is killed by the world - eg. by falling
                    if (e.Killer != null)
                        tick.tickevents.Add(jsonparser.assemblePlayerKilled(e));

                }

            };

            parser.PlayerHurt += (object sender, PlayerHurtEventArgs e) =>
            {
                if (hasMatchStarted)
                    //the attacker is null if vicitm is damaged by the world - eg. by falling
                    if (e.Attacker != null)
                        tick.tickevents.Add(jsonparser.assemblePlayerHurt(e));
            };
            #endregion

            #region Nadeevents
            //Nade (Smoke Fire Decoy Flashbang and HE) events
            parser.ExplosiveNadeExploded += (object sender, GrenadeEventArgs e) =>
            {
                if (e.ThrownBy != null && hasMatchStarted)
                    tick.tickevents.Add(jsonparser.assembleNade(e, "hegrenade_exploded"));
            };

            parser.FireNadeStarted += (object sender, FireEventArgs e) =>
            {
                if (e.ThrownBy != null && hasMatchStarted)
                    tick.tickevents.Add(jsonparser.assembleNade(e, "firenade_exploded"));
            };

            parser.FireNadeEnded += (object sender, FireEventArgs e) =>
            {
                if (e.ThrownBy != null && hasMatchStarted)
                    tick.tickevents.Add(jsonparser.assembleNade(e, "firenade_ended"));
            };

            parser.SmokeNadeStarted += (object sender, SmokeEventArgs e) =>
            {
                if (e.ThrownBy != null && hasMatchStarted)
                    tick.tickevents.Add(jsonparser.assembleNade(e, "smoke_exploded"));
            };


            parser.SmokeNadeEnded += (object sender, SmokeEventArgs e) =>
            {
                if (e.ThrownBy != null && hasMatchStarted)
                    tick.tickevents.Add(jsonparser.assembleNade(e, "smoke_ended"));
            };

            parser.DecoyNadeStarted += (object sender, DecoyEventArgs e) =>
            {
                if (e.ThrownBy != null && hasMatchStarted)
                    tick.tickevents.Add(jsonparser.assembleNade(e, "decoy_exploded"));
            };

            parser.DecoyNadeEnded += (object sender, DecoyEventArgs e) =>
            {
                if (e.ThrownBy != null && hasMatchStarted)
                    tick.tickevents.Add(jsonparser.assembleNade(e, "decoy_ended"));
            };

            parser.FlashNadeExploded += (object sender, FlashEventArgs e) =>
            {
                if (e.ThrownBy != null && hasMatchStarted)
                    tick.tickevents.Add(jsonparser.assembleNade(e, "flash_exploded"));
            };
            /*
            // Seems to be redundant with all exploded events
            parser.NadeReachedTarget += (object sender, NadeEventArgs e) =>
            {
                if (e.ThrownBy != null && hasMatchStarted)
                    tick.tickevents.Add(jsonparser.assembleNade(e, "nade_reachedtarget"));
            }; */

            #endregion

            #region Bombevents
            parser.BombAbortPlant += (object sender, BombEventArgs e) =>
            {
                tick.tickevents.Add(jsonparser.assembleBomb(e, "bomb_abort_plant"));
            };

            parser.BombAbortDefuse += (object sender, BombDefuseEventArgs e) =>
            {
                tick.tickevents.Add(jsonparser.assembleBombDefuse(e, "bomb_abort_defuse"));
            };

            parser.BombBeginPlant += (object sender, BombEventArgs e) =>
            {
                tick.tickevents.Add(jsonparser.assembleBomb(e, "bomb_begin_plant"));
            };

            parser.BombBeginDefuse += (object sender, BombDefuseEventArgs e) =>
            {
                tick.tickevents.Add(jsonparser.assembleBombDefuse(e, "bomb_begin_defuse"));
            };

            parser.BombPlanted += (object sender, BombEventArgs e) =>
            {
                tick.tickevents.Add(jsonparser.assembleBomb(e, "bomb_planted"));
            };

            parser.BombDefused += (object sender, BombEventArgs e) =>
            {
                tick.tickevents.Add(jsonparser.assembleBomb(e, "bomb_defused"));
            };

            parser.BombExploded += (object sender, BombEventArgs e) =>
            {
                tick.tickevents.Add(jsonparser.assembleBomb(e, "bomb_exploded"));
            };
            #endregion

            /*
            //Extraevents maybe useful
            parser.RoundFinal += (object sender, RoundFinalEventArgs e) => {

            };
            parser.RoundMVP += (object sender, RoundMVPEventArgs e) => {

            };
            parser.RoundOfficiallyEnd += (object sender, RoundOfficiallyEndedEventArgs e) => {

            };
            parser.LastRoundHalf += (object sender, LastRoundHalfEventArgs e) => {

            };
            */

            parser.FreezetimeEnded += (object sender, FreezetimeEndedEventArgs e) =>
            {
                hasFreeezEnded = true; //Just capture movement after freezetime has ended
            };

            //Assemble a tick object with the above gameevents
            parser.TickDone += (sender, e) =>
            {
                if (!hasMatchStarted) //Dont count ticks if the game has not started already (dismissing warmup and knife-phase for official matches)
                    return;
                // Every tick save id and time
                // Dumb playerpositions every positioninterval-ticks when freezetime has ended
                if ((tick_id % positioninterval == 0) && hasFreeezEnded)
                    foreach (var player in parser.PlayingParticipants)
                    {
                        tick.tickevents.Add(jsonparser.assemblePlayerFootstep(player));
                    }

                tick_id++;
            };

            //Parse tickwise and add the resulting tick to the round object
            bool hasnext = true;
            while (hasnext)
            {
                try
                {
                    hasnext = parser.ParseNextTick();
                }
                catch (System.IO.EndOfStreamException e)
                {
                    sv.getErrorBox().AppendText("Problem with tickparsing. Is your .dem valid? See this projects github page for more info.\n");
                    sv.getErrorBox().AppendText("Stacktrace: " + e.StackTrace+ "\n");
                    jsonparser.stopParser();
                    watch.Stop();
                    break;
                }

                if (hasRoundStarted)
                {
                    tick.tick_id = tick_id;
                    if (tick.tickevents.Count != 0)
                    {
                        //When something happend during the tick(more than 0 events)
                        round.ticks.Add(tick);
                        tick = new JSONTick();
                        tick.tickevents = new List<JSONGameevent>();
                    } /*else
                        {
                            round.ticks.Add(nulltick); // Add empty ticks !! Creates useless data and wastes memory
                        } */
                }

            }

            //Dump the complete gamestate object into JSON-file and do not pretty print(memory expensive)
            jsonparser.dump(gs, false);

            //Work is done.
            jsonparser.stopParser();

            gs = null;
            match = null;
            round = null;
            tick = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            var sec = elapsedMs / 1000.0f;
            sv.getErrorBox().AppendText("Time to parse: " + path + ": " + sec + "sec. \n");
            sv.getErrorBox().AppendText("You can find your .json in the same path. \n");
        }

        public static void setView(StartView nsv)
        {
            sv = nsv;
        }
    }

}