using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    /*
    [Header("对话框系统相关")]
    public GameObject dialogPanal;//对话框面板，用于控制是否关闭对话框
    public Text dialogText;//对话框文本，用于获取要显示的文本
    [TextArea(1, 3)] public string[] DialogTextList;//前面用于确定文本显示时的换行数，目前为3行。用于存放要显示的对话列表
    public int currentIndex;//对话数组的索引，用于确定当前对话进行的位置
    */

    /*
    [Header("获取场景中的所有物体")]
    GameObject[] all = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
    */

    public GameObject player;//获取守护者

    [Header("新版对话框系统相关")]
    public GameObject dialogPanalNew;//新版对话框面板
    public Image headImage;//要显示的人物头像图片
    public Text dialogTextNew;//对话框中要显示的文本

    public TextAsset textFile;//文本的存储文件，用于读取所有要用到的文本
    public int index;//记录当前索引的文本行数
    [SerializeField] public float textSpeed;//控制文本逐字显示的速度

    public Sprite headPlayer;//守护者头像
    [SerializeField] public Sprite[] headNPC;//NPC头像

    public bool textFinished;//文本是否显示完毕
    public bool isTyping;//是否正在逐字显示

    public int[] dialog;//用于判断某一段对话从文本列表的第几列开始读取
    public int dialogNum;//用于记录当前的对话为第几段

    public List<string> textList = new List<string>();

    [Header("控制屏幕明暗的面板相关")]
    public GameObject dimmingPanal;
    public int dimmingPanalState;//用于控制明暗控制面板的状态
    [SerializeField] float colorChangeSpeed;//控制面板颜色转换速度
    public int circleNum;//用于记录当前黑白转化次数
    [SerializeField] float waitTime;//控制屏幕变为纯色后的等待时间


    // Start is called before the first frame update
    private void Awake()//物品激活就执行
    {
        GetTextFromFile(textFile);//每次唤醒UI管理熊都重新获取一次文件中的文本
    }
    private void OnEnable()//物品和脚本代码都激活时执行
    {
        index = 0;//对话框隐藏后则重置对话，好像不需要
        textFinished = true;//对话框隐藏时则文本内容已结束
        StartCoroutine(setTextUI());
    }
    void Start()
    {
        dimmingPanal.SetActive(true);
        circleNum = 0;
        isTyping = true;
        //dialogText.text = DialogTextList[currentIndex];
        //dialog[0] = false;//初始第一段对话未完成
    }

    // Update is called once per frame
    void Update()
    {
        //对话框控制

        if (dialogNum >= 0)//有对话正在进行则开启对话框
            dialogPanalNew.active = true;
        if(dialogPanalNew.active == true)//如果对话框被激活
        {
            //if(dialogNum >= 0)//有对话被触发
            
               if(textList[index].Trim() == "对话结束")//如果当前一段对话恰好结束
                {
                    dialogNum = -1;//当前没有正在进行的对话
                    //dialogTextNew.text = "";//清空文本显示框
                    //dialogPanalNew.active = false;//关闭文本面板
                    if (Input.GetKeyDown(KeyCode.F))//一段对话结束时按下F键
                    {
                        dialogTextNew.text = "";//清空文本显示框
                        dialogPanalNew.SetActive(false);//关闭对话框
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.F))//对话未结束，按下F键
                    {
                        if (textFinished)//一句话已经结束
                        {
                            StartCoroutine(setTextUI());//逐字显示接下来的对话
                        }
                        else if (!textFinished)//文本未显示完成又按下F
                        {
                            isTyping = false;//直接显示当前文本
                        }
                    }
                }
            
        }

        //明暗面板控制

    if(dimmingPanalState == 1)//播放白色转场
        {
            //dimmingPanal.GetComponent<Image>().color = new Color(1,1,1,0);//将屏幕切换为白色
            if (Mathf.Abs(dimmingPanal.GetComponent<Image>().color.a - 1) > 0.01 && circleNum == 0)
            {
                dimmingPanal.GetComponent<Image>().color = Color.Lerp(dimmingPanal.GetComponent<Image>().color, new Color(1, 1, 1, 1), colorChangeSpeed * Time.deltaTime);//转换屏幕颜色为纯白色
            }
            else if (circleNum == 0)//循环第一次则颜色加深
            {
                Coroutine waitTime = StartCoroutine(WaitTime());//携程等待一段时间
            }
            if (Mathf.Abs(dimmingPanal.GetComponent<Image>().color.a) > 0.01 && circleNum == 1)//第二次循环则颜色减淡
            {
                dimmingPanal.GetComponent<Image>().color = Color.Lerp(dimmingPanal.GetComponent<Image>().color, new Color(1, 1, 1, 0), colorChangeSpeed * Time.deltaTime);//转换屏幕颜色为原来颜色
            }
            else if (circleNum == 1)
            {
                circleNum = 0;
                dimmingPanalState = 0;
            }
            //Color.Lerp(dimmingPanal.GetComponent<Image>().color, new Color(1, 1, 1, 0), colorChangeSpeed * Time.deltaTime);//转换屏幕颜色为原来颜色
           // Debug.Log(dimmingPanal.GetComponent<Image>().color);
            //dimmingPanalState = 0;//将明暗转换面板状态切换为无状态
        }
        if (dimmingPanalState == 2)//播放黑色转场
        {
            //dimmingPanal.GetComponent<Image>().color = new Color(0,0,0,0);//将屏幕切换为黑色
            if(Mathf.Abs(dimmingPanal.GetComponent<Image>().color.a - 1) > 0.01 && circleNum == 0)
            {
                dimmingPanal.GetComponent<Image>().color = Color.Lerp(dimmingPanal.GetComponent<Image>().color, new Color(0, 0, 0, 1), colorChangeSpeed * Time.deltaTime);//转换屏幕颜色为纯黑色
            }
            else if(circleNum == 0)
            {
                Coroutine waitTime = StartCoroutine(WaitTime());
            }
            if(Mathf.Abs(dimmingPanal.GetComponent<Image>().color.a) > 0.01 && circleNum == 1)
            {
                dimmingPanal.GetComponent<Image>().color = Color.Lerp(dimmingPanal.GetComponent<Image>().color, new Color(0, 0, 0, 0), colorChangeSpeed * Time.deltaTime);//转换屏幕颜色为原来颜色
            }
            else if(circleNum == 1)
            {
                circleNum = 0;
                dimmingPanalState = 0;
            }

            //Color.Lerp(dimmingPanal.GetComponent<Image>().color, new Color(0,0, 0, 0), colorChangeSpeed * Time.deltaTime);//转换屏幕颜色为原来颜色
            //Debug.Log(dimmingPanal.GetComponent<Image>().color);
            //dimmingPanalState = 0;//将明暗转换面板状态切换为无状态
        }
    }
    /*
    //-------------------------------对话框按钮相关方法-------------------------------------------

    public void CloseDialog()//关闭对话框
    {
        dialogPanal.SetActive(false);
    }
    public void ContinueDialog()//点击对话框触发下一条对话
    {
        if(currentIndex < DialogTextList.Length)
        {
            dialogText.text = DialogTextList[currentIndex];
        }
        else
        {
            CloseDialog();//对话结束关闭对话框
        }
        currentIndex++;//每次播放完后都将文本向后推1位，这样之后打开的文本始终是新的文本
    }
    //---------------------------------------------------------------------------------------------
    */
    //-------------------------------新版对话框方法------------------------------------------------

    public void GetTextFromFile(TextAsset textFile)//从文件中读取文本
    {
        textList.Clear();//每次重新启动UIController时都重新加载文本列表的内容

        var lineDate = textFile.text.Split('\n');//通过换行符对文本内容进行切割
        foreach(var line in lineDate)
        {
            textList.Add(line);//将每次切割得到的文本添加到要显示的文本列表中
        }
    }

    IEnumerator setTextUI()
    {
        textFinished = false;//将文本显示状态调整为开始显示
        dialogTextNew.text = "";//开始显示文本时现将文本内容清空
        isTyping = true;

        switch (textList[index].Trim())//trim忽略所有的空格，为了便于提取文本的身份信息
        {
            case "守护者"://如果身份是守护者，则将图像显示为守护者头像
                headImage.sprite = headPlayer;
                index++;//接下来要提取的文本行数为下一行
                break;
            case "小男孩"://身份为小男孩
                headImage.sprite = headNPC[0];
                index++;
                break;
        }
        int word = 0;//每点击一次按钮就播放一行的文字
        while(isTyping && word < textList[index].Length - 1)//逐字读取文本内容
        {
            dialogTextNew.text += textList[index][word];//在对话框中显示的文本逐字显现
            word++;
            yield return new WaitForSeconds(textSpeed);//等待一定时间再显示下一个文字
        }
        //点击两次按钮则快速显示文字
        dialogTextNew.text = textList[index];
        isTyping = false;
        textFinished = true;
        index++;//该行文本显示完毕，切换到下一行文本
    }
    //----------------------------------------------------------------------------------------------

    //------------------------------------明暗控制面板-----------------------------------------------

    IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(waitTime);
        circleNum = 1;
    }
}