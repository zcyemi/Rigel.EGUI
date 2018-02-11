using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI.Component
{
    public class GUIWindowView : GUIView
    {

        protected Vector4 m_rectContent;

        protected GUIDragState m_dragMove = new GUIDragState();
        protected GUIDragState m_dragResize = new GUIDragState();

        public bool Moveable { get; set; } = true;
        public bool Resizeable { get; set; } = true;
        protected Vector2 MinSize = new Vector2(400, 300);
        public string Caption { get; set; }


        protected bool m_onMove = false;

        public GUIWindowView(Rigel.GUI.GUIContent content) : base(content)
        {
            m_rect = new Vector4(100, 100, 400, 300);
            Caption = this.GetType().ToString();
        }

        protected override sealed void OnGUI(RigelGUIEvent e)
        {
            m_onMove = false;

            GUI.RectAbsolute(m_rect,IsFocused? GUIStyle.Current.ColorBackgroundL1: GUIStyle.Current.ColorBackground);
            Vector4 rectHeader = m_rect;
            rectHeader.w = 25;

            bool headerover = GUIUtility.RectContainsCheck(rectHeader, GUI.Event.Pointer);
            if (Moveable && m_dragMove.OnDrag(headerover))
            {
                m_rect = m_rect.Move(m_dragMove.OffSet);
                m_onMove = true;
            }
            if(Resizeable && !m_onMove)
            {
                var rectResize = m_rect;
                rectResize.Y += (rectResize.W - 6);
                rectResize.X += rectResize.Z - 6;
                rectResize.Z = 6;
                rectResize.W = 6;

                if (m_dragResize.OnDrag(rectResize))
                {
                    m_rect.z += m_dragResize.OffSet.x;
                    m_rect.w += m_dragResize.OffSet.y;

                    m_rect.z = Mathf.Max(m_rect.z, MinSize.x);
                    m_rect.w = Mathf.Max(m_rect.w, MinSize.y);
                }
            }

            GUI.RectAbsolute(rectHeader, headerover ? GUIStyle.Current.ColorActive : (IsFocused ? GUIStyle.Current.ColorActiveD: GUIStyle.Current.ColorBackgroundL2));
            if (!string.IsNullOrEmpty(Caption)) GUI.TextAbsolute(rectHeader, Caption, RigelColor.White, new Vector2(5,2));

            m_rectContent = m_rect;
            m_rectContent.y = 25;
            m_rectContent.x = 0;
            m_rectContent.w -= 25;

            GUI.BeginArea(m_rectContent);

            if (m_content != null) m_content.OnGUI(e);

            GUI.EndArea();


        }
    }
}
