using System;
class TestApp
{
	public static void Main(string[] args)
	{
		int number = 1234;
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
