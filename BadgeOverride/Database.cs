//Copyright (C) Silver Wolf 2023, All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Events;
using MEC;
using MySql.Data.MySqlClient;
using System.Runtime.Remoting.Messaging;

namespace BadgeOverride
{
    public class Database
    {
        public static List<Badge> badges = new List<Badge>();

        [PluginEvent(PluginAPI.Enums.ServerEventType.RoundRestart)]
        public void OnRoundRestart()
        {
            Task.Run(() => Reload());
        }

        public static void Reload()
        {
            using (MySqlConnection mySqlConnection = new MySqlConnection(Plugin.Instance.Config.Connectstring))
            {
                mySqlConnection.Open();
                badges.Clear();
                using (MySqlCommand command = new MySqlCommand("select * from `badge`", mySqlConnection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Badge badge = new Badge
                            {
                                text = reader.GetString("text"),
                                color = reader.GetString("color"),
                                reserveslot = reader.GetString("reserveslot"),
                                admin = reader.GetString("admin"),
                                userid = reader.GetString("userid"),
                                cover = reader.GetBoolean("cover")
                            };
                            badges.Add(badge);
                        }
                    }
                }
            }
            Log.Info($"Database reload {badges.Count}");
        }

        public static Badge GetBadge(string userid)
        {
            return badges.Find(p => p.userid == userid);
        }
    }
}
