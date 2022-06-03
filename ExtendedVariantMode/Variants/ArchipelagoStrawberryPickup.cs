using Celeste;
using Celeste.Mod;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExtendedVariants.Variants
{
    class ArchipelagoStrawberryPickup : AbstractExtendedVariant
    {
        private ILHook collectRoutineHook;

        public override object GetDefaultVariantValue()
        {
            return true;
        }

        public override Type GetVariantType()
        {
            return typeof(bool);
        }

        public override object GetVariantValue()
        {
            return Settings.Archipelago;
        }

        public override void Load()
        {
            On.Celeste.Strawberry.OnPlayer += onStrawberryCollected;

            //collectRoutineHook = new ILHook(typeof(Strawberry).GetMethod("CollectRoutine", BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), patchAllGoldenFlags);
        }

        private void onStrawberryCollected(On.Celeste.Strawberry.orig_OnPlayer orig, Strawberry self, Player player)
        {
            // if the strawberry is actually collected by the player, vanilla code switches ReturnHomeWhenLost to true.
            // so, we'll use that to spare us some reflection or IL modding.
            bool origReturnHome = self.ReturnHomeWhenLost;
            self.ReturnHomeWhenLost = false;

            orig(self, player);

            bool wasCollected = self.ReturnHomeWhenLost;
            self.ReturnHomeWhenLost = origReturnHome;

            if (wasCollected)
            {
                // store the inventory to restore it if needed.
                new DynData<Strawberry>(self)["playerInventoryOnCollection"] = player.Inventory;

                // Add AP logic here
                Logger.Log("strawberry", self.ID.ToString());
            }
        }

        public override void SetLegacyVariantValue(int value)
        {
            Settings.Archipelago = value != 0;
        }

        public override void Unload()
        {
            On.Celeste.Strawberry.OnPlayer -= onStrawberryCollected;
        }

        protected override void DoSetVariantValue(object value)
        {
            Settings.Archipelago = (bool) value;
        }
    }
}
