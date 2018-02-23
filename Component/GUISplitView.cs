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
        Horizontal =1,
        Vertical = 2,
    }

    public sealed class GUISplitView : GUIRectView
    {
        public GUISplitViewContentMode SplitMode { get; set; } = GUISplitViewContentMode.Container;
        public GUISplitViewOrientation Orientation { get; set; } = GUISplitViewOrientation.Horizontal;

        private Vector4 m_splitRect = new Vector4(0, 0, 3, 4);

        public override void Update(RigelGUIEvent e)
        {
            if(SplitMode == GUISplitViewContentMode.Content)
            {
                //DrawContent
                if (Content != null) Content.OnGUI(e);

            }
            else
            {
                //DrawSplitBar
                m_splitRect.w = GUI.CurAreaRect.w;
                m_splitRect.x = GUI.CurAreaRect.z * 0.5f;

                GUI.Rect(m_splitRect, GUIStyle.Current.ColorBackgroundL2);
            }

            
        }


    }
}
