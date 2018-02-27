using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI.Component
{
    public class GUIDebugContent : GUIContent
    {

        public GUIDebugContent()
        {
            ContentName = "DebugInfo";
        }

        public override void OnGUI(RigelGUIEvent e)
        {
            GUI.DrawDebugInfo();
        }
    }
}
