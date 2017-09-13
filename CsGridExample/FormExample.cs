using CsGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CsGridExample
{
	public partial class FormExample : Form
	{
		public FormExample()
		{
			InitializeComponent();

			gridPanel1.SetColumns("200px auto 2fr auto auto");
			gridPanel1.SetRows("30px auto auto");

			gridPanel1.DefineAreas(@"header header right1 right1 right2
										body . . . .
										body . . . .");
		}
	}
}
