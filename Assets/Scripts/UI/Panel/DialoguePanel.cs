using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePanel : MonoBehaviour
{
    public Text characterName;
    public Text contentText;
    public Text continueTips;
    public Text describeText;
    public Text functionText;
    public Text guideText;
    public Image Icon;
    public Button skipButton;
    public GameObject wsadMove;
    public GameObject qeCode;
    public GameObject mouse;
    public GameObject spaceIcon;


    //缓存区
    private Vector3 startPos;

    private void Awake() {
        characterName = transform.Find("CharacterName").GetComponent<Text>();
        contentText = transform.Find("ContentText").GetComponent<Text>();
        continueTips = transform.Find("ContinueTips").GetComponent<Text>();
        describeText = transform.Find("DesribeText").GetComponent<Text>();
        functionText = transform.Find("FunctionText").GetComponent<Text>();
        Icon = transform.Find("Icon").GetComponent<Image>();
        spaceIcon = transform.Find("KeyArea").Find("SpaceIcon").gameObject;
        wsadMove = transform.Find("KeyArea").transform.Find("WSAD").gameObject;
        qeCode = transform.Find("KeyArea").transform.Find("QE").gameObject;
        mouse = transform.Find("KeyArea").transform.Find("Mouse").gameObject;
        skipButton = transform.Find("SkipButton").GetComponent<Button>();
        guideText = transform.Find("GuideText").GetComponent<Text>();

        skipButton.onClick.AddListener(Skip);
        startPos = this.transform.localPosition;
        Icon.enabled = false;
        describeText.gameObject.SetActive(false);
        functionText.gameObject.SetActive(false);
        guideText.gameObject.SetActive(false);
        spaceIcon.SetActive(false);
        wsadMove.SetActive(false);
        qeCode.SetActive(false);
        mouse.SetActive(false);
        skipButton.gameObject.SetActive(false);
    }


    /// <summary>
    /// 设置文字
    /// </summary>
    /// <param name="_name">角色名字</param>
    /// <param name="_contentText">角色名字</param>
    /// <param name="_continueTips">按什么键继续</param>
    public void SetText(string _name,string _contentText)
    {
        characterName.text = _name;
        contentText.text = _contentText;
    }
    /// <summary>
    /// 动效-逐字设置文字
    /// </summary>
    /// <param name="_name">角色名字</param>
    /// <param name="_contentText">角色名字</param>
    public void SetTextByChar(string _name,string _contentText)
    {
        characterName.DOText(_name,3).SetEase(Ease.Linear);
        contentText.DOText(_contentText, 3).SetEase(Ease.Linear);
    }
    public void SetContinueTipsVis(bool state)
    {
        continueTips.gameObject.SetActive(state);
    }

    public void MovePosition(Vector2 positionBias)
    {
        this.transform.position += new Vector3(positionBias.x,positionBias.y,0);
    }

    public void SetDialogueCenter(bool state)
    {
        if(state)
        {
            contentText.alignment = TextAnchor.MiddleCenter;
        }
        else
        {
            contentText.alignment = TextAnchor.UpperLeft;
        }
    }

    public void SetCharacterNameCenter(bool state)
    {
        if(state)
        {
            contentText.alignment = TextAnchor.MiddleCenter;
        }
        else
        {
            contentText.alignment = TextAnchor.UpperRight;
        }
    }

    public void SetDialogueSize(int fontSize)
    {
        contentText.fontSize = fontSize;
    }

    public void SetCharacterNameSize(int fontSize)
    {
        characterName.fontSize = fontSize;
    }

    public void SetIconVisible(bool state)
    {
        Icon.enabled = state;
    }

    public void SetIconImage(Sprite sprite)
    {
        Icon.sprite = sprite;
    }
    

    public void SetDesribePanelVisble(bool state)
    {
        describeText.gameObject.SetActive(state);
        functionText.gameObject.SetActive(state);
        SetIconVisible(state);
    }

    public void SetSpaceIconVisible(bool state)
    {
        spaceIcon.SetActive(state);
    }
    public void SetwsadMoveVisible(bool state)
    {
        wsadMove.SetActive(state);
    }

    public void SetQEcodeVisible(bool state)
    {
        qeCode.SetActive(state);
    }

    public void SetMouseVisible(bool state)
    {
        mouse.SetActive(state);
    }

    public void SetQEcodeText(string text)
    {
        qeCode.transform.Find("Text").GetComponent<Text>().text = text;
    }

    public void SetMouseSprite(Sprite sprite)
    {
        mouse.GetComponent<Image>().sprite = sprite;
    }

    public void SetDesribePanelText(string _describe,string _functiontext)
    {
        describeText.text = _describe;
        functionText.text = _functiontext;
    }

    public void SetGuideText(string _guideText)
    {
        guideText.text = _guideText;
    }

    public void SetGuideTextVisble(bool state)
    {
        guideText.gameObject.SetActive(state);
    }

    public void SetSkipButtonVisble(bool state)
    {
        skipButton.gameObject.SetActive(state);
    }

    public void ResetTextAnchor()
    {
        contentText.alignment = TextAnchor.UpperLeft;
    }

    public void ResetPosition()
    {
        this.transform.localPosition = startPos;
    }
    public void ResetDialogueFontSize()
    {
        contentText.fontSize = 60;
    }

    public void Skip()
    {
        SceneLoadManager.Instance.LoadBattleScene(2);
    }

    public void ResetAll()
    {
        ResetPosition();
        ResetDialogueFontSize();
        ResetTextAnchor();
        SetDesribePanelVisble(false);
        SetSpaceIconVisible(false);
        SetwsadMoveVisible(false);
        SetQEcodeVisible(false);
        SetMouseVisible(false);
        SetGuideTextVisble(false);
        SetSkipButtonVisble(false);
    }
}
