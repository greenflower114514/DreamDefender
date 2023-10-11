using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnimyController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;//获取玩家
    [SerializeField] int attackPower;//攻击力
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.name == "player" && !collision.GetComponent<Controller>().invincibleProtection )//守护者不是无敌状态且敌人攻击时传递守护者一个受伤状态
        {
            //之后还要加上是否在进行攻击的判定，但现在直接传给Controller一个hurt = true即可
            collision.GetComponent<Controller>().hurt = true;
            collision.GetComponent<Controller>().knockoutDirection = (this.GetComponent<SpriteRenderer>().flipX = true) ? -1 : 1;//根据敌人当前朝向来判断守护者被击飞的方向
        }
    }
}
