using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets
{
    public class Pawn : MonoBehaviour
    {
        private GameManager GameManager;

        public Player Player;

        public FeatureArea FeatureArea = null;

        public LandArea LandArea = null;

        public bool OnMonestary = false;

        public Tile Tile = null;

        public SpriteRenderer sr;

        public Image PawnImage;

        private void Awake()
        {
            GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

            sr = GetComponent<SpriteRenderer>();
        }

        public Pawn PlaceOn(FeatureArea featureArea)
        {
            PawnImage.enabled = false;

            Tile = featureArea.Tile;
            transform.position = featureArea.Tile.transform.position + featureArea.PawnOffsets[featureArea.Tile.Rotation];

            sr.enabled = true;
            FeatureArea = featureArea;
            return this;
        }

        public Pawn PlaceOn(LandArea landArea)
        {
            PawnImage.enabled = false;

            Tile = landArea.Tile;
            transform.position = landArea.Tile.transform.position + landArea.PawnOffsets[landArea.Tile.Rotation];

            sr.enabled = true;
            LandArea = landArea;
            return this;
        }
    }
}