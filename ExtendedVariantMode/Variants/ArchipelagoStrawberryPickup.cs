using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Celeste;
using Celeste.Mod;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;
using Monocle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net.Packets;

namespace ExtendedVariants.Variants
{
    class ArchipelagoStrawberryPickup : AbstractExtendedVariant
    {
        private ILHook collectRoutineHook;
        private ArchipelagoSession currentAPSession;
        private Dictionary<long, string> itemLookupCache = new Dictionary<long, string>();

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
            On.Celeste.Strawberry.CollectRoutine += Strawberry_CollectRoutine;
            On.Celeste.Session.ctor += onSessionStart;

            //collectRoutineHook = new ILHook(typeof(Strawberry).GetMethod("CollectRoutine", BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), patchAllGoldenFlags);
        }

        private System.Collections.IEnumerator Strawberry_CollectRoutine(On.Celeste.Strawberry.orig_CollectRoutine orig, Strawberry self, int collectIndex)
        {
            Logger.Log("strawberry", self.ID.ToString());
            Logger.Log("collectIndex", collectIndex.ToString());

            Session session = self.SceneAs<Level>().Session;
            Logger.Log("Level", session.Area.GetSID());
            Logger.Log("Chapter", session.Area.ChapterIndex.ToString());
            Logger.Log("Count Safe", SaveData.Instance.TotalStrawberries_Safe.ToString());
            Logger.Log("Count", SaveData.Instance.TotalStrawberries.ToString());

            // Celeste/1-ForsakenCity
            string locationName = session.Area.GetSID().Split('-')[1] + ' ' + self.ID.Key.Split(':')[0];
            Logger.Log("Location Complete", locationName);

            long apID = this.currentAPSession.Locations.GetLocationIdFromName("Celeste", locationName);
            //long apID = locationIds[self.ID.Key];

            Logger.Log("AP ID", apID.ToString());
            if (apID != -1)
            {
                this.currentAPSession.Locations.CompleteLocationChecks(apID);
            }

            return orig(self, collectIndex);

        }

        public override void SetLegacyVariantValue(int value)
        {
            Settings.Archipelago = value != 0;
        }

        public override void Unload()
        {
            On.Celeste.Strawberry.CollectRoutine -= Strawberry_CollectRoutine;
            On.Celeste.Session.ctor -= onSessionStart;
        }

        protected override void DoSetVariantValue(object value)
        {
            Settings.Archipelago = (bool) value;
        }

        public void onSessionStart(On.Celeste.Session.orig_ctor orig, Session self)
        {
            orig(self);

            if (currentAPSession == null)
            {
                currentAPSession = ArchipelagoSessionFactory.CreateSession("localhost", 38281);

                currentAPSession.TryConnectAndLogin("Celeste", "Chadbert", new Version(0, 3, 2), ItemsHandlingFlags.AllItems);
                //result.Successful

                currentAPSession.Items.ItemReceived += onReceiveItem;
                //currentAPSession.Socket.PacketReceived += Socket_PacketReceived;

                // Set default storage values
                currentAPSession.DataStorage["StrawberryCount"].Initialize(0);
            }
        }

        /*private void Socket_PacketReceived(ArchipelagoPacketBase packet)
        {
            if (packet.PacketType == ArchipelagoPacketType.ReceivedItems)
            {
                var receivedItemsPacket = (ReceivedItemsPacket)packet;

                foreach(var item in receivedItemsPacket.Items)
                {
                    if (ItemMap.ContainsKey(item.Item))
                    {
                        string itemName = ItemMap[item.Item];
                        Logger.Log("AP Item Received", itemName);

                        switch (itemName)
                        {
                            case "Strawberry":
                                currentAPSession.DataStorage["StrawberryCount"]++;

                                Logger.Log("Strawberry Count", currentAPSession.DataStorage["StrawberryCount"]);
                                break;
                        }
                    }
                }
            }
            packet.ToString();
        }*/

        private void onReceiveItem(ReceivedItemsHelper receivedItemsHelper)
        {
            var itemReceivedName = receivedItemsHelper.PeekItemName();
            Logger.Log("AP Item recieved", itemReceivedName);

            if (itemReceivedName == "Strawberry")
            {
                currentAPSession.DataStorage["StrawberryCount"]++;

                Logger.Log("Strawberry Count", currentAPSession.DataStorage["StrawberryCount"]);
            }

            receivedItemsHelper.DequeueItem();
        }

        private static Dictionary<long, string> ItemMap = new Dictionary<long, string>
        {
            { 77000, "Strawberry" }
        };
    }
}
