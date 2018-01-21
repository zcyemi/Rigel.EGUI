using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI
{

    public enum GUILayerType : int
    {
        Main = 100,
        Window = 200,
        Modal = 300,
        Overlay = 400,
    }


    public class GUILayer
    {
        private int m_order;
        public int Order { get { return m_order; } }
        private List<GUIRegion> m_regions;
        private GUIRegion m_focusedRegion = null;
        private GUIRegion m_lastFocusedRegion = null;

        private IGUIBuffer m_bufferRect;
        private IGUIBuffer m_bufferRectDynamic;

        private IGUIBuffer m_bufferText = null;
        private IGUIBuffer m_bufferTextDynamic = null;

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


        public GUIRegion FocusedRegion { get { return m_focusedRegion; } }


        private List<GUIRegionBufferBlockInfo> BlockInfoRect = new List<GUIRegionBufferBlockInfo>();

        private bool m_syncAll = true;
        private GUIForm m_form;
        internal bool SyncAll
        {
            get { return m_syncAll; }
            set { m_syncAll = value; }
        }


        public GUILayer(GUIForm form,GUILayerType type)
        {
            m_order = (int)type;
            Init(form);
        }

        public GUILayer(GUIForm form,int order)
        {
            m_order = order;
            Init(form);
        }

        private void Init(GUIForm form)
        {
            m_form = form;
            m_bufferRect = form.GraphicsBind.CreateBuffer();
            m_bufferRectDynamic = form.GraphicsBind.CreateBuffer();
        }


        public void AddRegion(GUIRegion region)
        {
            if (m_regions == null) m_regions = new List<GUIRegion>();
            if (m_regions.Contains(region)) return;
            m_regions.Add(region);

            m_syncAll = true;
        }
        public bool CheckFocused(RigelGUIEvent e)
        {
            m_lastFocusedRegion = null;

            if(m_focusedRegion != null)
            {
                if (!m_focusedRegion.CheckFocused(e))
                {
                    m_lastFocusedRegion = m_focusedRegion;
                    m_focusedRegion.IsFocused = false;
                    m_focusedRegion = null;

                    Console.WriteLine("clear focus");
                }
            }

            if(m_focusedRegion == null)
            {
                foreach(var region in m_regions)
                {
                    if (region == m_lastFocusedRegion) continue;

                    if (region.CheckFocused(e))
                    {
                        Console.WriteLine("get focus");

                        m_syncAll = true;
                        region.IsFocused = true;
                        m_focusedRegion = region;
                        break;
                    }
                }
            }

            //Last Frame is focuseds
            if(m_focusedRegion == null && m_lastFocusedRegion != null)
            {
                m_syncAll = true;
            }

            m_regions.Sort((a, b) => { return a.Order.CompareTo(b.Order); });
            for(int i = 0; i < m_regions.Count; i++)
            {
                m_regions[i].Order = i;
            }
            if(m_focusedRegion != null)
            {
                m_focusedRegion.Order = 99;
            }

            return m_focusedRegion != null;
        }

        public void Update(RigelGUIEvent e)
        {
            if (m_regions == null) return;

            GUIDraw.StartGUILayer(this);

            if (m_syncAll)
            {
                m_bufferRect.Clear();
                m_bufferRectDynamic.Clear();

                foreach (var region in m_regions)
                {

                    region.ProcessGUIEvent(e);
                }

                m_bufferRectDynamic.IsBufferChanged = true;
                m_bufferRect.IsBufferChanged = true;

                m_lastFocusedRegion = null;
                m_syncAll = false;

                Console.WriteLine("sync all");
            }
            else
            {
                if(m_focusedRegion != null)
                {
                    m_bufferRectDynamic.Clear();
                    m_focusedRegion.ProcessGUIEvent(e);
                    m_bufferRectDynamic.IsBufferChanged = true;
                    //Console.WriteLine("sync dynamic");
                }
            }

            

            GUIDraw.EndGUILayer(this);

            m_bufferRectDynamic.IsBufferChanged = true;
        }

        public IGUIBuffer GetBufferRect(GUIRegion region)
        {
            DevUtility.Diagnostics(() => { return m_regions.Contains(region); });

            if(m_focusedRegion != null && m_focusedRegion == region)
            {
                //Console.WriteLine($"{region.DebugInfo} - Dynamic");
                return m_bufferRectDynamic;
            }
            else
            {
                //Console.WriteLine($"{region.DebugInfo} - Static");
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
