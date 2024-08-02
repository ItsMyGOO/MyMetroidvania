using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerMove : MonoBehaviour {
    Rigidbody2D myBody;
    PlayerInputManager playerInputManager;

    float xInput, yInput, jInput;
    bool jInputPressing;
    float sInput;
    bool sInputPressing;

    int facingDirection = 1;

    const float MOVE_SPEED = 3f;
    const float JUMP_HEIGHT = 5f;
    const int AIR_JUMP = 3;

    [SerializeField] bool isJumping, isJumpFall;
    [SerializeField] int jumpChance;
    bool jumpLong;
    float jumpLongCountDown;

    [SerializeField] bool onTop, onGround, onOneWay;

    Vector2 newVelocity, newForce;

    GroundSensor groundCheck;

    void Awake() {
        groundCheck = GetComponentInChildren<GroundSensor>();
        myBody = GetComponent<Rigidbody2D>();
    }

    void Start() {
        playerInputManager = PlayerInputManager.instance;
    }

    void FixedUpdate() {
        MovementDetermine();
        ApplyMovement(newVelocity);
    }

    void Update() {
        CheckInput();
        CheckGround();
    }

    void MovementDetermine() {
        MoveDetermine();
        JumpDetermine();
    }

    void ApplyMovement(Vector2 bodyMoveSpeed) {
        // float time = Time.deltaTime;

        float inputMoveX = bodyMoveSpeed.x;
        float inputMoveY = bodyMoveSpeed.y;

        #region 受伤、硬直影响速度

        #endregion

        if (onGround) Debug.Log("y = 0");
        newVelocity.Set(inputMoveX, onGround ? 0 : inputMoveY);

        myBody.velocity = newVelocity;
    }

    void MoveDetermine() {
        // horizontal
        float move = onGround && yInput > 0.1f ? 0 : MOVE_SPEED * xInput;

        if (xInput * facingDirection < 0) Flip();

        newVelocity.Set(move, myBody.velocity.y);
    }

    void JumpDetermine() {
        if (jInput > 0f) {
            if (!jInputPressing) Jump(false);
            else if (jumpLong && !isJumpFall) JumpLong();

            jInputPressing = true;
        }
        else {
            if (jumpLong) {
                JumpLongFunction(false);
                newForce.Set(0, -(myBody.velocity.y / 2));
                myBody.AddForce(newForce, ForceMode2D.Impulse);
            }

            jInputPressing = false;
        }

        JumpLongCountDown();

        // 顶头
        if (onTop && newVelocity.y > 0) newVelocity.y = 0;
    }

    void Flip() {
        facingDirection *= -1;
        transform.Rotate(0, 180, 0);
    }

    void Jump(bool jumpFall) {
        if (!jumpFall) {
            if (onGround || !onGround && jumpChance > 0) {
                if (jumpChance == AIR_JUMP && !onGround) jumpChance -= 2;
                else jumpChance -= 1;

                newVelocity.Set(myBody.velocity.x, 0);
                myBody.velocity = newVelocity;

                JumpLongFunction(true);

                float jumpH = onGround ? JUMP_HEIGHT : JUMP_HEIGHT * 0.8f;
                newForce.Set(0, jumpH);
                myBody.AddForce(newForce, ForceMode2D.Impulse);

                isJumping = true;

                Debug.Log("jump");
            }
        }
        else{}

        onGround = false;
        onOneWay = false;
    }

    void JumpLong() {
        newForce.Set(0, JUMP_HEIGHT / (15f / (1 + jumpLongCountDown)));
        myBody.AddForce(newForce, ForceMode2D.Impulse);
    }

    void JumpLongCountDown() {
        if (jumpLongCountDown > 0) jumpLongCountDown -= Time.deltaTime;
        else JumpLongFunction(false);
    }

    void JumpLongFunction(bool isOn) {
        jumpLong = isOn;
        jumpLongCountDown = isOn ? 0.135f : 0;
    }


    void CheckInput() {
        if (playerInputManager.GetPlayerInput(0) < 0 && playerInputManager.GetPlayerInput(1) > 0) yInput = 0;
        else yInput = playerInputManager.GetPlayerInput(0) < 0 ? playerInputManager.GetPlayerInput(0) : playerInputManager.GetPlayerInput(1);

        if (playerInputManager.GetPlayerInput(2) < 0 && playerInputManager.GetPlayerInput(3) > 0) xInput = 0;
        else xInput = playerInputManager.GetPlayerInput(2) < 0 ? playerInputManager.GetPlayerInput(2) : playerInputManager.GetPlayerInput(3);
        jInput = playerInputManager.GetPlayerInput(10);
        sInput = playerInputManager.GetPlayerInput(11);
    }

    void CheckGround() {
        onGround = groundCheck.OnGround;
        onOneWay = groundCheck.OnOneWay;

        if (groundCheck) JumpReturnOnGround();
    }

    void JumpReturnOnGround() {
        jumpChance = AIR_JUMP;
    }
}