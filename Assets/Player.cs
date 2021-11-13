using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets
{
    public class Player : MonoBehaviour
    {
        private GameManager GameManager;

        public Color Color;

        public int Score = 0;

        public List<Pawn> Pawns = new List<Pawn>();

        public List<Pawn> FreePawns
        {
            get
            {
                List<Pawn> freePawns = new List<Pawn>();
                foreach (Pawn pawn in Pawns) if (pawn.FeatureArea is null && pawn.LandArea is null && !pawn.OnMonestary) freePawns.Add(pawn);
                return freePawns;
            }
        }

        private IEnumerator FlyDestroy(GameObject ScorePopup)
        {
            Text Text = ScorePopup.GetComponent<Text>();
            Text.enabled = true;
            Text.color = new Color(Color.r, Color.g, Color.b, 150);
            ScorePopup.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0.25f);

            yield return new WaitForSeconds(2f);

            Destroy(ScorePopup);
        }

        public void GivePoints(int Total)
        {
            Score += Total;
        }

        public void GivePoints(int Total, Vector3 position)
        {
            // Creates a +Total over position
            GameObject ScorePopup = Instantiate(GameManager.ScoreGainText);
            ScorePopup.transform.SetParent(GameManager.PopupCanvas.transform);
            ScorePopup.transform.position = position;

            Text ScoreText = ScorePopup.GetComponent<Text>();
            ScoreText.text = "+" + Total;
            ScoreText.enabled = true;

            GivePoints(Total);

            StartCoroutine(FlyDestroy(ScorePopup));
        }

        private void Awake()
        {
            GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

            Color = GameManager.Colors[0];
            GameManager.Colors.RemoveAt(0);

            for (int i = 0; i < GameManager.PawnsPerPlayer; i++)
            {
                GameObject pawnImage = Instantiate(GameManager.PawnImages[GameManager.Players.Count].gameObject);
                pawnImage.transform.SetParent(GameManager.UICanvas.transform);
                pawnImage.transform.position = GameManager.PawnImages[GameManager.Players.Count].transform.position + new Vector3(0.75f * i * pawnImage.transform.localScale.x * pawnImage.GetComponent<Image>().sprite.rect.width, 0, 0);
                pawnImage.GetComponent<Image>().color = Color;
                pawnImage.GetComponent<Image>().enabled = true;

                GameObject pawnObject = new GameObject(name + "'s pawn " + i);
                SpriteRenderer sr = pawnObject.AddComponent<SpriteRenderer>();
                sr.enabled = false;
                sr.sprite = GameManager.PawnSprite;
                sr.color = Color;
                sr.sortingOrder = 2;
                Pawn pawn = pawnObject.AddComponent<Pawn>();
                pawn.Player = this;
                pawn.PawnImage = pawnImage.GetComponent<Image>();
                Pawns.Add(pawn);
            }
        }
    }
}