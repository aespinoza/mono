using System;
using Mono.CSharp;

public class MyTest {
	static void Run (string id, string stmt)
	{
		if (!Evaluator.Run (stmt))
			Console.WriteLine ("Failed on test {0}", id);
	}

	static void Evaluate (string id, string expr, object expected)
	{
		try {
			object res = Evaluator.Evaluate (expr);
			if (res == null && expected == null)
				return;

			if (!expected.Equals (res)){
				Console.WriteLine ("Failed on test {2} Expecting {0}, got {1}", expected, res, id);
				throw new Exception ();
			}
		} catch {
			Console.WriteLine ("Failed on test {0}", id);
			throw;
		}
	}
	
	static void Main ()
	{
		Evaluator.Init (new string [] { "-v", "-v" });
		Evaluate ("multiply", "1*2;", 2);
		Run ("1",      "System.Console.WriteLine (100);");
		Run ("Length", "var a = new int [] {1,2,3}; var b = a.Length;");
		
		Evaluate ("CompareString", "\"foo\" == \"bar\";", false);
		Evaluate ("CompareInt", "var a = 1; a+2;", 3);

		Evaluator.Run ("using System; using System.Linq;");
		Run ("LINQ-1", "var a = new int[]{1,2,3};\nfrom x in a select x;");
		Run ("LINQ-2", "var a = from f in System.IO.Directory.GetFiles (\"/tmp\") where f == \"passwd\" select f;");

		Evaluator.ReferenceAssembly (typeof (MyTest).Assembly);
		Evaluate ("assembly reference test", "typeof (MyTest) != null;", true);

		Run ("LINQ-3", "var first_scope = new int [] {1,2,3};");
		Run ("LINQ-4", "var second_scope = from x in first_scope select x;");

		string prefix = "";
		string [] res = Evaluator.GetCompletions ("ConsoleK", out prefix);
		if (res [0] != "ey" || res [1] != "eyInfo"){
			Console.WriteLine (res [0]);
			Console.WriteLine (res [1]);
			throw new Exception ("Expected two completions ConsoleKey and ConsoleKeyInfo");
		}

		res = Evaluator.GetCompletions ("Converte", out prefix);
		if (res [0] != "r<"){
			throw new Exception ("Expected one completion for Conveter<");
		}

		res = Evaluator.GetCompletions ("Sys", out prefix);
		if (res [0] != "tem"){
			throw new Exception ("Expected at least a conversion for System");
		}

		res = Evaluator.GetCompletions ("System.Int3", out prefix);
		if (res [0] != "2"){
			throw new Exception ("Expected completion to System.Int32");
		}
	}
	
}