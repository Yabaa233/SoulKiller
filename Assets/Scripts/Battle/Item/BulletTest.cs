using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>


////// 弾丸のテスト


///</summary>
public class BulletTest : MonoBehaviour
{
    //移動速度
    public float speed = 20f;
    public GameObject shotter;//Launcher
    public GameObject muzzlePrefab;
	public GameObject hitPrefab;
    public List<GameObject> trails;
    //物理学
    Rigidbody myRigidbody;

    //初期化
    public void Awake()
    {
        myRigidbody = gameObject.AddComponent<Rigidbody>();
        myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        myRigidbody.useGravity = false;
    }

    /// <summary>
    /// テストの初期化
    /// </summary>
    public void Init()
    {
        
    }

    public void SetShotter(GameObject gameObject)
    {
        shotter = gameObject;
    }

    private void Start()
    {
        //特殊効果リソースの接続
        // if(muzzlePrefab != null)
        // {
        //     var muzzleVFX = Instantiate (muzzlePrefab, transform.position, Quaternion.identity);
        //     muzzleVFX.transform.forward = gameObject.transform.forward;
        //     var ps = muzzleVFX.GetComponent<ParticleSystem>();
        //     // if (ps != null)
		// 	// 	// Destroy (muzzleVFX, ps.main.duration);
        //     //     // EffectManager.Instance.LetDestroyEffect(muzzleVFX);
		// 	// else {
		// 	// 	var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
		// 	// 	// Destroy (muzzleVFX, psChild.main.duration);
        //     //     // EffectManager.Instance.LetDestroyEffect(muzzleVFX);
		// 	// }
        // }
        // myRigidbody.velocity = transform.forward * speed;
    }

    private void OnEnable() {//往復活性化後に速度が同じであることを確認してください。
        // //特殊効果リソースの接続
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
        myRigidbody.useGravity = false;
    }

    /// <summary>
    /// テストの更新
    /// </summary>
    private void Update() {
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
    }

    private void OnCollisionEnter(Collision other)
    {
        // Debug.Log("Begin detection");
        //特殊効果処理
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

        //ダメージ計算
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            StartCoroutine(DestroyParticle(0f));
            return;
        }
        else
        {
            if (other.transform.tag == "Player")
            {
                if (shotter != null)
                {
                    GameManager.Instance.EnemyAttack(shotter.GetComponent<BaseEnemyControl>());
                }
            }
            else if (other.gameObject.layer == 12)//盾に当たっても一度ダメージを与える必要があります。
            {
                if (shotter != null)
                {
                    GameManager.Instance.EnemyAttack(shotter.GetComponent<BaseEnemyControl>());
                }
            }
        }

        StartCoroutine(DestroyParticle(0f));
    }

    ///<summary>


    ////// 試験の終了


    ///</summary>
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
        ObjectPool.Instance.RecycleObj("Bullet", gameObject);
	}


}
