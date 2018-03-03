using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI.Component
{
    public class GUIContentDocker
    {
        private List<GUIContent> m_contents = new List<GUIContent>();

        private GUIContent m_selectedContent = null;


        internal void Draw(Vector4 rectab)
        {
            GUILayout.Label(rectab.ToString());
            GUI.BeginAreaAbsolute(rectab);

            var headerRect = new Vector4(0, 0, rectab.z, 23);

            GUI.Rect(headerRect, GUIStyle.Current.ColorBackgroundL2);

            //Drop rect
            var dropHover = GUI.DropRect(headerRect, (GUIContent content, object context) =>
            {

                if (content != null)
                {
                    var ctxdocker = context as GUIContentDocker;
                    if (ctxdocker != null)
                    {
                        Console.WriteLine("drop");

                        if (ctxdocker == this)
                        {
                            //self reorder
                            var tarindex = Mathf.FloorToInt((GUI.Event.Pointer.x - rectab.x) / 100);
                            tarindex = Mathf.Clamp(tarindex, 0, m_contents.Count - 1);
                            var srcindex = m_contents.IndexOf(content);
                            if(srcindex != tarindex)
                            {
                                var temp = m_contents[tarindex];
                                m_contents[tarindex] = content;
                                m_contents[srcindex] = temp;
                            }
                        }
                        else
                        {
                            if (ctxdocker.RemoveContent(content))
                            {
                                var tarindex = Mathf.FloorToInt((GUI.Event.Pointer.x - rectab.x) / 100);
                                tarindex = Mathf.Clamp(tarindex, 0, m_contents.Count);
                                AddContent(content,tarindex);
                            }
                        }
                    }
                }
            }, "ContentDocker");


            if (m_contents.Count != 0)
            {
                GUILayout.BeginHorizontal();
                {
                    var itemrect = new Vector4(1, 1, 100, 23);
                    var dragIndex = -1;
                    for (var i = 0; i < m_contents.Count; i++)
                    {
                        var onDrag = GUI.DragRect(itemrect, m_contents[i], "ContentDocker", this);
                        if (onDrag)
                        {
                            dragIndex = i;
                            break;
                        }
                        itemrect.x += 100;
                    }

                    var tarindex = -1;
                    if (dropHover)
                    {
                        tarindex = Mathf.FloorToInt((GUI.Event.Pointer.x - rectab.x) / 100);

                        if (dragIndex > -1)
                        {
                            tarindex = Mathf.Clamp(tarindex, 0, m_contents.Count - 1);
                            //self drag
                            for (var i = 0; i < m_contents.Count; i++)
                            {
                                if (i == tarindex || i == dragIndex)
                                {
                                    var di = (i == tarindex ? dragIndex : tarindex);
                                    GUILayout.Button(m_contents[di].ContentName, (i == dragIndex ? GUIStyle.Current.ColorActive : GUIStyle.Current.ColorActiveD), GUIOption.Width(100));
                                }
                                else
                                {
                                    GUILayout.Button(m_contents[i].ContentName, GUIOption.Width(100));
                                }
                            }
                        }
                        else
                        {
                            tarindex = Mathf.Clamp(tarindex, 0, m_contents.Count);
                            //other drag

                            bool drawtar = false;
                            for (var i = 0; i < m_contents.Count; i++)
                            {
                                if (i == tarindex)
                                {
                                    drawtar = true;
                                    GUILayout.Button(" ", GUIOption.Width(100));
                                }
                                GUILayout.Button(m_contents[i].ContentName, GUIOption.Width(100));
                            }
                            if (!drawtar)
                            {
                                GUILayout.Button(" ", GUIOption.Width(100));
                            }
                        }
                    }
                    else
                    {
                        for (var i = 0; i < m_contents.Count; i++)
                        {
                            var c = m_contents[i];
                            if(c == m_selectedContent)
                            {
                                GUILayout.Button(c.ContentName,GUIStyle.Current.ColorActiveD, GUIOption.Width(100));

                            }
                            else
                            {
                                if(GUILayout.Button(c.ContentName, GUIOption.Width(100)))
                                {
                                    m_selectedContent = c;
                                }
                            }
                        }

                        
                    }
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                if (dropHover)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Button(" ", GUIOption.Width(100));
                    GUILayout.EndHorizontal();
                }
            }

            GUI.EndArea();
        }

        public bool AddContent(GUIContent content,int index)
        {
            if (m_contents.Contains(content)) return false;
            m_contents.Insert(index,content);
            m_selectedContent = content;
            return true;
        }

        public bool AddContent(GUIContent content)
        {
            if (m_contents.Contains(content)) return false;
            m_contents.Add(content);
            m_selectedContent = content;
            return true;
        }

        public bool RemoveContent(GUIContent content)
        {
            if (m_contents.Contains(content))
            {
                var index = m_contents.IndexOf(content);
                m_contents.Remove(content);
                var count = m_contents.Count;
                if (count == 0)
                {
                    m_selectedContent = null;
                    return true;
                }
                if (m_selectedContent == content)
                {
                    if(index >= count)
                    {
                        index = count - 1;
                    }
                    m_selectedContent = m_contents[index];
                }
                
                return true;
            }
            return false;
        }



    }
}
