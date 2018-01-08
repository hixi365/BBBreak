using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ゲームオブジェクト
//  反射する基本的な弾

public class ReflectBullet : MonoBehaviour {

    // 弾の状態
    //  NONE      未指定
    //  ENEMY     下に当たればミス扱い 敵はすり抜ける
    //  PLAYER    下に当たってもミスにならない 敵にダメージを与える
    //  EX_PLAYER 壁面でのみ反射する貫通弾 貫通した敵にダメージを与える
    enum ShotState { NONE, ENEMY, PLAYER, EX_PLAYER };

    // 衝突レイキャストオフセット
    enum RaycastOffset { CENTER, L90, R90, L45, R45 };
   
    private ShotState stateShot = ShotState.NONE;

    // const
    private const int MAX_REFRECT_RAYCAST_TRY = 10; // 反射時のレイキャスト再計算許容回数

    // transform
    private Vector3 vecMove;		// 弾の移動ベクトル
	private Vector3 posCurrent;		// 弾の座標 transformへ書き込む
	private float angleRoll;        // z軸回転速度
	private float angleCurrent;     // z軸回転 transformへ書き込む

    // SerializeField layer
    [SerializeField]
    private LayerMask layerRefrect;		// 反射するレイヤータグ
	[SerializeField]
    private LayerMask layerPlayer;		// プレイヤーレイヤータグ
	[SerializeField]
    private LayerMask layerFailed;		// ミスするレイヤータグ
	[SerializeField]
    private LayerMask layerBlock;		// ブロックレイヤータグ
	[SerializeField]
	private LayerMask layerLifeBlock;   // ライフブロックレイヤータグ
	[SerializeField]
	private LayerMask layerBroken;		// 破壊済ブロックレイヤータグ

    // SerializeField color
    [SerializeField]
    private Color colorEnemys;       // 敵弾色
    [SerializeField]
    private Color colorPlayers;      // 自弾色

    // SerializeField game setting
    [SerializeField]
    private float degMaxPlayerRefrect = 45;    // プレイヤーの端部に当たった最大反射角 (45 : 10.5時 ～ 1.5時)
	[SerializeField]
	private float autoDestroySecondsNoReflect = 5;	// 一定時間接触が無かった場合、削除する

	// private
	private SpriteRenderer rendererOwn; // 自身のスプライトレンダラ
	private int layerValueBroken;        // 破壊済ブロックレイヤー (書き込み用)
	private float pixelsSprite;           // スプライトの大きさ
	private float secondsNoReflect = 0f;	// 接触していない時間

	private void Awake()
	{

        // レンダラの取得
		rendererOwn = gameObject.GetComponent<SpriteRenderer>();
		SpritePixelSize();

		// 破壊済みブロックに書き込むレイヤ値の計算
		layerValueBroken = 0;
		int l = layerBroken;
        while (0 < (l >>= 1))
        {
			layerValueBroken++;
        }

		// NONEの場合 ランダムで初期化
		if (stateShot == ShotState.NONE)
		{
			SetRandomMoveVec();
		}

	}

    private void Start()
	{

		// transformの取得
		posCurrent = transform.localPosition;
		angleCurrent = transform.localRotation.eulerAngles.z;

		// 弾色の初回更新
		UpdateSpriteColor();

	}

	private void Update()
	{

		// 一定時間、反射が無ければ削除する
		secondsNoReflect += Time.deltaTime;
		if(secondsNoReflect > autoDestroySecondsNoReflect)
		{

			Destroy(gameObject);
			return;

		}

		// レイキャストによる弾の移動
		RaycastMove();

		// 弾の回転
		angleCurrent += angleRoll * Time.deltaTime;

		// transform 書き込み
		transform.position = posCurrent;
		transform.rotation = Quaternion.Euler(0f, 0f, angleCurrent);

	}

    // スプライトのピクセルサイズを取得
    private void SpritePixelSize()
    {

        if(rendererOwn != null)
        {

            if(rendererOwn.sprite != null)
            {

                // 取得してreturn
                pixelsSprite = rendererOwn.sprite.pixelsPerUnit;
                return;

            }

        }

        // 取得できなかった場合
        pixelsSprite = 0f;

    }

    // 弾の状態に合わせた色を返す
    private Color GetShotColor()
    {

        // 未指定時は白を返す
        Color ret = Color.white;

        switch (stateShot)
        {

            case ShotState.ENEMY:

                ret = colorEnemys;
                break;

            case ShotState.PLAYER:

                ret = colorPlayers;
                break;

        }

        return ret;

    }

