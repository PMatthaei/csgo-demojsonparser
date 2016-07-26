using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DemoInfo;
using Newtonsoft.Json;
using demojsonparser.src.JSON.objects;
using demojsonparser.src.JSON.events;
using demojsonparser.src.JSON.objects.subobjects;

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
            JSONPlayerKilled pk = new JSONPlayerKilled
            {
                attacker = assemblePlayerDetailed(pke.Killer),
                victim = assemblePlayerDetailed(pke.Killer),
                headhshot = pke.Headshot,
                penetrated = pke.PenetratedObjects,
                hitgroup = 0
            };
            return JsonConvert.SerializeObject(pk, Formatting.Indented);
        }

        public string parseWeaponFire(WeaponFiredEventArgs we)
        {
            JSONWeaponFire wf = new JSONWeaponFire
            {
                shooter = assemblePlayerDetailed(we.Shooter)
                
            };
            return JsonConvert.SerializeObject(wf, Formatting.Indented);
        }

        public string parsePlayerHurt(PlayerHurtEventArgs phe)
        {
            JSONPlayerHurt ph = new JSONPlayerHurt
            {
                attacker = assemblePlayer(phe.Attacker),
                vicitim = assemblePlayer(phe.Player),
                armor = phe.Armor,
                armor_damage = phe.ArmorDamage,
                HP = phe.Health,
                HP_damage = phe.HealthDamage,
                hitgroup = phe.Hitgroup.ToString(),
                //weapon = ph.Weapon
            };
            return JsonConvert.SerializeObject(ph, Formatting.Indented);
        }

        public string parsePlayerFootstep(Player p)
        {
            JSONPlayerFootstep pf = new JSONPlayerFootstep
            {
                player = assemblePlayer(p)
            };
            return JsonConvert.SerializeObject(pf, Formatting.Indented);
        }

        public string parseNade(EquipmentElement nadetype, Player thrownby, Vector position, Player[] ps)
        {
            JSONNades nd = new JSONNades
            {
                thrownby = assemblePlayer(thrownby),
                nadetype = nadetype.ToString(),
                position = new JSONPosition3D { x = position.X, y = position.Y, z = position.Z },
                flashedplayers = assemblePlayers(ps)
            };
            return JsonConvert.SerializeObject(nd, Formatting.Indented);
        }

        #region NADES
        public string parseHegrenadeDetonated(GrenadeEventArgs he)
        {
            return "\"hegrenade_detonated\": { " + parseNade(he.NadeType, he.ThrownBy, he.Position, null) + "}";
        }

        public string parseFlashbangDetonated(FlashEventArgs fbe)
        {
            return "\"flashbang_detonated\": {" + parseNade(fbe.NadeType, fbe.ThrownBy, fbe.Position, fbe.FlashedPlayers);
        }

        public string parseSmokegrenadeDetonated(SmokeEventArgs se)
        {
            return "\"smokegrenade_detonated\": {" + parseNade(se.NadeType, se.ThrownBy, se.Position, null) + "}";
        }

        public string parseFiregrenadeDetonated(FireEventArgs fe)
        {
            return "\"firegrenade_detonated\": {" + parseNade(fe.NadeType, fe.ThrownBy, fe.Position, null) + "}";
        }

        public string parseDecoyDetonated(DecoyEventArgs de)
        {
            return "\"decoy_detonated\": {" + parseNade(de.NadeType, de.ThrownBy, de.Position, null) + "}";
        }

        public string parseFiregrenadeEnded(FireEventArgs fe)
        {
            return "\"firegrenade_ended\": {" + parseNade(fe.NadeType, fe.ThrownBy, fe.Position, null) + "}";
        }

        public string parseSmokegrenadeEnded(SmokeEventArgs se)
        {
            return "\"smokegrenade_ended\": {" + parseNade(se.NadeType, se.ThrownBy, se.Position, null) + "}";
        }

        public string parseDecoyEnded(DecoyEventArgs de)
        {
            return "\"decoy_ended\":  {" + parseNade(de.NadeType, de.ThrownBy, de.Position, null) +  "}";
        }

        public string parseNadeReachedTarget(NadeEventArgs ne)
        {
            return "\"decoy_ended\":  {" + parseNade(ne.NadeType, ne.ThrownBy, ne.Position, null) + "}";
        }
        #endregion

        private string parseBomb(BombEventArgs be)
        {
            JSONBombEvents b = new JSONBombEvents
            {
                site = be.Site,
                player = assemblePlayer(be.Player)
            };
            return JsonConvert.SerializeObject(b, Formatting.Indented);
        }

        private string parseBombDefuse(BombDefuseEventArgs bde)
        {
            JSONBombEvents b = new JSONBombEvents
            {
                haskit = bde.HasKit,
                player = assemblePlayer(bde.Player)
            };
            return JsonConvert.SerializeObject(b, Formatting.Indented);
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
            return JsonConvert.SerializeObject(assemblePlayers(ps), Formatting.Indented);
        }

        private List<JSONPlayer> assemblePlayers(Player[] ps)
        {
            if (ps == null)
                return null;
            List<JSONPlayer> players = new List<JSONPlayer>();
            foreach (var player in ps)
                players.Add(assemblePlayer(player));

            return players;
        }

        public string parsePlayer(Player p)
        {
            return JsonConvert.SerializeObject(assemblePlayer(p), Formatting.Indented);
        }

        private JSONPlayer assemblePlayer(Player p)
        {
            JSONPlayer player = new JSONPlayer
            {
                playername = p.Name,
                player_id = p.EntityID,
                position = new JSONPosition3D { x = p.Position.X, y = p.Position.Y, z = p.Position.Z },
                facing = new JSONFacing { yaw = p.ViewDirectionY, pitch = p.ViewDirectionX },
                team = p.Team.ToString()
            };
            return player;
        }

        public string parsePlayerDetailed(Player p)
        {
            return JsonConvert.SerializeObject(assemblePlayerDetailed(p), Formatting.Indented);
        }

        private JSONPlayerDetailed assemblePlayerDetailed(Player p)
        {
            JSONPlayerDetailed playerdetailed = new JSONPlayerDetailed
            {
                playername = p.Name,
                player_id = p.EntityID,
                position = new JSONPosition3D { x = p.Position.X, y = p.Position.Y, z = p.Position.Z },
                facing = new JSONFacing { yaw = p.ViewDirectionY, pitch = p.ViewDirectionX },
                team = p.Team.ToString(),
                hasHelmet = p.HasHelmet,
                hasdefuser = p.HasDefuseKit,
                HP = p.HP,
                armor = p.Armor
            };

            return playerdetailed;
        }

        public string parsePlayerDetailedWithEquipment(Player p)
        {
            return JsonConvert.SerializeObject(assemblePlayerDetailedWithItems(p), Formatting.Indented);
        }

        private JSONPlayerDetailedWithItems assemblePlayerDetailedWithItems(Player p)
        {
            JSONPlayerDetailedWithItems playerdetailed = new JSONPlayerDetailedWithItems
            {
                playername = p.Name,
                player_id = p.EntityID,
                position = new JSONPosition3D { x = p.Position.X, y = p.Position.Y, z = p.Position.Z },
                facing = new JSONFacing { yaw = p.ViewDirectionY, pitch = p.ViewDirectionX },
                team = p.Team.ToString(),
                hasHelmet = p.HasHelmet,
                hasdefuser = p.HasDefuseKit,
                HP = p.HP,
                armor = p.Armor,
                items = assembleWeapons(p.Weapons)
            };

            return playerdetailed;
        }

        private List<JSONItem> assembleWeapons(IEnumerable<Equipment> wps)
        {
            List<JSONItem> jwps = new List<JSONItem>();
            foreach (var w in wps)
                jwps.Add(assembleWeapon(w));

            return jwps;
        }
        private JSONItem assembleWeapon(Equipment wp)
        {
            JSONItem jwp = new JSONItem
            {
                weapon = wp.Weapon.ToString(),
                ammoinmagazine = wp.AmmoInMagazine
            };

            return jwp;
        }

        #endregion

    }
}