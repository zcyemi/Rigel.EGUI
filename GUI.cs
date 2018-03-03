using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rigel.GUI.Collections;
using Rigel.GUI.Component;

namespace Rigel.GUI
{
    public static partial class GUI
    {

        /// v0                v1
        /// +-----------------+
        /// |                 |
        /// |                 |
        /// +-----------------+
        /// v4                v3
        ///
        public static void Rect(Vector4 rect, Vector4 color)
        {
            rect = rect.Move(CurArea.Rect);
            RectAbsolute(rect, color);
        }
        public static void RectAbsolute(Vector4 rect, Vector4 color, bool noclip = false)
        {
            if (GUI.CurArea.Clip && !noclip)
            {
                //RectIntersectCheck
                var arearect = GUI.CurArea.Rect;
                if (!GUIUtility.RectClipAbosolute(ref rect, GUI.CurArea.Rect))
                {
                    return;
                }
            }
            BufRect.AddVertices(new Vector4(rect.x, rect.y, DepthValue, 1), color, Vector2.zero);
            BufRect.AddVertices(new Vector4(rect.x + rect.z, rect.y, DepthValue, 1), color, Vector2.zero);
            BufRect.AddVertices(new Vector4(rect.x + rect.z, rect.y + rect.w, DepthValue, 1), color, Vector2.zero);
            BufRect.AddVertices(new Vector4(rect.x, rect.y + rect.w, DepthValue, 1), color, Vector2.zero);

            DepthValue -= DepthStep;
        }
        public static void Border(Vector4 rect, Vector4 color, int width = 1)
        {
            rect = rect.Move(CurArea.Rect);
            BorderAbsolute(rect, color, width);
        }
        public static void BorderAbsolute(Vector4 rect, Vector4 color, int width = 1)
        {
            var r = rect;
            var rh = rect;
            r.z = width;
            RectAbsolute(r, color);
            r.x += (rect.z - width);
            RectAbsolute(r, color);

            rh.w = 1;
            RectAbsolute(rh, color);
            rh.y += (rect.w - width);
            RectAbsolute(rh, color);
        }

