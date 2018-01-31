using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI.Component
{
    internal class GUIObjTabView : GUIObjBase
    {
        private static readonly int m_tabhederHeght = 23;

        private List<string> m_tabnames = null;
        private int m_tabindex = 0;

        private bool m_verticalMode = false;
        private int m_verticalTabWidth = 40;

        public override void Reset()
        {
            m_tabnames = null;
            m_tabindex = 0;

            m_verticalMode = false;
            m_verticalTabWidth = 40;
        }

        public void SetVerticalMode(int tabWidth)
        {
            m_verticalTabWidth = tabWidth;
            m_verticalMode = true;
        }

        public int Draw(Vector4 rect,int index,List<string>tabnames,Action<int> drawFunction)
        {
            m_tabnames = tabnames;
            m_tabindex = index;

            var rectHeader = rect;
            if(m_verticalMode)
            {
                rectHeader.Z = m_verticalTabWidth;
                m_tabindex = DrawTabHeader(rectHeader);

                rect.X += m_verticalTabWidth;
                rect.Z -= m_verticalTabWidth;
            }
            else
            {
                rectHeader.W = m_tabhederHeght;
                m_tabindex = DrawTabHeader(rectHeader);

                rect.Y += m_tabhederHeght;
                rect.W -= m_tabhederHeght;
            }

            GUI.BeginArea(rect);
            if (drawFunction != null) drawFunction.Invoke(index);
            GUI.EndArea();

            return m_tabindex;
        }

        private int DrawTabHeader(Vector4 rectHeader)
        {
            GUI.BeginArea(rectHeader);
            if (m_verticalMode)
            {
                GUILayout.BeginVertical();
                for (int i = 0; i < m_tabnames.Count; i++)
                {

                    if (GUILayout.Button(m_tabnames[i], GUIOption.Width(m_verticalTabWidth)))
                    {
                        m_tabindex = i;
                    }
                }
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.BeginHorizontal();
                for (int i = 0; i < m_tabnames.Count; i++)
                {

                    if (GUILayout.Button(m_tabnames[i],GUIOption.Width(75)))
                    {
                        m_tabindex = i;
                    }
                }
                GUILayout.EndHorizontal();
            }

            GUI.EndArea();

            return m_tabindex;
        }
    }
}
