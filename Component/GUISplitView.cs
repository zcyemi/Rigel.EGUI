using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI.Component
{
    public enum GUISplitViewContentMode
    {
        Content = 1,
        Container = 2,
    }

    public enum GUISplitViewOrientation
    {
        Horizontal = 1,
        Vertical = 2,
    }

    public sealed class GUISplitView : GUIRectView
    {
        public GUISplitViewContentMode SplitMode { get; private set; } = GUISplitViewContentMode.Content;
        public GUISplitViewOrientation Orientation { get; set; } = GUISplitViewOrientation.Horizontal;

        private Vector4 m_splitRect = new Vector4(0, 0, 3, 4);

        public GUISplitView m_leftView = null;
        public GUISplitView m_rightView = null;

        public List<GUIContent> m_contents = new List<GUIContent>();
        public List<string> m_contenName = new List<string>();

        private int m_contentIndex = 0;


        public GUISplitView()
        {
            SetSplitMode(GUISplitViewContentMode.Content);
        }

        public GUISplitView(GUISplitViewContentMode mode = GUISplitViewContentMode.Content)
        {
            SetSplitMode(mode);
        }

        public override void SetContent(GUIContent content)
        {
            return;
        }

        public void AddContent(GUIContent content)
        {
            if (SplitMode == GUISplitViewContentMode.Container) return;

            content.View = this;

            m_contents.Add(content);
            m_contenName.Add(content.ContentName);
        }




        public void SetSplitMode(GUISplitViewContentMode mode)
        {
            if (mode == SplitMode) return;
            if (mode == GUISplitViewContentMode.Container)
            {
                if (m_leftView == null) m_leftView = new GUISplitView();
                if (m_rightView == null) m_rightView = new GUISplitView();

                AddSubView(m_leftView);
                AddSubView(m_rightView);
            }
            else
            {
                RemoveSubView(m_leftView);
                RemoveSubView(m_rightView);
            }

            SplitMode = mode;
        }

        public void DrawDockerMask()
        {
            //GUI.Rect(new Vector4(100, 100, 50, 50), GUIStyle.Current.ColorActiveD);

            

        }

        public override void Update(RigelGUIEvent e)
        {
            if (SplitMode == GUISplitViewContentMode.Content)
            {
                if(m_contents.Count != 0)
                {

                    m_contentIndex = GUILayout.TabView(m_contentIndex, m_contenName, (i) =>
                    {
                        m_contents[i].OnGUI(e);
                    });

                }

            }
            else
            {
                //DrawSplitBar
                m_splitRect.w = GUI.CurAreaRect.w;
                m_splitRect.x = GUI.CurAreaRect.z * 0.5f;

                GUI.Rect(m_splitRect, GUIStyle.Current.ColorBackgroundL2);
            }

            DrawDockerMask();
        }


    }
}
