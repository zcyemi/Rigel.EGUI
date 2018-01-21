using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI
{

    public enum GUILayerType : int
    {
        Main = 0,
        Window = 100,
        Modal = 200,
        Overlay = 300,
    }


    public class GUILayer
    {
        private int m_order;
        private List<GUIRegion> m_regions;
        private bool m_focused = false;
        private GUIRegion m_focusedRegion = null;

        private IGUIBuffer m_bufferRect;
        private IGUIBuffer m_bufferRectDynamic;

        private IGUIBuffer m_bufferText;
        private IGUIBuffer m_bufferTextDynamic;

        public IGUIBuffer BufferRect
        {
            get { return m_bufferRect; }
        }
        public IGUIBuffer BufferRectDynamic {
            get
            {
                return m_bufferRectDynamic;
            }
        }
        public IGUIBuffer BufferText
        {
            get { return m_bufferText; }
        }
        public IGUIBuffer BufferTextDynamic
        {
            get { return m_bufferTextDynamic; }
        }

        public bool IsFocused { get { return m_focused; } }
        public GUIRegion FocusedRegion { get { return m_focusedRegion; } }


        private bool m_layerChanged = false;
        private bool m_syncAll = true;
        internal bool SyncAll
        {
            get { return m_syncAll; }
            set { m_syncAll = value; }
        }


        public GUILayer(GUILayerType type)
        {
            m_order = (int)type;
        }

        public GUILayer(int order)
        {
            m_order = order;
        }

        public void AddRegion(GUIRegion region)
        {
            if (m_regions == null) m_regions = new List<GUIRegion>();
            if (m_regions.Contains(region)) return;
            m_regions.Add(region);
        }

        public void Update(RigelGUIEvent e)
        {
            m_layerChanged = false;
            if (m_regions == null) return;
            foreach(var region in m_regions)
            {
                region.ProcessGUIEvent(e);
            }
        }

        public int GetBufferSize(GUIBufferType type)
        {
            return 0;
        }

        public IGUIBuffer GetBufferRect(GUIRegion region)
        {
            DevUtility.Diagnostics(() => { return m_regions.Contains(region); });

            if(m_focusedRegion != null && m_focusedRegion == region)
            {
                return m_bufferRectDynamic;
            }
            else
            {
                return m_bufferRect;
            }
        }

        public IGUIBuffer GetBufferText(GUIRegion region)
        {
            DevUtility.Diagnostics(() => { return m_regions.Contains(region); });
            if(m_focusedRegion != null && m_focusedRegion == region)
            {
                return m_bufferTextDynamic;
            }
            else
            {
                return m_bufferText;
            }
        }
    }
}