        public static void DebugFontTexture(Vector4 rect, Vector4 color)
        {

            BufText.AddVertices(new Vector4(rect.x, rect.y, DepthValue, 1), color, Vector2.zero);
            BufText.AddVertices(new Vector4(rect.x + rect.z, rect.y, DepthValue, 1), color, new Vector2(1, 0));
            BufText.AddVertices(new Vector4(rect.x + rect.z, rect.y + rect.w, DepthValue, 1), color, new Vector2(1, 1));
            BufText.AddVertices(new Vector4(rect.x, rect.y + rect.w, DepthValue, 1), color, new Vector2(0, 1));

            DepthValue -= DepthStep;
        }
        public static int Char(Vector4 rect, Char c, Vector4 color, Vector2 pos, bool clip = true)
        {
            rect = rect.Move(CurArea.Rect);
            return CharAbsolute(rect, c, color, pos, clip);
        }
        public static int CharAbsolute(Vector4 recta, Char c, Vector4 color, Vector2 pos, bool clip = true)
        {
            Vector2 posa = recta.Pos() + pos;

            var glyph = Font.GetGlyphInfo(c);

            Vector4 charrect = new Vector4(posa, glyph.PixelWidth, glyph.PixelHeight);
            charrect.X += glyph.LineOffsetX;
            charrect.Y += glyph.LineOffsetY;

            float x2 = charrect.X + charrect.Z;
            float y2 = charrect.Y + charrect.W;

            if (clip)
            {
                //TODO implement clip
                float rectx2 = (recta.x + recta.z);
                float recty2 = (recta.y + recta.w);

                Vector2 uv0 = glyph.UV[0];
                Vector2 uv3 = glyph.UV[3];
                Vector2 uv2 = glyph.UV[2];
                Vector2 uv1 = glyph.UV[1];


                if (x2 > rectx2)
                {
                    var xoff = (x2 - rectx2) * GUI.Font.UVUnit;
                    uv3.x -= xoff;
                    uv2.x -= xoff;

                    x2 = rectx2;
                }
                if (y2 > recty2)
                {
                    var yoff = (y2 - recty2) * GUI.Font.UVUnit;
                    uv1.y -= yoff;
                    uv2.y -= yoff;

                    y2 = recty2;
                }

                if (charrect.x < recta.x)
                {
                    var xoff = recta.x - charrect.x;
                    uv0.x += xoff;
                    uv1.x += xoff;

                    charrect.x = recta.x;
                }
                if (charrect.y < recta.y)
                {
                    var yoff = recta.y - charrect.y;
                    uv0.y += yoff;
                    uv3.y += yoff;

                    charrect.y = recta.y;
                }

                BufText.AddVertices(new Vector4(charrect.X, charrect.Y, DepthValue, 1), color, uv0);
                BufText.AddVertices(new Vector4(x2, charrect.Y, DepthValue, 1), color, uv3);
                BufText.AddVertices(new Vector4(x2, y2, DepthValue, 1), color, uv2);
                BufText.AddVertices(new Vector4(charrect.X, y2, DepthValue, 1), color, uv1);
            }
            else
            {
                BufText.AddVertices(new Vector4(charrect.X, charrect.Y, DepthValue, 1), color, glyph.UV[0]);
                BufText.AddVertices(new Vector4(x2, charrect.Y, DepthValue, 1), color, glyph.UV[3]);
                BufText.AddVertices(new Vector4(x2, y2, DepthValue, 1), color, glyph.UV[2]);
                BufText.AddVertices(new Vector4(charrect.X, y2, DepthValue, 1), color, glyph.UV[1]);
            }

            DepthValue -= DepthStep;

            return glyph.AdvancedX + 1;
        }
        public static void Text(Vector4 rect, String text, Vector4 color, Vector2 pos, bool clip = true)
        {
            if (GUI.CurArea.Clip)
            {
                var intersect = GUIUtility.RectIntersectCheck(GUI.CurArea.Rect, rect);
                if (!intersect) return;
            }
            rect = rect.Move(CurArea.Rect);
            TextAbsolute(rect, text, color, pos, clip);
        }
        /// <summary>
        /// Single line text
        /// </summary>
        /// <param name="recta"></param>
        /// <param name="text"></param>
        /// <param name="color"></param>
        /// <param name="pos"></param>
        /// <param name="noclip"></param>
        public static void TextAbsolute(Vector4 recta, String text, Vector4 color, Vector2 pos, bool clip = true)
        {
            if (GUI.CurArea.Clip)
            {
                Vector4 areaRect = GUI.CurArea.Rect;

                Vector4 rect = recta;
                rect.x -= areaRect.x;
                rect.y -= areaRect.y;

                if (rect.x < 0)
                {
                    pos.x += rect.x;
                    rect.x = 0;
                }
                if (rect.z + rect.x > areaRect.z)
                {
                    rect.z = areaRect.z - rect.x;
                }

                if (rect.y < 0)
                {
                    pos.y += rect.y;
                    rect.y = 0;
                }
                if (rect.w + rect.y > areaRect.w)
                {
                    rect.w = areaRect.w - rect.y;
                }

                recta = rect.Move(areaRect.Pos());
            }

            Vector2 startpos = pos;
            if (pos.y > recta.w) return;
            if (pos.x > recta.z) return;

            bool yclip = true;
            if (pos.y >= 0 && pos.y + Font.FontPixelSize < recta.w)
            {
                yclip = false;
            }

            if (yclip && GUIDebug.DebugTextClip) color = RigelColor.Green;

            foreach (var c in text)
            {
                if (c < 33)
                {
                    startpos.x += 6;
                    continue;
                }

                var charWidth = Font.GetCharWidth(c);
                int charendpos = (int)startpos.x + charWidth;

                if (charendpos < 0)
                {
                    startpos.x = charendpos;
                    continue;
                }

                if (startpos.x > recta.z) return;

                if (clip)
                {
                    if (startpos.x >= 0 && charendpos < recta.z && !yclip)
                    {
                        startpos.x += CharAbsolute(recta, c, color, startpos, false);
                    }
                    else
                    {
                        if (GUIDebug.DebugTextClip) color = RigelColor.Red;
                        startpos.x += CharAbsolute(recta, c, color, startpos, true);
                    }
                }
                else
                {
                    startpos.x += CharAbsolute(recta, c, color, startpos, false);
                }
            }
        }

