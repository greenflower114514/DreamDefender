using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenDimmingController : MonoBehaviour
{
    [SerializeField] int state;//��UI������崫�����ȿ�������״̬,1Ϊ��ɫת����2Ϊ��ɫת����3Ϊ��ɫ�������벢��ʾ����
    public GameObject UIController;//��ȡUI�������
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.name == "player" && Input.GetKeyDown(KeyCode.F))//�ػ��߽��뽻�����򲢰��½�����
        {
            Debug.Log("��ʼ������ɫת��");
            UIController.GetComponent<UIController>().dimmingPanalState = state;//�ı������������״̬
            this.gameObject.SetActive(false);//�رմ�����
        }
    }
}
