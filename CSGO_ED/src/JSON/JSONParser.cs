using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoInfo;

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
            outputStream.Write(s);
        }

        public void stopParser()
        {
            outputStream.Close();
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
            return "\"tick\": { \"tickid\": \""+parser.CurrentTick+"\", "+"\"ticktime\": \"" + parser.CurrentTime*1000.0f + "\"\n ";
        }


        #region Gameevents
        public string parsePlayerKilled(PlayerKilledEventArgs pke)
        {
            return "\"player_killed\": ";
        }


        public string parseWeaponFire(WeaponFiredEventArgs we)
        {
            return "\"weapon_fire\": ";
        }

        public string parsePlayerHurt(PlayerHurtEventArgs p)
        {
            return "\"player_hurt\": ";
        }

        public string parsePlayerFootstep(Player p)
        {
            return "\"player_footstep\": {\n\t\"player\": "+p.EntityID +"\n\t\"position";
        }


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
        #endregion



        public string createBrackets(string s)
        {
            return "{" + s + "}";
        }

        #region SUBEVENTS

        public string parseMap(string mapname)
        {
            return "\"mapname\": \"" + mapname + "\"";
        }

        public string parseTickRate(string tr)
        {
            return "\"tickrate\": \"" + tr + "\"";
        }

        public string parsePlayerMeta(Player p)
        {
            return "\"player\": { \"playerid\": \"" + p.EntityID + "\"," + "\"playername\": \""+ p.Name + ", \"team\": \"" + p.Team + "\"";
        }

        public string parsePlayer(Player p)
        {
            return "\"player\": { \"playerid\": \"" + p.EntityID + "\"" + parsePosition(p) + parseFacing(p) + "\"team\": \""+p.Team+"\"";
        }

        public string parsePlayerDetailed(Player p)
        {
            return parsePlayer(p) + "\"HP\": \""+p.HP+"\"" + "\"Armor\": \"" + p.Armor + "\"" + "\"HasHelmet\": \"" + p.HasHelmet + "\"" + "\"HasDefuser\": \"" + p.HasDefuseKit + "\"" + "\"IsDucking\": \"" + p.IsDucking + "\"";
        }

        public string parsePlayerDetailedWithEquipment(Player p)
        {
            return parsePlayerDetailed(p);
        }

        public string parsePosition(Player player)
        {
            return "\"position\": { \"x:\" \"" + player.Position.X + "\"," + "\"y:\" \"" + player.Position.Y + "\",t" + "\"z:\" \"" + player.Position.Z + "\"}";
        }

        public string parseFacing(Player player)
        {
            return " \"facing\": { \"pitch:\" \"" + player.ViewDirectionY + "\", " + "\"yaw:\" \"" + player.ViewDirectionY + "\"}";
        }

        public string parseEntity(Player player)
        {
            return "";
        }

        public string parseWeapons(IEnumerable<Equipment> wps)
        {
            string s = "";

            foreach(var wp in wps){
                s += parseWeapon(wp)+"\n";
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