using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CsGrid
{
	public enum GridUnit
	{
		Fraction,
		Pixels,
		Percent,
		Auto
	}

	public class GridLength
	{
		public GridUnit Unit;
		public float Length;

		public GridLength(GridUnit unit, float length)
		{
			Unit = unit;
			Length = length;
		}
	}

	public struct GridArea
	{
		public int ColumnStart;
		public int ColumnEnd;
		public int RowStart;
		public int RowEnd;

		public GridArea(int columnStart, int columnEnd, int rowStart, int rowEnd)
		{
			ColumnStart = columnStart;
			ColumnEnd = columnEnd;
			RowStart = rowStart;
			RowEnd = rowEnd;
		}
	}

	public class GridPanel : Panel
	{
		//Row and column lengths, in their originally defined units
		GridLength[] rows;
		GridLength[] columns;

		//Row and column lengths, in pixels
		float[] rowPixels;
		float[] columnPixels;
		float totalRowFractions;
		float totalColumnFractions;

		float[] cumulRowPixels;
		float[] cumulColumnPixels;

		public bool RenderInvalidControls { get; set; } = false;

		Dictionary<string, GridArea> gridAreas = new Dictionary<string, GridArea>();

		public GridPanel()
		{
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
		}

		public void SetColumns(params GridLength[] columnLengths)
		{
			columns = columnLengths;
			RecalculateGrid();
		}

		public void SetColumns(string columnsString)
		{
			string[] parts = columnsString.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
			GridLength[] lengths = new GridLength[parts.Length];
			for (int i = 0; i < parts.Length; i++)
				lengths[i] = ParseGridLength(parts[i]);
			columns = lengths;
		}

		public void SetRows(string rowsString)
		{
			string[] parts = rowsString.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
			GridLength[] lengths = new GridLength[parts.Length];
			for (int i = 0; i < parts.Length; i++)
				lengths[i] = ParseGridLength(parts[i]);
			rows = lengths;
		}

		private GridLength ParseGridLength(string s)
		{
			s = s.Trim();
			GridLength gl = null;
			bool success;

			if (s.EndsWith("px"))
			{
				success = int.TryParse(s.Substring(0, s.Length - 2), out int pixels);
				gl = new GridLength(GridUnit.Pixels, pixels);
			}
			else if (s.EndsWith("fr"))
			{
				success = float.TryParse(s.Substring(0, s.Length - 2), out float fractions);
				gl = new GridLength(GridUnit.Fraction, fractions);
			}
			else if (s.EndsWith("%"))
			{
				success = float.TryParse(s.Substring(0, s.Length - 1), out float percent);
				gl = new GridLength(GridUnit.Percent, percent);
			}
			else if (s == "auto")
			{
				success = true;
				gl = new GridLength(GridUnit.Auto, 0);
			}
			else
			{
				success = false;
			}

			if (!success)
				throw new Exception("Invalid grid length string '" + s + "'");
			return gl;
		}

		public void SetRows(params GridLength[] rowLengths)
		{
			rows = rowLengths;
			RecalculateGrid();
		}

		public void DefineArea(string name, int columnStart, int columnEnd, int rowStart, int rowEnd)
		{
			if (gridAreas.ContainsKey(name))
				throw new Exception("Cannot add grid area " + name + ": area already exists");
			gridAreas[name] = new GridArea(columnStart, columnEnd, rowStart, rowEnd);
			RecalculateGrid();
		}

		public void DefineArea(string name, string columns, string rows)
		{
			var (columnStart, columnEnd) = ParseGridAreaString(columns);
			var (rowStart, rowEnd) = ParseGridAreaString(rows);
			DefineArea(name, columnStart, columnEnd, rowStart, rowEnd);
		}

		public void DefineAreas(string areas)
		{
			gridAreas.Clear();

			string[] areaRows = areas.Trim().Split('\n');
			if (areaRows.Length != rows.Length)
				throw new Exception("Invalid area string - row count does not match");

			if (areaRows.Length == 0)
				return;

			var allNames = new HashSet<string>();
			var pendingNames = new HashSet<string>();
			var pendingAreas = new Dictionary<string, GridArea>();
			int columnCount = areaRows[0].Trim().Split(new char[0], StringSplitOptions.RemoveEmptyEntries).Length;
			for (int i = 0; i < areaRows.Length; i++)
			{
				string[] areaNames = areaRows[i].Trim().Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
				if (areaNames.Length != columnCount)
					throw new Exception("Line " + i + " of the specified area string has a mismatching column count");

				var rowNames = new HashSet<string>(areaNames);
				//Get counts for each area name on this row
				var nameLengths = CountAndTestRow(areaNames);
				var nameStarts = GetNameColumnStarts(areaNames);

				foreach (var name in areaNames)
				{
					if (name == ".")
						continue;

					//Check if we are in the middle of this area
					if (pendingNames.Contains(name))
					{
						//Check that the column count is the same
						int expectedWidth = pendingAreas[name].ColumnEnd - pendingAreas[name].ColumnStart;
						if (expectedWidth != nameLengths[name])
							throw new Exception("Area " + name + " on line " + i + " of the area string had mismatched columns; expected " + expectedWidth + " but had " + nameLengths[name]);
						if (nameStarts[name] != pendingAreas[name].ColumnStart)
							throw new Exception("Area " + name + " on line " + i + " is shifted");
					}
					else
					{
						if (allNames.Contains(name))
							throw new Exception("Cannot add duplicate area name " + name);

						pendingNames.Add(name);
						pendingAreas[name] = new GridArea(nameStarts[name], nameStarts[name] + nameLengths[name], i, 0);
					}
				}

				//Clear area names that have completed and add them to allNames
				var finishedNames = pendingNames.Except(rowNames).ToArray();
				foreach (var finishedName in finishedNames)
				{
					pendingNames.Remove(finishedName);
					allNames.Add(finishedName);
					GridArea ga = pendingAreas[finishedName];
					pendingAreas.Remove(finishedName);

					ga.RowEnd = i;
					gridAreas.Add(finishedName, ga);
				}
			}

			foreach (var leftoverName in pendingNames)
			{
				GridArea ga = pendingAreas[leftoverName];
				ga.RowEnd = areaRows.Length;
				gridAreas.Add(leftoverName, ga);
			}

			RecalculateGrid();
		}

		private Dictionary<string, int> GetNameColumnStarts(string[] row)
		{
			if (row.Length == 0)
				return new Dictionary<string, int>();

			var starts = new Dictionary<string, int>();
			for (int i = 0; i < row.Length; i++)
			{
				if (!starts.ContainsKey(row[i]))
					starts.Add(row[i], i);
			}

			return starts;
		}

		private Dictionary<string, int> CountAndTestRow(string[] row)
		{
			if (row.Length == 0)
				return new Dictionary<string, int>();

			var nameLengths = new Dictionary<string, int>();
			nameLengths.Add(row[0], 1);
			for (int i = 1; i < row.Length; i++)
			{
				string name = row[i];

				if (name != row[i - 1] && nameLengths.ContainsKey(name))
					throw new Exception("Area " + name + " is not a contiguous region");

				if (nameLengths.ContainsKey(name))
					nameLengths[name]++;
				else
					nameLengths[name] = 1;
			}

			return nameLengths;
		}

		private (int start, int end) ParseGridAreaString(string s)
		{
			int start, end;
			bool success = false;
			if (s.Contains("/"))
			{
				string[] parts = s.Split('/');
				if (parts.Length != 2)
					throw new Exception("Invalid grid area string - more than 2 parts found.");

				success = int.TryParse(parts[0].Trim(), out start);
				if (parts[1].Contains("span"))
					success = int.TryParse(parts[1].Trim().Substring(5), out end);
				else
					success = int.TryParse(parts[1].Trim(), out end);

			}
			else
			{
				success = int.TryParse(s.Trim(), out start);
				end = start + 1;
			}

			if (!success)
				throw new Exception("Invalid grid area string.");

			return (start, end);
		}

		private void RecalculateGrid()
		{
			if (columns != null && rows != null)
			{
				columnPixels = new float[columns.Length];
				rowPixels = new float[rows.Length];
				cumulColumnPixels = new float[columns.Length];
				cumulRowPixels = new float[rows.Length];

				totalColumnFractions = columns.Sum(gl => gl.Unit == GridUnit.Fraction ? gl.Length : 0);
				totalRowFractions = rows.Sum(gl => gl.Unit == GridUnit.Fraction ? gl.Length : 0);

				RecalculateAxis(columns, columnPixels, ClientRectangle.Width, totalColumnFractions);
				RecalculateAxis(rows, rowPixels, ClientRectangle.Height, totalRowFractions);
				CalculateCumulativeAxis(columnPixels, cumulColumnPixels);
				CalculateCumulativeAxis(rowPixels, cumulRowPixels);

				UpdateControls();
				Invalidate();
			}
		}

		//TODO: remove this method and make RecalculateAxis cumulative in the first place. We only separate the process here for debugging reasons.
		private void CalculateCumulativeAxis(float[] pixelLengths, float[] cumulPixelLengths)
		{
			float currentSum = 0;
			for (int i = 0; i < pixelLengths.Length; i++)
			{
				currentSum += pixelLengths[i];
				cumulPixelLengths[i] = currentSum;
			}
		}

		private void RecalculateAxis(GridLength[] lengths, float[] pixelLengths, float completeLength, float totalFractions)
		{
			float totalLength = 0;
			int autoCount = 0;
			//Allocate pixel and fraction lengths first
			for (int i = 0; i < lengths.Length; i++)
			{
				GridLength gl = lengths[i];
				float pixelValue = 0;

				if (gl.Unit == GridUnit.Pixels)
					pixelValue = gl.Length;
				else if (gl.Unit == GridUnit.Percent)
					pixelValue = completeLength * (gl.Length / 100);
				else
					autoCount++;

				totalLength += pixelValue;
				pixelLengths[i] = pixelValue;
			}

			float remainingLength = completeLength - totalLength;
			float afterFractionsLength = remainingLength;

			//Go back and assign fractional lengths now
			for (int i = 0; i < lengths.Length; i++)
			{
				GridLength gl = lengths[i];
				if (gl.Unit == GridUnit.Fraction)
				{
					float length = remainingLength * (gl.Length / totalFractions);
					pixelLengths[i] = length;
					afterFractionsLength -= length;
				}
			}

			float autoLength = afterFractionsLength / (autoCount == 0 ? 1 : autoCount);

			//Go back and assign auto values now
			for (int i = 0; i < lengths.Length; i++)
			{
				GridLength gl = lengths[i];
				if (gl.Unit == GridUnit.Auto)
					pixelLengths[i] = autoLength;
			}
		}


		protected override void OnResize(EventArgs e)
		{
			RecalculateGrid();
			base.OnResize(e);
		}

		private void UpdateControls()
		{
			for (int i = 0; i < Controls.Count; i++)
			{
				Control c = Controls[i];
				object gridMeta = c.Tag;
				if (gridMeta is string)
				{
					string areaName = gridMeta as string;
					if (string.IsNullOrWhiteSpace(areaName))
					{
						if (!RenderInvalidControls)
						{
							c.Width = 0;
							c.Height = 0;
							c.Location = Point.Empty;
						}
						continue;
					}

					if (!gridAreas.ContainsKey(areaName))
					{
						if (!RenderInvalidControls)
						{
							c.Width = 0;
							c.Height = 0;
							c.Location = Point.Empty;
						}
						continue;
					}

					GridArea area = gridAreas[areaName];
					float top = area.RowStart == 0 ? 0 : cumulRowPixels[area.RowStart - 1];
					float bottom = area.RowEnd == 0 ? 0 : cumulRowPixels[area.RowEnd - 1];
					float left = area.ColumnStart == 0 ? 0 : cumulColumnPixels[area.ColumnStart - 1];
					float right = area.ColumnEnd == 0 ? 0 : cumulColumnPixels[area.ColumnEnd - 1];

					c.Width = (int)(right - left);
					c.Height = (int)(bottom - top);
					c.Location = new Point((int)left, (int)top);
				}
				else if (gridMeta is GridArea)
				{

				}
			}
		}
	}
}
