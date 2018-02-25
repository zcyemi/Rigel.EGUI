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
        public bool ShowWindowCloseBtn { get; set; } = false;
        public bool ShowWindowMaximizeBtn { get; set; } = false;
        public bool ShowWindowMinimizeBtn { get; set; } = false;

        protected Vector2 MinSize = new Vector2(400, 300);
        public string Caption { get; set; }

        protected bool m_onMove = false;

        public GUIWindowView()
        {
            Rect = new Vector4(100, 100, 400, 300);
            Caption = this.GetType().ToString();
        }

        protected virtual void OnWindowDragMove()
        {
            Rect = Rect.Move(m_dragMove.OffSet);
        }

        protected virtual void OnClickCloseBtn()
        {

        }

        protected virtual void OnClickMaximizeBtn()
        {

        }

        protected virtual void OnClickMinimizeBtn()
        {

        }

        protected virtual void OnWindowDragResize()
        {
            Rect.z += m_dragResize.OffSet.x;
            Rect.w += m_dragResize.OffSet.y;

            Rect.z = Mathf.Max(Rect.z, MinSize.x);
            Rect.w = Mathf.Max(Rect.w, MinSize.y);
        }

        public override void OnViewStart()
        {
            base.OnViewStart();

            m_onMove = false;
            //Background
            GUI.RectAbsolute(Rect.Padding(1), GUIStyle.Current.ColorBackgroundL1);

            //Cache depth
            var headerBGdepth = GUI.GetDepth();

            GUILayout.BeginHorizontal();
            if (!string.IsNullOrEmpty(Caption)) GUILayout.Label(Caption, RigelColor.White);

            var btnRect = new Vector4(Rect.z - 25, 1, 22, 22);
            if (ShowWindowCloseBtn)
            {
                if (GUI.Button(btnRect, "X")) OnClickCloseBtn();
                btnRect.X -= 23;
            }
            if (ShowWindowMaximizeBtn)
            {
                if (GUI.Button(btnRect, "+", GUIOptionAlign.AlignCenter)) OnClickMaximizeBtn();
                btnRect.X -= 23;
            }
            if (ShowWindowMinimizeBtn)
            {
                if (GUI.Button(btnRect, "_", GUIOptionAlign.AlignCenter)) OnClickMinimizeBtn();
            }
            GUILayout.EndHorizontal();

            {
                //Header drag move
                Vector4 rectHeader = Rect;
                rectHeader.w = 25;

                var checkRect = rectHeader;
                checkRect.z = btnRect.x;

                bool headerover = GUIUtility.RectContainsCheck(checkRect, GUI.Event.Pointer);
                if (Moveable && m_dragMove.OnDrag(headerover))
                {
                    OnWindowDragMove();
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
                        OnWindowDragResize();
                    }
                }

                headerBGdepth = GUI.SetDepth(headerBGdepth);
                GUI.RectAbsolute(rectHeader, headerover ? GUIStyle.Current.ColorActive : GUIStyle.Current.BtnColor);
                GUI.SetDepth(headerBGdepth);

            }


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
