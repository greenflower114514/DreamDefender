using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnimyController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject player;//��ȡ���
    [SerializeField] int attackPower;//������
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.name == "player" && !collision.GetComponent<Controller>().invincibleProtection )//�ػ��߲����޵�״̬�ҵ��˹���ʱ�����ػ���һ������״̬
        {
            //֮��Ҫ�����Ƿ��ڽ��й������ж���������ֱ�Ӵ���Controllerһ��hurt = true����
            collision.GetComponent<Controller>().hurt = true;
            collision.GetComponent<Controller>().knockoutDirection = (this.GetComponent<SpriteRenderer>().flipX = true) ? -1 : 1;//���ݵ��˵�ǰ�������ж��ػ��߱����ɵķ���
        }
    }
}
