using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    [Header("�ػ�������")]

    [SerializeField] int maxLife;//�������ֵ
    public int life;//����ֵ

    [SerializeField] int attackPower;//������

    [SerializeField] float maxStrength;//�������
    public float strength;//����
    [SerializeField] float strengthRecoverSpeed;//�����ָ��ٶ�
    [SerializeField] float dashStrength;//������ĵ�����

    [SerializeField] float dashSpeed;//����ٶ�

    private Rigidbody2D rb;//��ײ��

    [SerializeField] float speed;//�ٶ�
    [SerializeField] float xSpeedWhenAttack;//����ʱˮƽ�ƶ��ٶ�
    [SerializeField] float ySpeedWhenAttack;//����ʱ��ֱ�ƶ��ٶ�

    [SerializeField] float chopAboveSpeed;//�趨�����������ٶ�
    [SerializeField] float chopBelowSpeed;//�趨�����������ٶ�
    
    [SerializeField] float wallSlideSpeed;//��ǽ�ٶ�
    [SerializeField] float wallGrabSpeed;//��ǽ�ٶ�

    [SerializeField] float jumpForce;//��Ծ����
    [SerializeField] float nextJumpForce;//�´���Ծ�ٶ�
    public int jumpNum;//��Ծ�����������жϽ�����Ծ���Ƕ�����
    public int jumpDirection;//��Ծ����

    // [SerializeField] float durationOfAttack;//��������ʱ��

    [SerializeField] float wallJump_XMultiplier;//��ǽ��X�������
    [SerializeField] float wallJump_YMultiplier;//��ǽ��y�������

    public bool blockFlip;//������ҳ���

    [Header("�������")]
    [SerializeField] float fallGravityMultiplier;//����ʱ��������
    [SerializeField] float normalGravity;
    public bool useNormalGravity;//�ж��Ƿ�ʹ����������ֵ
    [SerializeField] float lowJumpMultiplier;//������Ծʱ����ס�ո����Ծ�߶ȳͷ�
    public float tempGravity;//���ڴ洢��ǰ���������������л�

    [Header("�ж��ܷ���л��Ƿ������ز���")]
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

    [Header("����/ħ����")]
    public int skill_1Num;//����һ����
    public int skill_2Num;//���ܶ�����
    public int magic_1Num;//ħ��һ����
    public int magic_2Num;//ħ��������
    [Header("��ǽ����ǵ���")]
    public LayerMask groundLayer;//��ȡ����ͼ��
    public LayerMask wallLayer;//��ȡǽ��ͼ��

    public bool onGround;//�ڵ���
    public bool onLeftWall;//�����ǽ��
    public bool onWall;//��ǽ��

    //�����������������ƫ��
    public Vector2 bottomOffset;
    public Vector2 rightOffset;
    public Vector2 leftOffset;
    [SerializeField] float collisionRadius;//���������ײ�뾶

    [Header("��Ӱ��ز���")]
    public GameObject ghostObject;//��Ӱ����
    [SerializeField] int ghostNum;//��Ӱ����
    [SerializeField] float ghostTime;//��Ӱͣ��ʱ��

    [Header("CD����")]
    [SerializeField] float dashOverTime;//��̵�CD
    [SerializeField] float attackOverTime;//����CD�������Ϊ���ٵĸ���أ�Ӧ��С������������
    [SerializeField] float combinationSkillTime;//�����ж��Ƿ񴥷���ϼ�
    [SerializeField] float skill_1OverTime;
    [SerializeField] float skill_2OverTime;
    [SerializeField] float magic_1OverTime;
    [SerializeField] float magic_2OverTime;
    [SerializeField] float hurtOverTime;
    [SerializeField] float wallJumpOverTime;

    [Header("��������")]
    public int comboNum;//����������0��3
    [SerializeField] float maxComboInterval;//���������������������������ֵ�������ж�
    public float attackIntervalNow;//��ǰ��������������ж��Ƿ�������

    [Header("��ҷ���λ����Ϣ")]
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
        //��������������޳�̣���������Ϊ��ʹ��һ������
        rb.gravityScale = normalGravity;
        //��ȡ�������
        x = Input.GetAxis("Horizontal");//ˮƽ����
        y = Input.GetAxis("Vertical");//��ֱ����
        dir = new Vector2(x, y);//������

        //����Ƿ��ڵ���
        this.GetComponent<Animator>().SetBool("OnGround", onGround);
        //���y��������
        this.GetComponent<Animator>().SetFloat("yInput", dir.y);//���ػ���y��������봫�ݸ�״̬��
        //���y�����ٶ�
        this.GetComponent<Animator>().SetFloat("ySpeed", rb.velocity.y);//���ػ���y������ٶȴ��ݸ�״̬��

        //���x�����ٶ�
        this.GetComponent<Animator>().SetFloat("xSpeed", rb.velocity.x);//�����ˮƽ�������봫��״̬��

        //�ж��Ƿ��ڵ����ϣ�Ĭ�ϽŲ�ǽ����ߵ��涼�ڵ�����
        onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer) || Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, wallLayer);

        //�ж��Ƿ���ǽ��
        onWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, wallLayer)
               || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, wallLayer);//��߻��ұߴ�����ǽ���������ǽ��
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, wallLayer);//�����ײ�����⵽��ײ������ǽ��

        //������ҳ���
        if (onWall && !onGround)
            blockFlip = true;
        else
            blockFlip = false;
        if (!blockFlip)//�ڵ�������
        {
            if (x > 0)
            {
                GetComponent<SpriteRenderer>().flipX = false;//�����ҷ�
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
            if (onLeftWall)//����ǽ
            {
                GetComponent<SpriteRenderer>().flipX = false;//�ػ�������ǽ����ǽ��ͬ�������ҷ�����Ȼ����������ͷ���ܳ����󷽣�������������Ϊ��ʹ��ǽ����x������ٶȱ��ڼ���
            }
            if (!onLeftWall)//����ǽ
            {
                GetComponent<SpriteRenderer>().flipX = true;

            }
        }

        //�����Զ��ظ�
        if (strength + Time.deltaTime * strengthRecoverSpeed <= maxStrength)
        {
            strength += Time.deltaTime * strengthRecoverSpeed;
        }
        else
        {
            strength = maxStrength;
        }

        //---------------------------------------------------------------------------------�����ж���---------------------------------------------------------------------------------

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
        if (Input.GetKeyDown(KeyCode.Space) && onWall && !onGround && !wallJump)//���е�ǽ��
        {
            jumpDirection = (onLeftWall == true ? 1 : -1);//ȷ����Ծ����
            wallJump = true;
            jumpNum++;
            Coroutine waitWallJumpOver = StartCoroutine(WaitWallJumpOver());
        }

        //attack
        attackIntervalNow += Time.deltaTime;//�������ʱ������
        this.GetComponent<Animator>().SetInteger("ComboNum", comboNum);//���ݲ�����״̬��
        if (attackIntervalNow >= maxComboInterval)//��������ʱ�䣬����������������ʱ��
        {
            attackIntervalNow = 0;
            comboNum = 1;
        }
        if (Input.GetMouseButtonDown(0) && !dash && !skill_1 && !skill_2 && !magic_1 && !magic_2 && !hurt && !skill_1 && !skill_2 && attackIntervalNow >= attackOverTime && !attack)//û�н������ȼ��ߵĶ������ҹ�������ﵽ��С�������,����һ�ι��������Ѿ�����
        {
            this.GetComponent<Animator>().SetTrigger("Attack");

            comboNum++;//����������
            if (comboNum > 3)//һ��������������������
                comboNum = 1;
            attackIntervalNow = 0;//���ù������ʱ��
            Coroutine waitAttackOver = StartCoroutine(WaitAttackOver());//�ȴ������������������ڿ���attack��״̬
        }
        
        //dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && (!onWall || (onWall && onGround)) && !magic_1 && !magic_2 && !hurt && !skill_1 && !skill_2 && strength >= dashStrength && Mathf.Abs(dir.x) > 0.1)//������³�̼����Ҵ��ڿ��Գ�̵�״̬��û�н������ȼ��ߵĶ���
        {
            dash = true;
            strength -= dashStrength;//��������
            Coroutine waitDashOver = StartCoroutine(WaitDashOver());//�ȴ���̶�������,���ڿ���dash��״̬
        }
        this.GetComponent<Animator>().SetBool("Dash", dash);

        //wallGrab
        wallGrab = onWall && !onGround && !hurt && dir.y > 0.1 && !wallJump;//�ж��Ƿ񲥷���ǽ����
        this.GetComponent<Animator>().SetBool("WallGrab", wallGrab);

        //wallSlide
        wallSlide = onWall && !onGround && !hurt && dir.y < 0.1 && !wallJump;//�ж��Ƿ񲥷Ż�ǽ����
        this.GetComponent<Animator>().SetBool("WallSlide", wallSlide);

        //skill1
        if (!(!onGround && onWall) && skill_1Num > 0 && Input.GetKey(KeyCode.W) && Input.GetMouseButtonDown(0) && !magic_1 && !magic_2 && !hurt && !skill_2)
        {
            skill_1Num -= 1;
            this.GetComponent<Animator>().SetTrigger("Skill_1");
            Coroutine waitSkill_1Over = StartCoroutine(WaitSkill_1Over());//�ȴ�����1����������skill_1״̬
        }

        //skill2
        if (!(!onGround && onWall) && skill_2Num > 0 && Input.GetKey(KeyCode.S) && Input.GetMouseButtonDown(0) && !magic_1 && !magic_2 && !hurt && !skill_1 && !onGround)
        {
            skill_2Num -= 1;
            this.GetComponent<Animator>().SetTrigger("Skill_2");
            skill_2 = true;
            //Coroutine waitSkill_2Over = StartCoroutine(WaitSkill_2Over());//�ȴ�����2����������skill_2״̬
        }
        if (onGround)//���������skill2����
        {
            skill_2 = false;
        }

        //magic1
        if (!(!onGround && onWall) && skill_2Num > 0 && Input.GetKeyDown(KeyCode.Q) && !skill_2 && !magic_2 && !hurt && !skill_1)
        {
            magic_1Num -= 1;
            this.GetComponent<Animator>().SetTrigger("Magic_1");
            Coroutine waitMagic_1Over = StartCoroutine(WaitMagic_1Over());//�ȴ�����2����������magic_2״̬
        }

        //magic2
        if (!(!onGround && onWall) && skill_2Num > 0 && Input.GetKeyDown(KeyCode.E) && !magic_1 && !skill_2 && !hurt && !skill_1)
        {
            magic_2Num -= 1;
            this.GetComponent<Animator>().SetTrigger("Magic_2");
            Coroutine waitMagic_2Over = StartCoroutine(WaitMagic_2Over());//�ȴ�����2����������magic_2״̬
        }

        //hurt
        this.GetComponent<Animator>().SetBool("Hurt", hurt);
        if (hurt)//����ܵ��˺�,hurt�ڵ��˴������иı�Ϊtrue�����Ϊ��ײ��
        {
            Coroutine waitHurtOver = StartCoroutine(WaitHurtOver());//�ȴ����˵�Ӳֱ����
        }

        //-------------------------------------------------------------------------------------------�ƶ�������-------------------------------------------------------------------------------
        //��̬x�����ƶ�����
        rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
        //walk
        if (walk)
            rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);

        //jump
        if (onGround)//ֻҪ�ڵ����������Ծ����
            jumpNum = 1;
        if (Input.GetKeyDown(KeyCode.Space) && ((!onWall && onGround) || (onWall && onGround)) && !dash && !hurt && !skill_1 && !skill_2 && !magic_1 && !magic_2)//�ڵ�����Ծ
        {
            jumpNum++;//��Ծ��������
            rb.velocity = new Vector2(dir.x * speed, 0);
            rb.velocity += Vector2.up * jumpForce;//Ϊ�ػ����ṩy�����ٶ�
        }
        if (!onGround && !onWall && Input.GetKeyDown(KeyCode.Space) && jumpNum == 2 && rb.velocity.y < nextJumpForce)//������Ծ���ҿ��Զ�����,�Ҷ�����ʱy�����ٶ��Ѿ����ڶ������ṩ���ٶ�
        {
            // rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);��ԭ���ٶ��ϼ���һ������
            rb.velocity = new Vector2(dir.x * speed, 0);//�����ٶȣ�������һ������
            rb.velocity += Vector2.up * nextJumpForce;
            jumpNum++;//������Ծ��Ϊ��֮�����ǰ����ֹ
        }
        
        //wallJump
        if (onWall && !onGround && !wallJump)//������ǽ��ǽ״̬ʱ������Ծ����
            jumpNum = 1;
        if (wallJump)
            rb.velocity = new Vector2(jumpDirection * wallJump_XMultiplier, jumpForce * wallJump_YMultiplier);//ȷ����ǽ����x������ٶ�

        //fall
        if (useNormalGravity)//�����ɫ���䣬����������ʹ��������䣬�����ɫ������������û�а�����Ծ������lowJumpMulitplier����
        {
            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * fallGravityMultiplier * Time.deltaTime;//����ʱ������
            }
            else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * lowJumpMultiplier * Time.deltaTime;//����ʱ���¿ո������ĸ߶Ȼ�����
            }
        }

        //dash
        if (dash)
        {
            if(dir.x != 0)//����0������������
            {
                rb.velocity = new Vector2(dir.x / Mathf.Abs(dir.x) * dashSpeed, 0);//�õ�һ���泯������ٶ�
            }
            rb.gravityScale = 0;//���ʱ����������Ϊ0
            //strength -= dashStrength;//���������ڶ������ƴ�����ʵ�ֹ���
        }
        if (dash && Time.time > ghostTime)
        {
            Instantiate(ghostObject, transform.position, Quaternion.identity);//���ɲ�Ӱʵ��
            ghostTime = Time.time + dashOverTime / ghostNum; //ÿ�ξ���dashOverTime / ghostNumʱ��������һ����Ӱʵ��
        }
        if (!dash)//��̽�����ʹ������������
        {
            rb.gravityScale = normalGravity;
        }

        //wallGrab
        if (wallGrab)
        {
            rb.velocity = new Vector2(0, wallGrabSpeed);//�����ٶ�
        }

        //wallSlide
        if (wallSlide)
        {
            rb.velocity = new Vector2(0, -wallSlideSpeed);//�����ٶ�
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
            rb.velocity = new Vector2(0, chopAboveSpeed);//��������
            //ʹ�����������������ٶȲ��Լ���Ч��
            //rb.AddForce(new Vector2(0, chopAboveSpeed));
        }
        if (skill_2)
        {
            rb.velocity += new Vector2(0, chopBelowSpeed);//��������
        }


        //���ӻ���ⷶΧ
        void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            var position = new Vector2[] { bottomOffset, rightOffset, leftOffset };
            //����������
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
        /*��ʱ�趨skill2Ϊ����������ֻ�е������Ż�ֹͣ
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
        //ע�������Ҫ���CD
    }
}
