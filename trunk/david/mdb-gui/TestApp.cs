using System;
class TestApp
{
	public static void Main(string[] args)
	{
		Colours myColour = Colours.Green | Colours.Blue;
		TestClass testClass = new TestClass();
		testClass.TestFun(543, "asdf");
		Fun1();
	}
	
	public static void Fun1()
	{
		while(true) {}
		object obj = new object();
		Fun2();
	}
	
	public static void Fun2()
	{
		string msg = "Hello world";
		Console.WriteLine(msg);
		throw new Exception("Test unhandled exception");
		try {
			throw new Exception("Test handled exception");	
		} catch {
		}
	}
	
	public static void Arrays()
	{
		int[] empty = {};
		int[] oneDim = {1, 2, 3};
		int[,] twoDim = {{1,2},{3,4}};
		int[,,] threeDim = {{{1},{2}},{{3},{4}}};
		int[] oneDimBig = new int[20];
		int[,] twoDimBig = new int[20,20];
		int[][] jagged = {new int[2], new int[3]};
	}
}

[Flags]
enum Colours { Red = 1, Green = 2, Blue = 4 };

public class MyClass
{
	public int _int = 456;
}

public class TestClass: MyClass
{
	public static int staticInt = 31415;
	public int _int = 123;
	//public string _string = "text";
	public bool _bool = true;
	public double _double = 3.14;
	public int[,] _array = {{1,2}, {3,4}, {5,6}};
	
	public void TestFun(int i, string s)
	{
		Console.WriteLine(i + s);
	}
}
