using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public enum Side
    {
        Right,
        Up,
        Left,
        Down
    }

    public class TileSide
    {
        public static Side[] sides = { Side.Right, Side.Up, Side.Left, Side.Down };

        public static Side[] inverse_sides = { Side.Left, Side.Down, Side.Right, Side.Up };

        public Tile Tile;

        public Side Side;

        public Feature Feature
        {
            get
            {
                int relitiveFeature = (int)Side - Tile.Rotation;
                if (relitiveFeature < 0) relitiveFeature = 4 + relitiveFeature;
                if (!Tile.OrderedAreas.ContainsKey((Side)relitiveFeature)) return null;
                return Tile.OrderedAreas[(Side)relitiveFeature].Feature;
            }

            set
            {
                Tile.OrderedAreas[(Side)((Tile.Rotation + (int)Side) % 4)].Feature = value;
            }
        }

        public FeatureArea FeatureArea
        {
            get
            {
                int relitiveFeature = (int)Side - Tile.Rotation;
                if (relitiveFeature < 0) relitiveFeature = 4 + relitiveFeature;
                if (!Tile.OrderedAreas.ContainsKey((Side)relitiveFeature)) return null;
                return Tile.OrderedAreas[(Side)relitiveFeature];
            }

            set
            {
                Tile.OrderedAreas[(Side)((Tile.Rotation + (int)Side) % 4)] = value;
            }
        }

        public TileSide(Tile tile, Side side)
        {
            this.Tile = tile;
            this.Side = side;
        }
    }
}
