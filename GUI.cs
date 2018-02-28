using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rigel.GUI.Collections;

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

        public static bool ButtonAbsolute(Vector4 recta, string label, params GUIOption[] option)
        {
            recta = recta.Padding(1);

            GUIOptionAlign optAlign = null;
            if (option != null)
            {
                option.GetOption(out optAlign);
            }

            bool clicked = false;


            if (GUI.Event.Used)
            {
                GUI.RectAbsolute(recta, GUIStyle.Current.BtnColor);
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
                    }
                }
                else
                {
                    GUI.RectAbsolute(recta, GUIStyle.Current.BtnColor);
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

        public static void DragRect(string contract, Vector4 rect, object dropcontent = null, bool draw = false, bool drawondrag = true)
        {
            var rectab = GUI.GetAbsoluteRect(rect);
            DragRectAbsolute(contract, rectab, dropcontent, draw, drawondrag);
        }

        public static void DragRectAbsolute(string contract, Vector4 rectab, object dropcontent = null, bool draw = false, bool drawondrag = true)
        {
            if (draw)
                GUI.BorderAbsolute(rectab, GUIStyle.Current.ColorActiveD);

            var dragrect = GUI.GetDragRect(rectab);
            var ds = dragrect.DragStage;
            if (ds.OnDrag(rectab))
            {
                GUI.SetFrameDragDrop();

                rectab = rectab.Move(GUI.Event.Pointer - ds.EnterPos);

                if (ds.Stage == GUIDragStateStage.Update)
                {

                    GUI.HoverDrop(contract, dropcontent);

                }
                else if (ds.Stage == GUIDragStateStage.Exit)
                {
                    GUI.EmmitDrop(contract, dropcontent);
                }

                if (drawondrag)
                {
                    GUI.SetDepthLayer(GUILayerType.Overlay);
                    GUI.RectAbsolute(rectab, GUIStyle.Current.ColorActiveD, true);
                    GUI.RestoreDepthLayer();
                }
            }
        }


        public static void DragRect<T>(Vector4 rect, T dropcontent, string contract = "", bool draw = false, bool drawondrag = true)
        {
            var ractab = GUI.GetAbsoluteRect(rect);
            DragRectAbsolute(ractab, dropcontent, contract, draw, drawondrag);
        }

        public static void DragRectAbsolute<T>(Vector4 rectab, T dropcontent, string contract = "", bool draw = false, bool drawondrag = true)
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

                if (ds.Stage == GUIDragStateStage.Update)
                {
                    GUI.HoverDrop(contractfull, dropcontent);
                }
                else if (ds.Stage == GUIDragStateStage.Exit)
                {
                    GUI.EmmitDrop(contractfull, dropcontent);
                }

                if (drawondrag)
                {
                    GUI.SetDepthLayer(GUILayerType.Overlay);
                    GUI.RectAbsolute(rectab, GUIStyle.Current.ColorActiveD, true);
                    GUI.RestoreDepthLayer();
                }
            }
        }
    }
}
