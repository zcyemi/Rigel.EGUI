using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI
{
    public class GUIViewBufferInfo
    {

    }

    public class GUIView
    {

        public GUILayer Layer { get; internal set; }
        internal GUIView Parent { get; set; } = null;
        public GUIViewBufferInfo BufferInfo = null;

        public List<GUIView> m_childrens = null;

        public Vector4 Rect;
        public Vector4 ContentRect;

        public GUIContent Content { get; protected set; } = null;

        public int Order = 0;

        public bool IsFocused
        {
            get
            {
                if (Layer == null) return false;
                return Layer.m_focusedView == this;
            }
        }

        public bool HasChild
        {
            get { return m_childrens != null && m_childrens.Count != 0; }
        }


        public void RemoveFocused()
        {
            
        }

        public virtual void SetContent(GUIContent content)
        {
            Content = content;
            Content.View = this;
        }

        private bool m_debug = false;
        private string m_debugName = "";

        private Vector4 m_color = RigelColor.Random();

        public GUIView(string debugName = null)
        {
            if (!string.IsNullOrEmpty(debugName))
            {
                m_debug = true;
                m_debugName = debugName;
            }
        }

        public void SetDebugName(string debugName)
        {
            m_debugName = debugName;
            m_debug = true;
        }

        public void SetOrderFocused()
        {
            if(Parent != null)
            {
                Order = 10000;
                Parent.SetOrderFocused();
            }

        }

        public int SyncOrder(int baseOrder)
        {
            if (HasChild)
            {
                Order = baseOrder + 1;
                m_childrens.Sort((a, b) => { return a.Order.CompareTo(b.Order); });

                int orderMax = Order;

                for(var i=0;i< m_childrens.Count; i++)
                {
                    orderMax = m_childrens[i].SyncOrder(orderMax)+1;
                }
                return orderMax;
            }
            else
            {
                Order = baseOrder + 1;
                return Order;
            }
        }


        public bool CheckFocused(RigelGUIEvent e)
        {
            if (Layer == null) throw new Exception();

            if (HasChild)
            {
                for(var i= m_childrens.Count-1; i>=0; i--)
                {
                    //fix layer
                    if (m_childrens[i].Layer == null) m_childrens[i].Layer = Layer;

                    if (m_childrens[i].CheckFocused(e))
                    {
                        return true;
                    }
                }

                if (GUIUtility.RectContainsCheck(Rect, e.Pointer))
                {
                    Layer.m_focusedView = this;
                    return true;
                }
            }
            else
            {
                if (GUIUtility.RectContainsCheck(Rect, e.Pointer))
                {
                    Layer.m_focusedView = this;
                    return true;
                }
            }
            return false;
        }

        public virtual void Update(RigelGUIEvent e)
        {
            if(Content == null)
            {
                if (m_debug)
                {
                    m_color = RigelColor.Random();
                    GUI.RectAbsolute(Rect, m_color);
                }
            }
            else
            {
                Content.OnGUI(e);
            }

            
        }

        public virtual void OnViewStart()
        {
            ContentRect = Rect.Padding(1);
            GUI.BeginArea(ContentRect);
        }

        public virtual void OnViewEnd()
        {
            GUI.EndArea();
        }
        

        internal void InternalUpdate(RigelGUIEvent e,GUIView exclude = null,bool onlyself = false)
        {
            if (exclude != this)
            {
                GUI.StartGUIView(this);
                Update(e);
                GUI.EndGUIView(this);
            }

            if (onlyself) return;

            if(m_childrens != null)
            {
                for(var i = 0; i < m_childrens.Count; i++)
                {
                    m_childrens[i].InternalUpdate(e, exclude);
                }
            }
        }

        public bool AddSubView(GUIView view)
        {
            if (m_childrens == null) m_childrens = new List<GUIView>();

            if (m_childrens.Contains(view))
            {
                throw new Exception("View already added!");
            }

            if(view.Parent != null)
            {
                view.Parent.RemoveSubView(view);
            }

            m_childrens.Add(view);
            view.Parent = this;
            view.Layer = Layer;

            return true;
        }

        public bool RemoveSubView(GUIView view)
        {
            if (m_childrens == null) return false;

            if (!m_childrens.Contains(view))
            {
                return false;
            }

            m_childrens.Remove(view);
            view.Layer = null;
            view.Parent = null;
            return true;
        }

    }
}
