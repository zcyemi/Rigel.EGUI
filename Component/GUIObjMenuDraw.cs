using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI.Component
{
    internal class GUIObjMenuDraw : GUIObjBase
    {
        private bool m_onActive = false;

        private List<GUIMenuList> m_drawLevels = new List<GUIMenuList>();
        private List<float> m_drawOffset = new List<float>();


        public override void Reset()
        {
            m_onActive = false;
            m_drawLevels.Clear();
            m_drawOffset.Clear();
        }

        public void Draw(bool click, GUIMenuList menu, Vector4 rect)
        {
            if (menu == null || menu.Items.Count == 0) return;
            if (click)
            {
                m_onActive = true;
                m_drawLevels.Clear();
                m_drawLevels.Add(menu);
                m_drawOffset.Clear();
                m_drawOffset.Add(0);
            }

            bool active = false;

            if (m_onActive)
            {
                var containerRectA = rect.Move(0, rect.w).SetSize(800, 300);

                GUI.BeginArea(containerRectA);
                {
                    GUILayout.BeginHorizontal();
                    {
                        for (int i = 0; i < m_drawLevels.Count; i++)
                        {
                            GUILayout.Space(m_drawOffset[i]);
                            active |= DrawMenuList(m_drawLevels[i], i, active);
                            GUILayout.Indent(-1);
                        }
                    }
                    GUILayout.EndHorizontal();
                }
                GUI.EndArea();

                if (!active && GUI.Event.IsMouseActiveEvent() && !GUI.Event.Used)
                {
                    m_onActive = false;
                }
                else
                {
                    if (!GUI.RegionIsFocused) m_onActive = false;
                }
            }


        }

        private bool DrawMenuList(GUIMenuList menuList, int level, bool active)
        {
            GUILayout.BeginVertical();

            var bgdepth = GUI.GetDepth(1);

            foreach (var item in menuList.Items)
            {
                var offsety = GUI.CurLayout.Offset.y;
                if (GUILayout.Button(item.Label,GUIStyle.Current.ColorBackground, GUIOption.Width(100)))
                {
                    if (item is GUIMenuItem)
                    {
                        var menuitem = item as GUIMenuItem;
                        if (menuitem.Function != null) menuitem.Function.Invoke();

                        m_onActive = false;
                    }
                    else if (item is GUIMenuList)
                    {
                        var menulist = item as GUIMenuList;

                        var levelLastIndex = m_drawLevels.Count - 1;

                        if (m_drawLevels[levelLastIndex] != menulist)
                        {
                            if (levelLastIndex > level)
                            {
                                m_drawLevels.RemoveRange(level + 1, levelLastIndex - level);
                                m_drawOffset.RemoveRange(level + 1, levelLastIndex - level);
                            }
                            m_drawLevels.Add(menulist);
                            m_drawOffset.Add(offsety - m_drawOffset[levelLastIndex]);

                        }
                    }
                    
                }
                GUILayout.Space(-2);
            }
            GUILayout.Space(2);

            var layoutrect = GUILayout.EndVertical();
            var layoutrectab = GUI.GetAbsoluteRect(layoutrect);
            var depthprev = GUI.SetDepth(bgdepth);

            GUI.RectAbsolute(layoutrectab, GUIStyle.Current.ColorActiveD);
            GUI.SetDepth(depthprev);

            if (!GUI.Event.Used && !active)
            {
                return GUIUtility.RectContainsCheck(layoutrectab, GUI.Event.Pointer);
            }

            return active;
        }

    }

}
