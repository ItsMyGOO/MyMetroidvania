using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerMove : MonoBehaviour {
    PlayerInputManager playerInputManager;
    CharacterAnimateController animateController;
    GroundSensor groundCheck;

    Rigidbody2D myBody;

    public float xInput, yInput, jInput;
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

    public bool onTop, onGround, onSlope, onOneWay;

    public int dashing;
    public int sprintAirChance;
    float sprintCooldown;
    float sprintCountdown;
    Vector2 springMoveSpeed = Vector2.zero;
    Vector2 sprintAttackStatus = Vector3.zero;
    float sprintEvasionDelay;
    float sprintEvasionCooldownDelay;
    Vector2 sprintEvasionTime = Vector2.zero;
    Vector4 sprintCdLimit = Vector4.zero;

    public bool slideShovel;
    float slideShovelMoveSpeed;
    Vector3 slideShovelDeceleration = Vector3.zero;
    Vector4 slideShovelMoveChangeDelay = Vector4.zero;
    Vector2 slideShovelCooldown = Vector2.zero;
    int slideShovelMoveStep;

    public bool onCrouchTop;

    void Awake() {
        groundCheck = GetComponentInChildren<GroundSensor>();
        myBody = GetComponent<Rigidbody2D>();
        animateController = GetComponent<CharacterAnimateController>();
    }

    void Start() {
        playerInputManager = PlayerInputManager.instance;
    }

    void FixedUpdate() {
        PlayerMove();
        animateController.AnimateBaseMove();
        ApplyMove();
    }

    void Update() {
        CheckInput();
        CheckGround();
        animateController.Animate();
    }

    void PlayerMove() {
        if (dashing <= 0) {
            MoveDetermine();
        }

        JumpDetermine();
        SprintDetermine();
    }

    void MoveDetermine() {
        Move();
    }

    void Move() {
        float moveSpeed = onGround && yInput > 0.1f ? 0 : MOVE_SPEED * xInput;
        float declineSpeed = myBody.velocity.y;

        if (slideShovel) {
            if (CrouchActionGroundLeave(-2f)) {
                SlideShovel(false);
                myBody.velocity = new Vector2(moveSpeed, -1.5f);
            }
            else {
                float getSlideShovelDecelerate = slideShovelMoveStep switch {
                    0 => slideShovelDeceleration.x,
                    1 => slideShovelDeceleration.y,
                    2 => slideShovelDeceleration.z,
                    _ => 0
                };

                Vector2 slideMove = new Vector2(slideShovelMoveSpeed, MOVE_SPEED * getSlideShovelDecelerate);
                float value = Mathf.Abs(slideMove.x) - slideMove.y;
                if (value > 0) slideShovelMoveSpeed = value * facingDirection;
                else slideShovelMoveSpeed = 0;
                moveSpeed = slideShovelMoveSpeed;

                declineSpeed = -Mathf.Abs(myBody.velocity.y);
                Vector2 velocity = myBody.velocity;
                if (velocity.y > 0) myBody.velocity = new Vector2(myBody.velocity.x, -velocity.y * 2);

                SlideShovelMoveChange();
            }
        }
        else {
            SlideShovelCooldown();
            if (xInput * facingDirection < 0) Flip();
        }

        myBody.velocity = new Vector2(moveSpeed, myBody.velocity.y);
    }

    bool CrouchActionGroundLeave(float limitY) {
        return (!onGround || !onSlope && !onOneWay) && myBody.velocity.y < limitY;
    }

    void SlideShovelCooldown() {
    }

    void SlideShovelMoveChange() {
        float delay = slideShovelMoveStep switch {
            0 => slideShovelMoveChangeDelay.y,
            1 => slideShovelMoveChangeDelay.z,
            2 => slideShovelMoveChangeDelay.w,
            _ => 0
        };

        if (slideShovelMoveChangeDelay.x < delay) slideShovelMoveChangeDelay.x += Time.deltaTime;
        else {
            slideShovelMoveChangeDelay.x = 0;
            if (slideShovelMoveStep < 3) slideShovelMoveStep += 1;
            else SlideShovel(false);
        }
    }

    void JumpDetermine() {
        if (jInput > 0f) {
            if (onGround && yInput > 0 && !slideShovel) {
                if (slideShovelCooldown.x <= 0 && !jInputPressing) SlideShovel(true);
            }
            else {
                if (!onCrouchTop) {
                    if (!jInputPressing) Jump(false);
                    else if (jumpLong && !isJumpFall) JumpLong();
                }
            }


            jInputPressing = true;
        }
        else {
            if (jumpLong) {
                // Debug.Log("jumpLongEnd");
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

    void SlideShovel(bool isOn) {
        slideShovel = isOn;
        slideShovelMoveChangeDelay.x = 0;
        slideShovelMoveStep = 0;
        if (isOn) {
            slideShovelMoveSpeed = (MOVE_SPEED * 2.6f) * facingDirection;
            slideShovelCooldown.x = slideShovelCooldown.y;
        }
    }

    void SprintDetermine() {
        if (sInput > 0) {
            if (CrouchOnTopCheck(false)) ;
            else {
                if (!sInputPressing && sprintCooldown <= 0) {
                    int sprintSkillLevel = 2;
                    if (onGround) Sprint(sprintSkillLevel, sprintSkillLevel >= 2 ? 3 : 1);
                    else if (!onGround && sprintAirChance > 0) {
                    }
                }

                sInputPressing = false;
            }
        }
        else sInputPressing = false;

        if (dashing > 0 & (springMoveSpeed.x != 0 || springMoveSpeed.y != 0)) {
            if (sprintAttackStatus.x > 0) Sprint(0, 0);

            int sas = (int) sprintAttackStatus.y;
            if (sas < 100) myBody.velocity = springMoveSpeed;
            else if (sas < 200) {
            }

            if (onGround && dashing is 2 or 4) {
                Sprint(0, 0);

                if (sas < 100) myBody.velocity = Vector2.zero;
                else if (sas < 200) dashing -= 1;
            }
            else if (CrouchActionGroundLeave(-2) && dashing is 1 or 3) {
            }
        }
        else SprintCooldown();

        SprintCountdown();
    }

    bool CrouchOnTopCheck(bool value) {
        return false;
    }

    void Sprint(int springLevel, int sprintType) {
        dashing = sprintType;
        if (sprintType <= 0) {
            sprintCountdown = 0;
            springMoveSpeed.Set(0, 0);
            sprintAttackStatus = Vector3.zero;
        }
        else {
            switch (sprintType) {
                case 1:
                    sprintCountdown = 0.4f;
                    springMoveSpeed.Set(MOVE_SPEED * 1.8f * facingDirection, myBody.velocity.y);
                    sprintEvasionDelay = sprintEvasionTime.x;
                    break;
                case 2:
                    sprintCountdown = 0.23f;
                    springMoveSpeed.Set(MOVE_SPEED * 2.9f * facingDirection, 0);
                    sprintEvasionDelay = sprintEvasionTime.y;
                    sprintAirChance -= 1;
                    break;
            }

            sprintCooldown = springLevel switch {
                0 => sprintCdLimit.x,
                1 => sprintCdLimit.y,
                2 => sprintCdLimit.z,
                3 => sprintCdLimit.w,
                _ => 0
            };
            if (!onGround) sprintCooldown *= 1.5f;

            SlideShovel(false);
        }
    }

    void SprintCooldown() {
        float time = Time.deltaTime;

        if (sprintCooldown > 0) {
            sprintCooldown -= time;
            if (sprintCooldown <= 0) Sprint(0, 0);
        }

        if (sprintEvasionDelay > 0) sprintEvasionDelay -= time;

        if (sprintEvasionCooldownDelay > 0) sprintEvasionCooldownDelay -= time;
    }

    void SprintCountdown() {
        if (sprintCooldown > 0) sprintCooldown -= Time.deltaTime;
        else sprintCooldown = 0;
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

                // Debug.Log("jump");

                myBody.velocity.Set(myBody.velocity.x, 0);

                JumpLongFunction(true);

                float jumpH = onGround ? JUMP_HEIGHT : JUMP_HEIGHT * 0.8f;

                myBody.AddForce(new Vector2(0, jumpH), ForceMode2D.Impulse);

                isJumping = true;
            }
        }
        else {
            // Debug.Log("jumpFall");
        }

        onGround = false;
        onOneWay = false;
    }

    void JumpLong() {
        // Debug.Log("long jump force");
        myBody.AddForce(new Vector2(0, JUMP_HEIGHT / (15f / (1 + jumpLongCountDown))), ForceMode2D.Impulse);
    }

    void JumpLongCountDown() {
        if (jumpLongCountDown > 0) jumpLongCountDown -= Time.deltaTime;
        else if (jumpLong) JumpLongFunction(false);
    }

    void JumpLongFunction(bool isOn) {
        // Debug.Log("jumpLongEnable " + isOn);
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