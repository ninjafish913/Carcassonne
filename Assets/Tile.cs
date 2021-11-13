using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Linq;

namespace Assets
{
    public class Tile : MonoBehaviour
    {
        private GameManager GameManager;

        public Dictionary<Side, TileSide> Sides = new Dictionary<Side, TileSide>();
        public Dictionary<Octet, TileOctet> Octets = new Dictionary<Octet, TileOctet>();

        private Dictionary<Side, FeatureArea> orderedAreas = new Dictionary<Side, FeatureArea>();
        public Dictionary<Side, FeatureArea> OrderedAreas
        {
            get { return orderedAreas; }
        }

        private Dictionary<Octet, LandArea> orderedLandAreas = new Dictionary<Octet, LandArea>();
        public Dictionary<Octet, LandArea> OrderedLandAreas
        {
            get { return orderedLandAreas; }
        }

        private List<FeatureArea> features = new List<FeatureArea>();
        public List<FeatureArea> Features
        {
            get
            {
                return features;
            }
        }

        private List<LandArea> lands = new List<LandArea>();
        public List<LandArea> Lands
        {
            get
            {
                return lands;
            }
        }

        private Dictionary<Side, FeatureArea> orderedByRotation = new Dictionary<Side, FeatureArea>();
        public Dictionary<Side, FeatureArea> OrderedByRotation
        {
            get
            {
                return orderedByRotation;
            }
        }

        private Dictionary<Octet, LandArea> orderedLandByRotation = new Dictionary<Octet, LandArea>();
        public Dictionary<Octet, LandArea> OrderedLandByRotation
        {
            get
            {
                return orderedLandByRotation;
            }
        }

        private SpriteRenderer sr;

        public bool HasMonastery;

        public int Rotation = 0;

        public bool CanPlacePawnOn = false;

        private void Awake()
        {
            features = new List<FeatureArea>(GetComponentsInChildren<FeatureArea>());
            lands = new List<LandArea>(GetComponentsInChildren<LandArea>());

            GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

            foreach (Side side in TileSide.sides) Sides.Add(side, new TileSide(this, side));
            foreach (Octet octet in TileOctet.Octets) Octets.Add(octet, new TileOctet(this, octet));

            foreach (FeatureArea featureArea in features) foreach (Side side in featureArea.DefaultSides) orderedAreas.Add(side, featureArea);
            foreach (LandArea landArea in lands) foreach (Octet octet in landArea.DefaultOctet) orderedLandAreas.Add(octet, landArea);

            gameObject.AddComponent<BoxCollider2D>();
            sr = GetComponent<SpriteRenderer>();
        }

        Stopwatch Stopwatch = new Stopwatch();
        private const int RCooldown = 500;
        private void OnMouseDrag()
        {
            float distance_to_screen = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance_to_screen));

            sr.sortingOrder = 3;

            if (Stopwatch.ElapsedMilliseconds >= RCooldown && Stopwatch.IsRunning)
            {
                Stopwatch.Stop();
            }

            if (Input.GetKey(KeyCode.R) && !Stopwatch.IsRunning)
            {
                transform.Rotate(new Vector3(0, 0, 90));
                Rotation = (Rotation + 1) % 4;
                Stopwatch.Restart();
                Stopwatch.Start();
            }
        }

        private void OnMouseUp()
        {
            if (GameManager.TileBoard.CanAdd(this))
            {
                sr.sortingOrder = 0;
                GameManager.TileBoard.Add(this);
            }
        }

        public void OnPlaced()
        {
            foreach (Side side in TileSide.sides) OrderedByRotation.Add(side, Sides[side].FeatureArea);
            foreach (FeatureArea featureArea in features) featureArea.OnPlaced();

            foreach (Octet octet in TileOctet.Octets) OrderedLandByRotation.Add(octet, Octets[octet].LandArea);
            foreach (LandArea landArea in lands) landArea.OnPlaced();
        }
    }
}