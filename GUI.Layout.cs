using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rigel.GUI.Collections;

namespace Rigel.GUI
{
    public static class GUILayout
    {

        private static GUIFrame Frame { get { return GUI.m_frame; } }

        private static int LineHeight = 23;
        private static int LineOffset = 2;

        public static void BeginHorizontal()
        {
            Frame.LayoutStack.Push(GUI.CurLayout);
            GUI.CurLayout.AlignHorizontal = true;
            GUI.CurLayout.SizeMax = Vector3.zero;
            GUI.CurLayout.RectSize = GUI.CurArea.Rect.Size() - GUI.CurLayout.Offset;
        }

        public static void EndHorizontal()
        {
            var curLayout = GUI.CurLayout;
            var prevLayout = Frame.LayoutStack.Pop();


            prevLayout.Offset.y += curLayout.SizeMax.y;

            GUI.CurLayout = prevLayout;
        }

        public static void BeginVertical()
        {
            Frame.LayoutStack.Push(GUI.CurLayout);
            GUI.CurLayout.AlignHorizontal = false;
            GUI.CurLayout.SizeMax = Vector3.zero;
            GUI.CurLayout.RectSize = GUI.CurArea.Rect.Size() - GUI.CurLayout.Offset;
        }
        public static void EndVertical()
        {
            var curLayout = GUI.CurLayout;
            var prevLayout = Frame.LayoutStack.Pop();
            
            prevLayout.Offset.x += curLayout.SizeMax.x;
            GUI.CurLayout = prevLayout;
        }

        public static void Space(int offset)
        {
            GUI.CurLayout.Offset.y += offset;
        }
        public static void Indent(int offset)
        {
            GUI.CurLayout.Offset.x += offset;
        }

        internal static void AutoCaculateOffsetWidth(float w)
        {
            AutoCaculateOffset((int)w, LineHeight);
        }
        internal static void AutoCaculateOffsetWidth(int w)
        {
            AutoCaculateOffset(w, LineHeight);
        }
        internal static void AutoCaculateOffsetHeight(float h)
        {
            AutoCaculateOffset(0, (int)h);
        }
        internal static void AutoCaculateOffsetHeight(int h)
        {
            AutoCaculateOffset(0, h);
        }

        public static void AutoCaculateOffset(float w, float h)
        {
            AutoCaculateOffset((int)w, (int)h);
        }

        public static void AutoCaculateOffset(int w,int h)
        {

            var layout = GUI.CurLayout;

            if (layout.AlignHorizontal)
            {
                layout.Offset.x += w;
                
            }
            else
            {
                layout.Offset.y += h;

            }

            layout.SizeMax.y = Mathf.Max(h, layout.SizeMax.y);
            layout.SizeMax.x = Mathf.Max(w, layout.SizeMax.x);

            GUI.CurLayout = layout;
        }



        public static bool Button(string label,params GUIOption[] options)
        {
            float w = GUI.CurLayout.RectSize.x;
            float h = LineHeight;
            if(options != null)
            {
                GUIOptionGrid grid = null;
                GUIOptionWidth optWidth = null;
                GUIOptionHeight optHeight = null;
                options.GetOption(out grid,out optWidth,out optHeight);
                if(optWidth != null)
                {
                    w = optWidth.Value;
                }
                else if (grid != null)
                {
                    w *= grid.Value;
                }

                if(optHeight != null)
                {
                    h = optHeight.Value;
                }


            }

            var rect = new Vector4(GUI.CurLayout.Offset,w, h);
            GUI.Rect(rect,RigelColor.Random());

            AutoCaculateOffset(w, h);

            
            return false;
        }

    }
}
