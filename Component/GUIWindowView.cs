using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI
{
    public class GUIWindowView : GUIView
    {
        protected Vector4 RectContent;

        protected GUIDragState m_dragMove = new GUIDragState();
        protected GUIDragState m_dragResize = new GUIDragState();

        public bool Moveable { get; set; } = true;
        public bool Resizeable { get; set; } = true;
        protected Vector2 MinSize = new Vector2(400, 300);
        public string Caption { get; set; }


        protected bool m_onMove = false;

        public GUIWindowView()
        {
            Rect = new Vector4(100, 100, 400, 300);
            Caption = this.GetType().ToString();
        }

        public override void OnViewStart()
        {
            base.OnViewStart();

            m_onMove = false;

            GUI.RectAbsolute(Rect, IsFocused ? GUIStyle.Current.ColorBackgroundL1 : GUIStyle.Current.ColorBackground);
            Vector4 rectHeader = Rect;
            rectHeader.w = 25;

            bool headerover = GUIUtility.RectContainsCheck(rectHeader, GUI.Event.Pointer);
            if (Moveable && m_dragMove.OnDrag(headerover))
            {
                Rect = Rect.Move(m_dragMove.OffSet);
                m_onMove = true;
            }
            if (Resizeable && !m_onMove)
            {
                var rectResize = Rect;
                rectResize.Y += (rectResize.W - 6);
                rectResize.X += rectResize.Z - 6;
                rectResize.Z = 6;
                rectResize.W = 6;

                if (m_dragResize.OnDrag(rectResize))
                {
                    Rect.z += m_dragResize.OffSet.x;
                    Rect.w += m_dragResize.OffSet.y;

                    Rect.z = Mathf.Max(Rect.z, MinSize.x);
                    Rect.w = Mathf.Max(Rect.w, MinSize.y);
                }
            }

            GUI.RectAbsolute(rectHeader, headerover ? GUIStyle.Current.ColorActive : (IsFocused ? GUIStyle.Current.ColorActiveD : GUIStyle.Current.ColorBackgroundL2));
            if (!string.IsNullOrEmpty(Caption)) GUI.TextAbsolute(rectHeader, Caption, RigelColor.White, new Vector2(5, 2));

            RectContent = Rect;
            RectContent.y = 25;
            RectContent.x = 0;
            RectContent.w -= 25;

            GUI.BeginArea(RectContent);
        }

        public override void OnViewEnd()
        {
            GUI.EndArea();
            base.OnViewEnd();
        }
    }

}
