using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using CSGO_ED.src.JSON;
using DemoInfo;

namespace CSGO_ED.src
{
    class GameStateGenerator
    {
        static int tickcount = 0;
        static int stepcount = 0;
        static int hurtcount = 0;

        const int positioninterval = 8;


        public static void GenerateJSONFile(DemoParser parser, string path)
        {
            //Parser to transform DemoParser events to JSON format
            JSONParser jsonparser = new JSONParser(parser, path);

            //Accumulated string we will print
            string jsonstring = "";

            //Variables
            bool hasMatchStarted = false;
            bool hasRoundStarted = false;

            int roundcount = 0;
            List<Player> ingame_players = new List<Player>(); //All players

            //Measure time to roughly check performance
            var watch = System.Diagnostics.Stopwatch.StartNew();

            //Obligatory to use this parser
            parser.ParseHeader();

            //Write the gamestate object
            jsonparser.dump("\"gamestate\": {\n\t\"map\": \"" + parser.Map + "\",\n\t\"tickrate\": \"" + parser.TickRate + "\"\n}\n");
            
            //Start writing the match object
            parser.MatchStarted += (sender, e) => {
                hasMatchStarted = true;

                foreach (var player in parser.PlayingParticipants)
                    jsonparser.dump(jsonparser.parsePlayer(player));

                ingame_players.AddRange(parser.PlayingParticipants);
            };

            //Close gamestate object
            parser.WinPanelMatch += (sender, e) => {
                jsonparser.dump("\n}");
            };

            //Start writing a round object
            parser.RoundStart += (sender, e) => {
                hasRoundStarted = true;
                roundcount++;
                jsonparser.dump("\"round\": {\n\t\"roundid\": \"" + roundcount + "\",\n\t\"tickrate\": \"" + parser.TickRate + "\"\n");

            };

            //Close round object
            parser.RoundEnd += (sender, e) => {
                hasRoundStarted = false;
                jsonparser.dump("\n}");
            };


            //All events that happen during a tick


            parser.WeaponFired += (object sender, WeaponFiredEventArgs we) => {
                jsonparser.parseWeaponFire(we);
            };

            Console.WriteLine("Registered fired weapon event");

            parser.PlayerKilled += (object sender, PlayerKilledEventArgs e) => {
                //the killer is null if he`s killed by the world - eg. by falling
                if (e.Killer != null)
                {
                    jsonparser.parsePlayerKilled(e);
                }
            };

            parser.PlayerHurt += (object sender, PlayerHurtEventArgs e) => {
                jsonparser.parsePlayerHurt(e);
            };

            Console.WriteLine("Registered killevent");

            //Nade (Smoke Fire Decoy Flashbang and HE) events
            parser.ExplosiveNadeExploded += (object sender, GrenadeEventArgs e) => {
                if (e.ThrownBy != null)
                {
                    string name = e.ThrownBy.EntityID.ToString();
                    string entityid = e.ThrownBy.EntityID.ToString();
                    string position = e.ThrownBy.Position.ToString();
                    string facingX = e.ThrownBy.ViewDirectionX.ToString();
                    string facingY = e.ThrownBy.ViewDirectionY.ToString();
                    string nadeposition = e.Position.ToString();
                    jsonstring += "nade_exploded:{ player: " + name + " facing: " + facingX + ", " + facingY + "positon:" + position + " nade_position: " + nadeposition + "}\n";
                }
            };

            parser.FireNadeStarted += (object sender, FireEventArgs e) => {
                string nadeposition = e.Position.ToString();
                jsonstring += "firenade_started:{ nade_position: " + nadeposition + "}\n";
            };

            parser.FireNadeEnded += (object sender, FireEventArgs e) => {
                string nadeposition = e.Position.ToString();
                jsonstring += "firenade_ended:{ nade_position: " + nadeposition + "}\n";
            };

            parser.SmokeNadeStarted += (object sender, SmokeEventArgs e) => {
                string name = e.ThrownBy.EntityID.ToString();
                string entityid = e.ThrownBy.EntityID.ToString();
                string position = e.ThrownBy.Position.ToString();
                string facingX = e.ThrownBy.ViewDirectionX.ToString();
                string facingY = e.ThrownBy.ViewDirectionY.ToString();
                string nadeposition = e.Position.ToString();
                jsonstring += "smokenade_started:{ player: " + name + " facing: " + facingX + ", " + facingY + "positon:" + position + " nade_position: " + nadeposition + "}\n";
            };


            parser.SmokeNadeEnded += (object sender, SmokeEventArgs e) => {
                string name = e.ThrownBy.EntityID.ToString();
                string entityid = e.ThrownBy.EntityID.ToString();
                string position = e.ThrownBy.Position.ToString();
                string facingX = e.ThrownBy.ViewDirectionX.ToString();
                string facingY = e.ThrownBy.ViewDirectionY.ToString();
                string nadeposition = e.Position.ToString();
                jsonstring += "smokenade_ended:{ player: " + name + " facing: " + facingX + ", " + facingY + "positon:" + position + " nade_position: " + nadeposition + "}\n";
            };

            parser.DecoyNadeStarted += (object sender, DecoyEventArgs e) => {
                string name = e.ThrownBy.EntityID.ToString();
                string entityid = e.ThrownBy.EntityID.ToString();
                string position = e.ThrownBy.Position.ToString();
                string facingX = e.ThrownBy.ViewDirectionX.ToString();
                string facingY = e.ThrownBy.ViewDirectionY.ToString();
                string nadeposition = e.Position.ToString();
                jsonstring += "decoynade_ended:{ player: " + name + " facing: " + facingX + ", " + facingY + "positon:" + position + " nade_position: " + nadeposition + "}\n";
            };

            parser.DecoyNadeEnded += (object sender, DecoyEventArgs e) => {
                string name = e.ThrownBy.EntityID.ToString();
                string entityid = e.ThrownBy.EntityID.ToString();
                string position = e.ThrownBy.Position.ToString();
                string facingX = e.ThrownBy.ViewDirectionX.ToString();
                string facingY = e.ThrownBy.ViewDirectionY.ToString();
                string nadeposition = e.Position.ToString();
                jsonstring += "decoynade_ended:{ player: " + name + " facing: " + facingX + ", " + facingY + "positon:" + position + " nade_position: " + nadeposition + "}\n";
            };

            parser.NadeReachedTarget += (object sender, NadeEventArgs e) => {
                string nadeposition = e.Position.ToString();
                jsonstring += "nade_reachedtarget:{ nade_position: " + nadeposition + "}\n";
            };

            parser.FlashNadeExploded += (object sender, FlashEventArgs e) => {
                string name = e.ThrownBy.EntityID.ToString();
                string entityid = e.ThrownBy.EntityID.ToString();
                string position = e.ThrownBy.Position.ToString();
                string facingX = e.ThrownBy.ViewDirectionX.ToString();
                string facingY = e.ThrownBy.ViewDirectionY.ToString();
                string nadeposition = e.Position.ToString();
                jsonstring += "flashnade_exploded:{ player: " + name + " facing: " + facingX + ", " + facingY + "positon:" + position + " nade_position: " + nadeposition + "}\n";
            };


            parser.BombAbortDefuse += (sender, e) => {

            };

            parser.BombAbortPlant += (sender, e) => {

            };

            parser.BombBeginDefuse += (sender, e) => {

            };

            parser.BombBeginPlant += (sender, e) => {

            };

            parser.BombDefused += (sender, e) => {

            };

            parser.BombExploded += (sender, e) => {

            };

            parser.BombPlanted += (sender, e) => {

            };

            //Open a tick object
            parser.TickDone += (sender, e) => {
                // Every tick save id and time
                //jsonstring += "tick: { id: " + parser.CurrentTick + "}\n";
                jsonparser.dump(jsonparser.parseTick());

                // Dumb playerpositions every positioninterval-ticks
                foreach (var player in parser.PlayingParticipants)
                {
                    if (tickcount % positioninterval == 0)
                        jsonparser.dump(jsonparser.parsePlayer(player));
                }

                tickcount++;

            };

            //Parse tickwise
            bool hasnext = true;
            while (hasnext)
            {
                hasnext = parser.ParseNextTick();
                jsonparser.dump(jsonstring);
                jsonstring = "";
                jsonparser.dump("}\n");
            }


            jsonparser.dump("ticks: "+tickcount);
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