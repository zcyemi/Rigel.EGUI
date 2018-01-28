using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI.Component
{
    public class GUIDemoWindow : GUIWindow
    {
        public GUIDemoWindow(GUIForm form, int order = 0) : base(form, order)
        {
        }


        private void SampleLayout(RigelGUIEvent e)
        {
            //Absolute and relative
            {
                GUI.Rect(new Vector4(0, 0, 20, 20), RigelColor.Red);
                Vector4 rectab = new Vector4(m_rect.x + 21, m_rect.y + 25, 20, 20);
                GUI.RectAbsolute(rectab, RigelColor.Red);
            }

            //Nested area
            {
                GUI.BeginArea(new Vector4(0, 25, 40, 40));
                {
                    GUI.Rect(new Vector4(0, 20, 20, 20), RigelColor.Blue);
                    GUI.Rect(new Vector4(20, 0, 20, 20), RigelColor.Green);
                }
                GUI.EndArea();
            }

            //Layout
            {
                GUI.BeginArea(new Vector4(m_rect.z / 2, 0, m_rect.z / 2, m_rect.w - 25));
                {
                    GUILayout.Button("Button1");
                    GUILayout.Button("Button2");

                    GUILayout.BeginHorizontal();
                    {
                        //Option Grid
                        GUILayout.Button("Btn3", GUIOption.Grid(0.5f));
                        //Indent
                        GUILayout.Indent(10);
                        //Line extrude
                        GUILayout.Button("Btnh", GUIOption.Width(20), GUIOption.Height(50));
                        //Option Width
                        GUILayout.Button("Btn4", GUIOption.Width(10));
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.Button("Btn5", GUIOption.Width(20));

                    GUILayout.BeginHorizontal();
                    {
                        //OptionGrid base on LayoutInfo
                        GUILayout.Button("H1", GUIOption.Grid(0.5f));
                        GUILayout.Button("H2", GUIOption.Grid(0.5f));
                    }
                    GUILayout.EndHorizontal();


                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Button("H1", GUIOption.Width(30));
                        GUILayout.BeginVertical();
                        {
                            GUILayout.Button("Test", GUIOption.Width(15));

                            //GUILayout.Indent(10);
                            GUILayout.Button("Text", GUIOption.Width(60));

                        }
                        GUILayout.EndVertical();
                        GUILayout.Button("H3", GUIOption.Width(30));
                    }
                    GUILayout.EndHorizontal();

                    GUI.Rect(new Vector4(0, 0, 5, 5), RigelColor.Red);
                }
                GUI.EndArea();
            }

    }

        private void SampleText(RigelGUIEvent e)
        {
            //Draw Text
            GUI.Char(new Vector4(0, 0, 20, 20), 'R', RigelColor.Green, Vector2.Zero, false);
            GUI.Text(new Vector4(0, 20, 100, 20), "Hello World", RigelColor.Blue, Vector2.zero, false);


        }

        protected override void OnWindowGUI(RigelGUIEvent e)
        {
            //SampleLayout(e);
            SampleText(e);

        }
            
    }
}
