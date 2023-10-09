using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    [Header("守护者属性")]

    [SerializeField] int maxLife;//最大生命值
    public int life;//生命值

    [SerializeField] int attackPower;//攻击力

    [SerializeField] float maxStrength;//最大体力
    public float strength;//体力
    [SerializeField] float strengthRecoverSpeed;//体力恢复速度
    [SerializeField] float dashStrength;//冲刺消耗的体力

    [SerializeField] float dashSpeed;//冲刺速度

    private Rigidbody2D rb;//碰撞体

    [SerializeField] float speed;//速度
    [SerializeField] float xSpeedWhenAttack;//攻击时水平移动速度
    [SerializeField] float ySpeedWhenAttack;//攻击时垂直移动速度

    [SerializeField] float chopAboveSpeed;//设定向上劈砍的速度
    [SerializeField] float chopBelowSpeed;//设定向下劈砍的速度
    
    [SerializeField] float wallSlideSpeed;//滑墙速度
    [SerializeField] float wallGrabSpeed;//爬墙速度

    [SerializeField] float jumpForce;//跳跃力度
    [SerializeField] float nextJumpForce;//下次跳跃速度
    public int jumpNum;//跳跃次数，用于判断进行跳跃还是二段跳
    public int jumpDirection;//跳跃方向

    // [SerializeField] float durationOfAttack;//攻击持续时间

    [SerializeField] float wallJump_XMultiplier;//蹬墙跳X方向乘数
    [SerializeField] float wallJump_YMultiplier;//蹬墙跳y方向乘数

    public bool blockFlip;//控制玩家朝向

    [Header("重力相关")]
    [SerializeField] float fallGravityMultiplier;//掉落时重力乘数
    [SerializeField] float normalGravity;
    public bool useNormalGravity;//判断是否使用正常重力值
    [SerializeField] float lowJumpMultiplier;//决定跳跃时不按住空格的跳跃高度惩罚
    public float tempGravity;//用于存储当前重力，方便重力切换

    [Header("判断能否进行或是否进行相关操作")]
    public bool idle;
    public bool canIdle;
    public bool walk;
    public bool canWalk;
    public bool jump;
    public bool canJump;
    public bool nextJump;
    public bool canNextJump;
    public bool attack;
    public bool canAttack;
    public bool dash;
    public bool canDash;
    public bool wallSlide;
    public bool canWallSlide;
    public bool wallGrab;
    public bool canWallGrab;
    public bool wallJump;
    public bool canWallJump;
    public bool skill_1;
    public bool canSkill_1;
    public bool skill_2;
    public bool canSkill_2;
    public bool magic_1;
    public bool canMagic_1;
    public bool magic_2;
    public bool canMagic_2;
    public bool hurt;
    public bool canHurt;

    [Header("技能/魔法数")]
    public int skill_1Num;//技能一数量
    public int skill_2Num;//技能二数量
    public int magic_1Num;//魔法一数量
    public int magic_2Num;//魔法二数量
    [Header("在墙面或是地面")]
    public LayerMask groundLayer;//获取地面图层
    public LayerMask wallLayer;//获取墙壁图层

    public bool onGround;//在地面
    public bool onLeftWall;//在左侧墙面
    public bool onWall;//在墙面

    //检测球体的三个方向的偏移
    public Vector2 bottomOffset;
    public Vector2 rightOffset;
    public Vector2 leftOffset;
    [SerializeField] float collisionRadius;//检测球体碰撞半径

    [Header("残影相关参数")]
    public GameObject ghostObject;//残影精灵
    [SerializeField] int ghostNum;//残影数量
    [SerializeField] float ghostTime;//残影停留时间

    [Header("CD设置")]
    [SerializeField] float dashOverTime;//冲刺的CD
    [SerializeField] float attackOverTime;//攻击CD，可理解为攻速的负相关，应该小于最大连击间隔
    [SerializeField] float combinationSkillTime;//用于判定是否触发组合技
    [SerializeField] float skill_1OverTime;
    [SerializeField] float skill_2OverTime;
    [SerializeField] float magic_1OverTime;
    [SerializeField] float magic_2OverTime;
    [SerializeField] float hurtOverTime;
    [SerializeField] float wallJumpOverTime;

    [Header("连击设置")]
    public int comboNum;//连击数，从0到3
    [SerializeField] float maxComboInterval;//最大的连击间隔，攻击间隔超过这个值连击会中断
    public float attackIntervalNow;//当前攻击间隔，用于判断是否达成连击

    [Header("玩家方向位置信息")]
    public float x = 0;
    public float y = 0;
    public Vector2 dir = new Vector2(0, 0);
    // Start is called before the first frame update
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        jumpNum = 1;
        tempGravity = rb.gravityScale;
        rb.gravityScale = normalGravity;
    }

    // Update is called once per frame
    void Update()
    {
        //更新重力，如果无冲刺，攻击等行为，使用一般重力
        rb.gravityScale = normalGravity;
        //获取玩家输入
        x = Input.GetAxis("Horizontal");//水平输入
        y = Input.GetAxis("Vertical");//垂直输入
        dir = new Vector2(x, y);//总输入

        //玩家是否在地面
        this.GetComponent<Animator>().SetBool("OnGround", onGround);
        //玩家y方向输入
        this.GetComponent<Animator>().SetFloat("yInput", dir.y);//将守护者y方向的输入传递给状态机
        //玩家y方向速度
        this.GetComponent<Animator>().SetFloat("ySpeed", rb.velocity.y);//将守护者y方向的速度传递给状态机

        //玩家x方向速度
        this.GetComponent<Animator>().SetFloat("xSpeed", rb.velocity.x);//将玩家水平方向输入传给状态机

        //判断是否在地面上，默认脚踩墙面或者地面都在地面上
        onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer) || Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, wallLayer);

        //判断是否在墙上
        onWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, wallLayer)
               || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, wallLayer);//左边或右边触碰到墙体均看做在墙上
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, wallLayer);//左侧碰撞球体检测到碰撞则在左墙上

        //控制玩家朝向
        if (onWall && !onGround)
            blockFlip = true;
        else
            blockFlip = false;
        if (!blockFlip)//在地面或空中
        {
            if (x > 0)
            {
                GetComponent<SpriteRenderer>().flipX = false;//朝向右方
                //m_facingDirection = 1;
            }
            else if (x < 0)
            {
                GetComponent<SpriteRenderer>().flipX = true;
                //m_facingDirection = -1;
            }
        }
        if (blockFlip)
        {
            if (onLeftWall)//在左墙
            {
                GetComponent<SpriteRenderer>().flipX = false;//守护者在左墙上爬墙，同样朝向右方（虽然动画中人物头可能朝向左方），这样设置是为了使蹬墙跳在x方向的速度便于计算
            }
            if (!onLeftWall)//在右墙
            {
                GetComponent<SpriteRenderer>().flipX = true;

            }
        }

        //体力自动回复
        if (strength + Time.deltaTime * strengthRecoverSpeed <= maxStrength)
        {
            strength += Time.deltaTime * strengthRecoverSpeed;
        }
        else
        {
            strength = maxStrength;
        }

        //---------------------------------------------------------------------------------动画判定器---------------------------------------------------------------------------------

        //Idle,canIdle
        idle = onGround && Mathf.Abs(dir.x) < 0.01;
        this.GetComponent<Animator>().SetBool("Idle", idle);

        //walk
        walk = onGround && Mathf.Abs(dir.x) > 0.1;
        this.GetComponent<Animator>().SetBool("Walk", walk);

        //jump
        jump = (rb.velocity.y > 0 ? true : false) && !(!onGround && onWall);
        this.GetComponent<Animator>().SetBool("Jump", jump);

        //wallJump
        this.GetComponent<Animator>().SetBool("WallJump",wallJump);
        if (Input.GetKeyDown(KeyCode.Space) && onWall && !onGround && !wallJump)//进行蹬墙跳
        {
            jumpDirection = (onLeftWall == true ? 1 : -1);//确定跳跃方向
            wallJump = true;
            jumpNum++;
            Coroutine waitWallJumpOver = StartCoroutine(WaitWallJumpOver());
        }

        //attack
        attackIntervalNow += Time.deltaTime;//攻击间隔时间增加
        this.GetComponent<Animator>().SetInteger("ComboNum", comboNum);//传递参数给状态机
        if (attackIntervalNow >= maxComboInterval)//超出连击时间，重置连击树与连击时间
        {
            attackIntervalNow = 0;
            comboNum = 1;
        }
        if (Input.GetMouseButtonDown(0) && !dash && !skill_1 && !skill_2 && !magic_1 && !magic_2 && !hurt && !skill_1 && !skill_2 && attackIntervalNow >= attackOverTime && !attack)//没有进行优先级高的动作并且攻击间隔达到最小攻击间隔,即上一次攻击动画已经结束
        {
            this.GetComponent<Animator>().SetTrigger("Attack");

            comboNum++;//连击数增加
            if (comboNum > 3)//一套连击结束，重置连击
                comboNum = 1;
            attackIntervalNow = 0;//重置攻击间隔时间
            Coroutine waitAttackOver = StartCoroutine(WaitAttackOver());//等待攻击动画结束，用于控制attack的状态
        }
        
        //dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && (!onWall || (onWall && onGround)) && !magic_1 && !magic_2 && !hurt && !skill_1 && !skill_2 && strength >= dashStrength && Mathf.Abs(dir.x) > 0.1)//如果按下冲刺键并且处于可以冲刺的状态且没有进行优先级高的动作
        {
            dash = true;
            strength -= dashStrength;//减少体力
            Coroutine waitDashOver = StartCoroutine(WaitDashOver());//等待冲刺动画结束,用于控制dash的状态
        }
        this.GetComponent<Animator>().SetBool("Dash", dash);

        //wallGrab
        wallGrab = onWall && !onGround && !hurt && dir.y > 0.1 && !wallJump;//判断是否播放爬墙动画
        this.GetComponent<Animator>().SetBool("WallGrab", wallGrab);

        //wallSlide
        wallSlide = onWall && !onGround && !hurt && dir.y < 0.1 && !wallJump;//判断是否播放滑墙动画
        this.GetComponent<Animator>().SetBool("WallSlide", wallSlide);

        //skill1
        if (!(!onGround && onWall) && skill_1Num > 0 && Input.GetKey(KeyCode.W) && Input.GetMouseButtonDown(0) && !magic_1 && !magic_2 && !hurt && !skill_2)
        {
            skill_1Num -= 1;
            this.GetComponent<Animator>().SetTrigger("Skill_1");
            Coroutine waitSkill_1Over = StartCoroutine(WaitSkill_1Over());//等待技能1结束，控制skill_1状态
        }

        //skill2
        if (!(!onGround && onWall) && skill_2Num > 0 && Input.GetKey(KeyCode.S) && Input.GetMouseButtonDown(0) && !magic_1 && !magic_2 && !hurt && !skill_1 && !onGround)
        {
            skill_2Num -= 1;
            this.GetComponent<Animator>().SetTrigger("Skill_2");
            skill_2 = true;
            //Coroutine waitSkill_2Over = StartCoroutine(WaitSkill_2Over());//等待技能2结束，控制skill_2状态
        }
        if (onGround)//到达地面则skill2结束
        {
            skill_2 = false;
        }

        //magic1
        if (!(!onGround && onWall) && skill_2Num > 0 && Input.GetKeyDown(KeyCode.Q) && !skill_2 && !magic_2 && !hurt && !skill_1)
        {
            magic_1Num -= 1;
            this.GetComponent<Animator>().SetTrigger("Magic_1");
            Coroutine waitMagic_1Over = StartCoroutine(WaitMagic_1Over());//等待技能2结束，控制magic_2状态
        }

        //magic2
        if (!(!onGround && onWall) && skill_2Num > 0 && Input.GetKeyDown(KeyCode.E) && !magic_1 && !skill_2 && !hurt && !skill_1)
        {
            magic_2Num -= 1;
            this.GetComponent<Animator>().SetTrigger("Magic_2");
            Coroutine waitMagic_2Over = StartCoroutine(WaitMagic_2Over());//等待技能2结束，控制magic_2状态
        }

        //hurt
        this.GetComponent<Animator>().SetBool("Hurt", hurt);
        if (hurt)//如果受到伤害,hurt在敌人触发器中改变为true，玩家为碰撞体
        {
            Coroutine waitHurtOver = StartCoroutine(WaitHurtOver());//等待受伤的硬直结束
        }

        //-------------------------------------------------------------------------------------------移动控制器-------------------------------------------------------------------------------
        //常态x方向移动控制
        rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
        //walk
        if (walk)
            rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);

        //jump
        if (onGround)//只要在地面就重置跳跃次数
            jumpNum = 1;
        if (Input.GetKeyDown(KeyCode.Space) && ((!onWall && onGround) || (onWall && onGround)) && !dash && !hurt && !skill_1 && !skill_2 && !magic_1 && !magic_2)//在地面跳跃
        {
            jumpNum++;//跳跃次数增加
            rb.velocity = new Vector2(dir.x * speed, 0);
            rb.velocity += Vector2.up * jumpForce;//为守护者提供y方向速度
        }
        if (!onGround && !onWall && Input.GetKeyDown(KeyCode.Space) && jumpNum == 2 && rb.velocity.y < nextJumpForce)//按下跳跃键且可以二段跳,且二段跳时y方向速度已经低于二段跳提供的速度
        {
            // rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);在原有速度上加上一个常亮
            rb.velocity = new Vector2(dir.x * speed, 0);//重置速度，并加上一个常亮
            rb.velocity += Vector2.up * nextJumpForce;
            jumpNum++;//所有跳跃行为在之后落地前均禁止
        }
        
        //wallJump
        if (onWall && !onGround && !wallJump)//处于爬墙或滑墙状态时重置跳跃次数
            jumpNum = 1;
        if (wallJump)
            rb.velocity = new Vector2(jumpDirection * wallJump_XMultiplier, jumpForce * wallJump_YMultiplier);//确定蹬墙跳在x方向的速度

        //fall
        if (useNormalGravity)//如果角色下落，则增大重力使其快速下落，如果角色正在上升并且没有按下跳跃键则以lowJumpMulitplier下落
        {
            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * fallGravityMultiplier * Time.deltaTime;//掉落时的重力
            }
            else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * lowJumpMultiplier * Time.deltaTime;//升起时按下空格键跳起的高度会升高
            }
        }

        //dash
        if (dash)
        {
            if(dir.x != 0)//避免0做除数而出错
            {
                rb.velocity = new Vector2(dir.x / Mathf.Abs(dir.x) * dashSpeed, 0);//得到一个面朝方向的速度
            }
            rb.gravityScale = 0;//冲刺时将重力设置为0
            //strength -= dashStrength;//体力减少在动画控制代码中实现过了
        }
        if (dash && Time.time > ghostTime)
        {
            Instantiate(ghostObject, transform.position, Quaternion.identity);//生成残影实例
            ghostTime = Time.time + dashOverTime / ghostNum; //每次经过dashOverTime / ghostNum时间后就生成一个残影实例
        }
        if (!dash)//冲刺结束，使用正常的重力
        {
            rb.gravityScale = normalGravity;
        }

        //wallGrab
        if (wallGrab)
        {
            rb.velocity = new Vector2(0, wallGrabSpeed);//更新速度
        }

        //wallSlide
        if (wallSlide)
        {
            rb.velocity = new Vector2(0, -wallSlideSpeed);//更新速度
        }

        //magic1
        if (magic_1)
        {
            rb.velocity = new Vector2(0, 0);
        }

        //magic2
        if (magic_2)
        {
            rb.velocity = new Vector2(0, 0);
        }

        //skill1
        if (skill_1)
        {
            rb.velocity = new Vector2(0, chopAboveSpeed);//向上劈砍
            //使用添加力来代替添加速度测试技能效果
            //rb.AddForce(new Vector2(0, chopAboveSpeed));
        }
        if (skill_2)
        {
            rb.velocity += new Vector2(0, chopBelowSpeed);//向下劈砍
        }


        //可视化检测范围
        void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            var position = new Vector2[] { bottomOffset, rightOffset, leftOffset };
            //画线体球面
            Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
            Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
            Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);
        }

        IEnumerator WaitAttackOver()
        {
            attack = true;
            yield return new WaitForSeconds(attackOverTime);
            attack = false;
        }
        IEnumerator WaitDashOver()
        {
            dash = true;
            yield return new WaitForSeconds(dashOverTime);
            dash = false;
        }
        IEnumerator WaitSkill_1Over()
        {
            skill_1 = true;
            yield return new WaitForSeconds(skill_1OverTime);
            skill_1 = false;
        }
        /*暂时设定skill2为向下劈砍，只有到达地面才会停止
        IEnumerator WaitSkill_2Over()
        {
            skill_2 = true;
            yield return new WaitForSeconds(skill_2OverTime);
            skill_2 = false;
        }
        */
        IEnumerator WaitMagic_1Over()
        {
            magic_1 = true;
            yield return new WaitForSeconds(magic_1OverTime);
            magic_1 = false;
        }
        IEnumerator WaitMagic_2Over()
        {
            magic_2 = true;
            yield return new WaitForSeconds(magic_2OverTime);
            magic_2 = false;
        }
        IEnumerator WaitHurtOver()
        {
            yield return new WaitForSeconds(hurtOverTime);
            hurt = false;
        }
        IEnumerator WaitWallJumpOver()
        {
            yield return new WaitForSeconds(wallJumpOverTime);
            wallJump = false;
        }
        //注：冲刺需要添加CD
    }
}
