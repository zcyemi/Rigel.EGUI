using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rigel.GUI.Collections;
using Rigel.GUI.Component;

namespace Rigel.GUI
{
    public static class GUILayout
    {

        private static GUIFrame Frame { get { return GUI.m_frame; } }

        internal static int LineHeight = 23;

        #region Layouting

        public static void BeginHorizontal()
        {
            Frame.LayoutStack.Push(GUI.CurLayout);
            GUI.CurLayout.AlignHorizontal = true;
            GUI.CurLayout.SizeMax = Vector3.zero;
            GUI.CurLayout.RemainSize = GUI.CurArea.Rect.Size() - GUI.CurLayout.Offset;
        }
        public static Vector4 EndHorizontal()
        {
            var curLayout = GUI.CurLayout;
            var prevLayout = Frame.LayoutStack.Pop();
            var layoutrect = new Vector4(prevLayout.Offset, curLayout.SizeMax);
            prevLayout.Offset.y += curLayout.SizeMax.y;
            layoutrect.z = curLayout.Offset.x - prevLayout.Offset.x;
            GUI.CurLayout = prevLayout;

            return layoutrect;
        }
        public static void BeginVertical()
        {
            Frame.LayoutStack.Push(GUI.CurLayout);
            GUI.CurLayout.AlignHorizontal = false;
            GUI.CurLayout.SizeMax = Vector3.zero;
            GUI.CurLayout.RemainSize = GUI.CurArea.Rect.Size() - GUI.CurLayout.Offset;
        }
        public static Vector4 EndVertical()
        {
            var curLayout = GUI.CurLayout;
            var prevLayout = Frame.LayoutStack.Pop();
            var layoutrect = new Vector4(prevLayout.Offset, curLayout.SizeMax);
            prevLayout.Offset.x += curLayout.SizeMax.x;
            layoutrect.w = curLayout.Offset.y - prevLayout.Offset.y;
            GUI.CurLayout = prevLayout;

            return layoutrect;
        }

        #endregion

        #region Primitive

        public static void Rect(Vector2 size, Vector4 color)
        {
            GUI.Rect(new Vector4(GUI.CurLayout.Offset, size), color);
            AutoCaculateOffset(size.x, size.y);
        }

        #endregion

        #region Basic Flow Control

