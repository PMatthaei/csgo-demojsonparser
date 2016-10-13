using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using demojsonparser.src.JSON;
using DemoInfoModded;
using Newtonsoft.Json;
using demojsonparser.src.JSON.objects;
using demojsonparser.src.JSON.events;
using System.Diagnostics;

namespace demojsonparser.src
{
    public class GameStateGenerator
    {

        /// <summary>
        /// Parser assembling and disassembling objects later parsed with Newtonsoft.JSON
        /// </summary>
        private static JSONParser jsonparser;

        /// <summary>
        /// Current task containg parsing information and data
        /// </summary>
        private static ParseTask ptask;



        private static Stopwatch watch;


        //
        //
        // TODO:    1) use de-/serialization and streams for less GC and memory consumption?
        //          3) Why are DemoInfo objects kept after using statement? (in StartView.cs)
        //          4) Build up MVC or similar
        //          6) Improve code around jsonparser - too many functions for similar tasks(see player/playerdetailed/withitems)
        //          7) isducking never returns true
        //          8) implement bomb dropped and pickup events if necessary in DemoInfoModded
        //          9) gui communication - parsing is currently blocking UI update AND error handling missing(feedback again)
        //

        /// <summary>
        /// Writes the gamestate to a JSON file at the same path
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="path"></param>
        public static void GenerateJSONFile(DemoParser demoparser, ParseTask newtask)
        {
            ptask = newtask;

            var gs = GenerateGamestate(demoparser);

            //Dump the complete gamestate object into JSON-file and do not pretty print(memory expensive)
            jsonparser.dumpJSONFile(gs, ptask.usepretty);

            //Work is done.
            jsonparser.stopParser();

            gs = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            //Fancy calculations and feedback for 10/10 user reviews.
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            var sec = elapsedMs / 1000.0f;

            Console.WriteLine("Time to parse: " + ptask.srcpath + ": " + sec + "sec. \n");
            Console.WriteLine("You can find the corresponding JSON at the same path. \n");
        }

        /// <summary>
        /// Returns a string of the serialized gamestate object
        /// </summary>
        public static string GenerateJSONString(DemoParser demoparser, ParseTask newtask)
        {
            ptask = newtask;

            var gs = GenerateGamestate(demoparser);
            string gsstr = jsonparser.dumpJSONString(gs, ptask.usepretty);

            jsonparser.stopParser();

            gs = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            //Fancy calculations and feedback for 10/10 user reviews.
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            var sec = elapsedMs / 1000.0f;

            Console.WriteLine("Time to parse: " + ptask.srcpath + ": " + sec + "sec. \n");
            Console.WriteLine("You can find the corresponding JSON at the same path. \n");

            return gsstr;
        }

        /// <summary>
        /// Assembles the gamestate object from data given by the demoparser.
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static JSONGamestate GenerateGamestate(DemoParser parser)
        {

            initWatch();

            JSONMatch match = new JSONMatch();
            JSONRound round = new JSONRound();
            JSONTick tick = new JSONTick();

            //JSON holding the whole gamestate - delete this with GC to prevent unnecessary RAM usage
            JSONGamestate gs = new JSONGamestate();

            //
            // Helping variables
            //
            List<Player> steppers = new List<Player>();

            bool hasMatchStarted = false;
            bool hasRoundStarted = false;
            bool hasFreeezEnded = false;

            int positioninterval = ptask.positioninterval;

            int tick_id = 0;
            int round_id = 0;

            //
            //
            //

            //Parser to transform DemoParser events to JSON format
            jsonparser = new JSONParser(ptask.destpath);

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
                gs.meta = jsonparser.assembleGamemeta(parser.Map, parser.TickRate, parser.PlayingParticipants);
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

            parser.FreezetimeEnded += (object sender, FreezetimeEndedEventArgs e) =>
            {
                if (hasMatchStarted)
                    hasFreeezEnded = true; //Just capture movement after freezetime has ended
            };


            parser.WeaponFired += (object sender, WeaponFiredEventArgs we) =>
            {
                if (hasMatchStarted)
                    tick.tickevents.Add(jsonparser.assembleWeaponFire(we));
            };
            #endregion

            #region Player events
            
            parser.PlayerJumped += (sender, e) =>
            {
                if (hasMatchStarted)
                {
                    if (e.Jumper != null)
                        tick.tickevents.Add(jsonparser.assemblePlayerJumped(e));
                        steppers.Add(e.Jumper);
                }

            };

            parser.PlayerStepped += (sender, e) =>
            {
                if (hasMatchStarted)
                {
                    if (e.Stepper != null)
                        tick.tickevents.Add(jsonparser.assemblePlayerStepped(e));
                        steppers.Add(e.Stepper);
                }

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

            #region Futureevents
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
            #endregion



            //Assemble a tick object with the above gameevents
            parser.TickDone += (sender, e) =>
            {
                if (!hasMatchStarted) //Dont count ticks if the game has not started already (dismissing warmup and knife-phase for official matches)
                    return;


                // Dumb playerpositions every positioninterval-ticks when freezetime has ended
                if ((tick_id % positioninterval == 0) && hasFreeezEnded)
                {
                    foreach (var player in parser.PlayingParticipants)
                    {
                        if (!checkDoubleSteps(player, steppers)) // Check if we already measured a position update of a player(by jump or step event)
                            tick.tickevents.Add(jsonparser.assemblePlayerPosition(player));
                    }
                }


                tick_id++;
                steppers.Clear();
            };



            try
            {
                //Parse tickwise and add the resulting tick to the round object
                while (parser.ParseNextTick())
                {
                    if (hasMatchStarted)
                    {
                        tick.tick_id = tick_id;
                        //Tickevents were registered
                        if (tick.tickevents.Count != 0)
                        {
                            round.ticks.Add(tick);
                            tick = new JSONTick();
                            tick.tickevents = new List<JSONGameevent>();
                        }
                    }

                }
                Console.WriteLine("ticks: "+ tick_id+"\n");

            }
            catch (System.IO.EndOfStreamException e)
            {
                Console.WriteLine("Problem with tick-parsing. Is your .dem valid? See this projects github page for more info.\n");
                Console.WriteLine("Stacktrace: " + e.StackTrace + "\n");
                jsonparser.stopParser();
                watch.Stop();
            }


            return gs;

        }

        /// <summary>
        /// Check if a players position / footstep is already in this tick to prevent doubles
        /// </summary>
        /// <returns></returns>
        private static bool checkDoubleSteps(Player p, List<Player> steppers)
        {
            return steppers.Contains(p);
        }
        
        /// <summary>
        /// Measure time to roughly check performance
        /// </summary>
        private static void initWatch()
        {
            if (watch == null)
            {
                watch = System.Diagnostics.Stopwatch.StartNew();
            }
            else
            {
                watch.Restart();
            }
        }

    }

}