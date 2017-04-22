using UnityEngine;
namespace Global
{
	public enum ControllerType
	{
		continuous,     //Непрерывный
		discontinuous   //Прерывистый
	}
	public static class Variables
	{
		public static ControllerType TypeOfController = ControllerType.continuous;
	}
	public struct Vec2int
	{
		public int x;
		public int y;
		public Vec2int(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
	}

}
