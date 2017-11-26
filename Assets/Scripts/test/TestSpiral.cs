using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// テスト
// 敵 : 渦巻きクラス

public class TestSpiral : MonoBehaviour {

    // preafab
    public GameObject prefabBlock;              // 構成するブロック 破壊可能なブロック
//    public GameObject prefabDontBreakBlock;     // 構成するブロック 破壊不可能なブロック (跳ね返す)

    public Vector3 vecMove;			// 移動速度
	public Vector3 posCurrent;      // 現在の座標

	// 色
	public Color colIn = Color.blue;			// うずまきの色 中
	public Color colOut = Color.green;          // うずまきの色 外

    // preafabの大きさ (指定する)
    private Vector3 sizePrefabBlock;            // set preafab transform localScale
    private Vector3 sizePrefabDontBreakBlock;   // set preafab transform localScale

    // 大きさ
    private float scaleRad = 0.15f;

    // ブロックの数
    private int divRad = 15;		// 半径分のブロック分割数
    private int divAngle = 6;       // 360を何本の線で分割するか

	private float time = 0;
	private float timeTestShot = 0;

	private void Awake()
	{

		// 大きさの初期化 (本来はinspectorで)
		sizePrefabBlock = new Vector3(0.02f, 0.02f, 1f);
		sizePrefabDontBreakBlock = new Vector3(0.02f, 0.02f, 1f);

	}

	private void Start()
	{

		CreateSpiral();

		posCurrent = new Vector3();
		vecMove = new Vector3();

    }

    private void Update()
    {

        TestMove();

    }

    private void CreateSpiral()
    {

        // 角度分割
        for(int iAngle = 0; iAngle < divAngle; iAngle++)
        {

            float angle_base = 2 * Mathf.PI * iAngle / divAngle; 

            // 線分割
            for(int iRad = 0; iRad < divRad; iRad++)
            {

                float a_rad = (float)iRad / divRad;

                float rad = scaleRad * a_rad;
                float angle_add = 3.0f * a_rad;

                float x = rad * Mathf.Cos(angle_base + angle_add);
                float y = rad * Mathf.Sin(angle_base + angle_add);

                Color col = Color.Lerp(colIn, colOut, a_rad);

                Vector3 pos = transform.localPosition + new Vector3(x, y, 0f);
                GameObject obj = Instantiate(prefabBlock, pos, Quaternion.identity, transform);
                obj.transform.localScale = sizePrefabBlock;
                obj.GetComponent<SpriteRenderer>().color = col;
                obj.GetComponent<SpriteRenderer>().sortingLayerID = SortingLayer.NameToID("Head");
                AddRandomAppear(obj);

            }


        }

    }

    private void AddRandomAppear(GameObject obj, float delay = 0f)
    {

        TestAppearBlock appear = obj.AddComponent<TestAppearBlock>();

        float a = Random.Range(0, 2 * Mathf.PI);
        float rad = 1.0f;
        float roll = Random.Range(-Mathf.PI, +Mathf.PI) * 3f;

        Vector3 offsetPos = new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0f) * rad;
        Vector3 offsetRot = new Vector3(0f, 0f, roll);

        appear.fromPos = appear.transform.localPosition - offsetPos;
        appear.fromRot = appear.transform.localRotation.eulerAngles - offsetRot;

        appear.toPos = appear.transform.localPosition;
        appear.toRot = appear.transform.localRotation.eulerAngles;

        appear.delay = delay;

    }

    private void TestMove()
    {

        float dt = Time.deltaTime;
        transform.Rotate(0f, 0f, dt * 100.0f);

        time += dt;
        transform.position = new Vector3(0.4f * Mathf.Sin(time), -0.3f, 0f);

    }

}
