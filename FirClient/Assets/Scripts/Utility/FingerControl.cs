using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum FingerState
{
    NONE,//无状态
    CLICK,//单击
    LONG,//长按
    LONGWALK,//技能移动
    MOVE_CONTINUE_WALK,//行走
    MOVE_CONTINUR_RUN,//奔跑
}
/// <summary>
/// 手势控制
/// </summary>
public class FingerControl : GameBehaviour
{
    /// <summary>
    /// 摇杆骨骼1
    /// </summary>
    public Transform m_joketFirst = null;
    /// <summary>
    /// 摇杆拖动骨骼2
    /// </summary>
    public Transform m_joketTwo = null;
    /// <summary>
    /// 拖动点位置特效
    /// </summary>
    public GameObject touchLight = null;

    public bool m_isDown = false;

    public Vector3 oldPostion = Vector3.zero;
    public Vector3 newPostion = Vector3.zero;

    public Camera uiCamera;

    /// <summary>
    /// 保持水纹的圆形强制让m_joketTwo在固定的localPostion
    /// </summary>
    public Vector3 m_joketPostion = Vector3.zero;

    private float invertTime = 0f;   //按下持续时间

    /// <summary>
    /// 表示的是否触发显示水纹[点击特效]
    /// </summary>
    private bool isMoveJointTwo = false;

    private bool isDown = false;//是否按下
    private bool isLone = false;//长按技能状态
    private bool isMoving = false;//长按移动状态

    private bool isAttack = false;//正在攻击
    private bool isSkilling = false;//正在施放技能
    private bool isReadySkill = false;//准备好释放技能

    public float TouchUpInvertTime = 0f;//弹起的持续时间

    //连击时间阀值
    public float battleClickBoundingTime = 0.2f;
    public float battleClickBoundingLength = 50f;

    public float walkBoundingLength = 80f;//行走半径阈值
    public float rollBoundingLength = 50f;//翻滚半径阈值

    public float longPressSkillBoundingLength = 30f;//长按技能半径触发阈值

    public float longPressSkillBoundingTime = 0.5f;//满足技能触发所需要的时间
    public float longPressSkillWalkBoundingLengh = 40f;

    public FingerState fstate = FingerState.NONE;

    public GameObject testObj;
    public Canvas canvas;

    private void Awake()
    {
        m_joketPostion = m_joketTwo.localPosition;

        //testOne.SetActive(false);
        //testTwo.SetActive(false);
        //canvasScaler = GameObject.Find("Canvas").gameObject.GetComponent<CanvasScaler>();
        //float resolutionX = canvasScaler.referenceResolution.x;
        //float resolutionY = canvasScaler.referenceResolution.y;
        //float offect = (Screen.width / canvasScaler.referenceResolution.x) * (1 - canvasScaler.matchWidthOrHeight) + (Screen.height / canvasScaler.referenceResolution.y) * canvasScaler.matchWidthOrHeight;
        //Vector2 a = RectTransformUtility.WorldToScreenPoint(Camera.main, plant[index].transform.GetChild(0).transform.position);
        //return new Vector3(a.x / offect, a.y / offect, 0);
    }


