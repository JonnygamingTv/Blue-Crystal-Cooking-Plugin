using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Ocelot.BlueCrystalCooking
{
    public class BarrelObject
    {
        public byte progress = 0;
        public List<ushort> ingredients = new List<ushort>();
        public BarrelObject(List<ushort> ingredients, byte progress)
        {
            this.ingredients = ingredients;
            this.progress = progress;
        }
    }
}
