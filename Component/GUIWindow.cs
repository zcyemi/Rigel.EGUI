using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI.Component
{
    public class GUIWindow :GUIRegion
    {
        public GUIWindow(GUIForm form, int order = 0) : base(form, order)
        {
            m_rect = new Vector4(100, 100, 400, 300);
        }

        protected override sealed void OnGUI(RigelGUIEvent e)
        {
            GUIDraw.Rect(m_rect,IsFocused? GUIStyle.Current.ColorBackgroundL1: GUIStyle.Current.ColorBackground);

            Vector4 rectHeader = m_rect;
            rectHeader.w = 25;
            GUIDraw.Rect(rectHeader, GUIStyle.Current.ColorBackgroundL2);

            OnWindowGUI(e);
        }

        protected virtual void OnWindowGUI(RigelGUIEvent e)
        {

        }
    }
}
