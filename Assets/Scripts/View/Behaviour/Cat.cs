using Spine.Unity;
using UnityEngine;
using View.Behaviour;

public class Cat : MonoBehaviour
{
    [SerializeField] [LunaPlaygroundField("Cat Speed", 0, "Gameplay Adjustment")]private float speed = 2f;
    [SerializeField] private CatType catType;
    
    private int _registeredFingerId = -1;
    private SkeletonAnimation _skeletonAnimation;
    
    // ----------- Animation Constants ---------
    private const string CheerAnim = "Cheering_Happy";
    private const string MissAppeaseAnim = "Miss_Appease";
    private const string MissObjectAnim = "Miss_Object";
    private const string MissObjectLoseAnim = "Miss_Object_Lose";
    private const string MissObjectLoseAnim2 = "Miss_Object_Lose_2";
    
    private const string IdleHungryAnim = "Idle_Hungry";
    private const string IdleLickAnim = "Idle_Lick";
    private const string IdlePlayAnim = "Idle_Playing";
    private const string IdleStartAnim = "Idle_Start";
    private const string IdleYawnAnim = "Idle_Yawn";
    private const string IdleListenAnim = "Listening";
    private const string IdleTailAnim = "Tail";
    
    private const string EatingAnim = "Eating";
    private const string EatingSingleAnim = "Eating_Single_Object";
    private const string EatingSingleAnim2 = "Eating_Single_Object_2";
    private const string EatLongBeginAnim = "Eat_Long_Begin";
    private const string EatLongLoopAnim = "Eat_Long_Loop";
    private const string EatLongEndAnim = "Eat_Long_End";
    private const string EatShotAnim = "Eat_Shot";
    
    private void Awake()
    {
        _skeletonAnimation = GetComponent<SkeletonAnimation>();
    }
    
    private void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            if (_registeredFingerId == -1 &&
                touch.phase == TouchPhase.Began &&
                IsOnCatSide(touch.position))
            {
                _registeredFingerId = touch.fingerId;
            }

            if (touch.fingerId != _registeredFingerId)
                continue;

            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                MoveCat(touch.deltaPosition.x);

            if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                _registeredFingerId = -1;

            break;
        }
    }

    private bool IsOnCatSide(Vector2 touchPosition)
    {
        bool isLeftSide = touchPosition.x < Screen.width * 0.5f;
        return catType == CatType.Left && isLeftSide ||
               catType == CatType.Right && !isLeftSide;
    }

    private void MoveCat(float deltaX)
    {
        Vector3 position = transform.position;
        position.x += deltaX * speed * Time.deltaTime;
        UpdateFacing(deltaX);

        if (Environment.Instance != null &&
            Environment.Instance.GetPlatformBounds(catType, out Bounds platformBounds))
        {
            float halfWidth = GetComponent<Renderer>()?.bounds.extents.x ?? 0f;
            position.x = Mathf.Clamp(
                position.x,
                platformBounds.min.x + halfWidth,
                platformBounds.max.x - halfWidth);
        }

        transform.position = position;
    }

    private void UpdateFacing(float deltaX)
    {
        if (Mathf.Approximately(deltaX, 0f))
            return;

        transform.localRotation = Quaternion.Euler(
            0f,
            deltaX < 0f ? 180f : 0f,
            0f);
    }
    
    
}

public enum CatType
{
    Left,
    Right
}

public enum CatAnimation
{
    
}
