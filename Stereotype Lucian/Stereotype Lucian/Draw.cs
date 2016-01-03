using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Color = System.Drawing.Color;

namespace StereotypeLucian
{
    public class Draw
    {
        public static AIHeroClient Player = ObjectManager.Player;
        public static List<Spell.SpellBase> Spells = new List<Spell.SpellBase>();

        public Draw()
        {
            
        }

        public static void OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (StereotypeLucian.DrawMenu["dmode"].Cast<Slider>().CurrentValue == 0) DrawRangeLines();
            if (StereotypeLucian.DrawMenu["dmode"].Cast<Slider>().CurrentValue == 1) DrawRangeCircles();
        }

        private static void DrawRangeCircles()
        {
            if (StereotypeLucian.DrawMenu["dq"].Cast<CheckBox>().CurrentValue)
            {
                if (Spells[0].IsReady())
                    new Circle
                    {
                        Color = Color.Chartreuse,
                        Radius = Spells[0].Range,
                        BorderWidth = 1f
                    }.Draw(Player.Position);
                else
                    new Circle
                    {
                        Color = Color.Firebrick,
                        Radius = Spells[0].Range,
                        BorderWidth = 1f
                    }.Draw(Player.Position);
            }
            if (StereotypeLucian.DrawMenu["dw"].Cast<CheckBox>().CurrentValue)
            {
                if (Spells[1].IsReady())
                    new Circle
                    {
                        Color = Color.Chartreuse,
                        Radius = Spells[1].Range,
                        BorderWidth = 1f
                    }.Draw(Player.Position);
                else
                    new Circle
                    {
                        Color = Color.Firebrick,
                        Radius = Spells[1].Range,
                        BorderWidth = 1f
                    }.Draw(Player.Position);
            }
            if (StereotypeLucian.DrawMenu["de"].Cast<CheckBox>().CurrentValue)
            {
                if (Spells[2].IsReady())
                    new Circle
                    {
                        Color = Color.Chartreuse,
                        Radius = Spells[2].Range,
                        BorderWidth = 1f
                    }.Draw(Player.Position);
                else
                    new Circle
                    {
                        Color = Color.Firebrick,
                        Radius = Spells[2].Range,
                        BorderWidth = 1f
                    }.Draw(Player.Position);
            }
            if (StereotypeLucian.DrawMenu["dr"].Cast<CheckBox>().CurrentValue)
            {
                if (Spells[3].IsReady())
                    new Circle
                    {
                        Color = Color.Chartreuse,
                        Radius = Spells[3].Range,
                        BorderWidth = 1f
                    }.Draw(Player.Position);
                else
                    new Circle
                    {
                        Color = Color.Firebrick,
                        Radius = Spells[3].Range,
                        BorderWidth = 1f
                    }.Draw(Player.Position);
            }
        }

