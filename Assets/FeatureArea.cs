using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Assets
{
    public class FeatureArea : MonoBehaviour
    {
        private GameManager GameManager;

        public FeatureType FeatureType;

        public bool DoublePoints;

        public List<Side> DefaultSides = new List<Side>();

        private List<Side> sides = new List<Side>();
        public List<Side> Sides
        {
            get
            {
                return sides;
            }
        }

        private Feature feature;
        public Feature Feature
        {
            get { return feature; }
            set { feature = value; }
        }

        public Vector3[] PawnOffsets = new Vector3[4];

        private Tile tile;
        public Tile Tile
        {
            get { return tile; }
            set { tile = value; }
        }

        public List<TileSide> TileSides = new List<TileSide>();

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
            if (placed) if (Feature.Pawns.Count == 0 && Tile.CanPlacePawnOn)
            {
                Feature.Pawns.Add(GameManager.CurrentPlayer.FreePawns[0].PlaceOn(this));
                GameManager.OnPawnPlaced();
            }
        }

        private void OnMouseEnter()
        {
            if (placed) if (Feature.Pawns.Count == 0 && Tile.CanPlacePawnOn) foreach (FeatureArea featureArea in Feature.FeatureAreas) featureArea.sr.enabled = true;
        }

        private void OnMouseExit()
        {
            if (placed) if (sr.enabled) foreach (FeatureArea featureArea in Feature.FeatureAreas) featureArea.sr.enabled = false;
        }

        bool placed = false;
        public void OnPlaced()
        {
            if (!placed)
            {
                foreach (Side side in TileSide.sides) if (!(Tile.Sides[side].FeatureArea is null))
                        if (Tile.Sides[side].FeatureArea.Equals(this)) 
                            TileSides.Add(Tile.Sides[side]);

                foreach (TileSide tileSide in TileSides) Sides.Add(tileSide.Side);

                GameObject featureObject = new GameObject(FeatureType.ToString());
                Feature = featureObject.AddComponent<Feature>();
                Feature.FeatureType = this.FeatureType;
                Feature.FeatureAreas.Add(this);
                Feature.TileSides.AddRange(this.TileSides);
                Feature.EmptyTileSides.AddRange(this.TileSides);

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
