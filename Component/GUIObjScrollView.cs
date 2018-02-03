using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI.Component
{
    public enum GUIScrollType : byte
    {
        Vertical = 1,
        Horizontal = 2,
        All = 3
    }

    public class GUIObjScrollView : GUIObjBase
    {

        private Vector4 m_rectAbsolute;
        private bool m_scrollInit = false;
        private Vector2 m_scrollPos;
        public GUIScrollType m_scrollType;

        private GUIDragState m_scrollVertical = new GUIDragState();

        public override void Reset()
        {
            Checked = false;
        }

        public Vector2 Draw(Vector4 rectab,Vector2 pos,GUIScrollType scrolltype)
        {
            m_rectAbsolute = rectab;
            if (!m_scrollInit)
            {
                Console.WriteLine("new");

                m_scrollPos = pos;
                m_scrollType = scrolltype;
                m_scrollInit = true;
            }

            if ((m_scrollType & GUIScrollType.Vertical) > 0) GUILayout.Space(m_scrollPos.y);
            if ((m_scrollType & GUIScrollType.Horizontal) > 0) GUILayout.Indent(m_scrollPos.x);

            return pos;
        }

        public void LateDraw()
        {
            var e = GUI.Event;
            var curoffset = GUI.CurArea.ContentMax;

            bool overflowH = (curoffset.x - m_scrollPos.x) > m_rectAbsolute.z;
            bool overflowV = (curoffset.y - m_scrollPos.y) > m_rectAbsolute.w;

            overflowV &= (m_scrollType & GUIScrollType.Vertical) > 0;
            overflowH &= (m_scrollType & GUIScrollType.Horizontal) > 0;

            float contentV = (curoffset.Y - m_scrollPos.Y);
            float scrollVmax = m_rectAbsolute.W - contentV;

            float ch = m_rectAbsolute.W / contentV;
            float chinv = 1.0f / ch;

            bool containerContains = GUIUtility.RectContainsCheck(m_rectAbsolute, e.Pointer);

            bool scrollall = overflowV && overflowH;

            if (overflowV)
            {
                var rectSBV = new Vector4(m_rectAbsolute.Z - 10f, 0, 10, m_rectAbsolute.W);

                bool containsThumb = false;

                rectSBV.W *= ch;
                rectSBV.Y = (-m_scrollPos.Y) / contentV * m_rectAbsolute.W;

                if (!GUI.Event.Used)
                {
                    if (GUIUtility.RectContainsCheck(GUI.GetAbsoluteRect(rectSBV), GUI.Event.Pointer))
                    {
                        containsThumb = true;
                    }
                }

                bool thumbActive = containsThumb;


                if (m_scrollVertical.OnDrag(containsThumb))
                {
                    m_scrollPos.Y -= (int)(GUI.Event.DragOffset.Y * chinv);
                    m_scrollPos.Y = Mathf.Clamp(m_scrollPos.Y, scrollVmax, 0);
                    thumbActive = true;
                }
                //GUILayout.Rect(rectSBV, thumbActive ? GUIStyle.Current.ColorActive : GUIStyle.Current.ColorBackground);

                if (containerContains)
                {
                    if (!e.Used && e.EventType == RigelGUIEventType.MouseWheel)
                    {
                        m_scrollPos.Y += (int)(chinv * 12 * (e.Delta > 0 ? 1 : -1));
                        m_scrollPos.Y = Mathf.Clamp(m_scrollPos.Y, scrollVmax, 0);
                        e.Use();
                    }
                }
            }
        }
    }
}
