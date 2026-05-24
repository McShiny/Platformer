using UnityEngine;

public class FinishUI : MonoBehaviour
{

    private void Start() {
        Player.Instance.OnGameFinished += Player_OnGameFinished;
        Hide();
    }

    private void Player_OnGameFinished(object sender, System.EventArgs e) {
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

}
