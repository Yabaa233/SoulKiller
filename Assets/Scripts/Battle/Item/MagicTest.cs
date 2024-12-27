using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicTest : MonoBehaviour
{
    /// <summary>
    /// speed �ٶ�
    /// shotter:������
    /// target:Ŀ��
    /// </summary>
    [Header("速度")]
    public float speed;
    [Header("发射者")]
    public GameObject shotter;
    [Header("下落速度")]
    public float downVelocity = 0.2f;
    private Vector3 startPos, lastTargetPos, beforePos;
    private float beginTime;
    //����
    private Rigidbody myRigidbody;
    private Transform target;
    /*    Vector3 GetMiddlePosition(Vector3 startPos,Vector3 lastTargetPos)
        {
            Vector3 m = Vector3.Lerp(startPos, lastTargetPos, 0.2f),pos=startPos-lastTargetPos;
            Vector3 usePos = pos;
            usePos[0] -= 1;
            Vector3 normal = Vector3.Cross(midPos, usePos).normalized;
            float rd = Random.Range(1, 2f);
            float curveRatio = 0.3f;
            return m + (startPos - lastTargetPos).magnitude * curveRatio * rd * normal;
        }*/
    public GameObject muzzlePrefab;
	public GameObject hitPrefab;
    public List<GameObject> trails;

    private float updateTime = 0.5f;
    private void Awake()
    {
        myRigidbody = gameObject.AddComponent<Rigidbody>();
        myRigidbody.useGravity = false;
    }
    public void SetShotter(GameObject gameObject)
    {
        shotter = gameObject;
    }

    private void OnEnable()
    {
        Init();
        //��ǰ�ƶ�
        myRigidbody.velocity = transform.forward * speed;
        beginTime = Time.time;

        // //特效资源接入
        if(muzzlePrefab != null)
        {
            var muzzleVFX = Instantiate (muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVFX.transform.forward = gameObject.transform.forward;
            var ps = muzzleVFX.GetComponent<ParticleSystem>();
            if (ps != null)
				Destroy (muzzleVFX, ps.main.duration);
			else {
				var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
				Destroy (muzzleVFX, psChild.main.duration);
			}
        }
    }
    private void Update()
    {
        /*        percent += percentSpeed * Time.deltaTime;
                if (percent > 1)
                {
                    percent = 1;
                }
                Vector3 bezierPos = Untils.Bezier(percent, startPos, midPos, lastTargetPos);
                bezierPos[1] = y;
                transform.position = bezierPos;*/
        if(target!=null)
        {
            Vector3 dir = lastTargetPos - transform.position;
            dir.y = - downVelocity;
            myRigidbody.velocity = dir.normalized * speed;
            beforePos = target.position;
            if(Time.time > beginTime)
            {
                lastTargetPos = target.position;
                beginTime += updateTime;
            }
        }
        // Vector3 dir = lastTargetPos - transform.position;
        // dir.y = - downVelocity;
        // myRigidbody.velocity = dir.normalized * speed;
        // // transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
        // beforePos = target.position;
        // // if (Time.time > beginTime + overTime)
        // // {
        // //     ObjectPool.Instance.RecycleObj("Magic", gameObject);
        // // }
        // if(Time.time > beginTime)
        // {
        //     lastTargetPos = target.position;
        //     beginTime += updateTime;
        // }
    }
    
    private void OnCollisionEnter(Collision other)
    {
        //特效处理
        if(trails.Count > 0)
        {
            for (int i = 0; i < trails.Count; i++)
            {
                if(trails[i] == null)
                {
                    continue;
                }
                var ps = trails[i].GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Stop();
                    // Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
                }
            }
        }
        ContactPoint contact = other.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;

        if (hitPrefab != null)
        {
            var hitVFX = Instantiate(hitPrefab, pos, rot) as GameObject;

            var ps = hitVFX.GetComponent<ParticleSystem>();
            if (ps == null)
            {
                var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitVFX, psChild.main.duration);
            }
            else
                Destroy(hitVFX, ps.main.duration);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            StartCoroutine(DestroyParticle(0f));
            return;
        }
        else
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("PlayerBody"))
            {
                if (shotter != null)
                {
                    GameManager.Instance.EnemyAttack(shotter.GetComponent<BaseEnemyControl>());
                }
            }
            else if (other.gameObject.layer == 12)//打到盾上也要造成一次伤害
            {
                if (shotter != null)
                {
                    GameManager.Instance.EnemyAttack(shotter.GetComponent<BaseEnemyControl>());
                }
            }
        }

        StartCoroutine(DestroyParticle(0f));

    }


    public void Init()
    {
        myRigidbody.useGravity = false;
        startPos = gameObject.transform.position;
        transform.position = startPos;
        beforePos = startPos;
        if (GameManager.Instance != null && GameManager.Instance.currentPlayer != null)
        {
            target = GameManager.Instance.currentPlayer.transform;
        }
    }


    public IEnumerator DestroyParticle (float waitTime) {

		if (transform.childCount > 0 && waitTime != 0) {
			List<Transform> tList = new List<Transform> ();

			foreach (Transform t in transform.GetChild(0).transform) {
				tList.Add (t);
			}		

			while (transform.GetChild(0).localScale.x > 0) {
				yield return new WaitForSeconds (0.01f);
				transform.GetChild(0).localScale -= new Vector3 (0.1f, 0.1f, 0.1f);
				for (int i = 0; i < tList.Count; i++) {
					tList[i].localScale -= new Vector3 (0.1f, 0.1f, 0.1f);
				}
			}
		}
		yield return new WaitForSeconds (waitTime);
        ObjectPool.Instance.RecycleObj("Magic", gameObject);
	}
}
