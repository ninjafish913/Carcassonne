using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Assets
{
    public class Land : MonoBehaviour
    {
        private GameManager GameManager;

        public List<Pawn> Pawns = new List<Pawn>();

        public List<LandArea> LandAreas = new List<LandArea>();

        public List<TileOctet> TileOctets = new List<TileOctet>();

        public List<TileOctet> EmptyTileOctets = new List<TileOctet>();

        public List<Tile> Tiles
        {
            get
            {
                List<Tile> tiles = new List<Tile>();
                foreach (LandArea landArea in LandAreas)
                {
                    bool duplicate = false;
                    foreach (Tile tile in tiles)
                    {
                        if (landArea.Tile.Equals(tile))
                        {
                            duplicate = true;
                            break;
                        }
                    }
                    if (!duplicate) tiles.Add(landArea.Tile);
                }
                return tiles;
            }
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
                if (!tie)
                {
                    StartCoroutine(GivePoints(greatestPlayer));
                }
                foreach (Pawn pawn in Pawns)
                {
                    pawn.Tile = null;
                    pawn.FeatureArea = null;
                    pawn.sr.enabled = false;
                }
            }
        }

        private IEnumerator GivePoints(Player GreatestPlayer)
        {
            foreach (LandArea landArea in LandAreas) landArea.sr.enabled = true;

            List<Feature> AdjacentTowns = new List<Feature>();
            foreach (LandArea landArea in LandAreas) foreach (FeatureArea featureArea in landArea.AdjacentTownAreas) AdjacentTowns.Add(featureArea.Feature);

            foreach (Feature feature in AdjacentTowns.Distinct()) if (feature.FeatureType == FeatureType.Town && feature.EmptyTileSides.Count == 0)
                {
                    foreach (FeatureArea featureArea in feature.FeatureAreas) StartCoroutine(featureArea.FlashArea());
                    GreatestPlayer.GivePoints(3, feature.FeatureAreas[0].transform.position);
                    yield return new WaitForSeconds(1f);
                }

            foreach (LandArea landArea in LandAreas) landArea.sr.enabled = false;
        }

        public void Merge(Land land)
        {
            if (!(land.Equals(this)))
            {
                this.LandAreas.AddRange(land.LandAreas);
                this.TileOctets.AddRange(land.TileOctets);
                this.EmptyTileOctets.AddRange(land.EmptyTileOctets);
                this.Pawns.AddRange(land.Pawns);

                // Replaces All References of feature to this
                foreach (LandArea landArea in land.LandAreas) landArea.Land = this;
                GameManager.Lands.Remove(land);
                Destroy(land.gameObject);

                // Removes Inverse TileSides from Empty Tile Sides
                for (int i = 0; i < EmptyTileOctets.Count - 1; i++)
                {
                    TileOctet inverseOctet = GameManager.TileBoard.InverseOctet(EmptyTileOctets[i]);
                    if (!(inverseOctet is null))
                    {
                        EmptyTileOctets.RemoveAt(i);
                        EmptyTileOctets.Remove(inverseOctet);
                    }
                }
            }
        }

        private void Awake()
        {
            GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            GameManager.Lands.Add(this);
        }
    }
}