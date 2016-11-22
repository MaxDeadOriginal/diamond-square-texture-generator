using UnityEngine;
using System.Collections;

public class DiaSqu : MonoBehaviour {
	public int size = 32;
	[Range(0,1f)]
	public float Roughnees = 0.5f;
	private float[,] map;

	void Awake()
	{
		size++;
	}

	void Start () {
		Material mat = gameObject.GetComponent<Renderer> ().material;

		map = new float[size,size];
		Texture2D txt = new Texture2D (size, size);

		Vec2int Left = new Vec2int (0, 0);
		Vec2int Right = new Vec2int (size - 1, size - 1);

		map [Left.x, Left.y] = 0.5f;
		map [Left.x, Right.y] = 0.5f;
		map [Right.x, Left.y] = 0.5f;
		map [Right.x, Right.y] = 0.5f;

		int l = (Right.x - Left.x) / 2;

		square (Left, Right, l);
		diamondSquare(Left, Right, l);

		for (int i = 0; i < size; i++) {
			for (int j = 0; j < size; j++) {
				txt.SetPixel (i, j, new Color (map [i, j], map [i, j], map [i, j], 0));
			}
		}
		txt.filterMode = FilterMode.Point;
		txt.Apply ();

		mat.mainTexture = txt;
	}

	public void square(Vec2int L, Vec2int R, int l)
	{
		Vec2int Center = new Vec2int (R.x - l, R.y - l);
		float a = map [L.x, L.y];
		float b = map [L.x, R.y];
		float c = map [R.x, R.y];
		float d = map [R.x, L.y];
		map [Center.x, Center.y] = (a + b + c + d) / 4 + Random.Range (-l / (size - 1) * (Roughnees), l / (size - 1) * (Roughnees));
	}

	public void diamond(Vec2int point,int l)
	{
		float a, b, c, d;

		if (point.y - l >= 0)
			a = map [point.x, point.y - l];
		else
			a = map[point.x, size - l];  
		
		if (point.x - l >= 0)
			b = map [point.x - l, point.y];
		else
			b = map [size - l, point.y];
		
		if (point.y + l < size)
			c = map [point.x, point.y + l];
		else
			c = map [point.x, l];    
		
		if (point.x + l < size)
			d = map [point.x + l, point.y];
		else
			d = map [l, point.y];

		map [point.x, point.y] = (a + b + c + d) / 4 + Random.Range (-l / (size - 1) * Roughnees, l / (size - 1) * Roughnees);
	}

	public void diamondSquare(Vec2int L, Vec2int R, int l)
	{
		if (l > 0) {
			Vec2int[] points = GetPoints(L, R, l);
			foreach (Vec2int elem in points) {
				diamond (elem, l);
			}
			square (L, new Vec2int (points [3].x, points [2].y), l);
			square (new Vec2int (points [3].x, points [2].y), R, l);
			square (points [0], points [1], l);
			square (points [3], points [2], l);

			diamondSquare (L, new Vec2int (points [3].x, points [2].y), l / 2);
			diamondSquare (new Vec2int (points [3].x, points [2].y), R, l / 2);
			diamondSquare (points [0], points [1], l / 2);
			diamondSquare (points [3], points [2], l / 2);
		}
	}

	public static Vec2int[] GetPoints(Vec2int L, Vec2int R, int l)
	{
		return new Vec2int[] {
			new Vec2int (L.x, L.y + l),
			new Vec2int (R.x - l, R.y),
			new Vec2int (R.x, R.y - l),
			new Vec2int (L.x + l, L.y)
		};
	}
	// 0-3 это индексы массива
	//  p---- 1 ----p
	//  |           |
	//  0           2
	//  |           |
	//  p---- 3 ----p

	public struct Vec2int
	{
		public int x;
		public int y;
		public Vec2int(int x,int y){
			this.x=x;
			this.y=y;
		}
	}
}
