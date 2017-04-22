using System.Collections;
using System.Collections.Generic;
using Global;
using UnityEngine;

public class DiaSqu : MonoBehaviour
{
	public int size;
	public float Roughnees = 0.5f;

	Material mat;
	float[,] map;
	Texture2D GeneratedTexture;

	void Awake()
	{
		size++;

		mat = gameObject.GetComponent<Renderer>().material;
		GeneratedTexture = new Texture2D(size, size);

		map = new float[size, size];
		map[0, 0] = Random.Range(0.3f, 0.6f);
		map[0, size - 1] = Random.Range(0.3f, 0.6f);
		map[size - 1, size - 1] = Random.Range(0.3f, 0.6f);
		map[size - 1, 0] = Random.Range(0.3f, 0.6f);

	}
	void Start()
	{
		//	Задаем стартовые точки - левый нижний и правый верхний углы
		Vec2int Left = new Vec2int(0, 0);
		Vec2int Right = new Vec2int(size - 1, size - 1);
		GeterateMap (Left, Right);

		mat.mainTexture = GeneratedTexture;
		mat.SetTexture ("_BumpMap", GeneratedTexture);
	}
	public List<Quad[]> GenerateList(ref List<Quad[]> input)
	{
		//	Создание нового списка в который будут записываться подквадраты текущего
		List<Quad[]> next = new List<Quad[]>();
		foreach (Quad[] Arr in input)
			foreach (Quad item in Arr)
				next.Add(Square(item));
		foreach (Quad[] Arr in input)
			foreach (Quad item in Arr)
				diamond(item);
		input.Clear ();
		return next;
	}
	public Quad[] Square(Quad box)
	{
		Vec2int Center = new Vec2int(box.R.x - box.HalfLength, box.R.y - box.HalfLength);
		map[Center.x, Center.y] = (map[box.L.x, box.L.y] + map[box.L.x, box.R.y] + map[box.R.x, box.R.y] + map[box.R.x, box.L.y]) / 4 + Random.Range(-box.Length * Roughnees / (size - 1), box.Length * Roughnees / (size - 1));
		return new Quad[]{
			new Quad(box.L,Center),
			new Quad(Center,box.R),
			new Quad(new Vec2int (box.L.x, box.L.y + box.HalfLength),new Vec2int (box.R.x - box.HalfLength, box.R.y)),
			new Quad(new Vec2int (box.L.x+box.HalfLength, box.L.y),new Vec2int (box.R.x , box.L.y+box.HalfLength))
		};
	}
	public void diamond(Quad box)
	{
		float a, b, c, d;
		Vec2int Center = new Vec2int(box.L.x + box.HalfLength, box.R.y - box.HalfLength);
		Vec2int[] points = new Vec2int[] {
			new Vec2int (Center.x-box.HalfLength,Center.y), //Left
			new Vec2int (Center.x,Center.y+box.HalfLength), //Top
			new Vec2int (Center.x+box.HalfLength,Center.y), //Right
			new Vec2int (Center.x,Center.y-box.HalfLength)  //Bottom
		};
		foreach (Vec2int point in points)
		{
			if (point.y - box.HalfLength >= 0)
				a = map[point.x, point.y - box.HalfLength];
			else
				a = map[point.x, size - 1 - box.HalfLength];
			if (point.x - box.HalfLength >= 0)
				b = map[point.x - box.HalfLength, point.y];
			else
				b = map[size-1 - box.HalfLength, point.y];
			if (point.y + box.HalfLength < size - 1)
				c = map[point.x, point.y + box.HalfLength];
			else
				c = map[point.x, box.HalfLength];
			if (point.x + box.HalfLength < size - 1)
				d = map[point.x + box.HalfLength, point.y];
			else
				d = map[box.HalfLength, point.y];
			map[point.x, point.y] = (a + b + c + d) / 4 + Random.Range(-box.Length * Roughnees / (size - 1), box.Length * Roughnees / (size - 1));
		}
	}

	void GeterateMap (Vec2int Left, Vec2int Right)
	{
		//	Инициализация списка массивов квадратов
		List<Quad[]> box = new List<Quad[]> ();
		//	Добавление в список первого квадрата
		box.Add (new Quad[] {
			new Quad (Left, Right)
		});
		//Генерация Diamond-Square
		for (int l = size; l > 0; l /= 2)
			box = GenerateList (ref box);
		//	Запись в текстуру
		for (int i = 0; i < size; i++)
			for (int j = 0; j < size; j++)
				GeneratedTexture.SetPixel (i, j, new Color (map [i, j], map [i, j], map [i, j], 0));
		GeneratedTexture.filterMode = FilterMode.Trilinear;
		GeneratedTexture.Apply ();
		box.Clear ();
	}

	// Структура определяющая квадрат
	public struct Quad
	{
		//	Левая нижняя точка квадрата
		public Vec2int L;
		//	Правая верхняя точка квадрата
		public Vec2int R;
		//	Длинна ребра
		public int Length;
		//	Половина длинны ребра
		public int HalfLength;
		//	Конструкторы
		public Quad(Vec2int L, Vec2int R)
		{
			this.L = L;
			this.R = R;
			Length = R.x - L.x;
			HalfLength = Length / 2;	
		}
		public Quad(int LeftX , int LeftY, int RightX , int RightY)
		{
			this.L = new Vec2int(LeftX,LeftY);
			this.R = new Vec2int(RightX,RightY);
			Length = R.x - L.x;
			HalfLength = Length / 2;
		}
	}
}