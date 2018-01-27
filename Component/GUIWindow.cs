using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI.Component
{
    public class GUIWindow :GUIRegion
    {

        protected Vector4 m_rectContent;

        public GUIWindow(GUIForm form, int order = 0) : base(form, order)
        {
            m_rect = new Vector4(100, 100, 400, 300);
        }

        protected override sealed void OnGUI(RigelGUIEvent e)
        {
            GUI.RectAbsolute(m_rect,IsFocused? GUIStyle.Current.ColorBackgroundL1: GUIStyle.Current.ColorBackground);

            Vector4 rectHeader = m_rect;
            rectHeader.w = 25;
            GUI.RectAbsolute(rectHeader, GUIStyle.Current.ColorBackgroundL2);

            m_rectContent = m_rect;
            m_rectContent.y = 25;
            m_rectContent.x = 0;
            m_rectContent.w -= 25;

            GUI.BeginArea(m_rectContent);

            OnWindowGUI(e);

            GUI.EndArea();
        }

        protected virtual void OnWindowGUI(RigelGUIEvent e)
        {

        }
    }
}
