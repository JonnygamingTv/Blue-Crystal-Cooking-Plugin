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
    public static class BarrelFunctions
    {
        public static void OnGestureChanged(UnturnedPlayer player, EPlayerGesture gesture)
        {
            if (player == null)
                return;
            if (Physics.Raycast(player.Player.look.aim.position, player.Player.look.aim.forward, out RaycastHit raycastHit, 2, RayMasks.BARRICADE) && raycastHit.transform != null)
            {
                foreach (var barrel in BlueCrystalCookingPlugin.Instance.placedBarrelsTransformsIngredients.ToList())
                { // can we change this to placedBarrelsTransformsIngredients.tryGetValue()?
                    if (barrel.Key == null)
                        break;
                    if (barrel.Key.position == raycastHit.transform.position)
                    {
                        uint ingredientCount = 0;
                        for (int i = 0; i < BlueCrystalCookingPlugin.Instance.Configuration.Instance.drugIngredientIds.Count; i++)
                        {
                            if (barrel.Value.ingredients.Contains(BlueCrystalCookingPlugin.Instance.Configuration.Instance.drugIngredientIds[i]))
                            {
                                ingredientCount += 1;
                            }
                        }
                        if (ingredientCount == BlueCrystalCookingPlugin.Instance.Configuration.Instance.drugIngredientIds.Count)
                        {
                            if (BlueCrystalCookingPlugin.Instance.Configuration.Instance.EnableBarrelStirEffect)
                            {
                                ingredientCount = 0;
                                if (raycastHit.transform != null)
                                {
                                    EffectManager.sendEffect(BlueCrystalCookingPlugin.Instance.Configuration.Instance.BarrelStirEffectId, 4, raycastHit.transform.position);
                                }
                            }
                            barrel.Value.progress += BlueCrystalCookingPlugin.Instance.Configuration.Instance.StirProgressAddPercentage;
                        }
                        else
                        {
                            ingredientCount = 0;
                            ChatManager.serverSendMessage(BlueCrystalCookingPlugin.Instance.Translate("not_enough_ingredients"), Color.white, null, player.SteamPlayer(), EChatMode.SAY, BlueCrystalCookingPlugin.Instance.Configuration.Instance.IconImageUrl, true);
                        }
                    }
                    if (barrel.Value.progress >= 100)
                    {
                        barrel.Value.progress = 0;
                        for (int i = 0; i < BlueCrystalCookingPlugin.Instance.Configuration.Instance.drugIngredientIds.Count; i++)
                        {
                            barrel.Value.ingredients.Remove(BlueCrystalCookingPlugin.Instance.Configuration.Instance.drugIngredientIds[i]);
                        }
                        ChatManager.serverSendMessage(BlueCrystalCookingPlugin.Instance.Translate("stir_successful"), Color.white, null, player.SteamPlayer(), EChatMode.SAY, BlueCrystalCookingPlugin.Instance.Configuration.Instance.IconImageUrl, true);
                        ItemBarricadeAsset _asset = (ItemBarricadeAsset)Assets.find(EAssetType.RESOURCE, BlueCrystalCookingPlugin.Instance.Configuration.Instance.BlueCrystalTrayId);
                        if (_asset != null)
                        {
                            Barricade newBarr = new Barricade(_asset);
                            BarricadeManager.dropBarricade(newBarr, null, player.Position, 0, 0, 0, player.CSteamID.m_SteamID, player.Player.quests.groupID.m_SteamID);
                        }
                        else
                        {
                            BarricadeManager.dropBarricade(new Barricade(BlueCrystalCookingPlugin.Instance.Configuration.Instance.BlueCrystalTrayId), null, player.Position, 0, 0, 0, (ulong)player.CSteamID, (ulong)player.Player.quests.groupID);
                        }
                    }
                }
            }
        }
        //public static void OnPlayerUpdateGesture(UnturnedPlayer player, UnturnedPlayerEvents.PlayerGesture gesture)
        //{
            
        //    if (gesture == UnturnedPlayerEvents.PlayerGesture.PunchLeft || gesture == UnturnedPlayerEvents.PlayerGesture.PunchRight)
        //    {
        //        if (Physics.Raycast(player.Player.look.aim.position, player.Player.look.aim.forward, out RaycastHit raycastHit, 2, RayMasks.BARRICADE))
        //        {
        //            foreach (var barrel in BlueCrystalCookingPlugin.Instance.placedBarrelsTransformsIngredients.ToList())
        //            {
        //                if (barrel.Key.position == raycastHit.transform.position)
        //                {
        //                    uint ingredientCount = 0;
        //                    for (int i = 0; i < BlueCrystalCookingPlugin.Instance.Configuration.Instance.drugIngredientIds.Count; i++)
        //                    {
        //                        if (barrel.Value.ingredients.Contains(BlueCrystalCookingPlugin.Instance.Configuration.Instance.drugIngredientIds[i]))
        //                        {
        //                            ingredientCount += 1;
        //                        }
        //                        }
        //                    if (ingredientCount == BlueCrystalCookingPlugin.Instance.Configuration.Instance.drugIngredientIds.Count)
        //                    {
        //                        if (BlueCrystalCookingPlugin.Instance.Configuration.Instance.EnableBarrelStirEffect)
        //                        {
        //                            ingredientCount = 0;
        //                            EffectManager.sendEffect(BlueCrystalCookingPlugin.Instance.Configuration.Instance.BarrelStirEffectId, 4, raycastHit.transform.position);
        //                        }
        //                        barrel.Value.progress += BlueCrystalCookingPlugin.Instance.Configuration.Instance.StirProgressAddPercentage;
        //                    }
        //                    else
        //                    {
        //                        ingredientCount = 0;
        //                        ChatManager.serverSendMessage(BlueCrystalCookingPlugin.Instance.Translate("not_enough_ingredients"), Color.white, null, player.SteamPlayer(), EChatMode.SAY, BlueCrystalCookingPlugin.Instance.Configuration.Instance.IconImageUrl, true);
        //                    }
        //                }
        //                if (barrel.Value.progress >= 100)
        //                {
        //                    barrel.Value.progress = 0;
        //                    for (int i = 0; i < BlueCrystalCookingPlugin.Instance.Configuration.Instance.drugIngredientIds.Count; i++)
        //                    {
        //                        barrel.Value.ingredients.Remove(BlueCrystalCookingPlugin.Instance.Configuration.Instance.drugIngredientIds[i]);
        //                    }
        //                    ChatManager.serverSendMessage(BlueCrystalCookingPlugin.Instance.Translate("stir_successful"), Color.white, null, player.SteamPlayer(), EChatMode.SAY, BlueCrystalCookingPlugin.Instance.Configuration.Instance.IconImageUrl, true);
        //                    BarricadeManager.dropBarricade(new Barricade(BlueCrystalCookingPlugin.Instance.Configuration.Instance.BlueCrystalTrayId), null, player.Position, 0, 0, 0, (ulong)player.CSteamID, (ulong)player.Player.quests.groupID);
        //                }
        //            }
        //        }
        //    }
        //}
        public static void BarricadeDeployed(Barricade barricade, ItemBarricadeAsset asset, Transform hit, ref Vector3 point, ref float angle_x, ref float angle_y, ref float angle_z, ref ulong owner, ref ulong group, ref bool shouldAllow)
        {
            Vector3 pos = point;
            ulong ownerBarricade = owner;
            if (barricade.asset.id == BlueCrystalCookingPlugin.Instance.Configuration.Instance.BarrelObjectId)
            {
                BlueCrystalCookingPlugin.Instance.Wait(0.2f, () => {
                    List<ushort> ingredients = new List<ushort>();
                    BlueCrystalCookingPlugin.Instance.placedBarrelsTransformsIngredients.Add(BlueCrystalCookingPlugin.Instance.GetPlacedObjectTransform(pos), new BarrelObject(ingredients, 0));
                });
            }

            foreach (var drugObject in BlueCrystalCookingPlugin.Instance.Configuration.Instance.drugIngredientIds)
            {
                if (drugObject == barricade.asset.id)
                {
                    if (Physics.Raycast(pos, Vector3.down, out RaycastHit raycastHit, 10, RayMasks.BARRICADE))
                    {
                        BarricadeDrop drop = BarricadeManager.FindBarricadeByRootTransform(raycastHit.transform);
                        if (drop != null)
                        {
                            if (drop.asset.id == BlueCrystalCookingPlugin.Instance.Configuration.Instance.BarrelObjectId)
                            {
                                if(BlueCrystalCookingPlugin.Instance.placedBarrelsTransformsIngredients.TryGetValue(raycastHit.transform, out BarrelObject barrel))
                                {
                                    ChatManager.serverSendMessage(BlueCrystalCookingPlugin.Instance.Translate("ingredient_added", asset.itemName), Color.white, null, UnturnedPlayer.FromCSteamID(new CSteamID(ownerBarricade)).SteamPlayer(), EChatMode.SAY, BlueCrystalCookingPlugin.Instance.Configuration.Instance.IconImageUrl, true);
                                    barrel.ingredients.Add(barricade.asset.id);
                                    BlueCrystalCookingPlugin.Instance.Wait(0.2f, () => {
                                        BarricadeManager.destroyBarricade(drop, drop.asset.size_x, drop.asset.size_y, drop.asset.id);
                                        //BarricadeManager.tryGetInfo(BlueCrystalCookingPlugin.Instance.GetPlacedObjectTransform(pos), out byte xingredient, out byte yingredient, out ushort plantingredient, out ushort indexingredient, out BarricadeRegion regioningredient);
                                        //BarricadeManager.destroyBarricade(regioningredient, xingredient, yingredient, plantingredient, indexingredient);
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
