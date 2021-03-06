﻿using Oxide.Game.Rust.Cui;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Oxide.Plugins
{
    [Info("KD Ranks", "Marco Fernandes", 0.1)]
    [Description("A KD Ranking Plugin")]
    public class KDRanks : RustPlugin
    {
        //TODO:
        // - Find playerid in the database and extract their kill, death and kdr information and update it to fit the new kill/death counter upon a kill/death.
        // - Apply the above to both the killer and the victim (killer killCount increased = kdr increased = rank increased
        //                                                      victim deathCount increased = kdr decreased = rank decreased)
        // - Cui developing to show the leaderboards.
        // - Alternatively, implement command to show in the chat the available bounties.
        // - Incorporate bounties once all is said and done with the victim/killer stuff.
        // - Bounties will be based on rank leaderboard and killCount, so that if you are number 1, with 2 kills you arent worth 1000 scrap or something alike.
        // - Bounties will be placed and shown on the Cui that will be developed.

        //Dictionary will store
        // Rank (number on the leaderboards)
        // Bounty (Amount of scrap gotten for killing x)
        // Kill Count
        // Death Count
        // KD Ratio
        Dictionary<UInt64, List<string>> playerInfo = new Dictionary<UInt64, List<string>>();

        SQLiteConnection db_conn = new SQLiteConnection(@"Data Source=D:\Desktop\rust server\steamapps\common\rust_dedicated\oxide\plugins\PlayerData.db");
        void Loaded()
        {
            PrintToChat("SWAG");
            db_conn.Open();

            //foreach (BasePlayer playerInfo in BasePlayer.allPlayerList)
            //{
            //    for (int killCount = 1; killCount <=2;  killCount++)
            //    {
            //        string playerData = "INSERT INTO playerData(playerid, rank, bounty, kills, deaths, kdratio) VALUES (@playerID, @rankCount, 0, 0, 0, 0.0)";
            //        SQLiteCommand insertPlayerData = new SQLiteCommand(playerData, db_conn);
            //        insertPlayerData.Parameters.AddWithValue("@playerID", Convert.ToUInt64(playerInfo.userID));
            //        insertPlayerData.Parameters.AddWithValue("@rankCount", killCount);
            //        insertPlayerData.ExecuteNonQuery();
            //   }
            //}
        }

        void OnEntityDeath(BaseCombatEntity entity, HitInfo info)
        {
            Puts("OnEntityDeath works!");
            ulong attackerUserID;
            ulong victimUserID;
            if (entity != null)
            {
                Puts("a");
                if (entity.gameObject != null)
                {
                    Puts("b");
                    // '!' must be before the entity part here for real players, but for testing sake, its removed for scientists.
                    if ((entity is NPCPlayer))
                    {
                        Puts("c");
                        // '!' must NOT be before the entity part here for real players, but for testing sake, its added for scientists.
                        if (!(entity.lastDamage == Rust.DamageType.Bleeding))
                        {
                            Puts("d");
                            if (entity.lastAttacker != null)
                            {
                                // Establish stat counts
                                int rank;
                                int bounty;
                                int kills;
                                int deaths;
                                double kdr;
                                // Get the user that last attacked and award them the kill
                                attackerUserID = entity.lastAttacker.ToPlayer().userID;
                                victimUserID = entity.ToPlayer().userID;

                                // Get the current kill count, kdr and rank of the attacker from the database.
                                UInt64 useridval = Convert.ToUInt64(attackerUserID);

                                Console.WriteLine(useridval.ToString());

                                string playerDataQuery = "SELECT * FROM playerData WHERE playerid = @attackerid";
                                var cmd = new SQLiteCommand(playerDataQuery, db_conn);
                                cmd.Parameters.AddWithValue("@attackerid", useridval);
                                SQLiteDataReader reader = cmd.ExecuteReader();

                                while (reader.Read())
                                {
                                    //Console.WriteLine($"{reader.GetInt64(0)} {reader.GetInt32(1)} {reader.GetInt32(2)} {reader.GetInt32(3)} {reader.GetInt32(4)} {reader.GetDouble(5)}");
                                    playerInfo.Add(useridval, new List<string>());
                                    playerInfo[useridval].Add(reader.GetInt64(0).ToString());
                                    playerInfo[useridval].Add(reader.GetInt32(1).ToString());
                                    playerInfo[useridval].Add(reader.GetInt32(2).ToString());
                                    playerInfo[useridval].Add(reader.GetInt32(3).ToString());
                                    playerInfo[useridval].Add(reader.GetInt32(4).ToString());
                                    playerInfo[useridval].Add(reader.GetDouble(5).ToString());
                                }

                                foreach (KeyValuePair<UInt64, List<string>> kvp in playerInfo)
                                {
                                    List<string> withinList = kvp.Value;
                                    Console.WriteLine("Key {0}, contains {1} values:", kvp.Key, withinList.Count);
                                    rank = Int32.Parse(withinList[0]);
                                    bounty = Int32.Parse(withinList[1]);
                                    kills = Int32.Parse(withinList[2]);
                                    deaths = Int32.Parse(withinList[3]);
                                    kdr = Double.Parse(withinList[4]);
                                }

                                // Update the attacker stats.
                                int upKills = kills + 1;



                                //SQLiteCommand updatePlayerData = new SQLiteCommand(db_conn);

                                // Update the Kill/Death Counter in the database for the respective parties.
                                //updatePlayerData.CommandText = "UPDATE playerData SET kills = @killCount, kdratio = @kdr WHERE playerid = @id";
                                //updatePlayerData.Parameters.AddWithValue("@killCount", killCount);
                                //updatePlayerData.Parameters.AddWithValue("@kdr", kdr);
                                //updatePlayerData.Parameters.AddWithValue("@playerid", attackerUserID);





                            }
                        }
                    }
                }
            }
            if (!(entity is NPCPlayer))
            {
            }

            // Ignore - there is no victim for some reason
            if (entity == null)
                return;

            // Try to avoid error when entity was destroyed
            if (entity.gameObject == null)
                return;

            if (entity.lastDamage == Rust.DamageType.Bleeding)
                if (entity.lastAttacker == null)
                    return;
        }


        [ChatCommand("toplist")]
        void TopList(BasePlayer player)
        {
            var container = new CuiElementContainer();

            var panel = container.Add(new CuiPanel
            {

                Image ={
                    Color = "0.1 0.1 0.1 1"
                },

                RectTransform ={
                    AnchorMin = "0.1 0.95",
                    AnchorMax = "0.9 1"
                },

                CursorEnabled = false

            }, "Hud", "Position_panel");

            container.Add(new CuiLabel
            {
                Text ={

                    //Text = playerData.ToString(),
                    FontSize = 18
                },

                RectTransform ={
                    AnchorMin = "0.4 0",
                    AnchorMax = "0.7 1"
                }
            }, panel);

        }
    }
}
