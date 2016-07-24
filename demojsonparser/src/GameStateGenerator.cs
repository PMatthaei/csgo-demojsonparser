using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using CSGO_ED.src.JSON;
using DemoInfo;
using Newtonsoft.Json;

namespace CSGO_ED.src
{
    class GameStateGenerator
    {
        static int tick_id = 0;

        const int positioninterval = 8;


        public static void GenerateJSONFile(DemoParser parser, string path)
        {
            //Parser to transform DemoParser events to JSON format
            JSONParser jsonparser = new JSONParser(parser, path);

            //Accumulated string we will print
            string jsonstring = "";

            //Variables
            //Use this to differntiate between warmup(maybe even knife rounds in official matches) rounds and real "counting" rounds
            bool hasMatchStarted = false; 
            bool hasRoundStarted = false;

            int round_id = 0;

            int roundkills = 0;
            Team roundwinner = Team.Spectate; //This default value whill cause errors if not correctly assigned!

            List<Player> ingame_players = new List<Player>(); //All players

            //Measure time to roughly check performance
            var watch = System.Diagnostics.Stopwatch.StartNew();

            //Obligatory to use this parser
            parser.ParseHeader();


            
            
            //Start writing the gamestate object
            parser.MatchStarted += (sender, e) => {
                hasMatchStarted = true;

                //Write the gamestate object with its meta data about the game
                jsonparser.dump("\"gamestate\": { " + jsonparser.parseGameMeta() + "}");

                //jsonparser.dump(" \n\n\n\n\n MATCH STARTED \n\n\n\n\n");
            };

            //Close gamestate object
            parser.WinPanelMatch += (sender, e) => {
                if (hasMatchStarted)
                    jsonparser.dump("\n}");
                    hasMatchStarted = false;
            };

            //Start writing a round object
            parser.RoundStart += (sender, e) => {
                if (hasMatchStarted)
                {
                    hasRoundStarted = true;
                    round_id++;
                    jsonparser.dump("\"round\": {\n\t\"round_id\": \"" + round_id + "\",\n\t\"tickrate\": \"" + parser.TickRate + "\"\n");
                }

            };

            //Close round object
            parser.RoundEnd += (sender, e) => {
                if (hasMatchStarted)
                {
                    hasRoundStarted = false;
                    roundwinner = e.Winner;
                    jsonparser.dump("\n}");
                }

            };


            //All events that happen during a tick


            parser.WeaponFired += (object sender, WeaponFiredEventArgs we) => {
                if (hasMatchStarted)
                    jsonparser.dump(jsonparser.parseWeaponFire(we));
            };


            parser.PlayerKilled += (object sender, PlayerKilledEventArgs e) => {
                if (hasMatchStarted)
                    //the killer is null if vicitm is killed by the world - eg. by falling
                    if (e.Killer != null)
                    {
                        jsonparser.dump(jsonparser.parsePlayerKilled(e));
                    }
            };

            parser.PlayerHurt += (object sender, PlayerHurtEventArgs e) => {
                if (hasMatchStarted)
                    //the attacker is null if vicitm is damaged by the world - eg. by falling
                    if (e.Attacker != null)
                    {
                        jsonparser.dump(jsonparser.parsePlayerHurt(e));
                    }
            };

            #region Nadeevents
            //Nade (Smoke Fire Decoy Flashbang and HE) events
            parser.ExplosiveNadeExploded += (object sender, GrenadeEventArgs e) => {
                if (e.ThrownBy != null && hasMatchStarted)
                {
                    jsonparser.dump(jsonparser.parseHegrenadeDetonated(e));
                }
            };

            parser.FireNadeStarted += (object sender, FireEventArgs e) => {
                if (e.ThrownBy != null && hasMatchStarted)
                {
                    jsonparser.dump(jsonparser.parseFiregrenadeDetonated(e));
                }
            };

            parser.FireNadeEnded += (object sender, FireEventArgs e) => {
                if (e.ThrownBy != null && hasMatchStarted)
                {
                    jsonparser.dump(jsonparser.parseFiregrenadeEnded(e));
                }
            };

            parser.SmokeNadeStarted += (object sender, SmokeEventArgs e) => {
                if (e.ThrownBy != null && hasMatchStarted)
                {
                    jsonparser.dump(jsonparser.parseSmokegrenadeDetonated(e));
                }
            };


            parser.SmokeNadeEnded += (object sender, SmokeEventArgs e) => {
                if (e.ThrownBy != null && hasMatchStarted)
                {
                    jsonparser.dump(jsonparser.parseSmokegrenadeEnded(e));
                }
            };

            parser.DecoyNadeStarted += (object sender, DecoyEventArgs e) => {
                if (e.ThrownBy != null && hasMatchStarted)
                {
                    jsonparser.dump(jsonparser.parseDecoyDetonated(e));
                }
            };

            parser.DecoyNadeEnded += (object sender, DecoyEventArgs e) => {
                if (e.ThrownBy != null && hasMatchStarted)
                {
                    jsonparser.dump(jsonparser.parseDecoyEnded(e));
                }
            };

            parser.FlashNadeExploded += (object sender, FlashEventArgs e) => {
                if (e.ThrownBy != null && hasMatchStarted)
                {
                    jsonparser.dump(jsonparser.parseFlashbangDetonated(e));
                }
            };

            parser.NadeReachedTarget += (object sender, NadeEventArgs e) => {
                if (e.ThrownBy != null && hasMatchStarted)
                {
                    jsonparser.dump(jsonparser.parseNadeReachedTarget(e));
                }
            };
            #endregion

            #region Bombevents
            parser.BombAbortPlant += (object sender, BombEventArgs e) => {
                jsonparser.dump(jsonparser.parseBombAbortPlant(e));
            };

            parser.BombAbortDefuse += (object sender, BombDefuseEventArgs e) => {
                jsonparser.dump(jsonparser.parseBombAbortDefuse(e));
            };

            parser.BombBeginPlant += (object sender, BombEventArgs e) => {
                jsonparser.dump(jsonparser.parseBombBeginPlant(e));
            };

            parser.BombBeginDefuse += (object sender, BombDefuseEventArgs e) => {
                jsonparser.dump(jsonparser.parseBombBeginDefuse(e));
            };

            parser.BombPlanted += (object sender, BombEventArgs e) => {
                jsonparser.dump(jsonparser.parseBombPlanted(e));
            };

            parser.BombDefused += (object sender, BombEventArgs e) => {
                jsonparser.dump(jsonparser.parseBombDefused(e));
            };

            parser.BombExploded += (object sender, BombEventArgs e) => {
                jsonparser.dump(jsonparser.parseBombExploded(e));
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

            //Open a tick object
            parser.TickDone += (sender, e) => {
                if (!hasMatchStarted) //Dont count ticks if the game has not started already (dismissing warmup and knife-phase for official matches)
                    return;
                // Every tick save id and time
                jsonparser.dump(jsonparser.parseTick() + "}\n");

                // Dumb playerpositions every positioninterval-ticks
                if (tick_id % positioninterval == 0 )
                    foreach (var player in parser.PlayingParticipants)
                    {
                        jsonparser.dump(jsonparser.parsePlayerFootstep(player));
                    }

                tick_id++;

            };

            //Parse tickwise
            bool hasnext = true;
            while (hasnext)
            {
                hasnext = parser.ParseNextTick();
                jsonparser.dump(jsonstring);
                jsonstring = "";
                //if (hasMatchStarted) //When match has started close every tick object
                    //jsonparser.dump("}\n");
            }


            jsonparser.dump("ticks: "+tick_id);

            jsonparser.stopParser();

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            var sec = elapsedMs / 1000.0f;

            Console.Write("Time(in Seconds): " + sec + "\n");
        }


        public static void GenerateTick(DemoParser parser)
        {

        }
    }
}