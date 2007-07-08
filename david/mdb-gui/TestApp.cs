using System;
class TestApp
{
	public static void Main(string[] args)
	{
		int _int = 123;
		//string _string = "text";
		bool _bool = true;
		double _double = 3.14;
		int[] _array = {1, 2, 3};
		TestClass testObject = new TestClass();
		Fun1();
	}
	
	public static void Fun1()
	{
		object obj = new object();
		Fun2();
	}
	
	public static void Fun2()
	{
		string msg = "Hello world";
		Console.WriteLine(msg);
	}
}

public class TestClass
{
	public int _int = 123;
	//public string _string = "text";
	public bool _bool = true;
	public double _double = 3.14;
	public int[] _array = {1, 2, 3};
}
