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

        private Vector4 m_rectBarV;
        private Vector4 m_rectBarH;

        private Vector2 m_maxscroll;


        private bool m_scrollV;
        private bool m_scrollH;

        public override void Reset()
        {
            Checked = false;
            m_scrollInit = false;
        }

        public void Draw(Vector4 rect, Vector4 rectab, Vector2 pos, GUIScrollType scrolltype)
        {
            GUI.BeginArea(rect, true);

            m_rectAbsolute = rectab;

            if (!m_scrollInit)
            {
                //Console.WriteLine("new");

                m_scrollPos = pos;
                m_scrollType = scrolltype;
                m_scrollInit = true;
            }

            if ((m_scrollType & GUIScrollType.Vertical) > 0)
            {
                GUILayout.Space(m_scrollPos.y);
                m_scrollV = true;
            }
            if ((m_scrollType & GUIScrollType.Horizontal) > 0)
            {
                GUILayout.Indent(m_scrollPos.x);
                m_scrollH = true;
            }
        }

        public Vector2 LateDraw()
        {

            Vector2 content = GUI.CurArea.ContentMax - m_scrollPos;
            m_maxscroll = m_rectAbsolute.Size() - content;

            bool showScrollBarV = false;
            bool wheelScrollBarV = false;

            if (content.y > m_rectAbsolute.w && m_scrollV)
            {
                m_rectBarV = new Vector4(m_rectAbsolute.z - 6, 0, 6, m_rectAbsolute.w);
                GUI.Rect(m_rectBarV, GUIStyle.Current.ColorBackground);

                float ysize = m_rectAbsolute.w / content.y * m_rectAbsolute.w;
                float yoff = -m_scrollPos.y / content.y * m_rectAbsolute.w;

                var thumbRect = new Vector4(m_rectBarV.x, yoff, 6, ysize);
                var scrollBarVdrag = false;

                if (!GUI.Event.Used)
                {
                    if (GUI.Event.EventType == RigelGUIEventType.MouseWheel)
                    {
                        m_scrollPos.y += 0.2f * GUI.Event.Delta;
                        if (m_scrollPos.y > 0) m_scrollPos.y = 0;
                        if (m_scrollPos.y < m_maxscroll.y) m_scrollPos.y = m_maxscroll.y;

                        GUI.Event.Use();
                        wheelScrollBarV = true;
                    }
                    else
                    {

                        var thumbRectA = GUI.GetAbsoluteRect(thumbRect);

                        var contains = GUIUtility.RectContainsCheck(thumbRectA, GUI.Event.Pointer);
                        if (contains) scrollBarVdrag = true;

                        if (m_scrollVertical.OnDrag(contains))
                        {
                            m_scrollPos.y -= m_scrollVertical.OffSet.y;
                            if (m_scrollPos.y > 0) m_scrollPos.y = 0;
                            var maxscroll = m_rectAbsolute.w - content.y;
                            if (m_scrollPos.y < (maxscroll)) m_scrollPos.y = maxscroll;

                            scrollBarVdrag = true;
                        }
                    }
                }



                GUI.Rect(thumbRect, scrollBarVdrag ? GUIStyle.Current.ColorActiveD : GUIStyle.Current.ColorBackgroundL2);

                showScrollBarV = true;
            }

            if (content.x > m_rectAbsolute.x && m_scrollH)
            {
                m_rectBarH = new Vector4(0, m_rectAbsolute.w - 6, showScrollBarV ? m_rectAbsolute.z - 6 : m_rectAbsolute.z, 6);
                GUI.Rect(m_rectBarV, GUIStyle.Current.ColorBackgroundL2);
            }



            GUI.EndArea();

            return m_scrollPos;

        }
    }
}
