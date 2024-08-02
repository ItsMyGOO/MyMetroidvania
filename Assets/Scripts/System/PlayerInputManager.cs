using Keyboard.Framework;
using Unity.Mathematics;
using UnityEngine;

// ReSharper disable InconsistentNaming

public class PlayerInputManager : MonoBehaviour {
    public static PlayerInputManager instance;

    KeyCode[] saveKeyCode_Keyboard = new KeyCode[20];
    KeyCode[] saveKeyCode_Controller = new KeyCode[20];
    int[] saveID_Keyboard = new int[20];
    int[] saveID_Controller = new int[20];

    Vector2Int nowInput = Vector2Int.zero; //正在输入的设备:X=[0]键盘,[1]控制器,Y=控制器类型

    //---------------输入情况（反映值/状您)
    [SerializeField] private float playerMoveU_Float;
    [SerializeField] private float playerMoveD_Float;
    [SerializeField] private float playerMoveL_Float;
    [SerializeField] private float playerMoveR_Float;
    [SerializeField] private float playerCameraU_Float;
    [SerializeField] private float playerCameraD_Float;
    [SerializeField] private float playerCameraL_Float;
    [SerializeField] private float playerCameraR_Float;
    [SerializeField] private float playerStart_Float;

    [SerializeField] private float playerJump_Float;
    [SerializeField] private float playerSprint_Float;
    [SerializeField] private float playerAttackA_Float;
    [SerializeField] private float playerAttackB_Float;
    [SerializeField] private float playerAttackC_Float;

    [SerializeField] private float playerSkillShortcut_Float;

    [SerializeField] private float controllerDPad_InMenuX_Float;
    [SerializeField] private float controllerDPad_InMenuY_Float;

    //-----------------------------------------------------------------------
    bool keyDetect;

    bool2 ssKeyPress; //重要功能按键状态:X=開始键(Select),Y=地图键(Map)

    void Awake() {
        instance = this;

        Invoke(nameof(DelayRun), 0.25f);
    }

    void DelayRun() {
        keyDetect = true;
    }

    void Update() {
        if (keyDetect) {
            //角色方向:左右上下
            playerMoveU_Float = CheckKeyInput(0) ? -1 : 0;
            playerMoveD_Float = CheckKeyInput(1) ? 1 : 0;
            playerMoveL_Float = CheckKeyInput(2) ? -1 : 0;
            playerMoveR_Float = CheckKeyInput(3) ? 1 : 0;
            //镜颐方向:左右上下
            playerCameraU_Float = CheckKeyInput(4) ? -1 : 0;
            playerCameraD_Float = CheckKeyInput(5) ? 1 : 0;
            playerCameraL_Float = CheckKeyInput(6) ? -1 : 0;
            playerCameraR_Float = CheckKeyInput(7) ? 1 : 0;
            //菜罩:開始菜罪
            playerStart_Float = CheckKeyInput(8) ? 1 : 0;
            // ssKeyPress.x = CheckKeyInput (8);
            //位移:跳趾or衙刺
            playerJump_Float = CheckKeyInput(10) ? 1 : 0;
            playerSprint_Float = CheckKeyInput(11) ? 1 : 0;
            //攻擎:右、左、副
            playerAttackA_Float = CheckKeyInput(12) ? 1 : 0;
            playerAttackB_Float = CheckKeyInput(13) ? 1 : 0;
            playerAttackC_Float = CheckKeyInput(14) ? 1 : 0;
            //模式键:技能魔法、化身(按键反聘Z)
            playerSkillShortcut_Float = CheckKeyInput(17) ? 1 : 0;
        }
    }

    bool 
        
