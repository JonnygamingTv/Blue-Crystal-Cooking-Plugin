using Rocket.API;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Ocelot.BlueCrystalCooking
{
    public class BlueCrystalCookingConfiguration : IRocketPluginConfiguration
    {
        // CONFIG VARIABLES
        public bool UseDrugEffectSpeed;
        public bool UseDrugEffectJump;
        public bool EnableBlueCrystalFreezeEffect;
        public bool EnableBarrelStirEffect;
        public bool FreezerNeedsPower;
        public byte StirProgressAddPercentage;
        public ushort FreezerId;
        public ushort BarrelObjectId;
        public ushort BlueCrystalBagId;
        public ushort BlueCrystalTrayId;
        public ushort FrozenTrayId;
        public ushort LiquidTrayId;
        public ushort BlueCrystalFreezeEffectId;
        public ushort BarrelStirEffectId;
        public int DrugEffectDurationSecs;
        public int BlueCrystalTrayFreezingTimeSecs;
        public int BlueCrystalBagsAmountMin;
        public int BlueCrystalBagsAmountMax;
        public float DrugEffectSpeedMultiplier;
        public float DrugEffectJumpMultiplier;
        public string IconImageUrl;
        [XmlArrayItem(ElementName = "Id")]
        public List<ushort> drugIngredientIds;
        

        public void LoadDefaults()
        {
            IconImageUrl = "https://i.imgur.com/fpl2UL7.png";
            BarrelObjectId = 10102;
            drugIngredientIds = new List<ushort>() { 10103, 10104, 10105 };
            BlueCrystalBagId = 10108;
            UseDrugEffectSpeed = true;
            DrugEffectSpeedMultiplier = 2;
            UseDrugEffectJump = true;
            DrugEffectJumpMultiplier = 2;
            DrugEffectDurationSecs = 30;
            BlueCrystalTrayId = 10106;
            FrozenTrayId = 10107;
            LiquidTrayId = 10106;
            FreezerId = 10101;
            BlueCrystalTrayFreezingTimeSecs = 5;
            EnableBlueCrystalFreezeEffect = true;
            BlueCrystalFreezeEffectId = 63;
            EnableBarrelStirEffect = true;
            BarrelStirEffectId = 76;
            StirProgressAddPercentage = 10;
            BlueCrystalBagsAmountMin = 2;
            BlueCrystalBagsAmountMax = 5;
            FreezerNeedsPower = true;
        }
    }
}
