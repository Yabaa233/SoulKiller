using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    public AnimationCurve LineWidthCurve;

    [Range(0, 1)] public float globalProgress;
    public float Width = 20f;
    //public float Angle = 30f;
    //public Vector3 Axis;
    public float GlobalProgressSpeed;
    public float TimeRange = 3f;
    public GameObject HitVFX;
    public GameObject FireVFX;
    public float DesTime = 3.2f;

    private bool isLasering;
    private GameObject hitVFX;
    private GameObject fireVFX;
    [SerializeField] private LineRenderer Laser;
    //private Quaternion rotate;
    public float MaxLength = 80;
    public float damageToBoss = 20;
    void Start()
    {
        //rotate = Quaternion.AngleAxis(Angle,Axis);
        Laser = GetComponentInChildren<LineRenderer>();
    }

    public void DrawLine()
    {
        RaycastHit hit;
        //设置激光开始位置
        Laser.SetPosition(0, transform.position);
        var EndPos = transform.position + transform.forward * MaxLength;
        Laser.SetPosition(1, EndPos);

        //开火特效和击中特效
        if (isLasering && globalProgress > 1.3f)
        {
            if (fireVFX == null)
            {
                GameObject firevfx = Instantiate(FireVFX, transform.position, Quaternion.identity);
                firevfx.transform.SetParent(transform);
                fireVFX = firevfx;


                Destroy(firevfx, DesTime);
            }

            if (Physics.Raycast(transform.position,
                    transform.TransformDirection(Vector3.forward),
                    out hit, MaxLength))
            {
                if (hitVFX == null)
                {
                    GameObject hitvfx = Instantiate(HitVFX, hit.point, Quaternion.identity);
                    hitvfx.transform.SetParent(transform);
                    hitVFX = hitvfx;
                    Destroy(hitvfx, DesTime);
                }
            }
        }

        float width = LineWidthCurve.Evaluate(globalProgress) * Width;
        Laser.widthMultiplier = width;
    }

    //发射魔法大炮
    public void LasertShoot()
    {
        globalProgress = 0f;
        isLasering = true;
        FMODUnity.RuntimeManager.PlayOneShot("event:/Level/JiDu/big");
    }
    private void FixedUpdate()
    {
        if (globalProgress <= TimeRange)
        {
            globalProgress += 0.02f * GlobalProgressSpeed;
        }
        else
        {
            isLasering = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameManager.Instance.TrickAttackPlayer(AttackPlayer);
        }
        if (other.tag == "EmyBody")
        {
            GameManager.Instance.TrickAttackEnemy(AttackEnemy, other.transform.parent.GetComponent<BaseEnemyControl>());
        }
        if (other.tag == "BossBody")
        {
            GameManager.Instance.TrickAttackBoss(AttackBoss);
        }
    }
    private void AttackPlayer(PlayerControl curPlayer)
    {
        curPlayer.characterData.currentHealth = 0;
    }
    private float AttackEnemy(BaseEnemyControl enemy)
    {
        float damage = enemy.enemyData.currentHealth;
        enemy.enemyData.currentHealth -= damage;
        return damage;
    }
    private float AttackBoss(BossControl bossControl)
    {
        bossControl.bossData.currentHealth -= damageToBoss;
        return damageToBoss;
    }
}