        public static bool Button(Vector4 rect, string label, params GUIOption[] option)
        {
            rect = rect.Move(CurArea.Rect);
            return ButtonAbsolute(rect, label, option);
        }
        public static bool Button(Vector4 rect, string label, Vector4 color, params GUIOption[] option)
        {
            rect = rect.Move(CurArea.Rect);
            return ButtonAbsolute(rect, label, color, option);
        }
        public static bool ButtonAbsolute(Vector4 recta, string label, params GUIOption[] option)
        {
            return ButtonAbsolute(recta, label, GUIStyle.Current.BtnColor, option);
        }
        public static bool ButtonAbsolute(Vector4 recta, string label, Vector4 color, params GUIOption[] option)
        {
            recta = recta.Padding(1);

            GUIOptionAlign optAlign = null;
            if (option != null)
            {
                option.GetOption(out optAlign);
            }

            bool clicked = false;

            var e = GUI.Event;


            if (e.Used)
            {
                GUI.RectAbsolute(recta, color);
            }
            else
            {
                if (GUIUtility.RectContainsCheck(recta, e.Pointer))
                {

                    if (e.EventType == RigelGUIEventType.MouseClick)
                    {
                        GUI.RectAbsolute(recta, GUIStyle.Current.ColorActiveD);

                        GUI.Event.Use();
                        clicked = true;
                    }
                    else
                    {
                        if (e.EventType == RigelGUIEventType.MouseMove)
                        {
                            GUI.RectAbsolute(recta, GUIStyle.Current.ColorActive);
                        }
                        else
                        {
                            GUI.RectAbsolute(recta, color);
                        }

                    }
                }
                else
                {
                    GUI.RectAbsolute(recta, color);
                }
            }

            Vector2 offset = new Vector2(2, (int)(recta.w - GUI.Font.FontPixelSize) / 2);
            if (optAlign == null || optAlign == GUIOption.AlignCenter)
            {
                offset.x = Mathf.FloorToInt((recta.z - GUI.Font.GetTextWidth(label)) / 2);
            }
            else if (optAlign == GUIOption.AlignRight)
            {
                offset.x = recta.z - GUI.Font.GetTextWidth(label) - 2;
            }

            GUI.TextAbsolute(recta, label, RigelColor.White, offset, true);

            return clicked;
        }


