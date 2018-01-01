using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// テスト
//  敵弾の発射

public class Test_EnemyShotGenerator : MonoBehaviour {

	[SerializeField]
	private GameObject prefabShot;

	[SerializeField]
	private float interval = 1f;

	private float timeTotal = 0f;

	private void Update ()
	{

		float dt = Time.deltaTime;
		timeTotal += dt;

		if(timeTotal >= interval)
		{

			// 先に時間を更新する
			timeTotal -= interval;	

			GameObject obj = Instantiate(prefabShot, transform.position, Quaternion.identity, transform);
	
			if(obj == null)
			{

				return;

			}

			ReflectBullet bullet = obj.GetComponent<ReflectBullet>();

			if (bullet == null)
			{

				return;

			}

			bullet.SetEnemyShot(Vector3.zero, 0f);
			bullet.SetRandomMoveVec(1f);

		}

	}

}
