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
            //outputStream.Write(JsonConvert.SerializeObject(s, Formatting.Indented)); //Time consuming to read (and display) for editors
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
            //ticktime in milliseconds
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

        public string parseNade(EquipmentElement nadetype, Player thrownby, Vector position)
        {
            return "\"type\": \"" + nadetype.ToString() + "\", " + parsePlayer(thrownby) + ", " + parsePosition(position);
        }

        #region NADES
        public string parseHegrenadeDetonated(GrenadeEventArgs he)
        {
            return "\"hegrenade_detonated\": { " + parseNade(he.NadeType, he.ThrownBy, he.Position) + "}";
        }

        public string parseFlashbangDetonated(FlashEventArgs fbe)
        {
            return "\"flashbang_detonated\": {" + parseNade(fbe.NadeType, fbe.ThrownBy, fbe.Position) + "\"flashed\": {" + parsePlayers(fbe.FlashedPlayers) +"}";
        }

        public string parseSmokegrenadeDetonated(SmokeEventArgs se)
        {
            return "\"smokegrenade_detonated\": {" + parseNade(se.NadeType, se.ThrownBy, se.Position) + "}";
        }

        public string parseFiregrenadeDetonated(FireEventArgs fe)
        {
            return "\"firegrenade_detonated\": {" + parseNade(fe.NadeType, fe.ThrownBy, fe.Position) + "}";
        }

        public string parseDecoyDetonated(DecoyEventArgs de)
        {
            return "\"decoy_detonated\": {" + parseNade(de.NadeType, de.ThrownBy, de.Position) + "}";
        }

        public string parseFiregrenadeEnded(FireEventArgs fe)
        {
            return "\"firegrenade_ended\": {" + parseNade(fe.NadeType, fe.ThrownBy, fe.Position) + "}";
        }

        public string parseSmokegrenadeEnded(SmokeEventArgs se)
        {
            return "\"smokegrenade_ended\": {" + parseNade(se.NadeType, se.ThrownBy, se.Position) + "}";
        }

        public string parseDecoyEnded(DecoyEventArgs de)
        {
            return "\"decoy_ended\":  {" + parseNade(de.NadeType, de.ThrownBy, de.Position) +  "}";
        }

        public string parseNadeReachedTarget(NadeEventArgs ne)
        {
            return "\"decoy_ended\":  {" + parseNade(ne.NadeType, ne.ThrownBy, ne.Position) + "}";
        }
        #endregion

        private string parseBomb(BombEventArgs be) //TODO: maybe detailed player info? see bombdefuse
        {
            return "\"site:\" \"" + be.Site + "\", "+ "\"player:\" \"" + be.Player.EntityID + "\"";
        }

        private string parseBombDefuse(BombDefuseEventArgs bde)
        {
            return "\"player:\" \"" + bde.Player.EntityID + "\", " + "\"haskit:\" \"" + bde.HasKit + "\"";
        }

        #region Bombevents
        public string parseBombExploded(BombEventArgs be)
        {
            return "\"bomb_exploded\": {" + parseBomb(be);
        }


        public string parseBombPlanted(BombEventArgs be)
        {
            return "\"bomb_planted\": {" + parseBomb(be);
        }

        public string parseBombDefused(BombEventArgs be) //No bombdefuseeevent??
        {
            return "\"bomb_defused\": {" + parseBomb(be);
        }

        public string parseBombAbortPlant(BombEventArgs be)
        {
            return "\"bomb_abortplant\": {" + parseBomb(be);
        }

        public string parseBombAbortDefuse(BombDefuseEventArgs be)
        {
            return "\"bomb_abortdefuse\": {" + parseBombDefuse(be);
        }

        public string parseBombBeginPlant(BombEventArgs be)
        {
            return "\"bomb_beginplant\": {" + parseBomb(be);
        }

        public string parseBombBeginDefuse(BombDefuseEventArgs be)
        {
            return "\"bomb_begindefuse\": {" + parseBombDefuse(be);
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
            return "\"player\": { \"player_id\": \"" + p.EntityID + "\", " + "\"playername\": \""+ p.Name + "\", \"team\": \"" + p.Team + "\"" + ", \"clan\": \"" + getClan(p)+ "\"" + ", \"steam_id\": \"" + p.SteamID + "\"}";
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

        public string parsePlayers(Player[] ps)
        {
            string players = "";
            foreach (var player in ps)
                players += parsePlayerDetailed(player);
            return players;
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
            return parsePlayerDetailed(p) +", \"items\": {" + parseWeapons(p.Weapons)+"}";
        }

        public string parsePosition(Vector v)
        {
            return "\"position\": { \"x\": \"" + v.X + "\", " + "\"y\": \"" + v.Y + "\", " + "\"z:\" \"" + v.Z + "\"}";
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
            /*
            string s = "";
            Equipment last = wps.Last();
            foreach (var wp in wps){
                if(last)
                s += parseWeapon(wp);
            }*/

            string s = "";

            using (var enumerator = wps.GetEnumerator())
            {
                var last = !enumerator.MoveNext();
                Equipment current;
                while (!last)
                {
                    current = enumerator.Current;
                    //process item
                    s += parseWeapon(current);

                    last = !enumerator.MoveNext();
                    //process item extension according to flag; flag means item
                }
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