using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class LandArea : MonoBehaviour
    {
        private GameManager GameManager;

        public List<Octet> DefaultOctet = new List<Octet>();

        private List<Octet> octets = new List<Octet>();
        public List<Octet> Octets
        {
            get { return octets; }
        }

        public List<FeatureArea> AdjacentTownAreas = new List<FeatureArea>();

        private Land land;
        public Land Land
        {
            get { return land; }
            set { land = value; }
        }

        public Vector3[] PawnOffsets = new Vector3[4];

        private Tile tile;
        public Tile Tile
        {
            get { return tile; }
            set { tile = value; }
        }

        public List<TileOctet> TileOctets = new List<TileOctet>();

        private SpriteRenderer SpriteRenderer;
        public SpriteRenderer sr
        {
            get { return SpriteRenderer; }
            set { SpriteRenderer = value; }
        }

        private PolygonCollider2D PolygonCollider2D;
        public PolygonCollider2D pc
        {
            get { return PolygonCollider2D; }
            set { PolygonCollider2D = value; }
        }

        private void Awake()
        {
            GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            Tile = GetComponentInParent<Tile>();

            pc = gameObject.GetComponent<PolygonCollider2D>();
            pc.enabled = false;

            sr = gameObject.GetComponent<SpriteRenderer>();
            sr.enabled = false;
            sr.sortingOrder = 1;
        }

        private void OnMouseDown()
        {
            if (placed) if (Land.Pawns.Count == 0 && Tile.CanPlacePawnOn)
            {
                Land.Pawns.Add(GameManager.CurrentPlayer.FreePawns[0].PlaceOn(this));
                GameManager.OnPawnPlaced();
            }
        }

        private void OnMouseEnter()
        {
            if (placed) if (Land.Pawns.Count == 0 && Tile.CanPlacePawnOn) foreach (LandArea landArea in Land.LandAreas) landArea.sr.enabled = true;
        }

        private void OnMouseExit()
        {
            if (placed) if (sr.enabled) foreach (LandArea landArea in Land.LandAreas) landArea.sr.enabled = false;
        }

        bool placed = false;
        public void OnPlaced()
        {
            if (!placed)
            {
                foreach (Octet octet in TileOctet.Octets) if (!(Tile.Octets[octet].LandArea is null)) if (Tile.Octets[octet].LandArea.Equals(this)) TileOctets.Add(Tile.Octets[octet]);
                foreach (TileOctet tileOctet in TileOctets) Octets.Add(tileOctet.Octet);

                GameObject landObject = new GameObject("Land");
                Land = landObject.AddComponent<Land>();
                Land.LandAreas.Add(this);
                Land.TileOctets.AddRange(this.TileOctets);
                Land.EmptyTileOctets.AddRange(this.TileOctets);

                placed = true;
            }
        }

        public IEnumerator FlashArea()
        {
            sr.enabled = true;

            yield return new WaitForSeconds(0.2f);

            sr.enabled = false;
        }
    }
}
