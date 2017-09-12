using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsGrid
{
	public class GridHelper
	{
		public static GridLength[] RepeatGrid(int n, GridUnit unit, float length)
		{
			GridLength[] result = new GridLength[n];
			for (int i = 0; i < n; i++)
				result[i] = new GridLength(unit, length);

			return result;
		}
	}
}
