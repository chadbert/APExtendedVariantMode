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
    class Archipelago : AbstractExtendedVariant
    {
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
            On.Celeste.Cassette.CollectRoutine += Cassette_CollectRoutine;
            On.Celeste.Session.ctor += onSessionStart;
            On.Celeste.Level.RegisterAreaComplete += Level_RegisterAreaComplete;

            //collectRoutineHook = new ILHook(typeof(Strawberry).GetMethod("CollectRoutine", BindingFlags.NonPublic | BindingFlags.Instance).GetStateMachineTarget(), patchAllGoldenFlags);
        }

        private System.Collections.IEnumerator Cassette_CollectRoutine(On.Celeste.Cassette.orig_CollectRoutine orig, Cassette self, Player player)
        {
            Session session = self.SceneAs<Level>().Session;
            Logger.Log("Cassette collected", self.Tag.ToString());
            if (!CassetteMap.ContainsKey(session.Area.ChapterIndex))
            {
                Logger.Log("Cassette Location Not Mapped", session.Area.ChapterIndex.ToString());
            }
            else
            {
                long location = CassetteMap[session.Area.ChapterIndex];
                Logger.Log("Location Complete", location.ToString());
            }

            return orig(self, player);
        }

        private void Level_RegisterAreaComplete(On.Celeste.Level.orig_RegisterAreaComplete orig, Level self)
        {
            // Currently complete upon ForsakenCity completion
            // TODO: Only send completion once
            if (self.Session.Area.ChapterIndex == 1 && self.Session.Area.Mode == AreaMode.Normal)
            {
                Logger.Log("Finished level", self.Session.Area.GetSID());

                this.currentAPSession.Socket.SendPacket(new StatusUpdatePacket { Status = ArchipelagoClientState.ClientGoal });
            }

            // TODO: announce level completion? Store completed levels.

            orig(self);
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

            if (!LocationMap.ContainsKey(self.ID.Key))
            {
                Logger.Log("AP Location Not Mapped", self.ID.ToString());
            }
            else 
            {
                long location = LocationMap[self.ID.Key];
                Logger.Log("Location Complete", location.ToString());

                this.currentAPSession.Locations.CompleteLocationChecks(location);
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
            On.Celeste.Level.RegisterAreaComplete -= Level_RegisterAreaComplete;

            if (currentAPSession != null && currentAPSession.Socket.Connected)
            {
                currentAPSession.Socket.Disconnect();
            }
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

        private static Dictionary<string, long> LocationMap = new Dictionary<string, long>
        {
            { "2:11", 77200 }, // "Forsaken City 2 - First Strawberry" },
            { "3:9", 77201 }, // "Forsaken City 3" },
            { "3b:2", 77202 }, // "Forsaken City 3b - Winged Strawberry" },
            { "5z:10", 77203 }, // "Forsaken City 5z" },
            { "5:21", 77204 }, // "Forsaken City 5" },
            { "7zb:2", 77205 }, // "Forsaken City 7zb - Above Theo" },
            { "6:12", 77206 }, // "Forsaken City 6 - Behind Dirt" },
            { "s1:9", 77207 }, // "Forsaken City s1 - Computer Terminal" },
            { "7z:3", 77208 }, // "Forsaken City 7z - Hidden Path" },
            { "8zb:1", 77209 }, // "Forsaken City 8zb - Double Hidden Path" },
            { "9z:3", 77210 }, // "Forsaken City 9z" },
            { "8b:1", 77211 }, // "Forsaken City 8b" },
            { "9:14", 77212 }, // "Forsaken City 9" },
            { "9b:9", 77213 }, // "Forsaken City 9b" },
            { "9c:2", 77214 }, // "Forsaken City 9c - Winged Strawberry" },
            { "10zb:1", 77215 }, // "Forsaken City 10zb - Off the Cliff" },
            { "11:9", 77216 }, // "Forsaken City 11 - Post B-Side cassette" },
            { "12z:8", 77217 }, // "Forsaken City 12z - Diamond Room" }
            { "7a:12", 77218 } // Forsaken City 7a - Triple Platform Room
        };

        private static Dictionary<int, long> CassetteMap = new Dictionary<int, long>
        {
            { 1, 77400 } // Forsaken City
        };
    }
}
