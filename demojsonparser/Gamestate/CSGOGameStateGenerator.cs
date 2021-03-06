﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using DemoInfo;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Threading;
using JSON.Events;
using JSON.Entities;
using JSON;

namespace GameStateGenerator
{
    public class CSGOGameStateGenerator
    {

        /// <summary>
        /// Parser assembling and disassembling objects later parsed with Newtonsoft.JSON
        /// </summary>
        private static Demo2JsonParser jsonparser;

        /// <summary>
        /// CSGO replay parser
        /// </summary>
        private static DemoParser parser;

        /// <summary>
        /// Current task containg parsing information and data
        /// </summary>
        private static ParseTask ptask;


        //
        //  Objects for JSON-Serialization
        //
        static Match match;
        static Round round;
        static Tick tick;

        static ReplayGamestate gs; //JSON holding the whole gamestate - delete this with GC to prevent unnecessary RAM usage!!


        //
        // Helping variables for generating process
        //
        static List<DemoInfo.Player> alreadytrackedPlayers;

        static bool hasMatchStarted = false;
        static bool hasRoundStarted = false;
        static bool hasFreeezEnded = false;

        static int positioninterval; // in ms

        static int tick_id = 0;
        static int round_id = 0;

        static int tickcount = 0;

        //
        //
        //
        //
        //
        // TODO:    1) use de-/serialization and streams for less GC and memory consumption? - most likely not useful cause string parsing is shitty
        //          6) Improve code around jsonparser - too many functions for similar tasks(see player/playerdetailed/withitems, bomb, nades) -> maybe use anonymous types
        //          9) gui communication - parsing is currently blocking UI update AND error handling missing
        //          11) implement threads?
        //          12) finish new events
        //

        /// <summary>
        /// Watch to measure generation process
        /// </summary>
        private static Stopwatch watch;

        /// <summary>
        /// Initializes the generator or resets it if a demo was parser before
        /// </summary>
        private static void initializeGenerator()
        {
            match = new Match();
            round = new Round();
            tick = new Tick();
            gs = new ReplayGamestate();

            alreadytrackedPlayers = new List<DemoInfo.Player>();

            hasMatchStarted = false;
            hasRoundStarted = false;
            hasFreeezEnded = false;

            positioninterval = ptask.positioninterval;

            tick_id = 0;
            round_id = 0;

            tickcount = 0;

            initWatch();

            //Parser to transform DemoParser events to JSON format
            jsonparser = new Demo2JsonParser(ptask.destpath, ptask.settings);

            //Init lists
            match.rounds = new List<Round>();
            round.ticks = new List<Tick>();
            tick.tickevents = new List<Gameevent>();
        }

        /// <summary>
        /// Writes the gamestate to a JSON file at the same path
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="path"></param>
        public static void GenerateJSONFile(DemoParser newdemoparser, ParseTask newtask)
        {
            ptask = newtask;
            parser = newdemoparser;

            initializeGenerator();

            GenerateGamestate();

            //Dump the complete gamestate object into JSON-file and do not pretty print(memory expensive)
            jsonparser.dumpJSONFile(gs, ptask.usepretty);

            printWatch();

            //Work is done.
            cleanUp();

        }

        /// <summary>
        /// Returns a string of the serialized gamestate object
        /// </summary>
        public static string GenerateJSONString(DemoParser newdemoparser, ParseTask newtask)
        {
            ptask = newtask;
            parser = newdemoparser;

            initializeGenerator();

            GenerateGamestate(); // Fills variable gs with gamestateobject
            string gsstr = "";
            try
            {
                gsstr = jsonparser.dumpJSONString(gs, ptask.usepretty);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.ReadLine();
            }

            printWatch();

            cleanUp();

            return gsstr;
        }


        /// <summary>
        /// Assembles the gamestate object from data given by the demoparser.
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private static void GenerateGamestate()
        {
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
                        round.winner_team = e.Winner.ToString();
                        match.rounds.Add(round);
                        round = new Round();
                        round.ticks = new List<Tick>();
                    }

