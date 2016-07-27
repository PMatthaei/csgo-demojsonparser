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

namespace GameStateGenerator.src.JSON
{
    class JSONParser
    {

        StreamWriter outputStream;
        DemoParser parser;

        enum PlayerType {META, NORMAL, DETAILED, WITHEQUIPMENT };

        public JSONParser(DemoParser parser, string path)
        {
            this.parser = parser;
            string outputpath = path.Replace(".dem", "") + ".json";
            outputStream = new StreamWriter(outputpath);
        }

        public void dump(JSONGamestate gs)
        {
            outputStream.Write(JsonConvert.SerializeObject(gs, Formatting.Indented));
        }

        public void stopParser()
        {
            outputStream.Close();
        }


        public JSONGamemeta assembleGamemeta()
        {
            return new JSONGamemeta
            {
                gamestate_id = 0,
                mapname = parser.Map,
                tickrate = parser.TickRate,
                players = assemblePlayers(parser.PlayingParticipants),
            };
        }


        #region Gameevents

        public JSONPlayerKilled assemblePlayerKilled(PlayerKilledEventArgs pke)
        {
            return new JSONPlayerKilled
            {
                attacker = assemblePlayerDetailed(pke.Killer),
                victim = assemblePlayerDetailed(pke.Killer),
                headhshot = pke.Headshot,
                penetrated = pke.PenetratedObjects,
                hitgroup = 0
            };
        }

        public JSONWeaponFire assembleWeaponFire(WeaponFiredEventArgs we)
        {
            return new JSONWeaponFire
            {
                shooter = assemblePlayerDetailed(we.Shooter)

            };
        }

        public JSONPlayerHurt assemblePlayerHurt(PlayerHurtEventArgs phe)
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
            return ph;
        }

        public JSONPlayerFootstep assemblePlayerFootstep(Player p)
        {
            return new JSONPlayerFootstep
            {
                player = assemblePlayer(p)
            };
        }

        public string parseNade(EquipmentElement nadetype, Player thrownby, Vector position, Player[] ps)
        {
            JSONNades nd = assembleNade(nadetype, thrownby, position, ps);
            return JsonConvert.SerializeObject(nd, Formatting.None);
        }

        public JSONNades assembleNade(EquipmentElement nadetype, Player thrownby, Vector position, Player[] ps)
        {
            return new JSONNades
            {
                thrownby = assemblePlayer(thrownby),
                nadetype = nadetype.ToString(),
                position = new JSONPosition3D { x = position.X, y = position.Y, z = position.Z },
                flashedplayers = assemblePlayers(ps)
            };
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
            JSONBomb b = assembleBomb(be);
            return JsonConvert.SerializeObject(b, Formatting.Indented);
        }

        public JSONBomb assembleBomb(BombEventArgs be)
        {
            return new JSONBomb
            {
                site = be.Site,
                player = assemblePlayer(be.Player)
            };
        }

        private string parseBombDefuse(BombDefuseEventArgs bde)
        {
            JSONBomb b = assembleBombDefuse(bde);
            return JsonConvert.SerializeObject(b, Formatting.Indented);
        }

        public JSONBomb assembleBombDefuse(BombDefuseEventArgs bde)
        {
            return new JSONBomb
            {
                haskit = bde.HasKit,
                player = assemblePlayer(bde.Player)
            };
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
        
        private string getClan(Player p)
        {
            switch (p.Team)
            {
                case Team.CounterTerrorist : return parser.CTClanName;
                case Team.Terrorist : return parser.TClanName;
                default: return "NOCLAN";
            }
        }


        public List<JSONPlayer> assemblePlayers(Player[] ps)
        {
            if (ps == null)
                return null;
            List<JSONPlayer> players = new List<JSONPlayer>();
            foreach (var player in ps)
                players.Add(assemblePlayer(player));

            return players;
        }

        public List<JSONPlayerMeta> assemblePlayers(IEnumerable<Player> ps)
        {
            if (ps == null)
                return null;
            List<JSONPlayerMeta> players = new List<JSONPlayerMeta>();
            foreach (var player in ps)
                players.Add(assemblePlayerMeta(player));

            return players;
        }


        public JSONPlayer assemblePlayer(Player p)
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

        public JSONPlayerMeta assemblePlayerMeta(Player p)
        {
            JSONPlayerMeta player = new JSONPlayerMeta
            {
                playername = p.Name,
                player_id = p.EntityID,
                team = p.Team.ToString(),
                clanname = p.AdditionaInformations.Clantag,
                steam_id = p.SteamID
            
            };
            return player;
        }

        public JSONPlayerDetailed assemblePlayerDetailed(Player p)
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


        public JSONPlayerDetailedWithItems assemblePlayerDetailedWithItems(Player p)
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

        public List<JSONItem> assembleWeapons(IEnumerable<Equipment> wps)
        {
            List<JSONItem> jwps = new List<JSONItem>();
            foreach (var w in wps)
                jwps.Add(assembleWeapon(w));

            return jwps;
        }
        public JSONItem assembleWeapon(Equipment wp)
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