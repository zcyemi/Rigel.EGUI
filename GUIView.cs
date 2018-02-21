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

        public Vector4 Rect = new Vector4(0, 0, 400, 300);

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


        public bool CheckFocused(RigelGUIEvent e)
        {
            if (HasChild)
            {
                for(var i=0;i< m_childrens.Count; i++)
                {
                    if (m_childrens[i].CheckFocused(e))
                    {
                        return true;
                    }
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

        public void Update(RigelGUIEvent e)
        {
            GUI.RectAbsolute(Rect, RigelColor.Random());
        }

        internal void InternalUpdate(RigelGUIEvent e,GUIView exclude = null)
        {
            if (exclude == this) return;
            GUI.StartGUIRegion(this);
            Update(e);
            GUI.EndGUIRegion(this);

            if(m_childrens != null)
            {
                for(var i = 0; i < m_childrens.Count; i++)
                {
                    m_childrens[i].InternalUpdate(e, exclude);
                }
            }
        }

        public void AddSubView(GUIView view)
        {
            if (m_childrens == null) m_childrens = new List<GUIView>();

            if (m_childrens.Contains(view))
            {
                return;
            }

            if(view.Parent != null)
            {
                view.Parent.RemoveSubView(view);
            }

            m_childrens.Add(view);
            view.Parent = this;
            view.Layer = Layer;
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
