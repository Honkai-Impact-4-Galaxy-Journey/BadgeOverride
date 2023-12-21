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
using Mirror.LiteNetLib4Mirror;
using System.Threading;

namespace BadgeOverride
{
    public class Badge
    {
        public string reserveslot;
        public string text;
        public string userid;
        public string color;
        public string admin;
        public bool cover;
    }
    public class Plugin
    {
        [PluginConfig] public MainConfig Config;
        public static Plugin Instance;
        [PluginEntryPoint("BadgeOverride", "1.0", "-", "Silver Wolf")]
        public void OnEnabled()
        {
            Instance = this;
            EventManager.RegisterEvents<EventHandler>(this);
            EventManager.RegisterEvents<Database>(this);
            Database.Reload();
            Log.Info("Plugin Enabled");
        }
    }
    public class MainConfig
    {
        public string Connectstring { get; set; } = "Server=127.0.0.1;Database=scp;User=root;Password=123456;Charset=utf8";
        public int Slots { get; set; } = 5;
        public string RejectReason { get; set; } = "服务器已满人";
    }
    public class EventHandler
    {
       
        [PluginEvent(PluginAPI.Enums.ServerEventType.PlayerJoined)]
        public void OnPlayerJoined(PlayerJoinedEvent ev)
        {
            Timing.RunCoroutine(SetBadge(ev.Player));
        }

        public static IEnumerator<float> SetBadge(Player player)
        {
            yield return Timing.WaitForSeconds(5);
            Badge badge = Database.GetBadge(player.UserId);
            if (badge == null)
            {
                Log.Info($"Player {player.Nickname}({player.UserId}) has no badge");
                yield break;
            }
            Log.Info($"Player {player.Nickname}({player.UserId}) has a badge {badge}");
            UserGroup userGroup = new UserGroup();
            if (!string.Equals(badge.admin, "player", StringComparison.CurrentCultureIgnoreCase))
            {
                userGroup = ServerStatic.PermissionsHandler.GetGroup(badge.admin);
            }
            if (string.Equals("none", badge.text, StringComparison.CurrentCultureIgnoreCase))
            {
                if (badge.cover || string.IsNullOrEmpty(userGroup.BadgeText))
                {
                    userGroup.BadgeText = badge.text;
                }
                else
                {
                    userGroup.BadgeText = $"{userGroup.BadgeText}*{badge.text}*";
                }
                if (string.Equals(badge.color, "rainbow", StringComparison.CurrentCultureIgnoreCase))
                {
                    RainbowTag.RegisterPlayer(player);
                }
                else
                {
                   userGroup.BadgeColor = badge.color;
                }
            }
            player.SetGroup(userGroup);
        }
    }
 }
