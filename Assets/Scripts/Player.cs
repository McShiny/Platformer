using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{

    [SerializeField] private Rigidbody2D playerBody;
    [SerializeField] private CapsuleCollider2D playerCapsuleCollider;
    [SerializeField] private LayerMask enviromentLayerMask;

    // Movement Variables
    private float moveSpeed = 7f;

    // Jump Variables
    private float jumpVelocity = 18f;
    private float jumpTime = 0f;
    private float maxJumpTime = 0.2f;
    private bool playerJumpQued = false;

    private void Start() {
        GameInput.Instance.OnJumpPreformed += GameInput_OnJumpPreformed;
    }

    private void GameInput_OnJumpPreformed(object sender, System.EventArgs e) {
        if (IsGrounded()) {
            playerJumpQued = true;
            Debug.Log("Jump Qued");
        }
    }

    private void Update() {
        Debug.Log(IsGrounded());
        PlayerMove();
    }

    private void FixedUpdate() {
        if (playerJumpQued) {
            if (jumpTime < maxJumpTime && GameInput.Instance.GetJumpDown()) {
                PlayerJumping();
                jumpTime += Time.deltaTime;
            } else {
                playerJumpQued = false;
                jumpTime = 0f;
            }
        }
    }

    private float PlayerMoveDirectionNormalized() {
        Vector2 inputVector = new Vector2(0, 0);

        inputVector.x = GameInput.Instance.GetMovementVectorNormalized();

        return inputVector.x;
    }

    private void PlayerMove() {
        playerBody.linearVelocity = new Vector2(PlayerMoveDirectionNormalized() * moveSpeed, playerBody.linearVelocityY);
    }

    private void PlayerJumping() {
 
        playerBody.linearVelocity = new Vector2(playerBody.linearVelocityX, jumpVelocity);

    }

    private bool IsGrounded() {
        float extraHeight = 0.1f;
        RaycastHit2D raycastHit = Physics2D.CapsuleCast(playerCapsuleCollider.bounds.center,
            playerCapsuleCollider.bounds.size,
            playerCapsuleCollider.direction, 0f, Vector2.down,
            extraHeight, enviromentLayerMask);

        if (raycastHit.collider != null) {
            return true;
        }
        return false;
    }

}
