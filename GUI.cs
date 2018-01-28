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

        public static void RectAbsolute(Vector4 rect,Vector4 color)
        {
            BufRect.AddVertices(new Vector4(rect.x, rect.y, DepthValue, 1), color, Vector2.zero);
            BufRect.AddVertices(new Vector4(rect.x + rect.z, rect.y, DepthValue, 1), color, Vector2.zero);
            BufRect.AddVertices(new Vector4(rect.x + rect.z, rect.y + rect.w, DepthValue, 1), color, Vector2.zero);
            BufRect.AddVertices(new Vector4(rect.x, rect.y + rect.w, DepthValue, 1), color, Vector2.zero);

            DepthValue -= DepthStep;
        }

        public static void DebugFontTexture(Vector4 rect,Vector4 color)
        {

            BufText.AddVertices(new Vector4(rect.x, rect.y, DepthValue, 1), color, Vector2.zero);
            BufText.AddVertices(new Vector4(rect.x + rect.z, rect.y, DepthValue, 1), color, new Vector2(1,0));
            BufText.AddVertices(new Vector4(rect.x + rect.z, rect.y + rect.w, DepthValue, 1), color, new Vector2(1, 1));
            BufText.AddVertices(new Vector4(rect.x, rect.y + rect.w, DepthValue, 1), color, new Vector2(0, 1));

            DepthValue -= DepthStep;
        }

        public static int Char(Vector4 rect,Char c,Vector4 color,Vector2 pos,bool clip = true)
        {
            rect = rect.Move(CurArea.Rect);
            return CharAbsolute(rect, c, color, pos, clip);
        }

        public static int CharAbsolute(Vector4 recta,Char c,Vector4 color,Vector2 pos,bool clip = true)
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
                BufText.AddVertices(new Vector4(charrect.X, charrect.Y, DepthValue, 1), color, glyph.UV[0]);
                BufText.AddVertices(new Vector4(x2, charrect.Y, DepthValue, 1), color, glyph.UV[3]);
                BufText.AddVertices(new Vector4(x2, y2, DepthValue, 1), color, glyph.UV[2]);
                BufText.AddVertices(new Vector4(charrect.X, y2, DepthValue, 1), color, glyph.UV[1]);
            }
            else
            {
                BufText.AddVertices(new Vector4(charrect.X, charrect.Y, DepthValue, 1), color, glyph.UV[0]);
                BufText.AddVertices(new Vector4(x2, charrect.Y, DepthValue, 1), color, glyph.UV[3]);
                BufText.AddVertices(new Vector4(x2, y2, DepthValue, 1), color, glyph.UV[2]);
                BufText.AddVertices(new Vector4(charrect.X, y2, DepthValue, 1), color, glyph.UV[1]);
            }

            DepthValue -= DepthStep;

            return glyph.AdvancedX+1;
        }


        public static void Text(Vector4 rect,String text,Vector4 color,Vector2 pos,bool clip = true)
        {
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
        public static void TextAbsolute(Vector4 recta,String text,Vector4 color,Vector2 pos,bool clip = true)
        {

            Vector2 startpos = pos;
            if (pos.y > recta.w) return;
            if (pos.x > recta.z) return;

            bool yclip = true;
            if(pos.y > recta.y && pos.y + Font.FontPixelSize < recta.w)
            {
                yclip = false;
            }

            foreach(var c in text)
            {
                if(c< 33)
                {
                    startpos.x += 6;
                    continue;
                }

                var charWidth = Font.GetCharWidth(c);
                int charendpos = (int)startpos.x + charWidth;

                if(charendpos < 0)
                {
                    startpos.x = charendpos;
                    continue;
                }

                if (startpos.x > recta.z) return;

                if (clip)
                {
                    if (charendpos < recta.z && !yclip)
                    {
                        startpos.x += CharAbsolute(recta, c, color, startpos, false);
                    }
                    else
                    {
                        startpos.x += CharAbsolute(recta, c, color, startpos, true);
                    }
                }
                else
                {
                    startpos.x += CharAbsolute(recta, c, color, startpos, false);
                }
            }
        }


        public static bool Button(Vector4 rect,string label,params GUIOption[] option)
        {
            rect = rect.Move(CurArea.Rect);
            return ButtonAbsolute(rect, label, option);
        }

        public static bool ButtonAbsolute(Vector4 recta,string label,params GUIOption[] option)
        {
            recta = recta.Padding(1);

            GUIOptionAlign optAlign = null;
            if(option != null)
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
                if(GUIUtility.RectContainsCheck(recta, GUI.Event.Pointer))
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

            Vector2 offset = new Vector2(2,(recta.w - GUI.Font.FontPixelSize) / 2);
            if(optAlign == null || optAlign == GUIOption.AlignCenter)
            {
                offset.x = Mathf.FloorToInt((recta.z - GUI.Font.GetTextWidth(label)) / 2);
            }
            else if(optAlign == GUIOption.AlignRight)
            {
                offset.x = recta.z - GUI.Font.GetTextWidth(label) - 2;
            }

            GUI.TextAbsolute(recta, label, RigelColor.White, offset, true);

            return clicked;
        }
    }
}
