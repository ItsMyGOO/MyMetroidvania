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
    const float JUMP_HEIGHT = 10f;
    const int AIR_JUMP = 3;

    bool isJumping, isJumpFall;
    int jumpChance;
    bool jumpLong;
    float jumpLongCountDown;

    bool onTop, onGround, onOneWay;

    GroundSensor groundCheck;

    void Awake() {
        groundCheck = GetComponentInChildren<GroundSensor>();
        myBody = GetComponent<Rigidbody2D>();
    }

    void Start() {
        playerInputManager = PlayerInputManager.instance;
    }

    void FixedUpdate() {
        PlayerMove();
        ApplyMove();
    }

    void Update() {
        CheckInput();
        CheckGround();
    }

    void PlayerMove() {
        MoveDetermine();
        JumpDetermine();
    }

    void MoveDetermine() {
        // horizontal
        float move = onGround && yInput > 0.1f ? 0 : MOVE_SPEED * xInput;

        if (xInput * facingDirection < 0) Flip();

        myBody.velocity = new Vector2(move, myBody.velocity.y);
    }

    void JumpDetermine() {
        if (jInput > 0f) {
            if (!jInputPressing) Jump(false);
            else if (jumpLong && !isJumpFall) JumpLong();

            jInputPressing = true;
        }
        else {
            if (jumpLong) {
                Debug.Log("jumpLongEnd");
                JumpLongFunction(false);
                myBody.AddForce(new Vector2(0, -(myBody.velocity.y / 2)), ForceMode2D.Impulse);
            }

            jInputPressing = false;
            isJumping = false;
        }

        JumpLongCountDown();

        // 顶头
        if (onTop && myBody.velocity.y > 0) myBody.velocity.Set(myBody.velocity.x, 0);
    }

    void ApplyMove() {
        float time = Time.deltaTime;

        var moveSpeed = myBody.velocity;

        #region 受伤、硬直影响速度

        #endregion

        // todo 跳上平台的时候导致骤停
        if (onGround) {
            // Debug.Log("y = 0");
            moveSpeed.y = 0;
        }

        myBody.velocity = moveSpeed;
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

                Debug.Log("jump");

                myBody.velocity.Set(myBody.velocity.x, 0);

                JumpLongFunction(true);

                float jumpH = onGround ? JUMP_HEIGHT : JUMP_HEIGHT * 0.8f;

                myBody.AddForce(new Vector2(0, jumpH), ForceMode2D.Impulse);

                isJumping = true;
            }
        }
        else {
            Debug.Log("jumpFall");
        }

        onGround = false;
        onOneWay = false;
    }

    void JumpLong() {
        Debug.Log("long jump force");
        myBody.AddForce(new Vector2(0, JUMP_HEIGHT / (15f / (1 + jumpLongCountDown))), ForceMode2D.Impulse);
    }

    void JumpLongCountDown() {
        if (jumpLongCountDown > 0) jumpLongCountDown -= Time.deltaTime;
        else if (jumpLong) JumpLongFunction(false);
    }

    void JumpLongFunction(bool isOn) {
        Debug.Log("jumpLongEnable " + isOn);
        jumpLong = isOn;
        jumpLongCountDown = isOn ? 0.135f : 0;

        if (!isOn) isJumping = false;
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
        if (myBody.velocity.y > 0) return;

        onGround = groundCheck.OnGround;
        onOneWay = groundCheck.OnOneWay;

        if (onGround) JumpReturnOnGround();
    }

    void JumpReturnOnGround() {
        jumpChance = AIR_JUMP;
    }
}