        private static void DrawRangeLines()
        {
            if (!Spells.Any(s => s.IsLearned)) return;
            uint[] maxrange = { Spells.Where(s => s.IsLearned).Max(s => s.Range) };
            if (maxrange[0] == 0)
                try
                {
                    foreach (var ss in Spells.Where(s => s.IsLearned).Cast<Spell.Skillshot>().Where(ss => ss.Width > maxrange[0]))
                    {
                        maxrange[0] = (uint)ss.Width;
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            foreach (
                var hero in
                    EntityManager.Heroes.Enemies.Where(
                        e => e.Distance(Player) < maxrange[0] && !e.IsDead && e.IsValid && e.IsVisible && e.IsValidTarget(maxrange[0])))
            {
                var spellranges = new Dictionary<uint, List<SpellSlot>>();
                var s = from spell in Spells where spell.IsLearned select spell.Range;
                var spells = s.ToList();
                spells.Sort();
                foreach (var spell in spells)
                {
                    var range = (uint)0;
                    if (spell > 0)
                        range = spell;
                    else
                    {
                        try
                        {
                            var ss = (Spell.Skillshot)Spells.Find(sp => sp.Range == spell);
                            range = (uint)ss.Width;
                        }
                        catch (Exception)
                        {
                            // ignored
                        }
                    }
                    if (range < 50) continue;
                    if (range > hero.Distance(Player))
                        if (spellranges.ContainsKey((uint)hero.Distance(Player)))
                            spellranges[(uint)hero.Distance(Player)].Add(
                                Spells.Find(sp => sp.Range == spell).Slot);
                        else
                            spellranges.Add((uint)hero.Distance(Player),
                                new List<SpellSlot> { Spells.Find(sp => sp.Range == spell).Slot });
                    else if (spellranges.ContainsKey(range))
                        spellranges[range].Add(Spells.Find(sp => sp.Range == spell).Slot);
                    else
                        spellranges.Add(range,
                            new List<SpellSlot> { Spells.Find(sp => sp.Range == spell).Slot });
                }
                var i = 0;
                var lastloc = new Vector3(0, 0, 0);
                foreach (var range in spellranges)
                {
                    var angle = Math.Atan2(hero.Position.Y - Player.Position.Y, hero.Position.X - Player.Position.X);
                    var sin = (Math.Sin(angle) * (range.Key - 20)) + Player.Position.Y;
                    var cosin = (Math.Cos(angle) * (range.Key - 20)) + Player.Position.X;
                    var location = new Vector3((float)cosin, (float)sin, Player.Position.Z);
                    if (i == 0)
                    {
                        Drawing.DrawLine(Player.Position.WorldToScreen(), location.WorldToScreen(), 2, Color.Red);
                        sin = (Math.Sin(angle) * (range.Key)) + Player.Position.Y;
                        cosin = (Math.Cos(angle) * (range.Key)) + Player.Position.X;
                        location = new Vector3((float)cosin, (float)sin, Player.Position.Z);
                        if (Spells.Find(sp => range.Value.Contains(sp.Slot)).IsReady())
                            new Circle
                            {
                                Color = Color.Chartreuse,
                                Radius = 20,
                                BorderWidth = 1f
                            }.Draw(location);
                        else
                            new Circle
                            {
                                Color = Color.Firebrick,
                                Radius = 20,
                                BorderWidth = 1f
                            }.Draw(location);
                        string text = "";
                        text = range.Value.Aggregate(text, (current, spell) => current + spell);
                        Drawing.DrawText(location.WorldToScreen().X, location.WorldToScreen().Y - (float)7.5,
                            Color.White,
                            text, 15);
                        i++;
                        sin = (Math.Sin(angle) * (range.Key + 20)) + Player.Position.Y;
                        cosin = (Math.Cos(angle) * (range.Key + 20)) + Player.Position.X;
                        lastloc = new Vector3((float)cosin, (float)sin, Player.Position.Z);
                    }
                    else
                    {
                        Drawing.DrawLine(lastloc.WorldToScreen(), location.WorldToScreen(), 2, Color.Red);
                        sin = (Math.Sin(angle) * (range.Key)) + Player.Position.Y;
                        cosin = (Math.Cos(angle) * (range.Key)) + Player.Position.X;
                        location = new Vector3((float)cosin, (float)sin, Player.Position.Z);
                        if (Spells.Find(sp => range.Value.Contains(sp.Slot)).IsReady())
                            new Circle()
                            {
                                Color = Color.Chartreuse,
                                Radius = 20,
                                BorderWidth = 2f
                            }.Draw(location);
                        else
                            new Circle()
                            {
                                Color = Color.Firebrick,
                                Radius = 20,
                                BorderWidth = 2f
                            }.Draw(location);
                        string text = "";
                        text = range.Value.Aggregate(text, (current, spell) => current + spell);
                        Drawing.DrawText(location.WorldToScreen().X, location.WorldToScreen().Y - (float)7.5,
                            Color.White,
                            text, 15);
                        i++;
                        sin = (Math.Sin(angle) * (range.Key + 20)) + Player.Position.Y;
                        cosin = (Math.Cos(angle) * (range.Key + 20)) + Player.Position.X;
                        lastloc = new Vector3((float)cosin, (float)sin, Player.Position.Z);
                    }
                }
            }
        }
    }
}