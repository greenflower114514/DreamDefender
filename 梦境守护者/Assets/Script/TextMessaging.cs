using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextMessaging : MonoBehaviour
{
    public GameObject UIController;//��ȡUI����ϵͳ
    [SerializeField] int dialogNum;//���ڴ��ݸ�UIController��ǰ����������һ�ζԻ�
    [SerializeField] int index;//���ڸ�ֵ��UIController�е�index������������ǰҪ��ӡ�ĶԻ�,ע��Ӧ��ָ�����������


    private void OnTriggerEnter2D(Collider2D collision)//�ػ��߽��룬�����ı�
    {
        if (collision.gameObject.name == "player")//��������ı�������������Ϊ�ػ���
        {
            Debug.Log("�ػ��߽���");
            UIController.GetComponent<UIController>().dialogNum = dialogNum;
            UIController.GetComponent<UIController>().index = index;
        }
    }
}