        CheckKeyInput(int functionType) {
        bool isInput = false;
        int controllerType = 0;
        //暂畤固定为键盘
        switch (functionType) {
            //[0]移助:上
            case 0:
                KeyCode k_MoveUp = saveKeyCode_Keyboard[functionType] != KeyCode.None ? saveKeyCode_Keyboard[functionType] : KeyBoardInput.MoveUp;
                if (Input.GetKey(k_MoveUp)) {
                    isInput = true;
                    nowInput = new Vector2Int(Input.GetKey(k_MoveUp) ? 0 : 1, controllerType);
                }

                break;
            //[1]移勤:下
            case 1:
                KeyCode k_MoveDown = saveKeyCode_Keyboard[functionType] != KeyCode.None ? saveKeyCode_Keyboard[functionType] : KeyBoardInput.MoveDown;
                if (Input.GetKey(k_MoveDown)) {
                    isInput = true;
                    nowInput = new Vector2Int(Input.GetKey(k_MoveDown) ? 0 : 1, controllerType);
                }

                break;
            //[2]移助:左
            case 2:
                KeyCode k_MoveLeft = saveKeyCode_Keyboard[functionType] != KeyCode.None ? saveKeyCode_Keyboard[functionType] : KeyBoardInput.MoveLeft;
                if (Input.GetKey(k_MoveLeft)) {
                    isInput = true;
                    nowInput = new Vector2Int(Input.GetKey(k_MoveLeft) ? 0 : 1, controllerType);
                }

                break;
            //[3]移助:右
            case 3:
                KeyCode k_MoveRight = saveKeyCode_Keyboard[functionType] != KeyCode.None ? saveKeyCode_Keyboard[functionType] : KeyBoardInput.MoveRight;
                if (Input.GetKey(k_MoveRight)) {
                    isInput = true;
                    nowInput = new Vector2Int(Input.GetKey(k_MoveRight) ? 0 : 1, controllerType);
                }

                break;
            //[4]统头:上
            case 4:
                KeyCode k_CameraUp = saveKeyCode_Keyboard[functionType] != KeyCode.None ? saveKeyCode_Keyboard[functionType] : KeyBoardInput.CameraUp;
                if (Input.GetKey(k_CameraUp)) {
                    isInput = true;
                    nowInput = new Vector2Int(Input.GetKey(k_CameraUp) ? 0 : 1, controllerType);
                }

                break;
            //[5]镜头:下
            case 5:
                KeyCode k_CameraDown = saveKeyCode_Keyboard[functionType] != KeyCode.None ? saveKeyCode_Keyboard[functionType] : KeyBoardInput.CameraDown;
                if (Input.GetKey(k_CameraDown)) {
                    isInput = true;
                    nowInput = new Vector2Int(Input.GetKey(k_CameraDown) ? 0 : 1, controllerType);
                }

                break;
            //[6]镜要:左
            case 6:
                KeyCode k_CameraLeft = saveKeyCode_Keyboard[functionType] != KeyCode.None ? saveKeyCode_Keyboard[functionType] : KeyBoardInput.CameraLeft;
                if (Input.GetKey(k_CameraLeft)) {
                    isInput = true;
                    nowInput = new Vector2Int(Input.GetKey(k_CameraLeft) ? 0 : 1, controllerType);
                }

                break;
            //[7]镜要:右
            case 7:
                KeyCode k_CameraRight = saveKeyCode_Keyboard[functionType] != KeyCode.None ? saveKeyCode_Keyboard[functionType] : KeyBoardInput.CameraRight;
                if (Input.GetKey(k_CameraRight)) {
                    isInput = true;
                    nowInput = new Vector2Int(Input.GetKey(k_CameraRight) ? 0 : 1, controllerType);
                }

                break;
            //主菜单
            case 8:
                KeyCode k_Start = saveKeyCode_Keyboard[functionType] != KeyCode.None ? saveKeyCode_Keyboard[functionType] : KeyBoardInput.Start;
                if (Input.GetKey(k_Start)) {
                    isInput = true;
                    nowInput = new Vector2Int(Input.GetKey(k_Start) ? 0 : 1, controllerType);
                }

                break;
            //
            case 10:
                KeyCode k_Jump = saveKeyCode_Keyboard[functionType] != KeyCode.None ? saveKeyCode_Keyboard[functionType] : KeyBoardInput.Jump;
                if (Input.GetKey(k_Jump)) {
                    isInput = true;
                    nowInput = new Vector2Int(Input.GetKey(k_Jump) ? 0 : 1, controllerType);
                }

                break;
            //闪避
            case 11:
                KeyCode k_Sprint = saveKeyCode_Keyboard[functionType] != KeyCode.None ? saveKeyCode_Keyboard[functionType] : KeyBoardInput.Sprint;
                if (Input.GetKey(k_Sprint)) {
                    isInput = true;
                    nowInput = new Vector2Int(Input.GetKey(k_Sprint) ? 0 : 1, controllerType);
                }

                break;
            //攻馨:右
            case 12:
                KeyCode k_AttackA = saveKeyCode_Keyboard[functionType] != KeyCode.None ? saveKeyCode_Keyboard[functionType] : KeyBoardInput.AttackA;
                if (Input.GetKey(k_AttackA)) {
                    isInput = true;
                    nowInput = new Vector2Int(Input.GetKey(k_AttackA) ? 0 : 1, controllerType);
                }

                break;
            //攻歌: 左
            case 13:
                KeyCode k_AttackB = saveKeyCode_Keyboard[functionType] != KeyCode.None ? saveKeyCode_Keyboard[functionType] : KeyBoardInput.AttackB;
                if (Input.GetKey(k_AttackB)) {
                    isInput = true;
                    nowInput = new Vector2Int(Input.GetKey(k_AttackB) ? 0 : 1, controllerType);
                }

                break;
            //攻擎:副
            case 14:
                KeyCode k_AttackC = saveKeyCode_Keyboard[functionType] != KeyCode.None ? saveKeyCode_Keyboard[functionType] : KeyBoardInput.AttackC;
                if (Input.GetKey(k_AttackC)) {
                    isInput = true;
                    nowInput = new Vector2Int(Input.GetKey(k_AttackC) ? 0 : 1, controllerType);
                }

                break;
            //技能
            case 17:
                KeyCode k_Skill = saveKeyCode_Keyboard[functionType] != KeyCode.None ? saveKeyCode_Keyboard[functionType] : KeyBoardInput.Skill;
                if (Input.GetKey(k_Skill)) {
                    isInput = true;
                    nowInput = new Vector2Int(Input.GetKey(k_Skill) ? 0 : 1, controllerType);
                }

                break;
        }

        return isInput;
    }

