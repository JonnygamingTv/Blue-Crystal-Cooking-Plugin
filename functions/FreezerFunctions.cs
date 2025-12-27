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
    public static class FreezerFunctions
    {
        public static void BarricadeDeployed(Barricade barricade, ItemBarricadeAsset asset, Transform hit, ref Vector3 point, ref float angle_x, ref float angle_y, ref float angle_z, ref ulong owner, ref ulong group, ref bool shouldAllow)
        {
            Vector3 pos = point;
            if (barricade.asset.id == BlueCrystalCookingPlugin.Instance.Configuration.Instance.LiquidTrayId)
            {
                if (Physics.Raycast(pos, Vector3.down, out RaycastHit raycastHit, 10, RayMasks.BARRICADE))
                {
                    if (Physics.Raycast(pos, Vector3.up, out RaycastHit raycastHitUp, 10, RayMasks.BARRICADE))
                    {
                        if (raycastHit.transform == raycastHitUp.transform)
                        {
                            BarricadeDrop drop = BarricadeManager.FindBarricadeByRootTransform(raycastHit.transform);
                            if (drop.asset.id == BlueCrystalCookingPlugin.Instance.Configuration.Instance.FreezerId)
                            {
                                ulong ownerTray = owner;
                                ulong groupTray = group;
                                float angle_x_tray = angle_x;
                                float angle_y_tray = angle_y;
                                float angle_z_tray = angle_z;
                                BlueCrystalCookingPlugin.Instance.Wait(0.5f, () =>
                                {
                                    Transform tray = BlueCrystalCookingPlugin.Instance.GetPlacedObjectTransform(pos);
                                    BlueCrystalCookingPlugin.Instance.freezingTrays.Add(new FreezingTrayObject(tray, pos, ownerTray, groupTray, angle_x_tray, angle_y_tray, angle_z_tray, 0));
                                });
                            }
                        }
                    }
                }
            }
        }
        public static void Update()
        {
            for (int i=BlueCrystalCookingPlugin.Instance.freezingTrays.Count-1;i>-1;i--)
            {
                var tray = BlueCrystalCookingPlugin.Instance.freezingTrays[i];
                if (BlueCrystalCookingPlugin.Instance.Configuration.Instance.FreezerNeedsPower)
                {
                    foreach (var Generator in PowerTool.checkGenerators(tray.pos, PowerTool.MAX_POWER_RANGE, ushort.MaxValue))
                    {
                        if (Generator.fuel > 0 && Generator.isPowered && Generator.wirerange >= (tray.pos - Generator.transform.position).magnitude)
                        {
                            tray.freezingSeconds += 1;
                            if (tray.freezingSeconds >= BlueCrystalCookingPlugin.Instance.Configuration.Instance.BlueCrystalTrayFreezingTimeSecs)
                            {
                                BarricadeDrop drop = BarricadeManager.FindBarricadeByRootTransform(tray.transform);
                                if (drop != null && BarricadeManager.tryGetRegion(tray.transform, out byte x, out byte y, out ushort plant, out BarricadeRegion barricadeRegion))
                                {
                                    Rocket.Core.Utils.TaskDispatcher.QueueOnMainThread(()=>BarricadeManager.destroyBarricade(drop, x, y, plant));
                                    ItemBarricadeAsset _asset = (ItemBarricadeAsset)Assets.find(EAssetType.RESOURCE, BlueCrystalCookingPlugin.Instance.Configuration.Instance.FrozenTrayId);
                                    if (_asset != null)
                                    {
                                        Barricade newBarr = new Barricade(_asset);
                                        Rocket.Core.Utils.TaskDispatcher.QueueOnMainThread(()=>BarricadeManager.dropBarricade(newBarr, null, tray.pos, tray.angle_x, tray.angle_y, tray.angle_z, tray.owner, tray.group));
                                    }
                                    else
                                    {
                                        Rocket.Core.Utils.TaskDispatcher.QueueOnMainThread(()=>BarricadeManager.dropBarricade(new Barricade(BlueCrystalCookingPlugin.Instance.Configuration.Instance.FrozenTrayId), null, tray.pos, tray.angle_x, tray.angle_y, tray.angle_z, tray.owner, tray.group));
                                    }
                                    if (BlueCrystalCookingPlugin.Instance.Configuration.Instance.EnableBlueCrystalFreezeEffect)
                                    {
                                        Rocket.Core.Utils.TaskDispatcher.QueueOnMainThread(()=>EffectManager.sendEffect(BlueCrystalCookingPlugin.Instance.Configuration.Instance.BlueCrystalFreezeEffectId, 10, tray.pos));
                                    }
                                    BlueCrystalCookingPlugin.Instance.freezingTrays.RemoveAtFast(i);
                                }
                            }
                            break; // is multiple generators = faster intended?
                        }
                    }
                    continue;
                } else
                {
                    tray.freezingSeconds += 1;
                    if (tray.freezingSeconds >= BlueCrystalCookingPlugin.Instance.Configuration.Instance.BlueCrystalTrayFreezingTimeSecs)
                    {
                        BarricadeDrop drop = BarricadeManager.FindBarricadeByRootTransform(tray.transform);
                        if (drop != null && BarricadeManager.tryGetRegion(tray.transform, out byte x, out byte y, out ushort plant, out BarricadeRegion barricadeRegion))
                        {
                            Rocket.Core.Utils.TaskDispatcher.QueueOnMainThread(()=>BarricadeManager.destroyBarricade(drop, x, y, plant));
                            ItemBarricadeAsset _asset = (ItemBarricadeAsset)Assets.find(EAssetType.RESOURCE, BlueCrystalCookingPlugin.Instance.Configuration.Instance.FrozenTrayId);
                            if (_asset != null)
                            {
                                Barricade newBarr = new Barricade(_asset);
                                Rocket.Core.Utils.TaskDispatcher.QueueOnMainThread(()=>BarricadeManager.dropBarricade(newBarr, null, tray.pos, tray.angle_x, tray.angle_y, tray.angle_z, tray.owner, tray.group));
                            }
                            else
                            {
                                Rocket.Core.Utils.TaskDispatcher.QueueOnMainThread(()=>BarricadeManager.dropBarricade(new Barricade(BlueCrystalCookingPlugin.Instance.Configuration.Instance.FrozenTrayId), null, tray.pos, tray.angle_x, tray.angle_y, tray.angle_z, tray.owner, tray.group));
                            }
                            if (BlueCrystalCookingPlugin.Instance.Configuration.Instance.EnableBlueCrystalFreezeEffect)
                            {
                                Rocket.Core.Utils.TaskDispatcher.QueueOnMainThread(()=>EffectManager.sendEffect(BlueCrystalCookingPlugin.Instance.Configuration.Instance.BlueCrystalFreezeEffectId, 10, tray.pos));
                            }
                            BlueCrystalCookingPlugin.Instance.freezingTrays.RemoveAtFast(i);
                        }
                    }
                }
            }
        }
    }
}
