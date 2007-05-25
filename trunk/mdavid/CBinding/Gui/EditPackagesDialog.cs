using System;

namespace CBinding
{
	public partial class EditPackagesDialog : Gtk.Dialog
	{
		public EditPackagesDialog()
		{
			this.Build();
			
			buttonOk.Clicked += OnOkButtonClick;
			buttonCancel.Clicked += OnCancelButtonClick;
		}
		
		private void OnOkButtonClick (object sender, EventArgs e)
		{
			Destroy ();
		}
		
		private void OnCancelButtonClick (object sender, EventArgs e)
		{
			Destroy ();
		}
	}
}