        public static void Space(int offset)
        {
            GUI.CurLayout.Offset.y += offset;
        }
        public static void Space(float offset)
        {
            GUI.CurLayout.Offset.y += (int)offset;
        }
        public static void Indent(int offset)
        {
            GUI.CurLayout.Offset.x += offset;
        }
        public static void Indent(float offset)
        {
            GUI.CurLayout.Offset.x += (int)offset;
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
        public static void AutoCaculateOffset(int w, int h)
        {
            var layout = GUI.CurLayout;
            layout.LastDrawRect = new Vector4(layout.Offset, w, h);

            var offsetmax = layout.Offset;
            offsetmax.x += w;
            offsetmax.y += h;

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

            //ContentMax
            GUI.CurArea.ContentMax.Y = Mathf.Max(offsetmax.y, GUI.CurArea.ContentMax.y);
            GUI.CurArea.ContentMax.x = Mathf.Max(offsetmax.x, GUI.CurArea.ContentMax.x);

            GUI.CurLayout = layout;
        }

        #endregion

        #region Button

        /// <summary>
        /// Draw Button
        /// </summary>
        /// <param name="label"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static bool Button(string label, params GUIOption[] options)
        {
            return Button(label, GUIStyle.Current.BtnColor, options);
        }

        /// <summary>
        /// Draw Button
        /// </summary>
        /// <param name="label"></param>
        /// <param name="options">GUIOptionGrid | GUIOptionWidth | GUIOptionHeight | GUIOptionAlign </param>
        /// <returns></returns>
        public static bool Button(string label, Vector4 color, params GUIOption[] options)
        {
            float w = GUI.CurLayout.RemainSize.x;
            float h = LineHeight;

            GUIOptionAlign optAlign = null;

            if (options != null)
            {
                GUIOptionGrid grid = null;
                GUIOptionWidth optWidth = null;
                GUIOptionHeight optHeight = null;
                options.GetOption(out grid, out optWidth, out optHeight);

                options.GetOption(out optAlign);

                if (optWidth != null)
                {
                    w = optWidth.Value;
                }
                else if (grid != null)
                {
                    w *= grid.Value;
                }
                if (optHeight != null)
                {
                    h = optHeight.Value;
                }
            }

            var rect = new Vector4(GUI.CurLayout.Offset, w, h);
            rect = rect.Padding(1);

            bool clicked = false;

            var recta = rect.Move(GUI.CurArea.Rect);
            if (GUI.Event.Used)
            {
                GUI.RectAbsolute(recta, color);
            }
            else
            {
                if (GUIUtility.RectContainsCheck(recta, GUI.Event.Pointer))
                {
                    if (GUI.Event.EventType == RigelGUIEventType.MouseClick)
                    {
                        GUI.RectAbsolute(recta, GUIStyle.Current.ColorActiveD);
                        GUI.Event.Use();
                        clicked = true;
                    }
                    else
                    {
                        GUI.RectAbsolute(recta, GUIStyle.Current.ColorActive);
                        if(GUI.Event.EventType == RigelGUIEventType.MouseMove)
                        {
                            GUI.Event.Use();
                        }
                    }
                }
                else
                {
                    GUI.RectAbsolute(recta, color);
                }
            }

            //Draw label

            Vector2 offset = new Vector2(4, (rect.w - GUI.Font.FontPixelSize) / 2);
            if (optAlign == null || optAlign == GUIOption.AlignCenter)
            {
                offset.x = (int)((rect.z - GUI.Font.GetTextWidth(label)) / 2);
            }
            else if (optAlign == GUIOption.AlignRight)
            {
                offset.x = rect.z - GUI.Font.GetTextWidth(label) - 2;
            }
            GUI.Text(rect, label, RigelColor.White, offset, true);
            AutoCaculateOffset(w, h);

            return clicked;
        }

        #endregion

        #region Text

        public static void Label(string content)
        {
            Label(content, RigelColor.White);
        }
        public static void Label(string content, Vector4 color)
        {
            var offset = new Vector2(2, (int)(LineHeight - GUI.Font.FontPixelSize) / 2);
            GUI.Text(new Vector4(GUI.CurLayout.Offset, GUI.CurLayout.RemainSize.x, LineHeight), content, color, offset);

            int textWidth = GUI.Font.GetTextWidth(content);

            AutoCaculateOffset(textWidth, LineHeight);
        }

        #endregion

        #region TabView

        /// <summary>
        /// Draw TabView Horizontal
        /// </summary>
        /// <param name="index"></param>
        /// <param name="tabnames"></param>
        /// <param name="draw"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static int TabView(int index, List<string> tabnames, Action<int> draw, params GUIOption[] options)
        {
            var sizeRemain = GUI.CurLayout.RemainSize;
            var rect = new Vector4(GUI.CurLayout.Offset, sizeRemain);

            if (options != null)
            {
                GUIOptionHeight optHeight;
                options.GetOption(out optHeight);
                if (optHeight != null)
                {
                    rect.w = optHeight.Value;
                }
            }
            var rectab = GUI.GetAbsoluteRect(rect);
            var tabview = GUI.GetObjTabView(rectab);

            int ret = tabview.Draw(rect, index, tabnames, draw);
            AutoCaculateOffset(rect.z, rect.w);

            return ret;
        }

        /// <summary>
        /// Draw TabView Vertical
        /// </summary>
        /// <param name="index"></param>
        /// <param name="tabnames"></param>
        /// <param name="draw"></param>
        /// <param name="tabWidth"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static int TabViewVertical(int index, List<string> tabnames, Action<int> draw, int tabWidth, params GUIOption[] options)
        {
            var sizeRemain = GUI.CurLayout.RemainSize;
            var rect = new Vector4(GUI.CurLayout.Offset, sizeRemain);

            if (options != null)
            {

            }
            var rectab = GUI.GetAbsoluteRect(rect);
            var tabview = GUI.GetObjTabView(rectab, (tv) => { tv.SetVerticalMode(tabWidth); });

            int ret = tabview.Draw(rect, index, tabnames, draw);
            AutoCaculateOffset(rect.z, rect.w);

            return ret;
        }

        #endregion

        #region ScrollView

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="scrolltype"></param>
        /// <param name="options"></param>
        public static void BeginScrollView(Vector2 pos, GUIScrollType scrolltype = GUIScrollType.Vertical, params GUIOption[] options)
        {
            {
                var sizeremian = GUI.CurLayout.RemainSize;

                if (options != null)
                {
                    GUIOptionHeight optHeight;
                    options.GetOption(out optHeight);
                }

                var rect = new Vector4(GUI.CurLayout.Offset, sizeremian);
                var rectab = GUI.GetAbsoluteRect(rect);

                var scrollview = GUI.GetObjScrollView(rectab);

                scrollview.Draw(rect, rectab, pos, scrolltype);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Vector2 EndScrollView()
        {
            var rect = GUI.CurArea.Rect;
            var sv = GUI.GetObjScrollView(rect);

            var pos = sv.LateDraw();

            GUILayout.AutoCaculateOffset(rect.Z, rect.W);
            return pos;
        }

        #endregion

        #region Menu

        /// <summary>
        /// Draw GUIMenuList
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="option"></param>
        public static void DrawMenu(GUIMenuList menu, params GUIOption[] option)
        {
            var clicked = GUILayout.Button(menu.Label, option);
            var lastRect = GUI.CurLayout.LastDrawRect;
            var menuDraw = GUI.GetObjMenuDraw(menu.GetHashCode(), lastRect);
            GUI.SetDepthLayer(GUILayerType.Overlay);
            menuDraw.Draw(clicked, menu, lastRect);
            GUI.RestoreDepthLayer();

        }

        [TODO]
        public static void ContextMenu()
        {

        }

        #endregion

        #region Drag Drop

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="size"></param>
        /// <param name="dropcontent"></param>
        /// <param name="contract"></param>
        /// <returns>is ondrag</returns>
        public static bool DragRect<T>(Vector2 size,T dropcontent, string contract = "")
        {
            var rect = new Vector4(GUI.CurLayout.Offset, size);

            bool ondrag = GUI.DragRect(rect, dropcontent, contract,true,true);

            AutoCaculateOffset(rect.z, rect.w);
            GUILayout.Space(2);

            return ondrag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="size"></param>
        /// <param name="dropcontent"></param>
        /// <returns>is ondrag</returns>
        public static bool DragRect(string contract,Vector2 size,object dropcontent = null)
        {
            var rect = new Vector4(GUI.CurLayout.Offset, size);
            var onhover = GUI.DragRect(contract, rect, dropcontent, true, true);

            AutoCaculateOffset(rect.z, rect.w);
            GUILayout.Space(2);

            return onhover;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="size"></param>
        /// <param name="onDrop"></param>
        /// <param name="contract"></param>
        /// <param name="onHover"></param>
        /// <returns>is onhover</returns>
        public static bool DropRect<T>(Vector2 size,Action<T,object> onDrop, string contract = "")
        {
            var rect = new Vector4(GUI.CurLayout.Offset, size);
            var rectab = GUI.GetAbsoluteRect(rect);

            bool isonhover = GUI.DropRectAbsolute(rectab, onDrop, contract);
            AutoCaculateOffset(size.x, size.y);
            return isonhover;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <param name="contract"></param>
        /// <param name="onDrop"></param>
        /// <param name="onHover"></param>
        /// <returns>is ondrag</returns>
        public static bool DropRect(Vector2 size,string contract,Action<object,object> onDrop)
        {
            var rect = new Vector4(GUI.CurLayout.Offset, size);
            var rectab = GUI.GetAbsoluteRect(rect);

            bool isonhover = GUI.DropRectAbsolute(rectab, contract, onDrop);
            AutoCaculateOffset(size.x, size.y);
            return isonhover;
        }

        #endregion



    }
}
