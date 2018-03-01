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


        private GUIContentDocker m_mainDocker;

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

            m_mainDocker.AddContent(content);
        }




        public void SetSplitMode(GUISplitViewContentMode mode)
        {
            if(m_mainDocker== null)
            {
                m_mainDocker = new GUIContentDocker();
            }

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

                GUI.DrawContentDocker(m_mainDocker,new Vector4(1,0,GUI.CurAreaRect.z,GUI.CurAreaRect.w));

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
