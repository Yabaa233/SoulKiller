using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PieceMoveClip
{
    [Header("需要运动的棋子")]
    public Piece piece;
    [Header("需要运动到的位置")]
    public Transform targetPoint;
}
[System.Serializable]
public class CheckerBoard : MonoBehaviour
{
    [Header("运动点集合")]
    [SerializeField] public List<PieceMoveClip> pieceMove;
    [SerializeField] public List<PieceMoveClip> initPosition;
    [Tooltip("当前阶段")] public int nextStep = 0; //当前阶段
    [Tooltip("下棋步频")] public float moveCD = 1;    //下棋步频
    [Tooltip("初始化后静止时间")] public float initWaitTime = 2.0f;  //初始化后静止时间
    [Tooltip("王破碎特效")] public GameObject kingBreakEff_W;
    [Tooltip("王后破碎特效")] public GameObject queenBreakEff_W;
    [Tooltip("王破碎特效")] public GameObject kingBreakEff_B;
    [Tooltip("王后破碎特效")] public GameObject queenBreakEff_B;
    [Tooltip("棋子落地特效")] public GameObject pieceDownEff;
    public bool trapStart = false;
    public bool canNext = false;   //移动棋子是否完成
    private bool initOver = false;  //初始化棋子位置是否完成
    private bool needInit = false;  //是否需要初始化
    private float CDTime = 0;
    public int kingOrQueenCount = 0;
    public int pieceCount = 0;
    private Transform pieces;   //全部棋子
    private RoomTrigger roomTrigger;    //关卡的触发器，用于处理通关逻辑
    protected void Awake()
    {
        pieces = transform.Find("Pieces");
    }
    public void StartStep()
    {
        CanNext(true);
    }
    private void Start()
    {
        roomTrigger = transform.parent.GetComponent<RoomTrigger>();
        roomTrigger.clearCheck += () => kingOrQueenCount == 0;  //通关条件
        if (initPosition.Count == 0)
        {
            Debug.LogWarning("当前棋盘没有配置棋子出生点");
        }
        if (initPosition.Count != transform.GetChild(0).childCount)
        {
            Debug.LogWarning("当前棋盘没有配置全部棋子出生点");
        }
        StartCoroutine(InitCheckerBoard()); //不为空就初始化位置
    }

    private void OnEnable()
    {
        GetComponent<TrapTrigger>().openTarp += () => trapStart = true;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private void Update()
    {
        if (!trapStart) return;
        if (initOver && canNext)
        {
            CDTime += Time.deltaTime;
            if (CDTime > moveCD)
            {
                //保证当前移动棋子不为空
                while (!pieceMove[nextStep].piece.gameObject.activeSelf)
                {
                    //如果棋子全部被破坏就不再运动
                    if (pieceCount == 0)
                    {
                        Debug.Log("场景中没有棋子了");
                        CanNext(false);
                        return;
                    }
                    NextStep();
                }
                if (needInit)
                {
                    needInit = false;
                    StartCoroutine(InitCheckerBoard());
                    return;
                }
                pieceMove[nextStep].piece.Move(pieceMove[nextStep].targetPoint.position);
                NextStep();
                CanNext(false);
                CDTime = 0;
            }
        }
    }

    /// <summary>
    /// 可以执行下一步
    /// </summary>
    public void CanNext(bool can)
    {
        canNext = can;
    }

    /// <summary>
    /// 下一步棋
    /// </summary>
    public void NextStep()
    {
        nextStep = nextStep + 1 == pieceMove.Count ? 0 : nextStep + 1;
        if (nextStep == 0)
        {
            needInit = true;
        }
    }

    /// <summary>
    /// 初始化棋子位置
    /// </summary>
    IEnumerator InitCheckerBoard()
    {
        initOver = false;
        float time = 0;
        foreach (PieceMoveClip clip in initPosition)
        {
            if (clip.piece != null) clip.piece.InitMove(clip.targetPoint.position);
        }
        while (time < initWaitTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        initOver = true;
        yield break;
    }

    /// <summary>
    /// 确认King和Queen生存数量
    /// </summary>
    public void CheckKingAndQueen()
    {
        kingOrQueenCount--;
        if (kingOrQueenCount == 0)
        {
            BreakAllPiece();
        }
    }

    /// <summary>
    /// 摧毁所有棋子
    /// </summary>
    public void BreakAllPiece()
    {
        Piece temp;
        for (int i = 0; i < pieces.childCount; i++)
        {
            temp = pieces.GetChild(i).GetComponent<Piece>();
            if (!temp.CheckisDead())
            {
                temp.BreakAllPiece_One();
            }
        }
        roomTrigger.TrapClear();
    }
}
