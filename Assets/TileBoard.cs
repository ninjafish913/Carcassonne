using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class TileBoard
    {
        private GameManager GameManager;

        private List<Tile> tiles = new List<Tile>();

        private Dictionary<Tile, Dictionary<Side, Tile>> tileRelationships = new Dictionary<Tile, Dictionary<Side, Tile>>();

        private Dictionary<Tile, Dictionary<Octet, Tile>> tileOctetRelationships = new Dictionary<Tile, Dictionary<Octet, Tile>>();

        public TileBoard(Tile initialTile, GameManager gameManager)
        {
            GameManager = gameManager;

            initialTile.transform.position = new Vector2(0, 0);
            initialTile.GetComponent<BoxCollider2D>().enabled = false;
            tiles.Add(initialTile);

            Dictionary<Side, Tile> initialRelationship = new Dictionary<Side, Tile>();
            foreach (Side side in TileSide.sides) initialRelationship.Add(side, null);

            tileRelationships.Add(initialTile, initialRelationship);

            Dictionary<Octet, Tile> initialLandRelationship = new Dictionary<Octet, Tile>();
            foreach (Octet octet in TileOctet.Octets) initialLandRelationship.Add(octet, null);

            tileOctetRelationships.Add(initialTile, initialLandRelationship);
        }

        public Tile Get(int x, int y)
        {
            foreach (Tile tile in tiles) if ((int)tile.transform.position.x == x && (int)tile.transform.position.y == y) return tile;
            return null;
        }

        public string WhyCantIAdd(Tile tile)
        {
            int x = Mathf.RoundToInt(tile.transform.position.x);
            int y = Mathf.RoundToInt(tile.transform.position.y);

            // Checking if tile already in that position
            if (!(Get(x, y) is null)) return "Already be a tile there!";

            // Checking if there are adjacent tiles
            Tile[] tileAdjacent = { Get(x + 1, y), Get(x, y + 1), Get(x - 1, y), Get(x, y - 1) };
            bool hasAdjacent = false;
            foreach (Tile otherTile in tileAdjacent)
            {
                if (!(otherTile is null))
                {
                    hasAdjacent = true;
                    break;
                }
            }
            if (!hasAdjacent) return "That spot is in the middle of no where!";

            // Checking for like sides
            for (int side = 0; side < TileSide.sides.Length; side++)
            {
                if (!(tileAdjacent[side] is null))
                {
                    if (tileAdjacent[side].Sides[TileSide.inverse_sides[side]].FeatureArea is null || tile.Sides[TileSide.sides[side]].FeatureArea is null)
                    {
                        if (tileAdjacent[side].Sides[TileSide.inverse_sides[side]].FeatureArea is null ^ tile.Sides[TileSide.sides[side]].FeatureArea is null) return "One is has a Feature, but the other does not!";
                    }
                    else
                    {
                        if (!(tile.Sides[TileSide.sides[side]].Feature.FeatureType == tileAdjacent[side].Sides[TileSide.inverse_sides[side]].Feature.FeatureType)) return tile.Sides[TileSide.sides[side]].FeatureArea.FeatureType + " is not the same as " + tileAdjacent[side].Sides[TileSide.inverse_sides[side]].FeatureArea.FeatureType + "!";
                    }
                }
            }

            // Checking for like octets
            for (int octet = 0; octet < TileOctet.Octets.Length; octet++)
            {
                if (!(tileAdjacent[(int)TileOctet.OctetSides[octet]] is null))
                {
                    if (tileAdjacent[(int)TileOctet.OctetSides[octet]].Octets[TileOctet.Inverse_Octets[octet]].LandArea is null || tile.Octets[(Octet)octet].LandArea is null)
                    {
                        if (tileAdjacent[(int)TileOctet.OctetSides[octet]].Octets[TileOctet.Inverse_Octets[octet]].LandArea is null ^ tile.Octets[(Octet)octet].LandArea is null) return "One is land, but the other is not!";
                    }
                }
            }
            return "No, you can place it there.";
        }

        public bool CanAdd(Tile tile)
        {
            int x = Mathf.RoundToInt(tile.transform.position.x);
            int y = Mathf.RoundToInt(tile.transform.position.y);

            // Checking if tile already in that position
            if (!(Get(x, y) is null)) return false;

            // Checking if there are adjacent tiles
            Tile[] tileAdjacent = { Get(x + 1, y), Get(x, y + 1), Get(x - 1, y), Get(x, y - 1) };
            bool hasAdjacent = false;
            foreach (Tile otherTile in tileAdjacent)
            {
                if (!(otherTile is null))
                {
                    hasAdjacent = true;
                    break;
                }
            }
            if (!hasAdjacent) return false;

            // Checking for like sides
            for (int side = 0; side < TileSide.sides.Length; side++)
            {
                if (!(tileAdjacent[side] is null))
                {
                    if (tileAdjacent[side].Sides[TileSide.inverse_sides[side]].FeatureArea is null || tile.Sides[(Side)side].FeatureArea is null)
                    {
                        if (tileAdjacent[side].Sides[TileSide.inverse_sides[side]].FeatureArea is null ^ tile.Sides[(Side)side].FeatureArea is null) return false;
                    }
                    else
                    {
                        if (!(tile.Sides[(Side)side].FeatureArea.FeatureType == tileAdjacent[side].Sides[TileSide.inverse_sides[side]].FeatureArea.FeatureType)) return false;
                    }
                }
            }

            // Checking for like octets
            for (int octet = 0; octet < TileOctet.Octets.Length; octet++)
            {
                if (!(tileAdjacent[(int)TileOctet.OctetSides[octet]] is null))
                {
                    if (tileAdjacent[(int)TileOctet.OctetSides[octet]].Octets[TileOctet.Inverse_Octets[octet]].LandArea is null || tile.Octets[(Octet)octet].LandArea is null)
                    {
                        if (tileAdjacent[(int)TileOctet.OctetSides[octet]].Octets[TileOctet.Inverse_Octets[octet]].LandArea is null ^ tile.Octets[(Octet)octet].LandArea is null) return false;
                    }
                }
            }
            return true;
        }

        public void Add(Tile tile)
        {
            tile.OnPlaced();

            int x = Mathf.RoundToInt(tile.transform.position.x);
            int y = Mathf.RoundToInt(tile.transform.position.y);

            tile.transform.position = new Vector2(x, y);
            tiles.Add(tile);

            // Updating Relationships
            Dictionary<Side, Tile> tileRelationship = new Dictionary<Side, Tile>();
            Dictionary<Octet, Tile> tileOctetRelationship = new Dictionary<Octet, Tile>();

            Tile[] tileAdjacent = { Get(x + 1, y), Get(x, y + 1), Get(x - 1, y), Get(x, y - 1) };
            for (int side = 0; side < TileSide.sides.Length; side++)
            {
                tileRelationship.Add((Side)side, tileAdjacent[side]);
                if (!(tileAdjacent[side] is null)) tileRelationships[tileAdjacent[side]][TileSide.inverse_sides[side]] = tile;
            }

            tileRelationships.Add(tile, tileRelationship);

            for (int octet = 0; octet < TileOctet.Octets.Length; octet++)
            {
                tileOctetRelationship.Add((Octet)octet, tileAdjacent[(int)TileOctet.OctetSides[octet]]);
                if (!(tileAdjacent[(int)TileOctet.OctetSides[octet]] is null)) tileOctetRelationships[tileAdjacent[(int)TileOctet.OctetSides[octet]]][TileOctet.Inverse_Octets[octet]] = tile;
            }

            tileOctetRelationships.Add(tile, tileOctetRelationship);

            // Merging adjacent features
            for (int side = 0; side < TileSide.sides.Length; side++)
            {
                if (!(tile.Sides[(Side)side].Feature is null) && !(tileAdjacent[side] is null))
                {
                    tile.Sides[(Side)side].Feature.Merge(tileAdjacent[side].Sides[TileSide.inverse_sides[side]].Feature);
                }
            }

            for (int octet = 0; octet < TileOctet.Octets.Length; octet++)
            {
                if (!(tile.Octets[(Octet)octet].Land is null) && !(tileAdjacent[(int)TileOctet.OctetSides[octet]] is null))
                {
                    tile.Octets[(Octet)octet].Land.Merge(tileAdjacent[(int)TileOctet.OctetSides[octet]].Octets[TileOctet.Inverse_Octets[octet]].Land);
                }
            }

            // Cleanup
            tile.GetComponent<BoxCollider2D>().enabled = false;
            GameManager.OnTilePlaced();
        }

        public Tile AdjacentTo(Tile tile, Side side)
        {
            return tileRelationships[tile][side];
        }

        public Tile AdjacentTo(Tile tile, Octet octet)
        {
            return tileOctetRelationships[tile][octet];
        }

        public TileSide InverseSide(TileSide tileSide)
        {
            Tile adjacent = AdjacentTo(tileSide.Tile, tileSide.Side);
            if (adjacent is null) return null;
            else return adjacent.Sides[TileSide.inverse_sides[(int)tileSide.Side]];
        }

        public TileOctet InverseOctet(TileOctet tileOctet)
        {
            Tile adjacent = AdjacentTo(tileOctet.Tile, tileOctet.Octet);
            if (adjacent is null) return null;
            else return adjacent.Octets[TileOctet.Inverse_Octets[(int)tileOctet.Octet]];
        }

        public bool IsTileSurrounded(Tile tile)
        {
            int x = Mathf.RoundToInt(tile.transform.position.x);
            int y = Mathf.RoundToInt(tile.transform.position.y);

            Tile[] surroundingTiles = { Get(x, y + 1), Get(x, y - 1), Get(x + 1, y), Get(x + 1, y + 1), Get(x + 1, y - 1), Get(x - 1, y), Get(x - 1, y + 1), Get(x - 1, y - 1) };
            bool surrounded = true;
            foreach (Tile otherTile in surroundingTiles) if (otherTile is null)
                {
                    surrounded = false;
                    break;
                }

            return surrounded;
        }

        public int SurroundingCount(Tile tile)
        {
            int x = Mathf.RoundToInt(tile.transform.position.x);
            int y = Mathf.RoundToInt(tile.transform.position.y);

            Tile[] surroundingTiles = { Get(x, y + 1), Get(x, y - 1), Get(x + 1, y), Get(x + 1, y + 1), Get(x + 1, y - 1), Get(x - 1, y), Get(x - 1, y + 1), Get(x - 1, y - 1) };
            int surrounding = 0;
            foreach (Tile otherTile in surroundingTiles) if (!(otherTile is null)) surrounding++;

            return surrounding;
        }
    }
}
