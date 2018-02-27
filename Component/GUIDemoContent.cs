using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI.Component
{
    public class GUIDemoContent : GUIContent
    {

        private List<Action<RigelGUIEvent>> m_sampleFunctions = new List<Action<RigelGUIEvent>>();
        private int m_sampleIndex;

        public GUIDemoContent()
        {
            {
                m_sampleFunctions.Add(SampleMenuList);
                m_sampleFunctions.Add(SampleLayout);
                m_sampleFunctions.Add(SampleText);
                m_sampleFunctions.Add(SampleButton);
                m_sampleFunctions.Add(SampleWindow);
                m_sampleFunctions.Add(SampleTabView);
                m_sampleFunctions.Add(SampleScrollView);
                m_sampleFunctions.Add(SampleDragDrop);
            }
            {
                //SampleMenuList
                m_sampleMenuList.AddItem("Item1", null)
                    .AddItem("Item2", null).
                    AddItem(
                        new GUIMenuList("submeun")
                        .AddItem("subitem1", null)
                        .AddItem(new GUIMenuList("level3")
                            .AddItem("l3i1")
                            .AddItem("l3i2")
                        )
                        .AddItem("subitem2", null)
                    )
                    .AddItem(
                        new GUIMenuList("test")
                        .AddItem("test1")
                        .AddItem("test2")
                    );
            }
        }

        private GUIMenuList m_sampleMenuList = new GUIMenuList("Menu");
        private void SampleMenuList(RigelGUIEvent e)
        {
            GUILayout.Indent(250);
            GUILayout.DrawMenu(m_sampleMenuList,GUIOption.Width(150));
            GUILayout.Button("Button on nextline");
        }
        private void SampleLayout(RigelGUIEvent e)
        {
            //Absolute and relative
            {
                GUI.Rect(new Vector4(0, 0, 20, 20), RigelColor.Red);
                Vector4 rectab = new Vector4(View.Rect.x + 21, View.Rect.y + 25, 20, 20);
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
                var regionrect = View.Rect;
                GUI.BeginArea(new Vector4(regionrect.z / 2, 0, regionrect.z / 2, regionrect.w - 25));
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

                    GUILayout.Space(20);

                    GUILayout.Label("Current Pointer " + e.Pointer);

                    GUI.Rect(new Vector4(0, 0, 5, 5), RigelColor.Red);
                }
                GUI.EndArea();
            }

    }
        private void SampleText(RigelGUIEvent e)
        {
            //Draw Text
            GUI.Char(new Vector4(0, 0, 20, 20), 'R', RigelColor.Red, Vector2.Zero, false);
            GUI.Text(new Vector4(0, 20, 100, 20), "Hello World", RigelColor.White, Vector2.zero, false);

            GUILayout.Space(40);
            //Layout text
            GUILayout.Label("This is a label.", RigelColor.White);

            GUILayout.BeginHorizontal();
            {
                GUILayout.Button("TestBtn", GUIOption.Grid(0.5f));
                GUILayout.Label("HorizontalLabel", RigelColor.Green);
                GUILayout.Button("TestBtn", GUIOption.Width(30));
            }
            GUILayout.EndHorizontal();

            GUILayout.Rect(new Vector2(30, 10), RigelColor.Green);
            GUILayout.Label(">> Label <<", RigelColor.White);

            
        }
        private void SampleButton(RigelGUIEvent e)
        {
            //Button Align
            if(GUI.Button(new Vector4(0, 0, 100, 23), "BtnC"))
            {
                Console.WriteLine("GUI.Button Center Click");
            }
            if (GUI.Button(new Vector4(100, 0, 100, 38), "BtnL",GUIOption.AlignLeft))
            {
                Console.WriteLine("GUI.Button Left Click");
            }
            if (GUI.Button(new Vector4(200, 0, 100, 43), "BtnR", GUIOption.AlignRight))
            {
                Console.WriteLine("GUI.Button Right Click");
            }

            GUILayout.Space(45);

            GUILayout.BeginHorizontal();
            {
                GUILayout.Button("None", GUIOption.Grid(0.25f));
                GUILayout.Button("Left", GUIOption.Grid(0.25f), GUIOption.AlignLeft);
                GUILayout.Button("Center", GUIOption.Grid(0.25f), GUIOption.AlignCenter);
                GUILayout.Button("Right", GUIOption.Grid(0.25f), GUIOption.AlignRight);
            }
            GUILayout.EndHorizontal();
        }
        private List<string> m_sampleTabViewList = new List<string>() { "Tab1", "Tab2", "Tab3" };
        private int m_sampleTabViewIndex = 0;
        private void SampleTabView(RigelGUIEvent e)
        {
            m_sampleTabViewIndex = GUILayout.TabView(m_sampleTabViewIndex, m_sampleTabViewList, (i) =>
            {
                GUILayout.Button("Btn in tabview : " + i);
            },GUIOption.Height(100));

            m_sampleTabViewIndex = GUILayout.TabViewVertical(m_sampleTabViewIndex, m_sampleTabViewList, (i) =>
            {
                GUILayout.Label("label in vertical tabview : " + i,Vector4.one);
            }, 50);
        }
        private Vector2 m_sampleScrollPos = Vector2.zero;
        private void SampleScrollView(RigelGUIEvent e)
        {
            GUILayout.BeginScrollView(m_sampleScrollPos, GUIScrollType.All);
            {
                GUILayout.Label("Clip:" + GUI.CurArea.Clip);
                GUILayout.Button("Button", GUIOption.Width(100));
                GUILayout.Indent(300);
                for (int i = 0; i < 20; i++)
                {
                    GUILayout.Indent(5);
                    GUILayout.Label("Text" + i, Vector4.one);

                }
            }
            m_sampleScrollPos = GUILayout.EndScrollView();

        }
        //private IGUIContent m_sampleWindow = null;
        private void SampleWindow(RigelGUIEvent e)
        {
            //bool hasWindow = m_sampleWindow != null;
            //if (GUILayout.Button(hasWindow ? "Remove Window":"New window"))
            //{
            //    if (hasWindow)
            //    {
            //        GUI.Form.RemoveRegion(m_sampleWindow);
            //        m_sampleWindow = null;
            //        hasWindow = false;
            //    }
            //    else
            //    {
            //        m_sampleWindow = new IGUIContent(GUI.Form, 0);
            //        m_sampleWindow.Caption = "Dynamic Window";
            //        GUI.Form.AddRegion(m_sampleWindow, GUILayerType.Window);
            //    }
            //}

            //if (!hasWindow) return;

            //if (GUILayout.Button("Window Move : " + m_sampleWindow.Moveable))
            //{
            //    m_sampleWindow.Moveable = !m_sampleWindow.Moveable;
            //}

        }


        private string DropInfo1 = "None";
        private string DropInfo2 = "None";
        private void SampleDragDrop(RigelGUIEvent e)
        {
            //GUILayout.Rect(new Vector2(100, 40), GUIStyle.Current.ColorActiveD);


            GUILayout.DragRect("drag", new Vector2( 50, 20),"AAA");
            GUILayout.DragRect("drag", new Vector2( 50, 20), "BBB");


            GUI.DrawDebugInfo();

            //GUILayout.DropRect(new Vector2(100, 50),
            //    (o)=> {
            //        DropInfo1 = o.ToString();
            //    },
            //    (r)=> {
            //        GUI.RectAbsolute(r, RigelColor.Blue);
            //});
            //GUILayout.Label(DropInfo1);


            //GUILayout.DropRect(new Vector2(100, 50),
            //    (o) => {
            //        DropInfo2 = o.ToString();
            //    },
            //    (r) => {
            //        GUI.RectAbsolute(r, RigelColor.Green);
            //    });
            //GUILayout.Label(DropInfo2);
        }


        public override void OnGUI(RigelGUIEvent e)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("<", GUIOption.Grid(0.1f)))
            {
                m_sampleIndex += m_sampleFunctions.Count - 1;
                m_sampleIndex = m_sampleIndex % m_sampleFunctions.Count;
            }
            GUILayout.Button(m_sampleFunctions[m_sampleIndex].Method.Name, GUIOption.Grid(0.8f));
            if (GUILayout.Button(">", GUIOption.Grid(0.1f)))
            {
                m_sampleIndex++;
                m_sampleIndex = m_sampleIndex % m_sampleFunctions.Count;
            }
            GUILayout.EndHorizontal();

            GUI.BeginArea(new Vector4(GUI.CurLayout.Offset, GUI.CurLayout.RemainSize), true);
            m_sampleFunctions[m_sampleIndex](e);
            GUI.EndArea();
        }
    }
}
