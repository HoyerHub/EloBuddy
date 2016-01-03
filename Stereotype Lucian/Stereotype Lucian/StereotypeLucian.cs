using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Constants;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace StereotypeLucian
{
    public class StereotypeLucian
    {
        public static AIHeroClient BikeThief = ObjectManager.Player;
        public static Spell.Targeted Q;
        public static Spell.Skillshot Q1;
        public static Spell.Skillshot W;
        public static Spell.Skillshot E;
        public static Spell.Skillshot R;
        public static Menu CheapAssMenu;
        public static bool Passive;
        public static int RandomizerOne, RandomizerTwo;

        public StereotypeLucian()
        {
            Hacks.RenderWatermark = false;
            LetsLoadSomeSpells();
            AndThenLetsDoTheMenu();
            UpdateSliderOne();
            UpdateSliderTwo();
            Draw.Spells.Add(Q);
            Draw.Spells.Add(Q1);
            Draw.Spells.Add(W);
            Draw.Spells.Add(R);

            Game.OnUpdate += NowLetsDoThis50TimesPerSecond;
            Obj_AI_Base.OnProcessSpellCast += DoThisOnSpellCastsOnly;
            Orbwalker.OnPostAttack += PrepareYourAnus;
            Drawing.OnDraw += Draw.OnDraw;
        }

        private void PrepareYourAnus(AttackableUnit target, EventArgs args)
        {
            if (target.IsValidTarget())
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ||
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) ||
                    Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                {
                    Core.DelayAction(() => HereComesTheBrokenStuff(target, args),
                        Game.Ping * (new Random().Next(RandomizerOne, RandomizerTwo) / 10));
                }
            }

        }

        private static void DoThisOnSpellCastsOnly(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.NetworkId != BikeThief.NetworkId) return;
            if (args.Slot == SpellSlot.E)
            {
                Orbwalker.ResetAutoAttack();
            }
            if (args.Slot == SpellSlot.Q || args.Slot == SpellSlot.W || args.Slot == SpellSlot.E)
            {
                Passive = true;
            }
        }

        private static void HereComesTheBrokenStuff(AttackableUnit braveNigga, EventArgs args)
        {
            Passive = false;
            if (braveNigga is AIHeroClient)
            {
                if (E.IsReady() &&
                    ((Use("etc") && Mode(Orbwalker.ActiveModes.Combo))) ||
                    (Use("eth") && Mode(Orbwalker.ActiveModes.Harass)))
                {
                    switch (Get("emode"))
                    {
                        case 0:
                            if (Game.CursorPos.Distance(BikeThief.Position) >
                                BikeThief.AttackRange + BikeThief.BoundingRadius * 2 &&
                                !BikeThief.Position.Extend(Game.CursorPos, E.Range).IsUnderTurret())
                            {
                                E.Cast(BikeThief.Position.Extend(Game.CursorPos, E.Range).To3D());
                            }
                            else
                            {
                                E.Cast(RealBlackMagic(BikeThief.Position.To2D(), braveNigga.Position.To2D(), 65).To3D());
                            }
                            break;
                        case 1:
                            E.Cast(RealBlackMagic(BikeThief.Position.To2D(), braveNigga.Position.To2D(), 65).To3D());
                            break;
                        case 2:
                            E.Cast(Game.CursorPos);
                            break;
                    }
                }
                else if (Q.IsReady() && Passive == false &&
                         ((Use("qtc") && Mode(Orbwalker.ActiveModes.Combo)) ||
                          (Use("qth") && Mode(Orbwalker.ActiveModes.Harass))))
                {
                    Q.Cast(braveNigga as AIHeroClient);
                }
                else if (W.IsReady() && Passive == false &&
                         ((Use("wtc") && Mode(Orbwalker.ActiveModes.Combo)) ||
                          (Use("wth") && Mode(Orbwalker.ActiveModes.Harass))))
                {
                    if (Use("wcol"))
                    {
                        W.Cast(braveNigga as Obj_AI_Base);
                    }
                    else
                    {
                        W.Cast(braveNigga.Position);
                    }
                }
            }
            else if (braveNigga is Obj_AI_Minion && Mode(Orbwalker.ActiveModes.LaneClear) && BikeThief.ManaPercent > Get("mtf"))
            {
                var minions =
                    EntityManager.MinionsAndMonsters.EnemyMinions.Where(
                        m => m.Distance(BikeThief) < BikeThief.AttackRange).ToList();
                minions.AddRange(
                    EntityManager.MinionsAndMonsters.Monsters.Where(m => m.Distance(BikeThief) < BikeThief.AttackRange)
                        .ToList());
                if (!minions.Any()) return;
                if (E.IsReady() && Use("etl"))
                    E.Cast(RealBlackMagic(BikeThief.Position.To2D(), braveNigga.Position.To2D(), 65).To3D());
                else if (W.IsReady() && Passive == false && Use("wtl"))
                {
                    W.Cast(minions[0].ServerPosition);
                }
                else if (Q.IsReady() && Passive == false && Use("qtl"))
                {
                    var targetMinions =
                        EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.Distance(BikeThief) < Q.Range)
                            .ToList();
                    var hitMinions =
                        EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.Distance(BikeThief) < Q1.Range)
                            .ToList();
                    foreach (var minion in from minion in targetMinions
                        let qHit =
                            new Geometry.Polygon.Rectangle(BikeThief.Position,
                                BikeThief.Position.Extend(minion.Position, Q1.Range).To3D(), Q1.Width)
                        where hitMinions.Count(x => !qHit.IsOutside(x.Position.To2D())) >= 3
                        select minion)
                    {
                        Q.Cast(minion);
                        break;
                    }
                    if (EntityManager.MinionsAndMonsters.Monsters.Any(m => m.Distance(BikeThief) < Q.Range))
                    {
                        var targetMonsters =
                            EntityManager.MinionsAndMonsters.Monsters.Where(m => m.Distance(BikeThief) < Q.Range)
                                .OrderByDescending(m => m.MinionLevel)
                                .ToList();
                        Q.Cast(targetMonsters[0]);
                    }
                }
            }
        }

        private void NowLetsDoThis50TimesPerSecond(EventArgs args)
        {
            if (Mode(Orbwalker.ActiveModes.Harass)) DoSomeFunkyMoves();
            if (CheapAssMenu["user"].Cast<KeyBind>().CurrentValue && R.IsReady())
            {
                if (TargetSelector.GetTarget(R.Range, DamageType.Physical) != null)
                R.Cast(TargetSelector.GetTarget(R.Range, DamageType.Physical));
            }
        }

        
        private static void DoSomeFunkyMoves()
        {
            if (!Q.IsReady() || !Use("qth")) return;
            var braveNigga = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var pussyNigga = TargetSelector.GetTarget(Q1.Range, DamageType.Physical);

            if (braveNigga.IsValidTarget(Q.Range))
            {
                Q.Cast(braveNigga);
            }
            else if (pussyNigga.IsValidTarget(Q1.Range) && Use("eqth"))
            {
                var extendTargets =
                    EntityManager.MinionsAndMonsters.EnemyMinions.Where(m => m.Distance(BikeThief) < Q.Range)
                        .Cast<Obj_AI_Base>()
                        .ToList();
                extendTargets.AddRange(
                    EntityManager.MinionsAndMonsters.Monsters.Where(m => m.Distance(BikeThief) < Q.Range));
                extendTargets.AddRange(EntityManager.Heroes.Enemies.Where(m => m.Distance(BikeThief) < Q.Range));
                var qPred = Q1.GetPrediction(pussyNigga);
                foreach (var minion in extendTargets.Select(minion => new
                {
                    minion,
                    polygon = new Geometry.Polygon.Rectangle(
                        BikeThief.ServerPosition.To2D(),
                        BikeThief.ServerPosition.Extend(minion.ServerPosition, Q1.Range), 65f)
                }).Where(@t => @t.polygon.IsInside(qPred.CastPosition)).Select(@t => @t.minion))
                {
                    Q.Cast(minion);
                }
            }
        }

        private static void AndThenLetsDoTheMenu()
        {
            CheapAssMenu = MainMenu.AddMenu("Lucian", "luc", "StereoType Lucian");

            CheapAssMenu.Add("speed", new Slider("Combo Speed", 0, 0, 2)).OnValueChange += Speed;
            CheapAssMenu.AddSeparator();
            CheapAssMenu.AddGroupLabel("Q Settings");
            CheapAssMenu.Add("qtc", new CheckBox("Use Q in Combo"));
            CheapAssMenu.Add("qth", new CheckBox("Use Q in Harass"));
            CheapAssMenu.Add("eqth", new CheckBox("Use Extended Q in Harass"));
            CheapAssMenu.Add("qtl", new CheckBox("Use Q in Laneclear"));
            CheapAssMenu.AddGroupLabel("W Settings");
            CheapAssMenu.Add("wcol", new CheckBox("Check for W collision", false));
            CheapAssMenu.Add("wtc", new CheckBox("Use W in Combo"));
            CheapAssMenu.Add("wth", new CheckBox("Use W in Harass"));
            CheapAssMenu.Add("wtl", new CheckBox("Use W in Laneclear"));
            CheapAssMenu.AddGroupLabel("E Settings");
            CheapAssMenu.Add("emode", new Slider("Use E in Combo", 0, 0, 2)).OnValueChange += EMode;
            CheapAssMenu.Add("etc", new CheckBox("Use E in Combo"));
            CheapAssMenu.Add("eth", new CheckBox("Use E in Harass"));
            CheapAssMenu.Add("etl", new CheckBox("Use E in Laneclear"));
            CheapAssMenu.AddGroupLabel("R Settings");
            CheapAssMenu.Add("user", new KeyBind("Semi-Auto R (No Lock)", false, KeyBind.BindTypes.HoldActive, 'T'));
            CheapAssMenu.AddGroupLabel("General Settings");
            CheapAssMenu.Add("mtf", new Slider("ManaSlider for Farm", 75));

            var DrawMenu = CheapAssMenu.AddSubMenu("Drawings", "draw");
            CheapAssMenu.Add("dmode", new Slider("Draw Mode", 0, 0, 2)).OnValueChange += DrawMode;
            CheapAssMenu.AddSeparator();
            CheapAssMenu.Add("dq", new CheckBox("Draw Q Range"));
            CheapAssMenu.Add("dq1", new CheckBox("Draw Q Extended Range"));
            CheapAssMenu.Add("dw", new CheckBox("Draw W Range"));
            CheapAssMenu.Add("de", new CheckBox("Draw E Range"));
            CheapAssMenu.Add("dr", new CheckBox("Draw R Range"));
        }

        private static void DrawMode(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
        {
            UpdateSliderThree();
        }

        private static void UpdateSliderThree()
        {
            var sstring = "Draw Mode: ";

            switch (Get("dmode"))
            {
                case 0:
                    sstring = sstring + "Volatile© RangeLine™";
                    break;
                case 1:
                    sstring = sstring + "Range Circles";
                    break;
                case 2:
                    sstring = sstring + "None";
                    break;
            }
            CheapAssMenu["dmode"].Cast<Slider>().DisplayName = sstring;
        }

        private static void Speed(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
        {
            UpdateSliderTwo();
        }

        private static void UpdateSliderTwo()
        {
            var sstring = "Speed: ";

            switch (Get("speed"))
            {
                case 0:
                    sstring = sstring + "Lightning (still humanized)";
                    RandomizerOne = 14;
                    RandomizerTwo = 17;
                    break;
                case 1:
                    sstring = sstring + "Moderate";
                    RandomizerOne = 22;
                    RandomizerTwo = 26;
                    break;
                case 2:
                    sstring = sstring + "Whyyyy";
                    RandomizerOne = 26;
                    RandomizerTwo = 34;
                    break;
            }
            CheapAssMenu["speed"].Cast<Slider>().DisplayName = sstring;
        }

        private static void EMode(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
        {
            UpdateSliderOne();
        }

        private static void UpdateSliderOne()
        {
            var estring = "E Mode: ";

            switch (Get("emode"))
            {
                case 0:
                    estring = estring + "Auto";
                    break;
                case 1:
                    estring = estring + "Side";
                    break;
                case 2:
                    estring = estring + "To Mouse";
                    break;
            }
            CheapAssMenu["emode"].Cast<Slider>().DisplayName = estring;
        }

        internal static bool Use(string id)
        {
            return CheapAssMenu[id].Cast<CheckBox>().CurrentValue;
        }

        internal static bool Mode(Orbwalker.ActiveModes id)
        {
            return Orbwalker.ActiveModesFlags.HasFlag(id);
        }

        internal static int Get(string id)
        {
            return CheapAssMenu[id].Cast<Slider>().CurrentValue;
        }

        private static void LetsLoadSomeSpells()
        {
            Q = new Spell.Targeted(SpellSlot.Q, 675);
            Q1 = new Spell.Skillshot(SpellSlot.Q, 1150, SkillShotType.Linear, 250,
                int.MaxValue, 65) {AllowedCollisionCount = int.MaxValue};
            W = new Spell.Skillshot(SpellSlot.W, 1150, SkillShotType.Linear, 250, 1600, 80);
            E = new Spell.Skillshot(SpellSlot.E, 475, SkillShotType.Linear);
            R = new Spell.Skillshot(SpellSlot.R, 1400, SkillShotType.Linear, 500, 2800, 110);
        }

        public static Vector2 RealBlackMagic(Vector2 point1, Vector2 point2, double angle) //credits Hoola - This is used for the "side"-dash
        {
            angle *= Math.PI / 180.0;
            var temp = Vector2.Subtract(point2, point1);
            var result = new Vector2(0)
            {
                X = (float)(temp.X * Math.Cos(angle) - temp.Y * Math.Sin(angle)) / 4,
                Y = (float)(temp.X * Math.Sin(angle) + temp.Y * Math.Cos(angle)) / 4
            };
            result = Vector2.Add(result, point1);
            return result;
        }

    }
}