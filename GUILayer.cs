using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rigel.GUI
{

    public enum GUILayerType : int
    {
        Main = 400,
        Window = 300,
        Modal = 200,
        Overlay = 100,
    }


    public class GUILayer
    {
        private int m_order;
        public int Order { get { return m_order; } }
        private List<GUIView> m_views;
        public GUIView FocusedView { get { return m_focusedView; } }
        private GUIView m_focusedView = null;
        private GUIView m_lastFocusedView = null;

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


        public GUILayerType LayerType { protected set; get; }


        private List<GUIRegionBufferBlockInfo> BlockInfoRect = new List<GUIRegionBufferBlockInfo>();
        private List<GUIRegionBufferBlockInfo> BlockInfoText = new List<GUIRegionBufferBlockInfo>();

        private bool m_syncAll = true;
        private GUIForm m_form;
        internal bool SyncAll
        {
            get { return m_syncAll; }
            set { m_syncAll = value; }
        }


        public GUILayer(GUIForm form,GUILayerType type)
        {
            LayerType = type;
            m_order = (int)type;


            m_form = form;
            m_bufferRect = form.GraphicsBind.CreateBuffer(256);
            m_bufferRectDynamic = form.GraphicsBind.CreateBuffer(256);

            m_bufferText = form.GraphicsBind.CreateBuffer(256);
            m_bufferTextDynamic = form.GraphicsBind.CreateBuffer(256);
        }


        public void AddView(GUIView view)
        {
            if (m_views == null) m_views = new List<GUIView>();
            if (m_views.Contains(view)) return;
            m_views.Add(view);

            m_syncAll = true;
        }

        public void RemoveView(GUIView view)
        {
            if (m_views.Contains(view))
            {
                m_views.Remove(view);
                if (m_focusedView == view) m_focusedView = null;
                m_syncAll = true;
            }
            
        }

        public bool HasView(GUIView view)
        {
            foreach(var reg in m_views)
            {
                if (reg == view) return true;
            }
            return false;
        }

        public void RemoveFocus(RigelGUIEvent e)
        {
            m_lastFocusedView = null;
            if(m_focusedView != null)
            {
                m_focusedView.IsFocused = false;
                m_focusedView = null;
                m_syncAll = true;
            }
        }

        public bool CheckFocused(RigelGUIEvent e)
        {
            m_lastFocusedView = null;

            if(m_focusedView != null)
            {
                if (!m_focusedView.CheckFocused(e))
                {
                    m_lastFocusedView = m_focusedView;
                    m_focusedView.IsFocused = false;
                    m_focusedView = null;
                }
            }

            if(m_focusedView == null)
            {
                foreach(var view in m_views)
                {
                    if (view == m_lastFocusedView) continue;

                    if (view.CheckFocused(e))
                    {
                        m_syncAll = true;
                        view.IsFocused = true;
                        m_focusedView = view;
                        break;
                    }
                }
            }

            //Last Frame is focuseds
            if(m_focusedView == null && m_lastFocusedView != null)
            {
                m_syncAll = true;
            }

            m_views.Sort((a, b) => { return a.Order.CompareTo(b.Order); });
            for(int i = 0; i < m_views.Count; i++)
            {
                m_views[i].Order = i;
            }
            if(m_focusedView != null)
            {
                m_focusedView.Order = 99;
            }

            return m_focusedView != null;
        }

        public void Update(RigelGUIEvent e)
        {
            if (m_views == null) return;

            GUI.StartGUILayer(this);

            if (m_syncAll)
            {
                m_bufferRect.Clear();
                m_bufferRectDynamic.Clear();
                m_bufferText.Clear();
                m_bufferTextDynamic.Clear();

                foreach (var view in m_views)
                {
                    view.ProcessGUIEvent(e);
                }

                m_bufferRectDynamic.IsBufferChanged = true;
                m_bufferRect.IsBufferChanged = true;
                m_bufferTextDynamic.IsBufferChanged = true;
                m_bufferText.IsBufferChanged = true;

                m_lastFocusedView = null;
                m_syncAll = false;
            }
            else
            {
                if(m_focusedView != null)
                {
                    m_bufferRectDynamic.Clear();
                    m_bufferTextDynamic.Clear();
                    m_focusedView.ProcessGUIEvent(e);
                    m_bufferRectDynamic.IsBufferChanged = true;
                    m_bufferTextDynamic.IsBufferChanged = true;
                }
            }

            

            GUI.EndGUILayer(this);

        }

        public IGUIBuffer GetBufferRect(GUIView view)
        {
            DevUtility.Diagnostics(() => { return m_views.Contains(view); });

            if(m_focusedView != null && m_focusedView == view)
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

        public IGUIBuffer GetBufferText(GUIView view)
        {
            DevUtility.Diagnostics(() => { return m_views.Contains(view); });
            if(m_focusedView != null && m_focusedView == view)
            {
                return m_bufferTextDynamic;
            }
            else
            {
                return m_bufferText;
            }
        }

        public void Destroy()
        {
            m_bufferRect.Dispose();
            m_bufferRectDynamic.Dispose();
            m_bufferText.Dispose();
            m_bufferTextDynamic.Dispose();
        }
    }
}