        #region Drag Drop

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="rect"></param>
        /// <param name="dropcontent"></param>
        /// <param name="draw"></param>
        /// <param name="drawondrag"></param>
        /// <param name="drawonhover"></param>
        /// <returns>is ondrag</returns>
        public static bool DragRect(string contract, Vector4 rect, object dropcontent = null, bool draw = false, bool drawondrag = true, bool drawonhover = false)
        {
            var rectab = GUI.GetAbsoluteRect(rect);
            return DragRectAbsolute(contract, rectab, dropcontent, draw, drawondrag, drawonhover);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contract"></param>
        /// <param name="rectab"></param>
        /// <param name="dropcontent"></param>
        /// <param name="draw"></param>
        /// <param name="drawondrag"></param>
        /// <param name="drawonhover"></param>
        /// <returns>is ondrag</returns>
        public static bool DragRectAbsolute(string contract, Vector4 rectab, object dropcontent = null, bool draw = false, bool drawondrag = true, bool drawonhover = false)
        {
            if (draw)
                GUI.BorderAbsolute(rectab, GUIStyle.Current.ColorActiveD);

            var dragrect = GUI.GetDragRect(rectab);
            var ds = dragrect.DragStage;
            if (ds.OnDrag(rectab))
            {
                GUI.SetFrameDragDrop();

                rectab = rectab.Move(GUI.Event.Pointer - ds.EnterPos);


                bool onhover = false;

                if (ds.Stage == GUIDragStateStage.Update)
                {

                    onhover = GUI.HoverDrop(contract, dropcontent);

                }
                else if (ds.Stage == GUIDragStateStage.Exit)
                {
                    GUI.EmmitDrop(contract, dropcontent);
                }

                if (drawondrag && (drawonhover || !onhover))
                {
                    GUI.SetDepthLayer(GUILayerType.Overlay);
                    GUI.RectAbsolute(rectab, GUIStyle.Current.ColorActiveD, true);
                    GUI.RestoreDepthLayer();
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rect"></param>
        /// <param name="dropcontent"></param>
        /// <param name="contract"></param>
        /// <param name="draw"></param>
        /// <param name="drawondrag"></param>
        /// <param name="drawonhover"></param>
        /// <returns>is ondrag</returns>
        public static bool DragRect<T>(Vector4 rect, T dropcontent, string contract = "", bool draw = false, bool drawondrag = true, bool drawonhover = false)
        {
            var ractab = GUI.GetAbsoluteRect(rect);
            return DragRectAbsolute(ractab, dropcontent, contract, draw, drawondrag, drawonhover);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rectab"></param>
        /// <param name="dropcontent"></param>
        /// <param name="contract"></param>
        /// <param name="draw"></param>
        /// <param name="drawondrag"></param>
        /// <param name="drawonhover"></param>
        /// <returns>is ondrag </returns>
        public static bool DragRectAbsolute<T>(Vector4 rectab, T dropcontent, string contract = "", bool draw = false, bool drawondrag = true, bool drawonhover = false)
        {
            if (draw)
                GUI.BorderAbsolute(rectab, GUIStyle.Current.ColorActiveD);
            var dragrect = GUI.GetDragRect(rectab);
            var ds = dragrect.DragStage;
            if (ds.OnDrag(rectab))
            {
                GUI.SetFrameDragDrop();

                rectab = rectab.Move(GUI.Event.Pointer - ds.EnterPos);

                var contractfull = typeof(T).FullName + contract;

                bool onhover = false;

                if (ds.Stage == GUIDragStateStage.Update)
                {
                    onhover = GUI.HoverDrop(contractfull, dropcontent);
                }
                else if (ds.Stage == GUIDragStateStage.Exit)
                {
                    onhover = GUI.EmmitDrop(contractfull, dropcontent);
                }

                if (drawondrag && (drawonhover || !onhover))
                {
                    GUI.SetDepthLayer(GUILayerType.Overlay);
                    GUI.RectAbsolute(rectab, GUIStyle.Current.ColorActiveD, true);
                    GUI.RestoreDepthLayer();
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Create a drop rect region which receives all draggable object with contract.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="contract"></param>
        /// <param name="ondrop"></param>
        /// <param name="onHover"></param>
        /// <returns>is onhover</returns>
        public static bool DropRect(Vector4 rect, string contract, Action<object> ondrop, Action onHover = null)
        {
            var rectab = GetAbsoluteRect(rect);
            return DropRectAbsolute(rectab, contract, ondrop, onHover);
        }
        /// <summary>
        /// Create a drop rect region which receives all draggable object with contract.
        /// </summary>
        /// <param name="rectab"></param>
        /// <param name="contract"></param>
        /// <param name="ondrop"></param>
        /// <param name="onHover"></param>
        /// <returns>is onhover</returns>
        public static bool DropRectAbsolute(Vector4 rectab, string contract, Action<object> ondrop, Action onHover = null)
        {
            GUI.BorderAbsolute(rectab, GUIStyle.Current.ColorActiveD);
            var objDropRect = GUI.GetDropRect(rectab, (d) =>
            {
                d.Contract = contract;
            });


            if (objDropRect.CheckDropped())
            {
                if (ondrop != null)
                {
                    ondrop.Invoke(objDropRect.DropData);
                }
                return false;
            }

            return objDropRect.GetHoverStatus();
        }
        /// <summary>
        /// Create a drop rect region which receives draggable object with type of T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rect"></param>
        /// <param name="ondrop"></param>
        /// <param name="contract"></param>
        /// <param name="onhover"></param>
        /// <returns>is onhover</returns>
        public static bool DropRect<T>(Vector4 rect, Action<T> ondrop, string contract = "", Action onhover = null)
        {
            var rectab = GetAbsoluteRect(rect);
            return DropRectAbsolute(rectab, ondrop, contract, onhover);
        }
        /// <summary>
        /// Create a drop rect region which receives draggable object with type of T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rectab"></param>
        /// <param name="ondrop"></param>
        /// <param name="contract"></param>
        /// <param name="onhover"></param>
        /// <returns>is onhover</returns>
        public static bool DropRectAbsolute<T>(Vector4 rectab, Action<T> ondrop, string contract = "", Action onhover = null)
        {
            GUI.BorderAbsolute(rectab, GUIStyle.Current.ColorActiveD);

            var objDropRect = GUI.GetDropRect(rectab, (d) =>
            {
                d.Contract = typeof(T).FullName + contract;
            });

            if (objDropRect.CheckDropped())
            {
                if (ondrop != null)
                {
                    ondrop.Invoke((T)objDropRect.DropData);
                }
                return false;
            }

            return objDropRect.GetHoverStatus();
        }

        #endregion

        public static void DrawContentDocker(GUIContentDocker docker, Vector4 rect)
        {
            var rectab = GetAbsoluteRect(rect);
            DrawContentDockerAbsolute(docker, rectab);
        }
        public static void DrawContentDockerAbsolute(GUIContentDocker docker, Vector4 rectab)
        {
            docker.Draw(rectab);
        }

        #region ReorderedList

        public static bool ReorderedList<T>(IList<T> list, Vector4 rect) where T : class
        {
            var rectab = GetAbsoluteRect(rect);
            return ReorderedListAbsolute(list, rectab);
        }
        public static bool ReorderedListVertical<T>(IList<T> list, Vector4 rect) where T : class
        {
            var rectab = GetAbsoluteRect(rect);
            return ReorderedListVerticalAbsolute(list, rect);
        }
        public static bool ReorderedListAbsolute<T>(IList<T> list, Vector4 rectab) where T : class
        {
            if (list == null) return false;
            var itemrect = rectab;
            itemrect.z = 50;
            int dragindex = -1;
            int tarindex = -1;
            bool swap = false;
            for (var i = 0; i < list.Count; i++)
            {
                var dragstate = GUI.GetObjDragStete(itemrect);
                if (dragstate.OnDrag(itemrect))
                {
                    dragindex = i;
                    tarindex = Mathf.FloorToInt((GUI.Event.Pointer.x - rectab.x) / 50);
                    if (dragstate.Stage == GUIDragStateStage.Exit) swap = true;
                    break;
                }
                itemrect.x += 50;

            }
            //Swap
            tarindex = Mathf.Clamp(tarindex, 0, list.Count - 1);
            if (swap && dragindex != tarindex)
            {
                var temp = list[dragindex];
                list[dragindex] = list[tarindex];
                list[tarindex] = temp;
                dragindex = -1;
                tarindex = -1;
            }
            if (dragindex >= 0 && (tarindex != dragindex))
            {
                itemrect.x = rectab.x;
                for (var i = 0; i < list.Count; i++)
                {
                    if (i == tarindex || i == dragindex)
                    {
                        var di = (i == tarindex ? dragindex : tarindex);
                        GUI.ButtonAbsolute(itemrect, list[di].ToString(), di == dragindex ? GUIStyle.Current.ColorActive : GUIStyle.Current.ColorActiveD);
                    }
                    else
                    {
                        GUI.ButtonAbsolute(itemrect, list[i].ToString());
                    }
                    itemrect.x += 50;
                }
            }
            else
            {
                itemrect.x = rectab.x;
                for (var i = 0; i < list.Count; i++)
                {
                    GUI.ButtonAbsolute(itemrect, list[i].ToString());
                    itemrect.x += 50;
                }
            }
            GUI.BorderAbsolute(rectab, GUIStyle.Current.ColorActiveD);
            return false;
        }
        public static bool ReorderedListVerticalAbsolute<T>(IList<T> list, Vector4 rectab) where T : class
        {
            if (list == null) return false;
            var itemrect = rectab;
            itemrect.w = 25;
            int dragindex = -1;
            int tarindex = -1;
            bool swap = false;
            for (var i = 0; i < list.Count; i++)
            {
                var dragstate = GUI.GetObjDragStete(itemrect);
                if (dragstate.OnDrag(itemrect))
                {
                    dragindex = i;
                    tarindex = Mathf.FloorToInt((GUI.Event.Pointer.y - rectab.y) / 25);
                    if (dragstate.Stage == GUIDragStateStage.Exit) swap = true;
                    break;
                }

                itemrect.y += 25;
            }
            //Swap
            tarindex = Mathf.Clamp(tarindex, 0, list.Count - 1);
            if (swap && dragindex != tarindex)
            {
                var temp = list[dragindex];
                list[dragindex] = list[tarindex];
                list[tarindex] = temp;

                dragindex = -1;
                tarindex = -1;
            }
            if (dragindex >= 0 && (tarindex != dragindex))
            {
                itemrect.y = rectab.y;
                for (var i = 0; i < list.Count; i++)
                {
                    if (i == tarindex || i == dragindex)
                    {
                        var di = (i == tarindex ? dragindex : tarindex);
                        GUI.ButtonAbsolute(itemrect, list[di].ToString(), di == dragindex ? GUIStyle.Current.ColorActive : GUIStyle.Current.ColorActiveD);
                    }
                    else
                    {
                        GUI.ButtonAbsolute(itemrect, list[i].ToString());
                    }
                    itemrect.y += 25;
                }
            }
            else
            {
                itemrect.y = rectab.y;
                for (var i = 0; i < list.Count; i++)
                {
                    GUI.ButtonAbsolute(itemrect, list[i].ToString());
                    itemrect.y += 25;
                }
            }
            GUI.BorderAbsolute(rectab, GUIStyle.Current.ColorActiveD);
            return false;
        }

        #endregion
    }
}
