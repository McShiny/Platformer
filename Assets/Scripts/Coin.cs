using UnityEngine;

public class Coin : MonoBehaviour
{

    [SerializeField] CircleCollider2D coinCollider;
    [SerializeField] LayerMask playerLayerMask;

    private void Update() {
        if (playerContact()) {
            Destroy(this);
        }
    }

    private bool playerContact() {
        RaycastHit2D raycastHit = Physics2D.CircleCast(coinCollider.bounds.center,
           coinCollider.radius,
           Vector2.down,
           playerLayerMask);

        if (raycastHit.collider != null) {
            return true;
        }
        return false;
    }

}