    // 現在の状態で弾の色を更新
    private void UpdateSpriteColor()
    {

        if(rendererOwn != null)
        {

            rendererOwn.color = GetShotColor();

        }

    }

	// 弾速ベクトルと回転速度の代入
	private void SetMoveVecAndRoll(Vector3 vecMove, float zRoll)
	{

		this.vecMove = vecMove;
		angleRoll = zRoll;

	}

    // ランダムにNONE弾の生成 (テスト用)
    public void SetNoneShot(Vector3 vecMove, float zRoll)
    {

		stateShot = ShotState.NONE;

		SetMoveVecAndRoll(vecMove, zRoll);

	}

    // 敵弾の生成
    public void SetEnemyShot(Vector3 vecMove, float zRoll)
    {

		stateShot = ShotState.ENEMY;

		SetMoveVecAndRoll(vecMove, zRoll);

	}

	// 自弾の生成
	public void SetPlayerShot(Vector3 vecMove, float zRoll)
    {

        stateShot = ShotState.PLAYER;

		SetMoveVecAndRoll(vecMove, zRoll);

	}

	// 自弾状態へ更新
	private void ToPlayerShot()
	{

		// 敵弾であれば自弾へ変更
		if (stateShot == ShotState.ENEMY)
		{

			stateShot = ShotState.PLAYER;
			UpdateSpriteColor();

		}

	}

	// ランダムな移動方向で弾速ベクトルを決定する
	public void SetRandomMoveVec(float speed = 1f)
	{

		float angle = Random.Range(0f, Mathf.PI * 2);
		Vector3 vec = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * speed;

		vecMove = vec;

	}

	// 移動
	private void RaycastMove()
	{

		float circle_r = gameObject.transform.lossyScale.x * 2 / (pixelsSprite/ 2);

		Vector2 vec_n = (Vector2)Vector3.Normalize(vecMove);    // 進む方向
		Vector2 pos = (Vector2)posCurrent;                      // 座標

		float vec_power = Vector3.Distance(Vector2.zero, vecMove) * Time.deltaTime; // 進む距離

		// レイキャストを繰り返す
		for (int i_ray = 0; i_ray < MAX_REFRECT_RAYCAST_TRY; i_ray++)
		{

			bool hit = RayCastCircle(ref pos, ref vec_n, ref vec_power, circle_r);

			if (hit == false)
			{
				break;
			}

			else
			{
				// 接触していない時間をリセット
				secondsNoReflect = 0f;
			}

		}

		vecMove = Vector3.Distance(Vector3.zero, vecMove) * Vector3.Normalize(vec_n);
		posCurrent = pos + vec_n * vec_power;

	}

