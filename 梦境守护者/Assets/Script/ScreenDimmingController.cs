using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenDimmingController : MonoBehaviour
{
    [SerializeField] int state;//向UI控制面板传递亮度控制面板的状态,1为白色转场，2为黑色转场，3为黑色背景渐入并显示文字
    public GameObject UIController;//获取UI控制面板
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.name == "player" && Input.GetKeyDown(KeyCode.F))//守护者进入交互区域并按下交互键
        {
            Debug.Log("开始进行颜色转换");
            UIController.GetComponent<UIController>().dimmingPanalState = state;//改变明暗控制面板状态
            this.gameObject.SetActive(false);//关闭触发器
        }
    }
}
