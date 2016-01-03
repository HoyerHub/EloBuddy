using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using VolatileAIO.Organs;
using VolatileAIO.Organs.Brain;
using VolatileAIO.Organs.Brain.Data;
// ReSharper disable PossibleInvalidOperationException

namespace VolatileAIO.Extensions.Jungle
{
    internal class LeeSin : Heart
    {
        #region Spell and Menu Declaration

        public static Spell.Skillshot Q;
        public static Spell.Targeted W;
        public static Spell.Active E;
        public static Spell.Targeted R;

        public static Menu SpellMenu;

        public static bool Jumptoward = false;
        public static Vector3 JumpWard;
        #endregion

        #region Spell and Menu Loading

        public LeeSin()
        {
            PlayerData.Spells = new Initialize().Spells(Initialize.Type.Skillshot, Initialize.Type.Targeted, Initialize.Type.Active, Initialize.Type.Targeted);
            Q = (Spell.Skillshot)PlayerData.Spells[0];
            W = (Spell.Targeted)PlayerData.Spells[1];
            E = (Spell.Active) PlayerData.Spells[2];
            R = (Spell.Targeted)PlayerData.Spells[3];
            InitializeMenu();
        }

        private static void InitializeMenu()
        {
            SpellMenu = VolatileMenu.AddSubMenu("Spell Menu", "spellmenu");
            
            SpellMenu.AddGroupLabel("Keys");
            SpellMenu.Add("wardjump", new KeyBind("Ward Jump", false, KeyBind.BindTypes.HoldActive, 'G'));

        }

        public static bool Q1()
        {
            return Player.Spellbook.GetSpell(SpellSlot.Q).Name == "BlindMonkQOne";
        }

        public static bool Q2()
        {
            return Player.Spellbook.GetSpell(SpellSlot.Q).Name == "blindmonkqtwo";
        }

        public static bool W1()
        {
            return Player.Spellbook.GetSpell(SpellSlot.W).Name == "BlindMonkWOne";
        }

        public static bool W2()
        {
            return Player.Spellbook.GetSpell(SpellSlot.W).Name == "blindmonkwtwo";
        }

        public static bool E1()
        {
            return Player.Spellbook.GetSpell(SpellSlot.E).Name == "BlindMonkEOne";
        }

        public static bool E2()
        {
            return Player.Spellbook.GetSpell(SpellSlot.E).Name == "blindmonketwo";
        }

        #endregion

        protected override void Volatile_OnHeartBeat(EventArgs args)
        {
            if (SpellMenu["wardjump"].Cast<KeyBind>().CurrentValue && TickManager.NoLag(2))
            {
                WardJump();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void WardJump()
        {
            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            if (W1() && W.IsReady())
            {
                if (Jumptoward)
                {
                    var findobject =
                    ObjectManager.Get<Obj_AI_Base>()
                        .FirstOrDefault(
                            o => !o.IsMe && o.IsAlly && o.IsTargetableToTeam && o.Distance(JumpWard) < 200);
                    if (findobject != null)
                    {
                        W.Cast(findobject);
                        Jumptoward = false;
                    }
                }
                else if (GetWardSlot() != null)
                {
                    Player.Spellbook.CastSpell((SpellSlot) GetWardSlot(), JumpPos());
                    Jumptoward = true;
                    JumpWard = JumpPos();
                }

                /*
                var findobject =
                    ObjectManager.Get<Obj_AI_Base>()
                        .FirstOrDefault(
                            o => !o.IsMe && o.IsAlly && o.IsTargetableToTeam && o.Distance(JumpPos()) < 200);
                if (findobject != null)
                {
                    W.Cast(findobject);
                }
                else
                {
                    if (GetWardSlot() != null)
                        Player.Spellbook.CastSpell((SpellSlot)GetWardSlot(), JumpPos());
                }*/
            }
        }

        protected override void Volatile_OnObjectCreate(GameObject sender, EventArgs args)
        {
            if (sender.IsMe && SpellMenu["wardjump"].Cast<KeyBind>().CurrentValue)
                Chat.Print(args);
        }

        private static Vector3 JumpPos()
        {
            if (Game.CursorPos.Distance(Player.Position) > W.Range)
            {
                return Player.Position.Extend(Game.CursorPos, W.Range).To3D();
            }
            return Game.CursorPos;
        }

        private static SpellSlot? GetWardSlot()
        {
            if (Player.InventoryItems.Any(i => i.IsWard))
            {
                if (Player.InventoryItems.Any(i => i.Id == ItemId.Warding_Totem_Trinket && Player.Spellbook.GetSpell(i.SpellSlot).IsReady))
                {
                    var firstOrDefault = Player.InventoryItems.FirstOrDefault(i => i.Id == ItemId.Warding_Totem_Trinket);
                    if (firstOrDefault != null)
                    return firstOrDefault.SpellSlot;
                }
                if (Player.InventoryItems.Any(i => i.Id == ItemId.Sightstone && Player.Spellbook.GetSpell(i.SpellSlot).IsReady) ||
                    Player.InventoryItems.Any(i => i.Id == ItemId.Ruby_Sightstone && Player.Spellbook.GetSpell(i.SpellSlot).IsReady) )
                {
                    var firstOrDefault = Player.InventoryItems.FirstOrDefault(i => i.Id == ItemId.Sightstone);
                    if (firstOrDefault != null && firstOrDefault.Charges > 0)
                        return firstOrDefault.SpellSlot;
                    firstOrDefault = Player.InventoryItems.FirstOrDefault(i => i.Id == ItemId.Ruby_Sightstone);
                    if (firstOrDefault != null && firstOrDefault.Charges > 0)
                        return firstOrDefault.SpellSlot;
                }
            }
            return null;
        }
    }
}