using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public enum Octet
    {
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight
    }

    public class TileOctet
    {
        public static Octet[] Octets = { Octet.One, Octet.Two, Octet.Three, Octet.Four, Octet.Five, Octet.Six, Octet.Seven, Octet.Eight };

        public static Octet[] Inverse_Octets = { Octet.Six, Octet.Five, Octet.Eight, Octet.Seven, Octet.Two, Octet.One, Octet.Four, Octet.Three };

        public static Side[] OctetSides = { Side.Right, Side.Right, Side.Up, Side.Up, Side.Left, Side.Left, Side.Down, Side.Down };

        public Tile Tile;

        public Octet Octet;

        public Land Land
        {
            get
            {
                int relitiveLand = (int)Octet - (2 * Tile.Rotation);
                if (relitiveLand < 0) relitiveLand = 8 + relitiveLand;
                if (!Tile.OrderedLandAreas.ContainsKey((Octet)relitiveLand)) return null;
                return Tile.OrderedLandAreas[(Octet)relitiveLand].Land;
            }

            set
            {
                Tile.OrderedLandAreas[(Octet)((Tile.Rotation + (int)Octet) % 8)].Land = value;
            }
        }

        public LandArea LandArea
        {
            get
            {
                int relitiveLand = (int)Octet - (2 * Tile.Rotation);
                if (relitiveLand < 0) relitiveLand = 8 + relitiveLand;
                if (!Tile.OrderedLandAreas.ContainsKey((Octet)relitiveLand)) return null;
                return Tile.OrderedLandAreas[(Octet)relitiveLand];
            }

            set
            {
                Tile.OrderedLandAreas[(Octet)((Tile.Rotation + (int)Octet) % 8)] = value;
            }
        }

        public TileOctet(Tile tile, Octet octet)
        {
            this.Tile = tile;
            this.Octet = octet;
        }
    }
}