using System;

namespace Mono.Debugger.Frontend
{
	public static class Pixmaps
	{
		const string imageBase = "Mono.Debugger.Frontend.pixmaps.Icons.16x16.";
		
		public static Gdk.Pixbuf DebugBreak       = Load("Debug.Break.png");
		public static Gdk.Pixbuf DebugContinue    = Load("Debug.Continue.png");
		public static Gdk.Pixbuf DebugStart       = Load("Debug.Start.png");
		public static Gdk.Pixbuf DebugStartWithoutDebugging = Load("Debug.StartWithoutDebugging.png");
		public static Gdk.Pixbuf DebugStepInto    = Load("Debug.StepInto.png");
		public static Gdk.Pixbuf DebugStepOut     = Load("Debug.StepOut.png");
		public static Gdk.Pixbuf DebugStepOver    = Load("Debug.StepOver.png");
		public static Gdk.Pixbuf DebugStopProcess = Load("Debug.StopProcess.png");
		
		public static Gdk.Pixbuf Error = Load("Error.png");
		
		public static Gdk.Pixbuf PublicClass     = Load("PublicClass");
		public static Gdk.Pixbuf PublicDelegate  = Load("PublicDelegate");
		public static Gdk.Pixbuf PublicEnum      = Load("PublicEnum");
		public static Gdk.Pixbuf PublicEvent     = Load("PublicEvent");
		public static Gdk.Pixbuf PublicField     = Load("PublicField");
		public static Gdk.Pixbuf PublicInterface = Load("PublicInterface");
		public static Gdk.Pixbuf PublicMethod    = Load("PublicMethod");
		public static Gdk.Pixbuf PublicProperty  = Load("PublicProperty");
		public static Gdk.Pixbuf PublicStruct    = Load("PublicStruct");
		
		static Gdk.Pixbuf Load(string name)
		{
			return Gdk.Pixbuf.LoadFromResource(imageBase + name);
		}
	}
}