    void Update()
    {
        //Debug.Log(fstate);
        if (GameControl.Input.TouchCount > 1)
            return;
        if (GameControl.Input.IsClickDown)
        {
            oldPostion = GameControl.Input.MousePosition;
            m_isDown = true;

            isMoving = false;
            isLone = false;
            TouchUpInvertTime = 0f;
            ShowFingerClick(oldPostion);
        }

        if (GameControl.Input.IsClickUp)
        {
            if (isMoveJointTwo)
            {
                CloseFingerClick();
            }
            m_isDown = false;
            isAttack = false;
            isSkilling = false;
            fstate = FingerState.NONE;
            newPostion = GameControl.Input.MousePosition;
            //invertTime = 0f;
        }
        if (m_isDown)
        {
            invertTime += Time.deltaTime;
            newPostion = GameControl.Input.MousePosition;
            touchLight.SetActive(true);
            touchLight.transform.position = uiCamera.ScreenToWorldPoint(newPostion);
            if (invertTime > battleClickBoundingTime)//当按下的时间大于连击阈值的时候
            {
                if (!isLone && Vector3.Distance(oldPostion, newPostion) > longPressSkillBoundingLength && !isSkilling)
                {
                    if (Vector3.Distance(oldPostion, newPostion) > walkBoundingLength)
                    {
                        fstate = FingerState.MOVE_CONTINUR_RUN;
                    }
                    else if (Vector3.Distance(oldPostion, newPostion) <= walkBoundingLength && Vector3.Distance(oldPostion, newPostion) > 30f && !isAttack && !isSkilling)
                    {
                        fstate = FingerState.MOVE_CONTINUE_WALK;
                    }
                    isMoving = true;
                }
                else if (Vector3.Distance(oldPostion, newPostion) <= longPressSkillBoundingLength && !isMoving && !isAttack && !isSkilling)
                {
                    if (invertTime >= longPressSkillBoundingTime)
                    {
                        fstate = FingerState.LONG;
                        isLone = true;
                    }
                }
            }
            if (isLone && Vector3.Distance(oldPostion, newPostion) > longPressSkillWalkBoundingLengh && !isAttack && !isSkilling)
            {
                fstate = FingerState.LONGWALK;
            }
            else if (fstate == FingerState.LONGWALK && Vector3.Distance(oldPostion, newPostion) < longPressSkillBoundingLength && !isSkilling)
            {
                fstate = FingerState.LONG;
            }
        }
        else
        {
            TouchUpInvertTime += Time.deltaTime;
            if (TouchUpInvertTime >= 0.3f)
            {
                CloseDuaationEffect();
            }
        }


        if (GameControl.Input.IsClickUp)
        {
            if (invertTime < battleClickBoundingTime && Vector3.Distance(oldPostion, newPostion) < battleClickBoundingLength && !isSkilling)
            {
                fstate = FingerState.CLICK;
                ShowDuarrationAttack(oldPostion);//展示手势攻击特效
                isAttack = true;
            }
            if (isReadySkill)
            {
                ChoiceSkill(oldPostion, newPostion);
            }
            invertTime = 0f;
            isReadySkill = false;
        }
    }

    void FixedUpdate()
    {
        switch (fstate)
        {
            case FingerState.NONE:
                break;
            case FingerState.CLICK:
                Debug.Log("鼠标点击=====>>>攻击状态");

                break;
            case FingerState.LONG:
                Debug.Log("鼠标长按=====>>>提示释放技能状态");
                isReadySkill = true;
                //testOne.SetActive(true);
                //testTwo.SetActive(true);
                //testOne.transform.position = uiCamera.ScreenToWorldPoint(oldPostion);
                //testTwo.transform.position = uiCamera.ScreenToWorldPoint(oldPostion);
                Vector2 pos;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, oldPostion, uiCamera, out pos))
                {
                    Debug.Log("pos==================" + pos);
                    //testOne.transform.position = pos;
                }

                break;
            case FingerState.LONGWALK:
                PosFingerEffect();
                GetAngleByMouseTouch(newPostion, oldPostion);
                Debug.Log("长按技能行走状态");
                isReadySkill = true;

                break;
            case FingerState.MOVE_CONTINUE_WALK:
                PosFingerEffect();
                GetAngleByMouseTouch(newPostion, oldPostion);
                Debug.Log("人物正在行走");
                //testObj.transform.Translate (0,0.5f,0);
                break;
            case FingerState.MOVE_CONTINUR_RUN:
                PosFingerEffect();
                GetAngleByMouseTouch(newPostion, oldPostion);
                Debug.Log("人物正在奔跑");
                //testObj.transform.Translate (0,1f,0);
                break;
            default:
                isMoving = false;

