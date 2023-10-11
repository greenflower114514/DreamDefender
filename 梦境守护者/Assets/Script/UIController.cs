using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    /*
    [Header("�Ի���ϵͳ���")]
    public GameObject dialogPanal;//�Ի�����壬���ڿ����Ƿ�رնԻ���
    public Text dialogText;//�Ի����ı������ڻ�ȡҪ��ʾ���ı�
    [TextArea(1, 3)] public string[] DialogTextList;//ǰ������ȷ���ı���ʾʱ�Ļ�������ĿǰΪ3�С����ڴ��Ҫ��ʾ�ĶԻ��б�
    public int currentIndex;//�Ի����������������ȷ����ǰ�Ի����е�λ��
    */

    /*
    [Header("��ȡ�����е���������")]
    GameObject[] all = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
    */

    public GameObject player;//��ȡ�ػ���

    [Header("�°�Ի���ϵͳ���")]
    public GameObject dialogPanalNew;//�°�Ի������
    public Image headImage;//Ҫ��ʾ������ͷ��ͼƬ
    public Text dialogTextNew;//�Ի�����Ҫ��ʾ���ı�

    public TextAsset textFile;//�ı��Ĵ洢�ļ������ڶ�ȡ����Ҫ�õ����ı�
    public int index;//��¼��ǰ�������ı�����
    [SerializeField] public float textSpeed;//�����ı�������ʾ���ٶ�

    public Sprite headPlayer;//�ػ���ͷ��
    [SerializeField] public Sprite[] headNPC;//NPCͷ��

    public bool textFinished;//�ı��Ƿ���ʾ���
    public bool isTyping;//�Ƿ�����������ʾ

    public int[] dialog;//�����ж�ĳһ�ζԻ����ı��б�ĵڼ��п�ʼ��ȡ
    public int dialogNum;//���ڼ�¼��ǰ�ĶԻ�Ϊ�ڼ���

    public List<string> textList = new List<string>();

    [Header("������Ļ������������")]
    public GameObject dimmingPanal;
    public int dimmingPanalState;//���ڿ���������������״̬
    [SerializeField] float colorChangeSpeed;//���������ɫת���ٶ�
    public int circleNum;//���ڼ�¼��ǰ�ڰ�ת������
    [SerializeField] float waitTime;//������Ļ��Ϊ��ɫ��ĵȴ�ʱ��


    // Start is called before the first frame update
    private void Awake()//��Ʒ�����ִ��
    {
        GetTextFromFile(textFile);//ÿ�λ���UI�����ܶ����»�ȡһ���ļ��е��ı�
    }
    private void OnEnable()//��Ʒ�ͽű����붼����ʱִ��
    {
        index = 0;//�Ի������غ������öԻ���������Ҫ
        textFinished = true;//�Ի�������ʱ���ı������ѽ���
        StartCoroutine(setTextUI());
    }
    void Start()
    {
        dimmingPanal.SetActive(true);
        circleNum = 0;
        isTyping = true;
        //dialogText.text = DialogTextList[currentIndex];
        //dialog[0] = false;//��ʼ��һ�ζԻ�δ���
    }

    // Update is called once per frame
    void Update()
    {
        //�Ի������

        if (dialogNum >= 0)//�жԻ����ڽ��������Ի���
            dialogPanalNew.active = true;
        if(dialogPanalNew.active == true)//����Ի��򱻼���
        {
            //if(dialogNum >= 0)//�жԻ�������
            
               if(textList[index].Trim() == "�Ի�����")//�����ǰһ�ζԻ�ǡ�ý���
                {
                    dialogNum = -1;//��ǰû�����ڽ��еĶԻ�
                    //dialogTextNew.text = "";//����ı���ʾ��
                    //dialogPanalNew.active = false;//�ر��ı����
                    if (Input.GetKeyDown(KeyCode.F))//һ�ζԻ�����ʱ����F��
                    {
                        dialogTextNew.text = "";//����ı���ʾ��
                        dialogPanalNew.SetActive(false);//�رնԻ���
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.F))//�Ի�δ����������F��
                    {
                        if (textFinished)//һ�仰�Ѿ�����
                        {
                            StartCoroutine(setTextUI());//������ʾ�������ĶԻ�
                        }
                        else if (!textFinished)//�ı�δ��ʾ����ְ���F
                        {
                            isTyping = false;//ֱ����ʾ��ǰ�ı�
                        }
                    }
                }
            
        }

        //����������

    if(dimmingPanalState == 1)//���Ű�ɫת��
        {
            //dimmingPanal.GetComponent<Image>().color = new Color(1,1,1,0);//����Ļ�л�Ϊ��ɫ
            if (Mathf.Abs(dimmingPanal.GetComponent<Image>().color.a - 1) > 0.01 && circleNum == 0)
            {
                dimmingPanal.GetComponent<Image>().color = Color.Lerp(dimmingPanal.GetComponent<Image>().color, new Color(1, 1, 1, 1), colorChangeSpeed * Time.deltaTime);//ת����Ļ��ɫΪ����ɫ
            }
            else if (circleNum == 0)//ѭ����һ������ɫ����
            {
                Coroutine waitTime = StartCoroutine(WaitTime());//Я�̵ȴ�һ��ʱ��
            }
            if (Mathf.Abs(dimmingPanal.GetComponent<Image>().color.a) > 0.01 && circleNum == 1)//�ڶ���ѭ������ɫ����
            {
                dimmingPanal.GetComponent<Image>().color = Color.Lerp(dimmingPanal.GetComponent<Image>().color, new Color(1, 1, 1, 0), colorChangeSpeed * Time.deltaTime);//ת����Ļ��ɫΪԭ����ɫ
            }
            else if (circleNum == 1)
            {
                circleNum = 0;
                dimmingPanalState = 0;
            }
            //Color.Lerp(dimmingPanal.GetComponent<Image>().color, new Color(1, 1, 1, 0), colorChangeSpeed * Time.deltaTime);//ת����Ļ��ɫΪԭ����ɫ
           // Debug.Log(dimmingPanal.GetComponent<Image>().color);
            //dimmingPanalState = 0;//������ת�����״̬�л�Ϊ��״̬
        }
        if (dimmingPanalState == 2)//���ź�ɫת��
        {
            //dimmingPanal.GetComponent<Image>().color = new Color(0,0,0,0);//����Ļ�л�Ϊ��ɫ
            if(Mathf.Abs(dimmingPanal.GetComponent<Image>().color.a - 1) > 0.01 && circleNum == 0)
            {
                dimmingPanal.GetComponent<Image>().color = Color.Lerp(dimmingPanal.GetComponent<Image>().color, new Color(0, 0, 0, 1), colorChangeSpeed * Time.deltaTime);//ת����Ļ��ɫΪ����ɫ
            }
            else if(circleNum == 0)
            {
                Coroutine waitTime = StartCoroutine(WaitTime());
            }
            if(Mathf.Abs(dimmingPanal.GetComponent<Image>().color.a) > 0.01 && circleNum == 1)
            {
                dimmingPanal.GetComponent<Image>().color = Color.Lerp(dimmingPanal.GetComponent<Image>().color, new Color(0, 0, 0, 0), colorChangeSpeed * Time.deltaTime);//ת����Ļ��ɫΪԭ����ɫ
            }
            else if(circleNum == 1)
            {
                circleNum = 0;
                dimmingPanalState = 0;
            }

            //Color.Lerp(dimmingPanal.GetComponent<Image>().color, new Color(0,0, 0, 0), colorChangeSpeed * Time.deltaTime);//ת����Ļ��ɫΪԭ����ɫ
            //Debug.Log(dimmingPanal.GetComponent<Image>().color);
            //dimmingPanalState = 0;//������ת�����״̬�л�Ϊ��״̬
        }
    }
    /*
    //-------------------------------�Ի���ť��ط���-------------------------------------------

    public void CloseDialog()//�رնԻ���
    {
        dialogPanal.SetActive(false);
    }
    public void ContinueDialog()//����Ի��򴥷���һ���Ի�
    {
        if(currentIndex < DialogTextList.Length)
        {
            dialogText.text = DialogTextList[currentIndex];
        }
        else
        {
            CloseDialog();//�Ի������رնԻ���
        }
        currentIndex++;//ÿ�β�����󶼽��ı������1λ������֮��򿪵��ı�ʼ�����µ��ı�
    }
    //---------------------------------------------------------------------------------------------
    */
    //-------------------------------�°�Ի��򷽷�------------------------------------------------

    public void GetTextFromFile(TextAsset textFile)//���ļ��ж�ȡ�ı�
    {
        textList.Clear();//ÿ����������UIControllerʱ�����¼����ı��б������

        var lineDate = textFile.text.Split('\n');//ͨ�����з����ı����ݽ����и�
        foreach(var line in lineDate)
        {
            textList.Add(line);//��ÿ���и�õ����ı���ӵ�Ҫ��ʾ���ı��б���
        }
    }

    IEnumerator setTextUI()
    {
        textFinished = false;//���ı���ʾ״̬����Ϊ��ʼ��ʾ
        dialogTextNew.text = "";//��ʼ��ʾ�ı�ʱ�ֽ��ı��������
        isTyping = true;

        switch (textList[index].Trim())//trim�������еĿո�Ϊ�˱�����ȡ�ı��������Ϣ
        {
            case "�ػ���"://���������ػ��ߣ���ͼ����ʾΪ�ػ���ͷ��
                headImage.sprite = headPlayer;
                index++;//������Ҫ��ȡ���ı�����Ϊ��һ��
                break;
            case "С�к�"://���ΪС�к�
                headImage.sprite = headNPC[0];
                index++;
                break;
        }
        int word = 0;//ÿ���һ�ΰ�ť�Ͳ���һ�е�����
        while(isTyping && word < textList[index].Length - 1)//���ֶ�ȡ�ı�����
        {
            dialogTextNew.text += textList[index][word];//�ڶԻ�������ʾ���ı���������
            word++;
            yield return new WaitForSeconds(textSpeed);//�ȴ�һ��ʱ������ʾ��һ������
        }
        //������ΰ�ť�������ʾ����
        dialogTextNew.text = textList[index];
        isTyping = false;
        textFinished = true;
        index++;//�����ı���ʾ��ϣ��л�����һ���ı�
    }
    //----------------------------------------------------------------------------------------------

    //------------------------------------�����������-----------------------------------------------

    IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(waitTime);
        circleNum = 1;
    }
}