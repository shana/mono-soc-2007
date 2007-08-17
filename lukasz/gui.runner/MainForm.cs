using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MonoDevelop;
using System.Xml;
using Message=Gendarme.Framework.Message;

using System.Threading;

using Mono.Cecil;
using Gendarme.Framework;

namespace gui.runner
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		
		private string config;
        
		public string Config {
			get { return config; }
			set { config = value; }
		}
        private string set;
        
		public string Set {
			get { return set; }
			set { set = value; }
		}
        GendarmeRunner runner;

        double progress_step = 0;
        double current_progress = 0;
        double previous_progress = 0;
        
		[STAThread]
		public static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		void Button1Click(object sender, EventArgs e)
		{
			openFileDialog1.Multiselect = true;
			openFileDialog1.ShowDialog();
			foreach (string filename in openFileDialog1.FileNames)
			{
				if (!listBox1.Items.Contains(filename))
				{
					listBox1.Items.Add(filename);
				}
			}
		}
		
		void Button2Click(object sender, EventArgs e)
		{
			//GendarmeDisplay dis = new GendarmeDisplay();
			config = button1.Text;
			//this.set = textBox1.Text;
			RunTests(label2.Text);
		}
		
		void Button3Click(object sender, EventArgs e)
		{
			foreach (int selectedIndex in listBox1.SelectedIndices)
			{
				listBox1.Items.RemoveAt(selectedIndex);
			}
		}
		
		
        string GetAttribute (XmlElement xel, string name, string defaultValue) {
            XmlAttribute xa = xel.Attributes [name];
            if (xa == null)
                return defaultValue;
            return xa.Value;
        }

        bool LoadConfiguration () {
            
			bool result = false;
			foreach (object obj in listBox1.Items)
			{
				int n = runner.LoadRulesFromAssembly (obj.ToString(), "*", "");
				result = result | n > 0;
			}
			return result;
            
        }
		
	        public void RunTests(object item) {
            Thread thread = new Thread(OnDoWork);
            thread.IsBackground = true;
            thread.Start(item);            
        }
        
        private void SetProgress(double number)
       { 
       }
        
        public void OnProgressChanged(int progress){
            double addp = (double)progress / progress_step;
            double savep = previous_progress; 
            previous_progress = addp;
            current_progress +=  addp - savep;
            SetProgress(current_progress);
        }        
       
        void OnDoWork (object item){
            runner = new GendarmeRunner();
            runner.ProgressChanged += new GendarmeRunner.ProgressChangedHandler(OnProgressChanged);
            LoadConfiguration ();
            current_progress = 0;
		if (item is string)
		{
			AssemblyDefinition ass = AssemblyFactory.GetAssembly(item as string);
            runner.ProcessWithProgress(ass);
			
		}
		string s = "";
		
		foreach (Violation v in runner.Violations)
		{
			RuleInformation ri = RuleInformationManager.GetRuleInformation (v.Rule);
			foreach (Gendarme.Framework.Message m in v.Messages)
			{
				
				this.Invoke((ThreadStart) delegate() {
				listView1.BeginUpdate();
				ListViewItem listItem = new ListViewItem();
        		listItem.ImageIndex = 0;
				listItem.Text =  m.Type.ToString();
        		listItem.SubItems.Add(m.Location.ToString());
        		listItem.SubItems.Add(m.Text);

        		listView1.Items.Add(listItem);		
        		listView1.EndUpdate();
				            });
			}
		}
		
		
                SetProgress(1);

        }
        
        string getMessageLine(MessageCollection ms) {
            if (ms != null && ms.Count > 0) {
                foreach(Message m in ms) {
                    return m.Text;
                }
            }
            return String.Empty;
        }
        string getSource(MessageCollection ms) {
            if (ms != null && ms.Count > 0) {
                foreach(Message m in ms) {
                    return m.Location.ToString();
                }
            }
            return String.Empty;
        }


		
		void ExitToolStripMenuItemClick(object sender, EventArgs e)
		{
			this.Close();
		}
		
		void Button4Click(object sender, EventArgs e)
		{
			openFileDialog1.ShowDialog();
			label2.Text = openFileDialog1.FileName;
		}
	}
	
}
