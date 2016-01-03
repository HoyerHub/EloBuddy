﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using VolatileAIO.Extensions.ADC;
using VolatileAIO.Extensions.Jungle;
using VolatileAIO.Extensions.Mid;
using VolatileAIO.Extensions.Support;
using VolatileAIO.Extensions.Top;

namespace VolatileAIO.Organs.Brain
{
    internal class ExtensionLoader
    {
        private static bool _loaded;

        internal readonly List<Champion> Champions = new List<Champion>
        {
            new Champion("Alistar", State.FullyDeveloped, "turkey"),
            new Champion("Annie", State.FullyDeveloped, "Bloodimir"),
            new Champion("Blitzcrank", State.FullyDeveloped, "turkey"),
            new Champion("Brand", State.BeingOptimized, "turkey"),
            new Champion("Cassiopeia", State.Outdated, "turkey"),
            new Champion("Evelynn", State.FullyDeveloped, "Bloodimir"),
            new Champion("Ezreal", State.FullyDeveloped, "turkey"),
            new Champion("Morgana", State.FullyDeveloped, "Bloodimir"),
            new Champion("Tristana", State.FullyDeveloped, "turkey"),
            new Champion("Vladimir", State.FullyDeveloped, "Bloodimir"),
            new Champion("Ziggs", State.FullyDeveloped, "Bloodimir")
        };

        internal struct Champion
        {
            internal string Name;
            internal State State;
            internal string Developer;

            public Champion(string name, State state, string developer)
            {
                Developer = developer;
                State = state;
                Name = name;
            }

        }

        public enum State
        {
            Outdated = 0,
            PartDeveloped = 1,
            BeingOptimized = 2,
            FullyDeveloped = 3,
        }

        public bool IncludesChampion(string champ)
        {
            return Champions.Any(champs => champs.Name == champ);
        }

        private void WelcomeChat()
        {
            Chat.Print("<font color = \"#00FF00\">Succesfully loaded Extension: </font><font color = \"#FFFF00\">" +
                       ObjectManager.Player.ChampionName + "</font>");
            var state = Champions.Find(c => c.Name == ObjectManager.Player.ChampionName).State;
            if ((int) state < 3)
                Chat.Print("<font color = \"#FFCC00\">Please note:</font> <font color = \"#FFFF00\">" +
                           ObjectManager.Player.ChampionName +
                           "</font><font color = \"#FFCC00\"> is still </font>!<font color = \"#800000\">" +
                           Enum.GetName(state.GetType(), state) + "</font>!");
        }

        public ExtensionLoader()
        {
            if (_loaded) return;
            if (Champions.Any(c => c.Name == ObjectManager.Player.ChampionName)) WelcomeChat();
            switch (ObjectManager.Player.ChampionName.ToLower())
            {
                case "alistar":
                    new Alistar();
                    _loaded = true;
                    break;
                case "annie":
                    new Annie();
                    _loaded = true;
                    break;
                case "blitzcrank":
                    new Blitzcrank();
                    _loaded = true;
                    break;
                case "brand":
                    new Brand();
                    _loaded = true;
                    break;
                case "cassiopeia":
                    new Cassiopeia();
                    _loaded = true;
                    break;
                case "evelynn":
                    new Evelynn();
                    _loaded = true;
                    break;
                case "ezreal":
                    new Ezreal();
                    _loaded = true;
                    break;
                //case "leesin":
                    //new LeeSin();
                    //_loaded = true;
                    //break;
                case "morgana":
                    new Morgana();
                    _loaded = true;
                    break;
                case "tristana":
                    new Tristana();
                    _loaded = true;
                    break;
                case "vladimir":
                    new Vladimir();
                    _loaded = true;
                    break;
                case "ziggs":
                    new Ziggs();
                    _loaded = true;
                    break;
                default:
                        Chat.Print("<font color = \"#740000\">Volatile AIO</font> doesn't support " +
                                   ObjectManager.Player.ChampionName + " yet.");
                    break;
            }
        }

    }
}