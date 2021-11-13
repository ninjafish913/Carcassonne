using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets
{
    public class GameManager : MonoBehaviour
    {
        // Features & Lands

        private List<Feature> features = new List<Feature>();
        public List<Feature> Features
        {
            get { return features; }
        }

        private List<Land> lands = new List<Land>();
        public List<Land> Lands
        {
            get { return lands; }
        }

        // Player Managment

        public List<Color> Colors = new List<Color>();

        public List<string> ColorNames = new List<string>();

        public int TotalPlayers = 2;

        public int PawnsPerPlayer = 5;

        private bool playerPlacingPawn = false;
        public bool PlayerPlacingPawn
        {
            get { return playerPlacingPawn; }
            set { playerPlacingPawn = value; }
        }

        private List<Player> players = new List<Player>();
        public List<Player> Players
        {
            get { return players; }
        }

        private int player = 0;
        public Player CurrentPlayer
        {
            get
            {
                return Players[player];
            }
        }

        // Tile Management

        public int TotalTiles = 20;

        public Sprite PawnSprite;

        private GameObject[] tilePrefabs;

        public TileBoard TileBoard;

        private List<Pawn> pawnsOnMonastery = new List<Pawn>();
        public List<Pawn> PawnsOnMonastery
        {
            get { return pawnsOnMonastery; }
        }

        private Tile currentTile;
        public Tile CurrentTile
        {
            get { return currentTile; }
            set { currentTile = value; }
        }

        private List<GameObject> tileStack = new List<GameObject>();
        public List<GameObject> TileStack
        {
            get { return tileStack; }
        }

        // Scene Config

        public GameObject MainCamera;

        public Canvas PopupCanvas;

        public Canvas UICanvas;

        public GameObject ScoreGainText;
        
        public Text CurrentTurnText;

        public List<Image> PawnImages;

        public List<Text> PlayerText = new List<Text>();

        public List<Text> ScoreText = new List<Text>();

        public void OnTilePlaced()
        {
            CurrentTile.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);
            if (CurrentPlayer.FreePawns.Count > 0)
            {
                if (CurrentTile.HasMonastery)
                {
                    Pawn CurrentPawn = CurrentPlayer.FreePawns[0];
                    CurrentPawn.transform.position = CurrentTile.transform.position;
                    CurrentPawn.sr.enabled = true;
                    CurrentPawn.OnMonestary = true;
                    CurrentPawn.PawnImage.enabled = false;
                    CurrentPawn.Tile = CurrentTile;
                    PawnsOnMonastery.Add(CurrentPawn);

                    OnNextTile();
                }
                else
                {
                    foreach (FeatureArea featureArea in CurrentTile.Features) featureArea.GetComponent<PolygonCollider2D>().enabled = true;
                    foreach (LandArea landArea in CurrentTile.Lands) landArea.GetComponent<PolygonCollider2D>().enabled = true;
                    CurrentTile.CanPlacePawnOn = true;
                }
            }
            else
            {
                OnNextTile();
            }
        }

        public void OnPawnPlaced()
        {
            CurrentTile.CanPlacePawnOn = false;

            OnNextTile();
        }

        private void UpdateScores()
        {
            List<Feature> RemoveableFeatures = new List<Feature>();
            foreach (Feature feature in Features) if (feature.OnCheck())
                {
                    foreach (Pawn pawn in feature.Pawns) pawn.PawnImage.enabled = true;
                    RemoveableFeatures.Add(feature);
                }
            foreach (Feature feature in RemoveableFeatures) Features.Remove(feature);

            List<Pawn> RemoveablePawns = new List<Pawn>();
            foreach (Pawn pawn in PawnsOnMonastery) if (TileBoard.IsTileSurrounded(pawn.Tile))
                {
                    pawn.PawnImage.enabled = true;
                    pawn.Player.GivePoints(9, pawn.Tile.transform.position);
                    pawn.Tile = null;
                    pawn.OnMonestary = false;
                    pawn.sr.enabled = false;
                    RemoveablePawns.Add(pawn);
                }
            foreach (Pawn pawn in RemoveablePawns) PawnsOnMonastery.Remove(pawn);
            for (int p = 0; p < PlayerText.Count; p++)
                if (p < TotalPlayers) ScoreText[p].text = "SCORE " + Players[p].Score;
        }

        private void OnNextTile()
        {
            UpdateScores();

            if (TileStack.Count > 0)
            {
                player = (player + 1) % TotalPlayers;

                CurrentTurnText.text = Players[player].name + "'s TURN";

                CurrentTile = NextTile(2.5f, 2.5f);
            }
            else
            {
                StartCoroutine(IEndGame());
            }
        }

        private IEnumerator IEndGame()
        {
            // Count Remaining Points
            CurrentTurnText.text = "Counting Monasterys...";
            foreach (Pawn pawn in PawnsOnMonastery)
            {
                pawn.Player.GivePoints(TileBoard.SurroundingCount(pawn.Tile) + 1, pawn.Tile.transform.position);
                pawn.Tile = null;
                pawn.OnMonestary = false;
                pawn.sr.enabled = false;
                for (int p = 0; p < PlayerText.Count; p++)
                    if (p < TotalPlayers) ScoreText[p].text = "SCORE " + Players[p].Score;
                yield return new WaitForSeconds(0.2f);
            }

            CurrentTurnText.text = "Counting Features...";
            foreach (Feature feature in Features)
            {
                feature.GiveEndPoints();
                for (int p = 0; p < PlayerText.Count; p++)
                    if (p < TotalPlayers) ScoreText[p].text = "SCORE " + Players[p].Score;
                yield return new WaitForSeconds(0.3f);
            }

            CurrentTurnText.text = "Counting Lands...";
            foreach (Land land in Lands)
            {
                land.GiveEndPoints();
                for (int p = 0; p < PlayerText.Count; p++)
                    if (p < TotalPlayers) ScoreText[p].text = "SCORE " + Players[p].Score;
                yield return new WaitForSeconds(0.3f);
            }

            Player Greatest = Players[0];
            foreach (Player player in Players) if (player.Score > Greatest.Score) Greatest = player;
            CurrentTurnText.text = Greatest.name + " Wins!";
        }

        public Tile NextTile(float x, float y)
        {
            GameObject newObject = Instantiate(TileStack[0]);
            TileStack.RemoveAt(0);
            newObject.transform.position = new Vector2(x, y);
            newObject.layer = 0;
            newObject.GetComponent<SpriteRenderer>().color = new Color(175, 175, 175, 255);
            return newObject.GetComponent<Tile>();
        }
        public Tile NextTile()
        {
            return NextTile(0, 0);
        }

        public void InitializeTileStack(int totalTiles)
        {
            for (int tile = 0; tile < totalTiles; tile++)
            {
                TileStack.Add(tilePrefabs[Random.Range(0, tilePrefabs.Length)]);
            }
        }

        private void Start()
        {
            foreach (Image pawnImage in PawnImages) pawnImage.enabled = false;

            for (int i = 0; i < TotalPlayers; i++)
            {
                GameObject player = new GameObject(ColorNames[0]);
                ColorNames.RemoveAt(0);
                Players.Add(player.AddComponent<Player>());
            }

            for (int p = 0; p < PlayerText.Count; p++)
            {
                if (p >= TotalPlayers)
                {
                    ScoreText[p].text = "";
                    PlayerText[p].text = "";
                }
                else
                {
                    ScoreText[p].text = "SCORE 0";
                    PlayerText[p].text = Players[p].name;
                }
            }

            CurrentTurnText.text = Players[0].name + "'s TURN";

            tilePrefabs = Resources.LoadAll<GameObject>("Tiles");

            InitializeTileStack(TotalTiles);

            CurrentTile = NextTile();
            CurrentTile.OnPlaced();
            TileBoard = new TileBoard(CurrentTile, this);

            OnTilePlaced();
        }

        public float CameraMoveSpeed = 0.1f;
        /*private Vector3 dragOrigin;
        private bool dragging = false;*/
        private void FixedUpdate()
        {
            if (Input.GetKey(KeyCode.W)) MainCamera.transform.position = new Vector3(MainCamera.transform.position.x, MainCamera.transform.position.y + CameraMoveSpeed, MainCamera.transform.position.z);
            else if (Input.GetKey(KeyCode.S)) MainCamera.transform.position = new Vector3(MainCamera.transform.position.x, MainCamera.transform.position.y - CameraMoveSpeed, MainCamera.transform.position.z);

            if (Input.GetKey(KeyCode.D)) MainCamera.transform.position = new Vector3(MainCamera.transform.position.x + CameraMoveSpeed, MainCamera.transform.position.y, MainCamera.transform.position.z);
            else if (Input.GetKey(KeyCode.A)) MainCamera.transform.position = new Vector3(MainCamera.transform.position.x - CameraMoveSpeed, MainCamera.transform.position.y, MainCamera.transform.position.z);

            /*if (Input.GetMouseButton(1))
            {
                if (!dragging)
                {
                    dragOrigin = Input.mousePosition;
                    Cursor.lockState = CursorLockMode.Locked;
                    dragging = true;
                }
                else
                {
                    Vector3 offset = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
                    MainCamera.transform.Translate(offset, Space.World);
                }
            }
            else
            {
                dragging = false;
                Cursor.lockState = CursorLockMode.None;
            }*/

            if (Input.GetKey(KeyCode.E) && CurrentTile.CanPlacePawnOn)
            {
                OnPawnPlaced();
            }
        }
    }
}