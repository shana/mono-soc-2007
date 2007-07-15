
using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel.Design;
using System.ComponentModel;
using Mono.Design;

namespace Designer
{
	
	
	public class Controls
	{
		
		public Controls()
		{
		}
	}
	
	[Designer("Mono.Design.DocumentDesigner, Mono.Design", typeof(IRootDesigner))] 
	public class MyForm : Form
	{
		public MyForm ()
		{
			this.TopLevel = false;
			this.DoubleBuffered = true;
		}
		
	}

	[Designer("Mono.Design.ControlDesigner, Mono.Design", typeof(IDesigner))]
	public class MyButton : Button
	{
	}
	
	[Designer("Mono.Design.ScrollableControlDesigner, Mono.Design")]
	
	public class MyPanel : Panel
	{
		public MyPanel ()
		{
			this.BackColor = Color.Red;
		}
	}
}
