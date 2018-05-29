using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MineStudio.frmMain;


namespace MineStudio
{
    public class My_Controls
    {
        public delegate void update_Control_text_Callback(Control owner, Control control, string text);
        public delegate void update_ListViewItems_Callback(Control owner, Control control, ListViewItem[] items);
        public delegate void clear_ListViewControlItems_Callback(Control owner, Control control);
        public delegate void setControlVisibility_Callback(Control owner, Control control, bool visible);
        public delegate void enableControl_Callback(Control owner, Control control, bool enable);

        public My_Controls() { }

        public void update_Control_Text(Control owner, Control control, string text)
        {
            if (owner.InvokeRequired)
            {
                update_Control_text_Callback d = new update_Control_text_Callback(update_Control_Text);
                owner.Invoke(d, new object[] { owner, control, text });
            }
            else
            {
                control.Text = text;
            }
        }
        public void clear_ListViewControlItems(Control owner, Control control)
        {
            if (owner.InvokeRequired)
            {
                clear_ListViewControlItems_Callback d = new clear_ListViewControlItems_Callback(clear_ListViewControlItems);
                owner.Invoke(d, new object[] { owner, control });
            }
            else
            {
                ListView lv = (ListView)control;
                lv.Items.Clear();
            }
        }
        public void update_ListViewItems(Control owner, Control control, ListViewItem[] items)
        {
            if (owner.InvokeRequired)
            {
                update_ListViewItems_Callback d = new update_ListViewItems_Callback(update_ListViewItems);
                owner.Invoke(d, new object[] { owner, control, items });
            }
            else
            {
                ListView lv = (ListView)control;
                foreach (ListViewItem i in items)
                {
                    lv.Items.Add(i);
                }
            }
        }
        public void setControlVisibility(Control owner, Control control, bool visible)
        {
            if (owner.InvokeRequired)
            {
                setControlVisibility_Callback d = new setControlVisibility_Callback(setControlVisibility);
                owner.Invoke(d, new object[] { owner, control, visible });
            }
            else
            {
                /*UITabPage t = (UITabPage)control;
                t.TabVisible = visible;
                UITab ow = (UITab)owner;
                ow.SelectedTab = t;*/
            }
        }
        public void enableControl(Control owner, Control control, bool enable)
        {
            if (owner.InvokeRequired)
            {
                enableControl_Callback d = new enableControl_Callback(enableControl);
                owner.Invoke(d, new object[] { owner, control, enable });
            }
            else
            {
                control.Enabled = enable;
            }
        }
    }
}
