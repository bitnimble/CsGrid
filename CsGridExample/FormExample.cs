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

			gridPanel1.SetColumns(new GridLength(GridUnit.Pixels, 300), new GridLength(GridUnit.Auto, 0));
			gridPanel1.SetRows(new GridLength(GridUnit.Pixels, 54), new GridLength(GridUnit.Auto, 0), new GridLength(GridUnit.Pixels, 50));

			gridPanel1.DefineAreas(@".    title
									 info body
									 .    footer
									");
		}
	}
}
