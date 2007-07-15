/* vi:set ts=4 sw=4: */
//
// System.Windows.Forms.Design.ControlDesigner
//
// Authors:
//	  Ivan N. Zlatev (contact i-nZ.net)
//
// (C) 2006 Ivan N. Zlatev

//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;

using System.Windows.Forms.Design;



namespace Mono.Design
{
	
	public sealed class MenuCommands : StandardCommands
	{
		
	   private MenuCommands ()
	   {
	   }
	   
	   public static readonly CommandID ComponentTrayMenu;
	   public static readonly CommandID ContainerMenu;
	   public static readonly CommandID DesignerProperties;
	   public static readonly CommandID KeyCancel;
	   public static readonly CommandID KeyDefaultAction;
	   public static readonly CommandID KeyMoveDown;
	   public static readonly CommandID KeyMoveLeft;
	   public static readonly CommandID KeyMoveRight;
	   public static readonly CommandID KeyMoveUp;
	   public static readonly CommandID KeyNudgeDown;
	   public static readonly CommandID KeyNudgeHeightDecrease;
	   public static readonly CommandID KeyNudgeHeightIncrease;
	   public static readonly CommandID KeyNudgeLeft;
	   public static readonly CommandID KeyNudgeRight;
	   public static readonly CommandID KeyNudgeUp;
	   public static readonly CommandID KeyNudgeWidthDecrease;
	   public static readonly CommandID KeyNudgeWidthIncrease;
	   public static readonly CommandID KeyReverseCancel;
	   public static readonly CommandID KeySelectNext;
	   public static readonly CommandID KeySelectPrevious;
	   public static readonly CommandID KeySizeHeightDecrease;
	   public static readonly CommandID KeySizeHeightIncrease;
	   public static readonly CommandID KeySizeWidthDecrease;
	   public static readonly CommandID KeySizeWidthIncrease;
	   public static readonly CommandID KeyTabOrderSelect;
	   public static readonly CommandID SelectionMenu;
	   public static readonly CommandID TraySelectionMenu;

	   static MenuCommands ()
	   {
		   // All of the following values were retrieved by using ToString() on each CommandID
		   //
		   Guid guidA = new Guid ("74d21312-2aee-11d1-8bfb-00a0c90f26f7");
		   Guid guidB = new Guid ("1496a755-94de-11d0-8c3f-00c04fc2aae2");

		   ComponentTrayMenu = new CommandID (guidA, 1286);
		   ContainerMenu = new CommandID (guidA, 1281);
		   DesignerProperties = new CommandID (guidA, 4097);
		   KeyCancel = new CommandID (guidB, 103);
		   KeyDefaultAction = new CommandID (guidB, 3);
		   KeyMoveDown = new CommandID (guidB, 13);
		   KeyMoveLeft = new CommandID (guidB, 7);
		   KeyMoveRight = new CommandID (guidB, 9);
		   KeyMoveUp = new CommandID (guidB, 11);
		   KeyNudgeDown = new CommandID (guidB, 1225);
		   KeyNudgeHeightDecrease = new CommandID (guidB, 1229);
		   KeyNudgeHeightIncrease = new CommandID (guidB, 1228);
		   KeyNudgeLeft = new CommandID (guidB, 1224);
		   KeyNudgeRight = new CommandID (guidB, 1226);
		   KeyNudgeUp = new CommandID (guidB, 1227);
		   KeyNudgeWidthDecrease = new CommandID (guidB, 1230);
		   KeyNudgeWidthIncrease = new CommandID (guidB, 1231);
		   KeyReverseCancel = new CommandID (guidB, 16385);
		   KeySelectNext = new CommandID (guidA, 4);
		   KeySelectPrevious = new CommandID (guidB, 5);
		   KeySizeHeightDecrease = new CommandID (guidB, 14);
		   KeySizeHeightIncrease = new CommandID (guidB, 12);
		   KeySizeWidthDecrease = new CommandID (guidB, 8);
		   KeySizeWidthIncrease = new CommandID (guidB, 10);
		   KeyTabOrderSelect = new CommandID (guidA, 16405);
		   SelectionMenu = new CommandID (guidA, 1280);
		   TraySelectionMenu = new CommandID (guidA, 1283);
	   }
	}
}
