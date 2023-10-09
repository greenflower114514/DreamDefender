using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextMessaging : MonoBehaviour
{
    public GameObject UIController;//获取UI管理系统
    [SerializeField] int dialogNum;//用于传递给UIController当前触发的是哪一段对话
    [SerializeField] int index;//用于赋值给UIController中的index，便于索引当前要打印的对话,注意应该指向人物身份行


    private void OnTriggerEnter2D(Collider2D collision)//守护者进入，触发文本
    {
        if (collision.gameObject.name == "player")//如果进入文本触发器的物体为守护者
        {
            Debug.Log("守护者进入");
            UIController.GetComponent<UIController>().dialogNum = dialogNum;
            UIController.GetComponent<UIController>().index = index;
        }
    }
}
