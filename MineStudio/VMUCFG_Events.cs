using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MineStudio.frmMain;

namespace MineStudio
{
    public class VMUCFG_Events
    {
        public event formClosedCallback formClosedEvent;

        public VMUCFG_Events()
        {

        }

        public void fireFormClosedEvent(string formName)
        {
            //this.formClosedEvent(this, new Form_Closed_EventArgs(formName));
        }
    }
}
