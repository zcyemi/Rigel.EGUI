using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI
{

    public interface IGUIMenuItem
    {
        string Label { get; set; }
    }

    public class GUIMenuItem : IGUIMenuItem
    {
        public string Label { get; set; }
        public Action Function;

        public GUIMenuItem(string label,Action function)
        {
            this.Label = label;
            this.Function = function;
        }
    }

    public class GUIMenuList : IGUIMenuItem
    {
        public string Label { get; set; }
        private List<IGUIMenuItem> m_items = new List<IGUIMenuItem>();
        public List<IGUIMenuItem> Items { get { return m_items; } }

        public GUIMenuList(string label)
        {
            this.Label = label;
        }

        public GUIMenuList AddItem(string label,Action function = null)
        {
            m_items.Add(new GUIMenuItem(label, function));
            return this;
        }

        public GUIMenuList AddItem(GUIMenuList list)
        {
            m_items.Add(list);
            return this;
        }

        public IGUIMenuItem FindItem(string label)
        {
            for(int i = 0; i < m_items.Count; i++)
            {
                if (m_items[i].Label == label) return m_items[i];
            }
            return null;
        }

        public void RemoveItem(string label)
        {
            for (int i = 0; i < m_items.Count; i++)
            {
                if (m_items[i].Label == label)
                {
                    m_items.RemoveAt(i);
                    break;
                }
            }
        }

        public void RemoveItem(IGUIMenuItem item)
        {
            if (m_items.Contains(item))
            {
                m_items.Remove(item);
            }
        }


    }
}