    public float GetPlayerInput(int position) {
        float returnFloat = 0f;
        switch (position) {
            case 0:
                returnFloat = playerMoveU_Float;
                break; //[0]取得"方向:上”的反底值
            case 1:
                returnFloat = playerMoveD_Float;
                break; //[1]取得"方向:下”的反愿值
            case 2:
                returnFloat = playerMoveL_Float;
                break; //[2] 取得"方向:左”的反恋值
            case 3:
                returnFloat = playerMoveR_Float;
                break; //[3] 取得"方向:右”的反愿值
            case 4:
                returnFloat = playerCameraU_Float;
                break; //[4]取得"镜头方向:上”的反鹰值
            case 5:
                returnFloat = playerCameraD_Float;
                break; //[5]取得"镜头方向:下"的反恋值
            case 6:
                returnFloat = playerCameraL_Float;
                break; //[6]取得"镜瞋方向:左”的反愿值
            case 7:
                returnFloat = playerCameraR_Float;
                break; //[7]取得“镜头方向:右”的反愿值
            case 8:
                returnFloat = playerStart_Float;
                break; //[8] 取得"菜罪:開始菜覃"的反愿值
            case 10:
                returnFloat = playerJump_Float;
                break; //[10]取得"位移:跳濯”的反愿值
            case 11:
                returnFloat = playerSprint_Float;
                break; //[11]取得"位移:街刺"的反鹰值
            case 12:
                returnFloat = playerAttackA_Float;
                break; //[12]取得"攻擎:右"的反鹰值
            case 13:
                returnFloat = playerAttackB_Float;
                break; //[13]取得"攻擎:左”的反磨值
            case 14:
                returnFloat = playerAttackC_Float;
                break; //[14]取得"攻馨:副”的反底值
            case 17:
                returnFloat = playerSkillShortcut_Float;
                break; //[17]取得"模式键:技能”的反愿值
            case 50:
                returnFloat = controllerDPad_InMenuX_Float;
                break; // [50]取得"控制器"十字键X轴的反鹰值
            case 51:
                returnFloat = controllerDPad_InMenuY_Float;
                break; //[51]取得"控制器”十字键Y轴的反愿值
        }

        return returnFloat;
    }
}