using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MineStudio.frmMain;

namespace MineStudio
{
    public class Form_Closed_EventArgs
    {
        private readonly string fname;

        public Form_Closed_EventArgs(string formName)
        {
            this.fname = formName;
        }

        public string getFormName
        {
            get { return this.fname; }
        }
    }
}
