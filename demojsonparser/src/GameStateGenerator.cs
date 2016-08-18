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

        //
        //
        // TODO: use de-/serialization and streams for less GC and memory consumption?
        //
        //
        public static JSONMatch match = new JSONMatch();
        public static JSONRound round = new JSONRound();
        public static JSONTick tick = new JSONTick();

        public static void GenerateJSONFile(DemoParser parser, string path)
        {

            //JSONTick nulltick = new JSONTick(); Just for testing or if empty ticks are wanted

            bool hasMatchStarted = false;
            bool hasRoundStarted = false;
            bool hasFreeezEnded = false;

            int tick_id = 0;
            int eventcount = 0;
            int round_id = 0;

            //Measure time to roughly check performance
            var watch = System.Diagnostics.Stopwatch.StartNew();
           
            //JSON holding the whole gamestate
            JSONGamestate gs = new JSONGamestate();

            try
            {
                //Init lists
                match.rounds = new List<JSONRound>();
                round.ticks = new List<JSONTick>();
                tick.tickevents = new List<JSONGameevent>();

                //Parser to transform DemoParser events to JSON format
                JSONParser jsonparser = new JSONParser(parser, path);

                //Obligatory to use this parser
                parser.ParseHeader();


                //Start writing the gamestate object
                parser.MatchStarted += (sender, e) =>
                {
                    hasMatchStarted = true;
                    //Assign Gamemetadata
                    gs.meta = jsonparser.assembleGamemeta();
                    eventcount++;
                };

                //Assign match object
                parser.WinPanelMatch += (sender, e) =>
                {
                    if (hasMatchStarted)
                        gs.match = match;
                    hasMatchStarted = false;
                    eventcount++;

                };

                //Start writing a round object
                parser.RoundStart += (sender, e) =>
                {
                    if (hasMatchStarted)
                    {
                        hasRoundStarted = true;
                        round_id++;
                        round.round_id = round_id;
                        eventcount++;
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
                            eventcount++;
                        }

                        hasRoundStarted = false;

                    }

                };



                parser.WeaponFired += (object sender, WeaponFiredEventArgs we) =>
                {
                    if (hasMatchStarted)
                        tick.tickevents.Add(jsonparser.assembleWeaponFire(we));
                    eventcount++;
                };


                parser.PlayerKilled += (object sender, PlayerKilledEventArgs e) =>
                {
                    if (hasMatchStarted)
                    {
                        //the killer is null if vicitm is killed by the world - eg. by falling
                        if (e.Killer != null)
                            tick.tickevents.Add(jsonparser.assemblePlayerKilled(e));
                        eventcount++;

                    }

                };

                parser.PlayerHurt += (object sender, PlayerHurtEventArgs e) =>
                {
                    if (hasMatchStarted)
                        //the attacker is null if vicitm is damaged by the world - eg. by falling
                        if (e.Attacker != null)
                            tick.tickevents.Add(jsonparser.assemblePlayerHurt(e));
                    eventcount++;
                };

                #region Nadeevents
                //Nade (Smoke Fire Decoy Flashbang and HE) events
                parser.ExplosiveNadeExploded += (object sender, GrenadeEventArgs e) =>
                {
                    if (e.ThrownBy != null && hasMatchStarted)
                        tick.tickevents.Add(jsonparser.assembleHEGrenade(e));
                    eventcount++;
                };

                parser.FireNadeStarted += (object sender, FireEventArgs e) =>
                {
                    if (e.ThrownBy != null && hasMatchStarted)
                        tick.tickevents.Add(jsonparser.assembleFiregrenade(e));
                    eventcount++;
                };

                parser.FireNadeEnded += (object sender, FireEventArgs e) =>
                {
                    if (e.ThrownBy != null && hasMatchStarted)
                        tick.tickevents.Add(jsonparser.assembleFiregrenadeEnded(e));
                    eventcount++;
                };

                parser.SmokeNadeStarted += (object sender, SmokeEventArgs e) =>
                {
                    if (e.ThrownBy != null && hasMatchStarted)
                        tick.tickevents.Add(jsonparser.assembleSmokegrenade(e));
                    eventcount++;
                };


                parser.SmokeNadeEnded += (object sender, SmokeEventArgs e) =>
                {
                    if (e.ThrownBy != null && hasMatchStarted)
                        tick.tickevents.Add(jsonparser.assembleSmokegrenadeEnded(e));
                    eventcount++;
                };

                parser.DecoyNadeStarted += (object sender, DecoyEventArgs e) =>
                {
                    if (e.ThrownBy != null && hasMatchStarted)
                        tick.tickevents.Add(jsonparser.assembleDecoy(e));
                    eventcount++;
                };

                parser.DecoyNadeEnded += (object sender, DecoyEventArgs e) =>
                {
                    if (e.ThrownBy != null && hasMatchStarted)
                        tick.tickevents.Add(jsonparser.assembleDecoyEnded(e));
                    eventcount++;
                };

                parser.FlashNadeExploded += (object sender, FlashEventArgs e) =>
                {
                    if (e.ThrownBy != null && hasMatchStarted)
                        tick.tickevents.Add(jsonparser.assembleFlashbang(e));
                    eventcount++;
                };

                parser.NadeReachedTarget += (object sender, NadeEventArgs e) =>
                {
                    if (e.ThrownBy != null && hasMatchStarted)
                        tick.tickevents.Add(jsonparser.assembleNade(e.NadeType, e.ThrownBy, e.Position, null));
                    eventcount++;
                };

                #endregion

                #region Bombevents
                parser.BombAbortPlant += (object sender, BombEventArgs e) =>
                {
                    JSONBomb b = jsonparser.assembleBomb(e);
                    b.gameevent = "bomb_abort_plant";
                    tick.tickevents.Add(b);
                    eventcount++;
                };

                parser.BombAbortDefuse += (object sender, BombDefuseEventArgs e) =>
                {
                    JSONBomb b = jsonparser.assembleBombDefuse(e);
                    b.gameevent = "bomb_abort_defuse";
                    tick.tickevents.Add(b);
                    eventcount++;
                };

                parser.BombBeginPlant += (object sender, BombEventArgs e) =>
                {
                    JSONBomb b = jsonparser.assembleBomb(e);
                    b.gameevent = "bomb_begin_plant";
                    tick.tickevents.Add(b);
                    eventcount++;
                };

                parser.BombBeginDefuse += (object sender, BombDefuseEventArgs e) =>
                {
                    JSONBomb b = jsonparser.assembleBombDefuse(e);
                    b.gameevent = "bomb_begin_defuse";
                    tick.tickevents.Add(b);
                    eventcount++;
                };

                parser.BombPlanted += (object sender, BombEventArgs e) =>
                {
                    JSONBomb b = jsonparser.assembleBomb(e);
                    b.gameevent = "bomb_planted";
                    tick.tickevents.Add(b);
                    eventcount++;
                };

                parser.BombDefused += (object sender, BombEventArgs e) =>
                {
                    JSONBomb b = jsonparser.assembleBomb(e);
                    b.gameevent = "bomb_defused";
                    tick.tickevents.Add(b);
                    eventcount++;
                };

                parser.BombExploded += (object sender, BombEventArgs e) =>
                {
                    JSONBomb b = jsonparser.assembleBomb(e);
                    b.gameevent = "bomb_exploded";
                    tick.tickevents.Add(b);
                    eventcount++;
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
                    eventcount++;
                };

                //Create a tick object
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
                            eventcount++;
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
                        jsonparser.dump("Problem with tickparsing. Is your .dem valid?");
                        jsonparser.dump(e.StackTrace);
                        return;
                    }

                    if (hasRoundStarted)
                    {
                        tick.tick_id = tick_id;
                        if (eventcount != 0)
                        {
                            //When something happend during the tick(more than 0 events)
                            round.ticks.Add(tick);
                            tick = new JSONTick();
                            tick.tickevents = new List<JSONGameevent>();
                            eventcount = 0;
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

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                var sec = elapsedMs / 1000.0f;

                //logger.LogWrite("Time to parse:" + path + ": " + sec + "\n");
            }
            catch (Exception e)
            {
                // logger.LogWrite("Exception in "+path+": "+ e.StackTrace);
            }

        }

    }
}