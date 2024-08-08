using System;
using UnityEngine;

public class CharacterAnimateController : MonoBehaviour {
    Animator animator;
    Animator[] animators;
    Rigidbody2D myBody;

    CharacterControllerMove moveControl;

    CapsuleCollider2D myCapsule;
    CapsuleCollider2D attributesCapsule;
    PlatformEffector2D myPlatform;

    int saveColliderNumber;
    Vector2 capsuleColliderSize;
    Vector2 attributesColliderSize;

    //***角色助击状熊***
    Vector3Int animate_Save = Vector3Int.zero; //保存现在的动画参数状态
    int animateAvatarType_Save = -999; //保存现在的助隶主頫型
    bool animateChance = false; //单次刷新"动画路线”的机会

    int animate_ActionStatus = 0; //[0]动画"主要"状态:0=站著，1=动态,2=步行，3=疾跑，4=跳曜，5=移助跳耀，6=冲刺(闪避)，10=武器技能(人形)，1
    int animate_StunStatus = 0; //[1]动画"受伤"状态:0=没有受伤，1=受伤(轻)，2=受伤(中)，3=受伤(重)，4=PV破防
    int animate_CrouchStatus = 0; //[2〕动画"动态"状态:0=蹲下，1=滑行，2=爬墙(固定)，3=爬墙(下滑)，4 =悬挂(開始)，5=悬挂(停顿)，6=悬挂(移动)，7#
    int animate_JumpStatus = 0; //[3]动画"跳跃"状态:0=没有跳跃，1=跳跃上升(JumpUp),2=跳跃下降(JumpDown) . 3=高处堕下中(Falling)，4 =高处堕下著地缓冲
    int animate_DashStatus = 0; //[4]动画"冲刺"状态∶0=没有冲刺，1=地上冲刺,2=空中冲刺
    int animate_AttackStatus = 0; //[5]动画"攻擎"状态:0=没在攻擎，1-N=攻擎步骤，50=闪避反击，+100= 冲刺攻擎步骤
    int animate_AttackSubStatus = 0; //[6]动画"攻擎阶段"状态:>0=攻擎阶段进行中

    void Awake() {
        animator = GetComponent<Animator>();
        animators = GetComponentsInChildren<Animator>();
        myBody = GetComponent<Rigidbody2D>();
        myCapsule = GetComponent<CapsuleCollider2D>();
        // attributesCapsule = transform.GetChild(0).GetComponent<CapsuleCollider2D>();

        moveControl = GetComponent<CharacterControllerMove>();
    }

    void Start() {
        animateAvatarType_Save = 0;
    }

    public void AnimateBaseMove() {
        var move = myBody.velocity;
        int typeAction = animate_Save.x;
        int mainAction = animate_Save.y;
        int subAction = animate_Save.z;

        if (moveControl.onGround) {
            if (moveControl.dashing > 0) {
                mainAction = 6;
                subAction = moveControl.dashing;
            }
            else if (move.x != 0) {
                if (moveControl.slideShovel) {
                    mainAction = 1;
                    subAction = 1;
                }
                else {
                    mainAction = 2;
                }
            }
            else {
                if (moveControl.slideShovel) {
                }
                else if (moveControl.yInput > 0) {
                }
                else mainAction = 0;
            }
        }
        else if (Mathf.Abs(move.y) > 0.1f || moveControl.dashing > 0) {
        }

        typeAction = mainAction switch {
            1 => 1,
            4 or 5 => 2,
            6 => 3,
            < 4 => 0,
            _ => typeAction
        };

        if (animate_Save.x != typeAction || animate_Save.y != mainAction || animate_Save.z != subAction) {
            AnimateStatus(typeAction, mainAction, subAction);

            if (animate_AttackStatus <= 0 || animateChance) AnimateRefresh();
        }
    }

    void AnimateStatus(int type, int mainStatus, int subStatus) {
        animate_ActionStatus = mainStatus;

        switch (type) {
            case 0:
                animate_StunStatus = 0;
                break;
            case 1:
                animate_CrouchStatus = subStatus;
                break;
            case 2:
                animate_JumpStatus = subStatus;
                break;
            case 3:
                animate_DashStatus = subStatus;
                break;
            case 4:
                animate_AttackStatus = subStatus;
                break;
            case 99:
                animate_StunStatus = subStatus;
                break;
        }

        animate_Save = new Vector3Int(type, mainStatus, subStatus);

        int actionNumber = 0;
        if (animate_ActionStatus == 1 && animate_CrouchStatus < 2) actionNumber = 1;

        int avatarNumber = animateAvatarType_Save * 100;
        int totalColliderNumber = actionNumber + avatarNumber;
        SetCollider(totalColliderNumber);
    }

    void AnimateRefresh() {
        animateChance = false;
        animator.Play(AnimateAvatarName(animateAvatarType_Save));
    }

    string AnimateAvatarName(int avatar) {
        string avatarName = avatar switch {
            0 => "Human",
            1 => "AvatarA",
            2 => "AvatarB",
            3 => "AvatarC",
            4 => "Awake",
            _ => string.Empty
        };
        return avatarName;
    }

    void SetCollider(int newType) {
        if (saveColliderNumber == newType)
            return;

        saveColliderNumber = newType;
        Vector2 zero = Vector2.zero;
        switch (newType) {
            case 0:
                myCapsule.size = new Vector2(0.56f, 1.2f);
                myCapsule.offset = zero;
                myCapsule.direction = CapsuleDirection2D.Vertical;
                break;
            case 1:
                myCapsule.size = new Vector2(0.69f, 0.53f);
                myCapsule.offset = new Vector2(0, -0.3f);
                myCapsule.direction = CapsuleDirection2D.Horizontal;
                break;
            case 2:
                break;
        }

        capsuleColliderSize = myCapsule.size;
        // attributesColliderSize = attributesCapsule.size;
    }

    public void Animate() {
        foreach (var anim in animators) {
            anim.SetBool(_onGround, moveControl.onGround);
            anim.SetInteger(_avatarStatus, animateAvatarType_Save);
            anim.SetInteger(_actionStatus, animate_ActionStatus);
            anim.SetInteger(_stunStatus, animate_StunStatus);
            anim.SetInteger(_crouchStatus, animate_CrouchStatus);
            anim.SetInteger(_jumpStatus, animate_JumpStatus);
            anim.SetInteger(_dashStatus, animate_DashStatus);
            anim.SetInteger(_attackStatus, animate_AttackStatus);
            anim.SetInteger(_attackSubStatus, animate_AttackSubStatus);
        }
    }

    static readonly int _onGround = Animator.StringToHash("OnGround");
    static readonly int _avatarStatus = Animator.StringToHash("AvatarStatus");
    static readonly int _actionStatus = Animator.StringToHash("ActionStatus");
    static readonly int _stunStatus = Animator.StringToHash("StunStatus");
    static readonly int _crouchStatus = Animator.StringToHash("CrouchStatus");
    static readonly int _jumpStatus = Animator.StringToHash("JumpStatus");
    static readonly int _dashStatus = Animator.StringToHash("DashStatus");
    static readonly int _attackStatus = Animator.StringToHash("AttackStatus");
    static readonly int _attackSubStatus = Animator.StringToHash("AttackSubStatus");
}