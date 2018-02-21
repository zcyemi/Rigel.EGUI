using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI
{
    public class GUIRectView :GUIView
    {
        public override void OnViewStart()
        {
            base.OnViewStart();

            GUI.RectAbsolute(Rect, GUIStyle.Current.ColorBackground);
        }
    }
}
