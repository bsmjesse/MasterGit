using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using HGICommonLib ;
namespace StateMachineTest
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.ListBox lbxOutput;
		private System.Windows.Forms.TextBox txtToken;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private void CB1( object token, long stateTo )
		{
			lbxOutput.Items.Add( 
				string.Format( "token: {0}, curr state: {1}", token, stateTo ) 
								) ;
		}

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			stm = new BasicStateMachine() ;

			// add states
			STMEvent evt = new STMEvent( CB1 ) ;
			stm.AddTransition( 0, 0, "0", evt, TransitionType.Default ) ;
			stm.AddTransition( 0, 1, "A", evt, TransitionType.Normal ) ;
			stm.AddTransition( 0, 2, "B", evt, TransitionType.Normal ) ;

			stm.AddTransition( 1, 0,"B", evt, TransitionType.Normal ) ;
			stm.AddTransition( 1, 1,"0", evt, TransitionType.Default ) ;
			stm.AddTransition( 1, 2,"A", evt, TransitionType.Normal ) ;

			stm.AddTransition( 2, 0, "A", evt, TransitionType.Normal ) ;
			stm.AddTransition( 2, 1, "B", evt, TransitionType.Normal ) ;
			stm.AddTransition( 2, 2, "0", evt, TransitionType.Default ) ;

			stm.Start() ;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.button1 = new System.Windows.Forms.Button();
			this.txtToken = new System.Windows.Forms.TextBox();
			this.lbxOutput = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(128, 239);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(64, 24);
			this.button1.TabIndex = 0;
			this.button1.Text = "Process";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// txtToken
			// 
			this.txtToken.Location = new System.Drawing.Point(24, 240);
			this.txtToken.Name = "txtToken";
			this.txtToken.TabIndex = 1;
			this.txtToken.Text = "";
			// 
			// lbxOutput
			// 
			this.lbxOutput.Location = new System.Drawing.Point(32, 24);
			this.lbxOutput.Name = "lbxOutput";
			this.lbxOutput.Size = new System.Drawing.Size(184, 199);
			this.lbxOutput.TabIndex = 2;
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 273);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.lbxOutput,
																		  this.txtToken,
																		  this.button1});
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

		private BasicStateMachine stm ;
		private void button1_Click(object sender, System.EventArgs e)
		{
			stm.ProcessToken( txtToken.Text ) ;
		}

		private void Form1_Load(object sender, System.EventArgs e)
		{
		
		}
	}
}