    // 弾の半径分でレイキャスト
	//  戻り値が false であれば再計算の必要なし
    private bool RayCastCircle(ref Vector2 pos, ref Vector2 n_vec, ref float power, float radius)
    {

        // 円のレイキャスト
        Vector2[] offset = new Vector2[5];
        offset[(int)RaycastOffset.CENTER] = radius * n_vec;                                             // raycast offset (正面)
        offset[(int)RaycastOffset.L90] =
            new Vector2(-offset[(int)RaycastOffset.CENTER].y, +offset[(int)RaycastOffset.CENTER].x);    // raycast offset (90度)
		offset[(int)RaycastOffset.R90] = -offset[(int)RaycastOffset.L90];                               // raycast offset (90度)
		offset[(int)RaycastOffset.L45] =
            (offset[(int)RaycastOffset.CENTER] + offset[(int)RaycastOffset.L90]) / 2;                   // raycast offset (45度)
		offset[(int)RaycastOffset.R45] =
            (offset[(int)RaycastOffset.CENTER] + offset[(int)RaycastOffset.R90]) / 2;                   // raycast offset (45度)

		//// ray 確認
		//Debug.DrawRay((Vector3)(pos + offset[0]) + new Vector3(0, 0, 1), n_vec * power, new Color(1f, 0f, 0f));
		//Debug.DrawRay((Vector3)(pos + offset[1]) + new Vector3(0, 0, 1), n_vec * power, new Color(1f, 0f, 0f));
		//Debug.DrawRay((Vector3)(pos + offset[2]) + new Vector3(0, 0, 1), n_vec * power, new Color(1f, 0f, 0f));
		//Debug.DrawRay((Vector3)(pos + offset[3]) + new Vector3(0, 0, 1), n_vec * power, new Color(1f, 0f, 0f));
		//Debug.DrawRay((Vector3)(pos + offset[4]) + new Vector3(0, 0, 1), n_vec * power, new Color(1f, 0f, 0f));

		// 衝突対象
		LayerMask collisionLayer = layerRefrect + layerFailed + layerLifeBlock + layerPlayer;

		// プレイヤー弾 敵のブロックに衝突
		if(stateShot == ShotState.PLAYER || stateShot == ShotState.EX_PLAYER)
		{
			collisionLayer += layerBlock;
		}

		RaycastHit2D[] c_hit = new RaycastHit2D[5];
		for (int i = 0; i < 5; i++)
			c_hit[i] = Physics2D.Raycast(pos + offset[i], n_vec, power, collisionLayer);

		// 衝突
		int c_index = -1;
		for (int i = 0; i < 5; i++)
		{
			if (c_hit[i].collider && c_hit[i].fraction > 0f)
			{
				c_index = i;
				break;
			}
		}

		// 衝突がなければ
		if (c_index == -1)
		{
			return false;
		}

		// 計算に用いる衝突情報
		RaycastHit2D c = c_hit[c_index];

		// 進んだ距離の計算
		float dis = c.fraction * power;

		Vector2 new_n_vec = n_vec;

		// 衝突対象が壁であれば反射
		if (1 << c.collider.gameObject.layer == layerRefrect.value)
		{

			new_n_vec = DefaultRefrect(c, n_vec);

		}

		// プレイヤーであれば角度を当たった位置で変更 (側面は端に当たったとして前方に飛ばす)
		else if (1 << c.collider.gameObject.layer == layerPlayer.value)
		{

			new_n_vec = PlayerRefrect(c);
			ToPlayerShot();

		}

		// ブロックであれば破壊
		else if (1 << c.collider.gameObject.layer == layerBlock.value)
		{

			BrokenBlock(c.collider.gameObject);

			// 通常弾であれば反射
			// EX弾であれば反射しない

			if (stateShot == ShotState.PLAYER)
			{

				new_n_vec = DefaultRefrect(c, n_vec);

			}

		}

		// ライフブロックであれば衝突通知を行うか判定し、自身を削除
		else if (1 << c.collider.gameObject.layer == layerLifeBlock.value)
		{

			// 敵弾であればライフブロックに通知
			if(stateShot == ShotState.ENEMY)
			{

				LifePointBlock lifeBlock = c.collider.gameObject.GetComponent<LifePointBlock>();
				
				if(lifeBlock != null)
				{

					lifeBlock.OnHit();

				}

			}

			Destroy(gameObject);
			return false;

		}

		// ミス扱い
		else if (1 << c.collider.gameObject.layer == layerFailed.value)
		{

			BrokenBlock(c.collider.gameObject);	// レイヤーにフラグ書き込み
			Destroy(gameObject);
			return false;

		}

		pos = c.point - offset[c_index];

		n_vec = new_n_vec;
		power -= dis;

		return true;

	}

	// 通常オブジェクトの反射
	private Vector2 DefaultRefrect(RaycastHit2D c, Vector2 n_vec)
	{

		// 回転
		angleRoll = -angleRoll;

		return n_vec - 2 * c.normal * Vector2.Dot(n_vec, c.normal);

	}

	// プレイヤーによる反射
	private Vector2 PlayerRefrect(RaycastHit2D c)
	{

		// プレイヤー座標
		Transform tPlayer = c.collider.transform;
		Transform tPlayerParent = tPlayer.parent;
		float player_w = tPlayer.localScale.x;
		float x_left = tPlayerParent.localPosition.x - player_w;
		float x_right = tPlayerParent.localPosition.x + player_w;

		// プレイヤーのどこに当たったか  ( -1 < p < +1 )
		float p = (c.point.x - x_left) / (x_right - x_left);
		p = (p - 0.5f) * 2f;

		// x成分から反射角度の計算
		float abs_x = Mathf.Sin(degMaxPlayerRefrect);
		float new_x = abs_x * p;
		float angle = Mathf.Asin(new_x);

		// 回転
		angleRoll = -angle * 1000f;

		return new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));

	}

    // ブロックの破壊
    private void BrokenBlock(GameObject block)
	{

		block.layer = layerValueBroken;

	}

}
