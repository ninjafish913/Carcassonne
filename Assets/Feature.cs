using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Assets
{
    public enum FeatureType
    {
        Road,
        Town
    }

    public class Feature : MonoBehaviour
    {
        private GameManager GameManager;

        public FeatureType FeatureType;

        public List<Pawn> Pawns = new List<Pawn>();

        public List<FeatureArea> FeatureAreas = new List<FeatureArea>();

        public List<TileSide> TileSides = new List<TileSide>();

        public List<TileSide> EmptyTileSides = new List<TileSide>();

        public List<Tile> Tiles
        {
            get
            {
                List<Tile> tiles = new List<Tile>();
                foreach (FeatureArea featureArea in FeatureAreas)
                {
                    bool duplicate = false;
                    foreach (Tile tile in tiles)
                    {
                        if (featureArea.Tile.Equals(tile))
                        {
                            duplicate = true;
                            break;
                        }
                    }
                    if (!duplicate) tiles.Add(featureArea.Tile);
                }
                return tiles;
            }
        }

        public bool OnCheck()
        {
            if (EmptyTileSides.Count == 0)
            {
                if (Pawns.Count > 0)
                {
                    Dictionary<Player, int> totalPlayerPawns = new Dictionary<Player, int>();
                    foreach (Pawn pawn in Pawns)
                    {
                        if (totalPlayerPawns.ContainsKey(pawn.Player)) totalPlayerPawns[pawn.Player]++;
                        else totalPlayerPawns.Add(pawn.Player, 1);
                    }
                    bool tie = false;
                    Player greatestPlayer = null;
                    foreach (Player player in totalPlayerPawns.Keys)
                    {
                        if (greatestPlayer is null) greatestPlayer = player;
                        else if (totalPlayerPawns[greatestPlayer] < totalPlayerPawns[player])
                        {
                            greatestPlayer = player;
                            tie = false;
                        }
                        else if (totalPlayerPawns[greatestPlayer] == totalPlayerPawns[player]) tie = true;
                    }
                    if (!tie)
                    {
                        GivePoints(greatestPlayer);
                    }
                    foreach (Pawn pawn in Pawns)
                    {
                        pawn.PawnImage.enabled = false;
                        pawn.Tile = null;
                        pawn.FeatureArea = null;
                        pawn.sr.enabled = false;
                    }
                }
                return true;
            }
            else return false;
        }

        public void GiveEndPoints()
        {
            if (Pawns.Count > 0)
            {
                Dictionary<Player, int> totalPlayerPawns = new Dictionary<Player, int>();
                foreach (Pawn pawn in Pawns)
                {
                    if (totalPlayerPawns.ContainsKey(pawn.Player)) totalPlayerPawns[pawn.Player]++;
                    else totalPlayerPawns.Add(pawn.Player, 1);
                }
                bool tie = false;
                Player greatestPlayer = null;
                foreach (Player player in totalPlayerPawns.Keys)
                {
                    if (greatestPlayer is null) greatestPlayer = player;
                    else if (totalPlayerPawns[greatestPlayer] < totalPlayerPawns[player])
                    {
                        greatestPlayer = player;
                        tie = false;
                    }
                    else if (totalPlayerPawns[greatestPlayer] == totalPlayerPawns[player]) tie = true;
                }
                if (!tie) GiveDiscountedPoints(greatestPlayer);

                foreach (Pawn pawn in Pawns)
                {
                    pawn.Tile = null;
                    pawn.FeatureArea = null;
                    pawn.sr.enabled = false;
                }
            }
        }

        private void GivePoints(Player GreatestPlayer)
        {
            switch ((int)FeatureType)
            {
                case 0:
                    foreach (Tile tile in Tiles)
                    {
                        foreach (FeatureArea featureArea in tile.Features)
                        {
                            if (featureArea.Feature.Equals(this)) StartCoroutine(featureArea.FlashArea());
                        }
                        GreatestPlayer.GivePoints(1, tile.transform.position);
                    }
                    break;
                case 1:
                    foreach (Tile tile in Tiles)
                    {
                        int multiplier = 1;
                        foreach (FeatureArea featureArea in tile.Features)
                        {
                            if (featureArea.DoublePoints) multiplier++;
                            if (featureArea.Feature.Equals(this)) StartCoroutine(featureArea.FlashArea());
                        }
                        GreatestPlayer.GivePoints(multiplier * 2, tile.transform.position);
                    }
                    break;
            }
        }

        public void GiveDiscountedPoints(Player GreatestPlayer)
        {
            switch ((int)FeatureType)
            {
                case 0:
                    foreach (Tile tile in Tiles)
                    {
                        foreach (FeatureArea featureArea in tile.Features)
                        {
                            if (featureArea.Feature.Equals(this)) StartCoroutine(featureArea.FlashArea());
                        }
                        GreatestPlayer.GivePoints(1, tile.transform.position);
                    }
                    break;
                case 1:
                    foreach (Tile tile in Tiles)
                    {
                        int multiplier = 1;
                        foreach (FeatureArea featureArea in tile.Features)
                        {
                            if (featureArea.DoublePoints) multiplier++;
                            if (featureArea.Feature.Equals(this)) StartCoroutine(featureArea.FlashArea());
                        }
                        GreatestPlayer.GivePoints(multiplier, tile.transform.position);
                    }
                    break;

            }
        }

        public void Merge(Feature feature)
        {
            if (feature.FeatureType.Equals(this.FeatureType))
            {
                if (!(feature.Equals(this)))
                {
                    this.FeatureAreas.AddRange(feature.FeatureAreas);
                    this.TileSides.AddRange(feature.TileSides);
                    this.EmptyTileSides.AddRange(feature.EmptyTileSides);
                    this.Pawns.AddRange(feature.Pawns);

                    // Replaces All References of feature to this
                    foreach (FeatureArea featureArea in feature.FeatureAreas) featureArea.Feature = this;
                    GameManager.Features.Remove(feature);
                    Destroy(feature.gameObject);

                    // Removes Inverse TileSides from Empty Tile Sides
                    for (int i = 0; i < EmptyTileSides.Count - 1; i++)
                    {
                        TileSide inverseSide = GameManager.TileBoard.InverseSide(EmptyTileSides[i]);
                        if (!(inverseSide is null))
                        {
                            EmptyTileSides.RemoveAt(i);
                            EmptyTileSides.Remove(inverseSide);
                        }
                    }
                }
            }
            else
            {
                throw new InvalidOperationException("Invalid call on Merge, feature is not the same as this!");
            }
        }

        private void Awake()
        {
            GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            GameManager.Features.Add(this);
        }
    }
}
