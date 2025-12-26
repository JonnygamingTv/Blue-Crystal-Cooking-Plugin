using JetBrains.Annotations;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Enumerations;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Ocelot.BlueCrystalCooking.functions
{
    public static class MethBagFunctions
    {
        public static void OnGestureChanged(UnturnedPlayer player, EPlayerGesture gesture)
        {
            if (Physics.Raycast(player.Player.look.aim.position, player.Player.look.aim.forward, out RaycastHit raycastHit, 2, RayMasks.BARRICADE))
            {
                BarricadeDrop drop = BarricadeManager.FindBarricadeByRootTransform(raycastHit.transform);
                if (drop != null && drop.asset.id == BlueCrystalCookingPlugin.Instance.Configuration.Instance.FrozenTrayId && BarricadeManager.tryGetRegion(raycastHit.transform, out byte x, out byte y, out ushort plant, out BarricadeRegion barricadeRegion))
                {
                    int random = UnityEngine.Random.Range(BlueCrystalCookingPlugin.Instance.Configuration.Instance.BlueCrystalBagsAmountMin, BlueCrystalCookingPlugin.Instance.Configuration.Instance.BlueCrystalBagsAmountMax);
                    for (int i = 0; i < random; i++)
                    {
                        ItemManager.dropItem(new Item(BlueCrystalCookingPlugin.Instance.Configuration.Instance.BlueCrystalBagId, true), new Vector3(raycastHit.transform.position.x, raycastHit.transform.position.y + 2, raycastHit.transform.position.z), false, true, false);
                        if (BlueCrystalCookingPlugin.Instance.Configuration.Instance.EnableBlueCrystalFreezeEffect)
                        {
                            EffectManager.sendEffect(BlueCrystalCookingPlugin.Instance.Configuration.Instance.BlueCrystalFreezeEffectId, 10, raycastHit.transform.position);
                        }
                    }
                    ChatManager.serverSendMessage(BlueCrystalCookingPlugin.Instance.Translate("bluecrystalbags_obtained", random), Color.white, null, player.SteamPlayer(), EChatMode.SAY, BlueCrystalCookingPlugin.Instance.Configuration.Instance.IconImageUrl, true);
                    BarricadeManager.destroyBarricade(drop, x, y, plant);
                }
            }
        }
        //public static void OnPlayerUpdateGesture(UnturnedPlayer player, UnturnedPlayerEvents.PlayerGesture gesture)
        //{
        //    if (gesture == UnturnedPlayerEvents.PlayerGesture.PunchLeft || gesture == UnturnedPlayerEvents.PlayerGesture.PunchRight)
        //    {
        //        if (Physics.Raycast(player.Player.look.aim.position, player.Player.look.aim.forward, out RaycastHit raycastHit, 2, RayMasks.BARRICADE))
        //        {
        //            if (BarricadeManager.tryGetInfo(raycastHit.transform, out byte x, out byte y, out ushort plant, out ushort index, out BarricadeRegion region, out BarricadeDrop drop))
        //            {
        //                if (drop.asset.id == BlueCrystalCookingPlugin.Instance.Configuration.Instance.FrozenTrayId)
        //                {
        //                    int random = UnityEngine.Random.Range(BlueCrystalCookingPlugin.Instance.Configuration.Instance.BlueCrystalBagsAmountMin, BlueCrystalCookingPlugin.Instance.Configuration.Instance.BlueCrystalBagsAmountMax);
        //                    for (int i = 0; i < random; i++)
        //                    {
        //                        ItemManager.dropItem(new Item(BlueCrystalCookingPlugin.Instance.Configuration.Instance.BlueCrystalBagId, true), new Vector3(raycastHit.transform.position.x, raycastHit.transform.position.y + 2, raycastHit.transform.position.z), false, true, false);
        //                        if (BlueCrystalCookingPlugin.Instance.Configuration.Instance.EnableBlueCrystalFreezeEffect)
        //                        {
        //                            EffectManager.sendEffect(BlueCrystalCookingPlugin.Instance.Configuration.Instance.BlueCrystalFreezeEffectId, 10, raycastHit.transform.position);
        //                        }
        //                    }
        //                    ChatManager.serverSendMessage(BlueCrystalCookingPlugin.Instance.Translate("bluecrystalbags_obtained", random), Color.white, null, player.SteamPlayer(), EChatMode.SAY, BlueCrystalCookingPlugin.Instance.Configuration.Instance.IconImageUrl, true);
        //                    BarricadeManager.destroyBarricade(region, x, y, plant, index);
        //                }
        //            }
        //        }
        //    }
        //}

        public static void ConsumeAction(Player instigatingPlayer, ItemConsumeableAsset consumeableAsset)
        {
            UnturnedPlayer player = UnturnedPlayer.FromPlayer(instigatingPlayer);
            if (consumeableAsset.id == BlueCrystalCookingPlugin.Instance.Configuration.Instance.BlueCrystalBagId)
            {
                BlueCrystalCookingPlugin.Instance.drugeffectPlayersList.Add(new DrugeffectTimeObject(player.Id));
                if (BlueCrystalCookingPlugin.Instance.Configuration.Instance.UseDrugEffectSpeed)
                {
                    player.Player.movement.sendPluginSpeedMultiplier(BlueCrystalCookingPlugin.Instance.Configuration.Instance.DrugEffectSpeedMultiplier);
                }
                if (BlueCrystalCookingPlugin.Instance.Configuration.Instance.UseDrugEffectJump)
                {
                    player.Player.movement.sendPluginJumpMultiplier(BlueCrystalCookingPlugin.Instance.Configuration.Instance.DrugEffectJumpMultiplier);
                }
            }
        }

        public static void Update()
        {
            foreach (var drugeffect in BlueCrystalCookingPlugin.Instance.drugeffectPlayersList)
            {
                if (BlueCrystalCookingPlugin.getCurrentTime() - drugeffect.time >= BlueCrystalCookingPlugin.Instance.Configuration.Instance.DrugEffectDurationSecs)
                {
                    BlueCrystalCookingPlugin.Instance.drugeffectPlayersList.Remove(drugeffect);
                    UnturnedPlayer player = UnturnedPlayer.FromCSteamID(new CSteamID(ulong.Parse(drugeffect.playerId)));
                    if (BlueCrystalCookingPlugin.Instance.Configuration.Instance.UseDrugEffectSpeed)
                    {
                        player.Player.movement.sendPluginSpeedMultiplier(1);
                    }
                    if (BlueCrystalCookingPlugin.Instance.Configuration.Instance.UseDrugEffectJump)
                    {
                        player.Player.movement.sendPluginJumpMultiplier(1);
                    }
                }
            }
        }
    }
}