                    hasRoundStarted = false;

                }

            };

            parser.FreezetimeEnded += (object sender, FreezetimeEndedEventArgs e) =>
            {
                if (hasMatchStarted)
                    hasFreeezEnded = true; //Just capture movement after freezetime has ended
            };


            #endregion

            #region Playerevents

            parser.WeaponFired += (object sender, WeaponFiredEventArgs we) =>
            {
                if (hasMatchStarted)
                    tick.tickevents.Add(jsonparser.assembleWeaponFire(we));
            };

            parser.PlayerSpotted += (sender, e) =>
            {
                if (hasMatchStarted)
                    tick.tickevents.Add(jsonparser.assemblePlayerSpotted(e));
            };

            parser.WeaponReload += (object sender, WeaponReloadEventArgs we) =>
            {
                if (hasMatchStarted && ptask.specialevents)
                    tick.tickevents.Add(jsonparser.assembleWeaponReload(we));

            };

            parser.WeaponFiredEmpty += (object sender, WeaponFiredEmptyEventArgs we) =>
            {
                if (hasMatchStarted && ptask.specialevents)
                    tick.tickevents.Add(jsonparser.assembleWeaponFireEmpty(we));

            };

            parser.PlayerJumped += (sender, e) =>
            {
                if (hasMatchStarted)
                {
                    if (e.Jumper != null && ptask.specialevents)
                    {
                        tick.tickevents.Add(jsonparser.assemblePlayerJumped(e));
                        alreadytrackedPlayers.Add(e.Jumper);
                    }
                }

            };

            parser.PlayerFallen += (sender, e) =>
            {
                if (hasMatchStarted)
                    if (e.Fallen != null && ptask.specialevents)
                        tick.tickevents.Add(jsonparser.assemblePlayerFallen(e));

            };

            parser.PlayerStepped += (sender, e) =>
            {
                if (hasMatchStarted)
                {
                    if (e.Stepper != null && parser.PlayingParticipants.Contains(e.Stepper))
                    { //Prevent spectating players from producing steps 
                        tick.tickevents.Add(jsonparser.assemblePlayerStepped(e));
                        alreadytrackedPlayers.Add(e.Stepper);
                    }
                }

            };

            parser.PlayerKilled += (object sender, PlayerKilledEventArgs e) =>
            {
                if (hasMatchStarted)
                {
                    //the killer is null if vicitm is killed by the world - eg. by falling
                    if (e.Killer != null)
                    {
                        tick.tickevents.Add(jsonparser.assemblePlayerKilled(e));
                        alreadytrackedPlayers.Add(e.Killer);
                        alreadytrackedPlayers.Add(e.Victim);
                    }
                }

            };

            parser.PlayerHurt += (object sender, PlayerHurtEventArgs e) =>
            {
                if (hasMatchStarted)
                    //the attacker is null if vicitm is damaged by the world - eg. by falling
                    if (e.Attacker != null)
                    {
                        tick.tickevents.Add(jsonparser.assemblePlayerHurt(e));
                        alreadytrackedPlayers.Add(e.Attacker);
                        alreadytrackedPlayers.Add(e.Victim);
                    }
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
                if (ptask.specialevents)
                    tick.tickevents.Add(jsonparser.assembleBomb(e, "bomb_abort_plant"));
            };

            parser.BombAbortDefuse += (object sender, BombDefuseEventArgs e) =>
            {
                if (ptask.specialevents)
                    tick.tickevents.Add(jsonparser.assembleBombDefuse(e, "bomb_abort_defuse"));
            };

            parser.BombBeginPlant += (object sender, BombEventArgs e) =>
            {
                if (ptask.specialevents)
                    tick.tickevents.Add(jsonparser.assembleBomb(e, "bomb_begin_plant"));
            };

            parser.BombBeginDefuse += (object sender, BombDefuseEventArgs e) =>
            {
                if (ptask.specialevents)
                    tick.tickevents.Add(jsonparser.assembleBombDefuse(e, "bomb_begin_defuse"));
            };

            parser.BombPlanted += (object sender, BombEventArgs e) =>
            {
                if (ptask.specialevents)
                    tick.tickevents.Add(jsonparser.assembleBomb(e, "bomb_planted"));
            };

            parser.BombDefused += (object sender, BombEventArgs e) =>
            {
                if (ptask.specialevents)
                    tick.tickevents.Add(jsonparser.assembleBomb(e, "bomb_defused"));
            };

            parser.BombExploded += (object sender, BombEventArgs e) =>
            {
                if (ptask.specialevents)
                    tick.tickevents.Add(jsonparser.assembleBomb(e, "bomb_exploded"));
            };


            parser.BombDropped += (object sender, BombDropEventArgs e) =>
            {
                if (ptask.specialevents)
                    tick.tickevents.Add(jsonparser.assembleBombState(e, "bomb_dropped"));
            };

            parser.BombPicked += (object sender, BombPickUpEventArgs e) =>
            {
                if (ptask.specialevents)
                    tick.tickevents.Add(jsonparser.assembleBombState(e, "bomb_picked"));
            };
            #endregion

            #region Serverevents

            parser.PlayerBind += (sender, e) =>
            {
                if (hasMatchStarted && parser.PlayingParticipants.Contains(e.Player))
                {
                    Console.WriteLine("Tickid: " + tick_id);
                    tick.tickevents.Add(jsonparser.assemblePlayerBind(e.Player));
                }
            };

            parser.PlayerDisconnect += (sender, e) =>
            {
                if (hasMatchStarted && parser.PlayingParticipants.Contains(e.Player))
                {
                    Console.WriteLine("Tickid: " + tick_id);
                    tick.tickevents.Add(jsonparser.assemblePlayerDisconnected(e.Player));
                }
            };

            parser.BotTakeOver += (sender, e) =>
            {
                if (hasMatchStarted && parser.PlayingParticipants.Contains(e.Taker))
                {
                    Console.WriteLine("Tickid: " + tick_id);
                    tick.tickevents.Add(jsonparser.assemblePlayerTakeOver(e));
                }
            };

            /*parser.PlayerTeam += (sender, e) =>
            {
                if (e.Swapped != null)
                    Console.WriteLine("Player swapped: " + e.Swapped.Name + " " + e.Swapped.SteamID + " Oldteam: "+ e.OldTeam + " Newteam: " + e.NewTeam +  "IsBot: " + e.IsBot + "Silent: " + e.Silent);
            };*/

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

            // TickDone at last!! Otherwise events following this region are not tracked
            #region Tickevent / Ticklogic
            //Assemble a tick object with the above gameevents
            parser.TickDone += (sender, e) =>
            {
                if (!hasMatchStarted) //Dont count ticks if the game has not started already (dismissing warmup and knife-phase for official matches)
                    return;

                // 8 = 250ms, 16 = 500ms usw
                var updaterate = 8 * (int)(Math.Ceiling(parser.TickRate / 32));

                if (updaterate == 0)
                    throw new Exception("Updaterate cannot be Zero");
                // Dump playerpositions every at a given updaterate according to the tickrate
                if ((tick_id % updaterate == 0) && hasFreeezEnded)
                    foreach (var player in parser.PlayingParticipants.Where(player => !alreadytrackedPlayers.Contains(player)))
                        tick.tickevents.Add(jsonparser.assemblePlayerPosition(player));


                tick_id++;
                alreadytrackedPlayers.Clear();
            };

            //
            // MAIN PARSING LOOP
            //
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
                            tickcount++;
                            tick = new Tick();
                            tick.tickevents = new List<Gameevent>();
                        }

                    }

                }
                Console.WriteLine("Parsed ticks: " + tick_id + "\n");
                Console.WriteLine("NOT empty ticks: " + tickcount + "\n");

            }
            catch (System.IO.EndOfStreamException e)
            {
                Console.WriteLine("Problem with tick-parsing. Is your .dem valid? See this projects github page for more info.\n");
                Console.WriteLine("Stacktrace: " + e.StackTrace + "\n");
                jsonparser.stopParser();
                watch.Stop();
            }
            #endregion

        }






        //
        //
        // HELPING FUNCTIONS
        //
        //


        /// <summary>
        /// Measure time to roughly check performance
        /// </summary>
        private static void initWatch()
        {
            if (watch == null)
                watch = System.Diagnostics.Stopwatch.StartNew();
            else
                watch.Restart();
        }

        private static void printWatch()
        {
            //Fancy calculations and feedback for 10/10 user reviews.
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            var sec = elapsedMs / 1000.0f;

            Console.WriteLine("Time to parse: " + ptask.srcpath + ": " + sec + "sec. \n");
            Console.WriteLine("You can find the corresponding JSON at the same path. \n");
        }

        /// <summary>
        /// Clean data from parser and gamestate
        /// </summary>
        public static void cleanUp()
        {
            jsonparser.stopParser();
            ptask = null;
            gs = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }

}