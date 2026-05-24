using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI scoreText;

    private int score = 0;

    private void Start() {
        Player.Instance.OnCoinCollected += Player_OnCoinCollected;
    }

    private void Player_OnCoinCollected(object sender, System.EventArgs e) {
        score += 1;
        scoreText.text = "Score: " + score;
    }
}
