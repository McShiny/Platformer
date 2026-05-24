using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{

    public static Player Instance { get; private set; }

    public event EventHandler OnCoinCollected;
    public event EventHandler OnGameFinished;
    public event EventHandler<OnPlayerDashedEventArgs> OnPlayerDashed;
    public class OnPlayerDashedEventArgs : EventArgs {
        public float progressNormalized;
    }

    [SerializeField] private Rigidbody2D playerBody;
    [SerializeField] private CapsuleCollider2D playerCapsuleCollider;
    [SerializeField] private LayerMask enviromentLayerMask;
    [SerializeField] private LayerMask coinLayerMask;
    [SerializeField] private LayerMask borderLayerMask;
    [SerializeField] private LayerMask finishLayerMask;

    // Movement Variables
    private float moveSpeed = 7f;
    private float lastMoveDirection;

    // Jump Variables
    private float jumpVelocity = 18f;
    private float jumpTime = 0f;
    private float maxJumpTime = 0.2f;
    private bool playerJumpQued = false;

    // Dash Variables
    private float dashVelocity = 30f;
    private bool dashQued = false;
    private float dashingTime = 0f;
    private float dashingTimeMax = 0.25f;
    private bool dashAvailable = true;
    private float dashCooldown = 0f;
    private float dashCooldownTime = 2.5f;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        GameInput.Instance.OnJumpPreformed += GameInput_OnJumpPreformed;
        GameInput.Instance.OnDashPreformed += GameInput_OnDashPreformed;
    }

    private void GameInput_OnDashPreformed(object sender, EventArgs e) {
        if (dashAvailable) {
            dashQued = true;
            OnPlayerDashed?.Invoke(this, new OnPlayerDashedEventArgs {
                progressNormalized = 1f
            });
            dashAvailable = false;
        }
    }

    private void GameInput_OnJumpPreformed(object sender, System.EventArgs e) {
        if (IsGrounded()) {
            playerJumpQued = true;
        }
    }

    private void Update() {
        PlayerMove();
        CoinCollision();

        if (OutOfBounds()) {
            transform.position = new Vector3(0, -2.30f, 0);
        }

        if (Finished()) {
            OnGameFinished?.Invoke(this, EventArgs.Empty);
        }

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

        if (dashQued) {
            if (dashingTime < dashingTimeMax) {
                PlayerDashing();
                dashingTime += Time.deltaTime;
            }
            else {
                dashQued = false;
                dashingTime = 0f;
            }
        }

        if (!dashAvailable) {
            if (dashCooldown < dashCooldownTime) {
                dashCooldown += Time.deltaTime;
                OnPlayerDashed?.Invoke(this, new OnPlayerDashedEventArgs {
                    progressNormalized = 1 - dashCooldown / 2.5f
                });
            } else {
                dashAvailable = true;
                dashCooldown = 0f;
                OnPlayerDashed?.Invoke(this, new OnPlayerDashedEventArgs {
                    progressNormalized = 1
                });
            }
        }
    }

    private float PlayerMoveDirectionNormalized() {
        Vector2 inputVector = new Vector2(0, 0);

        inputVector.x = GameInput.Instance.GetMovementVectorNormalized();
        lastMoveDirection = inputVector.x;

        return inputVector.x;
    }

    private void PlayerMove() {
        playerBody.linearVelocity = new Vector2(PlayerMoveDirectionNormalized() * moveSpeed, playerBody.linearVelocityY);
    }

    private void PlayerJumping() {
        playerBody.linearVelocity = new Vector2(playerBody.linearVelocityX, jumpVelocity);
    }

    private void PlayerDashing() {
        playerBody.linearVelocity = new Vector2(dashVelocity * lastMoveDirection, playerBody.linearVelocityY);
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

    private void CoinCollision() {
        Collider2D coinCollider = Physics2D.OverlapCapsule(
        playerCapsuleCollider.bounds.center,
        playerCapsuleCollider.bounds.size,
        playerCapsuleCollider.direction,
        0f,
        coinLayerMask
    );

        if (coinCollider != null) {
            Destroy(coinCollider.gameObject);
            OnCoinCollected?.Invoke(this, EventArgs.Empty);
        }
    }

    private bool OutOfBounds() {
        float extraHeight = 0.1f;
        RaycastHit2D raycastHit = Physics2D.CapsuleCast(playerCapsuleCollider.bounds.center,
            playerCapsuleCollider.bounds.size,
            playerCapsuleCollider.direction, 0f, Vector2.down,
            extraHeight, borderLayerMask);

        if (raycastHit.collider != null) {
            return true;
        }
        return false;
    }

    private bool Finished() {
        float extraHeight = 0.1f;
        RaycastHit2D raycastHit = Physics2D.CapsuleCast(playerCapsuleCollider.bounds.center,
            playerCapsuleCollider.bounds.size,
            playerCapsuleCollider.direction, 0f, Vector2.down,
            extraHeight, finishLayerMask);

        if (raycastHit.collider != null) {
            return true;
        }
        return false;
    }
}

