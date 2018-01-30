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

        public static void Rect(Vector2 size,Vector4 color)
        {
            GUI.Rect(new Vector4(GUI.CurLayout.Offset,size), color);
            AutoCaculateOffset(size.x,size.y);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        /// <param name="options">GUIOptionGrid | GUIOptionWidth | GUIOptionHeight | GUIOptionAlign </param>
        /// <returns></returns>
        public static bool Button(string label,params GUIOption[] options)
        {
            float w = GUI.CurLayout.RectSize.x;
            float h = LineHeight;


            GUIOptionAlign optAlign = null;

            if(options != null)
            {
                GUIOptionGrid grid = null;
                GUIOptionWidth optWidth = null;
                GUIOptionHeight optHeight = null;
                options.GetOption(out grid,out optWidth,out optHeight);

                options.GetOption(out optAlign);

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
            rect = rect.Padding(1);

            bool clicked = false;

            var recta = rect.Move(GUI.CurArea.Rect);
            if (GUI.Event.Used)
            {
                GUI.RectAbsolute(recta, GUIStyle.Current.BtnColor);
            }
            else
            {
                if (GUIUtility.RectContainsCheck(recta, GUI.Event.Pointer))
                {
                    if(GUI.Event.EventType == RigelGUIEventType.MouseClick)
                    {
                        GUI.RectAbsolute(recta, GUIStyle.Current.ColorActiveD);
                        GUI.Event.Use();
                        clicked = true;
                    }
                    else
                    {
                        GUI.RectAbsolute(recta, GUIStyle.Current.ColorActive);
                    }
                }
                else
                {
                    GUI.RectAbsolute(recta, GUIStyle.Current.BtnColor);
                }
            }
            
            //Draw label

            Vector2 offset = new Vector2(4, (rect.w - GUI.Font.FontPixelSize) / 2);
            if (optAlign == null || optAlign == GUIOption.AlignCenter)
            {
                offset.x = (int)((rect.z - GUI.Font.GetTextWidth(label)) / 2);
            }
            else if(optAlign == GUIOption.AlignRight)
            {
                offset.x = rect.z - GUI.Font.GetTextWidth(label) - 2;
            }
            GUI.Text(rect, label, RigelColor.White, offset, true);

            AutoCaculateOffset(w, h);

            return clicked;
        }

        public static void Label(string content,Vector4 color)
        {
            var offset = new Vector2(2,(int) (LineHeight - GUI.Font.FontPixelSize) / 2);
            GUI.Text(new Vector4(GUI.CurLayout.Offset, GUI.CurLayout.RectSize), content,color, offset);

            int textWidth = GUI.Font.GetTextWidth(content);

            AutoCaculateOffset(textWidth, LineHeight);
        }

    }
}
