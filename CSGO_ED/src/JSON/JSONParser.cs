using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoInfo;
using Newtonsoft.Json;

namespace CSGO_ED.src.JSON
{
    class JSONParser
    {

        StreamWriter outputStream;
        DemoParser parser;

        public JSONParser(DemoParser parser, string path)
        {
            this.parser = parser;
            string outputpath = path.Replace(".dem", "") + ".json";
            outputStream = new StreamWriter(outputpath);
        }

        public void dump(string s)
        {
            //outputStream.Write(JsonConvert.SerializeObject(s, Formatting.Indented)); //Time consuming to read and display for editors
            outputStream.Write(s);
        }

        public void stopParser()
        {
            outputStream.Close();
        }

        public string parseGameMeta()
        {
            string playersmeta = "\"players\" : [";
            foreach (var player in parser.PlayingParticipants)
                playersmeta += parsePlayerMeta(player);
            return parseMap() + ", " + parseTickRate() + ", " + playersmeta + "]";
        }

        public string parseGameState(DemoParser parser)
        {
            return "\"gamestate\": { map: " + parser.Map + "},  { tickrate: " + parser.TickRate + "}\n";
        }

        public string parseMatch()
        {
            return "\"match\": ";
        }

        public string parseRound()
        {
            return "\"round\": ";
        }

        public string parseTick()
        {
            return "\"tick\": { \"tick_id\": \""+parser.CurrentTick+"\", "+"\"ticktime\": \"" + parser.CurrentTime*1000.0f + "\"";
        }


        #region Gameevents
        public string parsePlayerKilled(PlayerKilledEventArgs pke)
        {
            return "\"player_killed\": { \"attacker\": {" + parsePlayerDetailed(pke.Killer)+"}, " + "\"victim\": {"+parsePlayerDetailed(pke.Victim) + "}, " + "\"penetrated\": \"" + pke.PenetratedObjects + "\", " + "\"headshot\": \"" + pke.Headshot + "\"}\n";
        }

        public string parseWeaponFire(WeaponFiredEventArgs we)
        {
            return "\"weapon_fire\": { \"shooter\": {" + parsePlayerDetailedWithEquipment(we.Shooter)+ "} }\n";
        }

        public string parsePlayerHurt(PlayerHurtEventArgs ph)
        {
            return "\"player_hurt\": { \"attacker\": {" + parsePlayerDetailed(ph.Attacker) + "}, " + "\"victim\": {" + parsePlayerDetailed(ph.Player) + "},  " + "\"hitgroup\": \"" + ph.Hitgroup + "\", " + "\"HP_damage\": \"" + ph.HealthDamage + "\", "+ "\"armor_damage\": \"" + ph.ArmorDamage + "\"}\n";
        }

        public string parsePlayerFootstep(Player p)
        {
            return "\"player_footstep\": {" + parsePlayer(p) + "}"; ;
        }


        #region NADES
        public string parseHegrenadeDetonated(GrenadeEventArgs he)
        {
            return "\"hegrenade_detonated\": ";
        }

        public string parseFlashbangDetonated(FlashEventArgs fbe)
        {
            return "\"flashbang_detonated\": ";
        }

        public string parseSmokegrenadeDetonated(SmokeEventArgs se)
        {
            return "\"smokegrenade_detonated\": ";
        }

        public string parseFiregrenadeDetonated(FireEventArgs fe)
        {
            return "\"firegrenade_detonated\": ";
        }

        public string parseDecoyDetonated(DecoyEventArgs de)
        {
            return "\"decoy_detonated\": ";
        }

        public string parseFiregrenadeEnded(FireEventArgs fe)
        {
            return "\"firegrenade_ended\": ";
        }

        public string parseSmokegrenadeEnded(SmokeEventArgs se)
        {
            return "\"smokegrenade_ended\": ";
        }

        public string parseDecoyEnded(DecoyEventArgs de)
        {
            return "\"decoy_ended\": ";
        }

        public string parseNadeReachedTarget(NadeEventArgs ne)
        {
            return "\"decoy_ended\": ";
        }
        #endregion

        #region Bombevents
        public string parseBombExploded(BombEventArgs ne)
        {
            return "\"bomb_exploded\"";
        }


        public string parseBombPlanted(BombEventArgs ne)
        {
            return "\"bomb_planted\"";
        }

        public string parseBombDefused(BombEventArgs ne) //No bombdefuseeevent??
        {
            return "\"bomb_defused\"";
        }

        public string parseBombAbortPlant(BombEventArgs ne)
        {
            return "\"bomb_abortplant\"";
        }

        public string parseBombAbortDefuse(BombDefuseEventArgs ne)
        {
            return "\"bomb_abortdefuse\"";
        }

        public string parseBombBeginPlant(BombEventArgs ne)
        {
            return "\"bomb_beginplant\"";
        }

        public string parseBombBeginDefuse(BombDefuseEventArgs ne)
        {
            return "\"bomb_begindefuse\"";
        }
        #endregion

        #endregion



        public string createBrackets(string s)
        {
            return "{" + s + "}";
        }

        #region SUBEVENTS

        public string parseMap()
        {
            return "\"mapname\": \"" + parser.Map + "\"";
        }

        public string parseTickRate()
        {
            return "\"tickrate\": \"" + parser.TickRate + "\"";
        }

        public string parsePlayerMeta(Player p)
        {
            return "\"player\": { \"player_id\": \"" + p.EntityID + "\", " + "\"playername\": \""+ p.Name + "\", \"team\": \"" + p.Team + "\"" + ", \"clan\": \"" + getClan(p)+ "\"}";
        }

        private string getClan(Player p)
        {
            switch (p.Team)
            {
                case Team.CounterTerrorist : return parser.CTClanName;
                case Team.Terrorist : return parser.TClanName;
                default: return "NOCLAN";
            }
        }

        public string parsePlayer(Player p)
        {
            return "\"player\": { \"player_id\": \"" + p.EntityID + "\", " + parsePosition(p) + ", "+ parseFacing(p) + ", \"team\": \""+p.Team+ "\"";
        }

        public string parsePlayerDetailed(Player p)
        {
            return parsePlayer(p) + ", \"HP\": \""+p.HP+"\", " + "\"Armor\": \"" + p.Armor + "\", " + "\"HasHelmet\": \"" + p.HasHelmet + "\", " + "\"HasDefuser\": \"" + p.HasDefuseKit + "\", " + "\"IsDucking\": \"" + p.IsDucking + "\"}";
        }

        public string parsePlayerDetailedWithEquipment(Player p)
        {
            return parsePlayerDetailed(p);
        }

        public string parsePosition(Player player)
        {
            return "\"position\": { \"x\": \"" + player.Position.X + "\", " + "\"y\": \"" + player.Position.Y + "\", " + "\"z:\" \"" + player.Position.Z + "\"}";
        }

        public string parseFacing(Player player)
        {
            return " \"facing\": { \"pitch\": \"" + player.ViewDirectionY + "\", " + "\"yaw\": \"" + player.ViewDirectionY + "\"}";
        }

        public string parseEntity(Player player)
        {
            return "";
        }

        public string parseWeapons(IEnumerable<Equipment> wps)
        {
            string s = "";

            foreach(var wp in wps){
                s += parseWeapon(wp);
            }

            return s;
        }

        public string parseWeapon(Equipment e)
        {
            return "\"weapon\": \"" + e.Weapon + "\"\n\t" + "\"silenced\": \"" + e.Class + "\"";
        }
        #endregion

    }
}