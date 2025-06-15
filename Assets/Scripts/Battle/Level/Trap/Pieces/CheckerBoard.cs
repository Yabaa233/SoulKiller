using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PieceMoveClip
{
    [Header("The chess piece that needs to be moved")]
    public Piece piece;
    [Header("The position that needs to be moved to.")]
    public Transform targetPoint;
}
[System.Serializable]
public class CheckerBoard : MonoBehaviour
{
    [Header("Set of Moving Points")]
    [SerializeField] public List<PieceMoveClip> pieceMove;
    [SerializeField] public List<PieceMoveClip> initPosition;
    [Tooltip("現在の段階")] public int nextStep = 0; //現在の段階
    [Tooltip("チェスのステップ頻度")] public float moveCD = 1;    //チェスのステップ頻度
    [Tooltip("初期化後の静止時間")] public float initWaitTime = 2.0f;  //初期化後の静止時間
    [Tooltip("Broken King Special Effects")] public GameObject kingBreakEff_W;
    [Tooltip("Queen's Shattered Special Effects")] public GameObject queenBreakEff_W;
    [Tooltip("Broken King Special Effects")] public GameObject kingBreakEff_B;
    [Tooltip("Queen's Shattered Special Effects")] public GameObject queenBreakEff_B;
    [Tooltip("Chess Piece Landing Special Effects")] public GameObject pieceDownEff;
    public bool trapStart = false;
    public bool canNext = false;   //駒の移動は完了しましたか？
    private bool initOver = false;  //駒の初期位置の設定は完了しましたか？
    private bool needInit = false;  //初期化が必要ですか？
    private float CDTime = 0;
    public int kingOrQueenCount = 0;
    public int pieceCount = 0;
    private Transform pieces;   //すべてのチェスピース
    private RoomTrigger roomTrigger;    //ステージのトリガー、クリアロジックの処理に使用されます
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
        roomTrigger.clearCheck += () => kingOrQueenCount == 0;  //クリア条件
        if (initPosition.Count == 0)
        {
            Debug.LogWarning("The current chessboard does not have a configured spawn point for the chess pieces.");
        }
        if (initPosition.Count != transform.GetChild(0).childCount)
        {
            Debug.LogWarning("The current chessboard does not have all the chess pieces' starting points set up.");
        }
        StartCoroutine(InitCheckerBoard()); //空でなければ位置を初期化します
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
                //現在の移動駒が空でないことを保証します
                while (!pieceMove[nextStep].piece.gameObject.activeSelf)
                {
                    //駒がすべて破壊されたら、もう動かない。
                    if (pieceCount == 0)
                    {
                        Debug.Log("There are no chess pieces left in the scene.");
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

    ///<summary>


    ////// あなたは次のステップを実行することができます。


    ///</summary>
    public void CanNext(bool can)
    {
        canNext = can;
    }

    ///<summary>


    ////// 次の一手


    ///</summary>
    public void NextStep()
    {
        nextStep = nextStep + 1 == pieceMove.Count ? 0 : nextStep + 1;
        if (nextStep == 0)
        {
            needInit = true;
        }
    }

    ///<summary>


    ////// 駒の位置を初期化します


    ///</summary>
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
    /// KingとQueenの生存数を確認してください。
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
    /// すべてのチェスピースを破壊する
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