                break;
        }
        if (fstate == FingerState.CLICK)
        {
            fstate = FingerState.NONE;
            isAttack = false;
        }
    }
    /// <summary>
    /// 设置点击效果
    /// </summary>
    /// <param name="clickPos"></param>
    private void ShowFingerClick(Vector3 clickPos)
    {
        isMoveJointTwo = true;
        Vector3 pos = uiCamera.ScreenToWorldPoint(clickPos);
        m_joketFirst.transform.position = pos;
        touchLight.transform.position = pos;
        m_joketTwo.localPosition = m_joketPostion;
        m_joketFirst.gameObject.SetActive(true);
        touchLight.SetActive(true);
    }
    /// <summary>
    /// 拖动谷歌点及拖动特效位置
    /// </summary>
    public void PosFingerEffect()
    {
        isMoveJointTwo = true;
        Vector3 touchPos = uiCamera.ScreenToWorldPoint(newPostion);
        Vector3 joint2 = uiCamera.ScreenToWorldPoint(oldPostion);
        touchLight.transform.position = touchPos;
        touchLight.transform.position = uiCamera.ScreenToWorldPoint(newPostion);
        if ((touchPos - joint2).magnitude > 1f)
            m_joketTwo.position = uiCamera.ScreenToWorldPoint(newPostion);
        else
            m_joketTwo.localPosition = m_joketPostion;
    }
    private void CloseFingerClick()
    {
        m_joketFirst.gameObject.SetActive(false);
        touchLight.SetActive(false);
        isMoveJointTwo = false;
    }
    /// <summary>
    /// 隐藏拖动点特效
    /// </summary>
    private void CloseDuaationEffect()
    {
        touchLight.SetActive(false);
    }
    /// <summary>
    /// 设置拖动骨骼拉伸方向
    /// </summary>
    /// <param name="newPostion"></param>
    /// <param name="oldPostion"></param>
    /// <returns></returns>
    public float GetAngleByMouseTouch(Vector3 newPostion, Vector3 oldPostion)
    {
        Vector3 move_vector = newPostion - oldPostion;
        Vector3 dir = move_vector.normalized;
        //PlayerActionSystem.instance.directionNormal = new Vector3(dir.x, 0, dir.y);
        float angle = Vector2.Angle(dir, Vector2.up);
        if (dir.x > 0)
            angle = 360 - angle;
        //         target.transform.rotation = Quaternion.identity;
        //         target.transform.Rotate(new Vector3(0, -angle, 0));
        m_joketFirst.eulerAngles = new Vector3(-90, 0, 0);
        m_joketFirst.Rotate(new Vector3(0, -angle + 90, 0));

        //testObj.transform.eulerAngles = new Vector3 (0, 0, angle);

        //testObj.transform.Rotate (new Vector3(0, 0, -angle + 90));

        return 0;
    }
    /// <summary>
    /// 展现攻击特效
    /// </summary>
    /// <param name="newPostion"></param>
    private void ShowDuarrationAttack(Vector3 oldPostion)
    {
        Vector3 pos = uiCamera.ScreenToWorldPoint(newPostion);
        touchLight.transform.position = pos;
        m_joketFirst.gameObject.SetActive(false);
        touchLight.SetActive(true);

    }
    /// <summary>
    /// 通过计算拖拽角度判断释放技能
    /// </summary>
    /// <param name="oldPostion"></param>
    /// <param name="newPostion"></param>
    private void ChoiceSkill(Vector2 oldPostion, Vector2 newPostion)
    {
        float a = Vector2.Angle(Vector2.up, (newPostion - oldPostion));
        Vector2 nor = (newPostion - oldPostion).normalized;
        float mo = nor.magnitude;
        if (mo < 3f && a > 20 && a < 50)
        {
            isSkilling = true;
            if (nor.x < 0f)
            {
                Debug.Log("施放技能1");
                isSkilling = false;
            }
            else
            {
                Debug.Log("施放技能2");
                isSkilling = false;
            }
        }


    }
}
