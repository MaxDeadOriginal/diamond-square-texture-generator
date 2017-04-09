using UnityEngine;
using System.Collections;

public class DiaSqu : MonoBehaviour {
	// Объявление и инициализация размера текстуры и шерховотости(рельефности)
	public int size = 32;
	[Range(0,1f)]
	public float Roughnees = 1.5f;
	
	// Объявление массива высот и координат, левого нижнего и правого верхнего, углов
	private float[,] map;
	private Vec2int Left;
	private Vec2int Right;

	void Start () {
		size++;
		map = new float[size,size];
		Texture2D txt = new Texture2D (size * 2-1, size);

		// Инициализация координат, левого нижнего и правого верхнего, углов
		Left = new Vec2int (0, 0);
		Right = new Vec2int (size-1, size-1);

		// Инициализация значений на углах
		map [Left.x, Left.y] = 0.5f;
		map [Left.x, Right.y] = 0.5f;
		map [Right.x, Left.y] = 0.5f;
		map [Right.x, Right.y] = 0.5f;
	
		Generate();

		// Запись значений из массива в текстуру
		for (int i = 0, a = 0; i < size*2-1; i++, a++) {
			for (int j = 0, b = 0; j < size; j++, b++) {
				if (a > size-2) {
					a = 0;
					b = 0;
				}
				txt.SetPixel (i, j, new Color (map [a, b], map [a, b], map [a, b], 0));
			}
		}
		txt.filterMode = FilterMode.Point;
		txt.Apply ();
		
		// Вывод текстуры
		Material mat = gameObject.GetComponent<Renderer> ().material;
		mat.mainTexture = txt;
	}

	// Получение высоты цнтра квадрата опредеоенного точками: L(левая нижняя)
	//							  R(правая верхняя)
	// используя значения 
	private void square(Vec2int L, Vec2int R)
	{
		int l = (R.x - L.x) / 2;
		if (l > 1) {
			Vec2int Center = new Vec2int (R.x - l, R.y - l);
			float a = map [L.x, L.y];
			float b = map [L.x, R.y];
			float c = map [R.x, R.y];
			float d = map [R.x, L.y];
			map [Center.x, Center.y] = (a + b + c + d) / 4 + Random.Range (-l * (Roughnees / size), l * (Roughnees / size));
		}
	}

	// Получение высот ребер квадрата опредеоенного по его центру(point) и длинне ребра(l)
	private void diamond(Vec2int point,int l)
	{
		float a, b, c, d;
		
		if (point.y - l >= 0)
			a = map [point.x, point.y - l];
		else
			a = map [point.x, size - l];  
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

		map [point.x, point.y] = (a + b + c + d) / 4 + Random.Range (-l * (Roughnees / size), l * (Roughnees / size));
	}

	private void diamondSquare(Vec2int L,Vec2int R)
	{
		int l = (R.x - L.x) / 2;
		if (l > 1) {
			//square (L, R);
			Vec2int[] points = GetPoints (L, R, l);
			foreach (Vec2int elem in points)
				diamond (elem, l);
			
			square (L, new Vec2int (l, l));
			square (new Vec2int (l, l), R);
			square (points [0], points [3]);
			square (points [1], points [2]);

			diamondSquare (L, new Vec2int (l, l));
			diamondSquare (new Vec2int (l, l), R);
			diamondSquare (points [0], points [3]);
			diamondSquare (points [1], points [2]);
		}
	}

	// Возвращает массив состоящий из координат центров ребер квадрата определенного L и R
	// Так же принимает hl(тип halfLenght) половину длинны ребра
	private static Vec2int[] GetPoints(Vec2int L,Vec2int R, int hl)
	{
		return new Vec2int[] { 
			new Vec2int (L.x, L.y + hl),
			new Vec2int (L.x + hl, L.y), 
			new Vec2int (R.x, R.y - hl),
			new Vec2int (R.x - hl, R.y)
		};
	}
	
	public void Generate(){
		square (Left, Right);
		diamondSquare (Left, Right);
	}

	private struct Vec2int
	{
		public int x;
		public int y;
		public Vec2int(int x,int y){
			this.x=x;
			this.y=y;
		}
	}
}
