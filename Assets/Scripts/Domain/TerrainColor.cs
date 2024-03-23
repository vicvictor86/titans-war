using System.Collections.Generic;
using UnityEngine;

namespace Domain
{
    public static class TerrainColorConstants
    {
        public static readonly Dictionary<TerrainType, Color> colors = new() {
            { TerrainType.RIVER, new Color(0.4784314f, 0.4980392f, 0.8431373f, 1f) },
            { TerrainType.DESERT, new Color(1f, 0.7529413f, 0.4745098f, 1f) },
            { TerrainType.MOUNTAINS, new Color(0.764706f, 0.7568628f, 0.7411765f, 1f) },
            { TerrainType.PLAINS, new Color(0.4745098f, 0.7490196f, 0.4745098f, 1f) },
        };
    }
}