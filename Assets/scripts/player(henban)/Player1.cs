using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player1 : entity
{
    public bool isdead=false;


    [SerializeField] public Transform summoncheck;
    [SerializeField] private float colorlosingspeed;
    private SpriteRenderer sr;
    
    [Header("Attack details")]
    public float[] Attackmovement;
    public float counterattackduration;
   

    [Header("Speed info")]
    
    public float jumpforce;

    [Header("Dash info")]
    public float dashspeed;
    public float dashduration;
    public bool isdashing=false;
    public bool canclone;
    public bool candoublejump=true;
    public bool isWallDashing = false;
    private float dashtimer;
    public int wallDashDirection;           // 墙壁冲刺方向 (-1 或 1)
    public float wallDashDuration = 0.2f;   // 墙壁冲刺持续时间
    public float wallDashSpeedH = 15f;      // 墙壁冲刺水平速度
    public float wallDashSpeedV = 12f;      // 墙壁冲刺垂直速度
    [Header("Wall Bounce Settings - 墙壁反弹")]
    public float wallBounceSpeedH = 10f;   
    public float wallBounceSpeedV = 12f;
    public bool cancontrol = true;
    float dietime = 5f;
    public float dashdirection { get; private set; }
   
    #region Conponents

    public Playerstatemachine statemachine { get; private set; }
    public Playeridlestate idlestate { get; private set; }
    public Playermovestate movestate { get; private set; }
    public Playerjumpstate jumpstate { get; private set; }
    public Playerairstate airstate { get; private set; }
    public Playerdashstate dashstate { get; private set; }
    public Playerwallslidstate wallslide { get; private set; }
    public Playerwalljump walljump { get; private set; }
    #endregion
    public Playerprimaryattack primaryattack { get; private set; }
    public Playerhitstate hitstate { get; private set; }
    public Playercounterattackstate counterattack { get; private set; }
    public Playeraimswordstate aimsword { get; private set; }
    public playercatchswordstate catchsword { get; private set; }
    public Playerdiestate diestate { get; private set; }
    public Plyersummonswordstate summonstate { get; private set; }

    public string lastanimboolname { get; private set; }
    public float jumpchecktime = 0.1f;
    

    protected override void Awake()
    {
        base.Awake();
        statemachine = new Playerstatemachine();
        idlestate = new Playeridlestate(this, statemachine, "Idle");
        movestate = new Playermovestate(this, statemachine, "Move");
        jumpstate=  new Playerjumpstate(this, statemachine, "Jump");
        airstate=new Playerairstate(this, statemachine, "Jump");
        dashstate = new Playerdashstate(this, statemachine, "Dash");
        wallslide = new Playerwallslidstate(this, statemachine, "Wallslide");
        walljump = new Playerwalljump(this, statemachine, "Jump");
        primaryattack = new Playerprimaryattack(this, statemachine, "Attack");
        hitstate=new Playerhitstate(this, statemachine, "Hit");
        counterattack=new Playercounterattackstate(this, statemachine, "Counterattack");
         aimsword=new Playeraimswordstate(this, statemachine, "Aimsword");
        catchsword=new playercatchswordstate(this, statemachine, "Catchsword");
        diestate=new Playerdiestate(this, statemachine, "Die");
        summonstate=new Plyersummonswordstate(this, statemachine, "Summon");

    }
    protected override void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        base.Start();

        statemachine.Initialized(idlestate);
    }
    protected override void Update()
    {
        base.Update();
        checkfordash();
        quickfall();
        statemachine.currentstate.Update();

    }
    public void turnblue(float duration, float spreadTime) => StartCoroutine(CircularBlueSpread(duration, spreadTime));

    private Sprite circleSprite;

    // 调用示例：StartCoroutine(CircularBlueSpread(2f, 0.5f));
    IEnumerator CircularBlueSpread(float totalDuration, float spreadTime)
    {
        // 加载圆形图片（确保Resources文件夹中有"CircleMask"图片）
        circleSprite = Resources.Load<Sprite>("CircleMask");
        if (circleSprite == null)
        {
            Debug.LogError("请在Resources文件夹中添加名为CircleMask的圆形图片");
            yield break;
        }

        // 创建UI画布
        GameObject canvasObj = new GameObject("CircularBlueCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000; // 确保在最上层
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // 创建圆形扩散的Image
        GameObject circleObj = new GameObject("BlueCircle");
        circleObj.transform.SetParent(canvasObj.transform);
        Image blueImage = circleObj.AddComponent<Image>();
        blueImage.sprite = circleSprite;
        blueImage.color = new Color(0, 0, 1, 0.3f); // 初始较浅的半透明蓝色

        // 设置初始位置和大小（中心点，极小尺寸）
        RectTransform rt = circleObj.GetComponent<RectTransform>();
        rt.pivot = new Vector2(0.5f, 0.5f); // 中心点为 pivot
        rt.anchoredPosition = Vector2.zero; // 屏幕中心
        rt.sizeDelta = new Vector2(10, 10); // 初始大小

        // 计算目标大小（确保覆盖全屏的圆形）
        float diagonal = Mathf.Sqrt(Screen.width * Screen.width + Screen.height * Screen.height);
        float targetSize = diagonal * 1.2f; // 稍微放大确保覆盖

        // 圆形扩散并变色动画
        float elapsed = 0;
        GameObject[] allEventObjects = GameObject.FindGameObjectsWithTag("event");

        // 2. 遍历数组，逐个设置激活状态
        foreach (GameObject eventObj in allEventObjects)
        {
            if (eventObj.GetComponentInChildren<ParticleSystem>())
            {
                eventObj.GetComponentInChildren<ParticleSystem>().Pause(); // 也可根据需求改为 false（批量隐藏）
            }
            
        }

        while (elapsed < spreadTime)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / spreadTime);
            // 从中心向外平滑扩散
            float currentSize = Mathf.Lerp(10, targetSize, progress);
            rt.sizeDelta = new Vector2(currentSize, currentSize);
            // 颜色从浅蓝逐渐变深蓝
            float blueRamp = Mathf.Lerp(0.3f, 1f, progress);
            blueImage.color = new Color(0, 0, blueRamp, blueImage.color.a);
            yield return null;
        }

       
        float holdTime = totalDuration - spreadTime;
        if (holdTime > 0)
            yield return new WaitForSeconds(holdTime);

        // 逐渐变淡的动画
        float fadeTime = 1f; // 淡入淡出的时间，可根据需要调整
        elapsed = 0;
        while (elapsed < fadeTime)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / fadeTime);
            // 透明度逐渐降低
            blueImage.color = new Color(blueImage.color.r, blueImage.color.g, blueImage.color.b, Mathf.Lerp(1f, 0f, progress));
            yield return null;
        }
        foreach (GameObject eventObj in allEventObjects)
        {
            if (eventObj.GetComponentInChildren<ParticleSystem>())
            {
                eventObj.GetComponentInChildren<ParticleSystem>().Play(); // 也可根据需求改为 false（批量隐藏）
            }
        }
        // 淡入淡出结束后销毁canvasObj
        Destroy(canvasObj);
        
    }
    public virtual void assignanimname(string _aimboolname)
    {
        lastanimboolname = _aimboolname;
    
   

    }

    public virtual void damageofplayer()
    {
        fx.StartCoroutine("FlashFX");
        StartCoroutine("hitknockback");
        statemachine.changestate(hitstate);
    }
    private void checkfordash()
    {
        if (iswalldetected())
        {
            return;
        }
        


        
    }
    private void quickfall()
    {
        if (Input.GetKeyDown(KeyCode.S)&&rb.velocity.y<0)
        {
            Setvelocity(rb.velocity.x,rb.velocity.y*4);
        }


    }
    public void Animationtrigger() => statemachine.currentstate.Animationfinshedtrigger();

    public override void die()
    {
        isdead = true;
        statemachine.changestate(diestate);
        base.die();
        disappear();
       
        //dietime -= Time.deltaTime;
        if (dietime < 0)
        {
            Destroy(gameObject);
        }


        

    }
    private void disappear()
    {
        sr.color = new Color(1, 1, 1, sr.color.a - (Time.deltaTime * colorlosingspeed));
    }



}
