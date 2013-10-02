using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace Hanbo.ToolStripUI.Helper
{
	public class ToolStripUIHelper
	{
		/// <summary>
		/// ToolStripButton 有設定 CheckOnClick 屬性為 true 的按鈕們，當 任一按鈕 Checked 時，其他按鈕 unChecked
		/// </summary>
		/// <param name="sender"></param>
		public static void SetSingleToolStripButtonOn(ToolStripButton btn, ToolStripButton[] excludeButtons)
		{
			if (btn.Checked)
			{
				var checkOnClickItems = btn.GetCurrentParent().Items.OfType<ToolStripButton>()
										.Where(p => p.CheckOnClick == true && !p.Equals(btn));
				if(excludeButtons != null)
				{
					checkOnClickItems = checkOnClickItems.Where(p => !excludeButtons.Contains(p));
				}
										
				foreach (ToolStripButton checkOnClickToolStrip in checkOnClickItems)
				{
					checkOnClickToolStrip.Checked = false;
				}
			}
		}
	}
}
