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

        private bool m_syncAll = true;
        internal bool SyncAll
        {
            get { return m_syncAll; }
            set { m_syncAll = value; }
        }

        private GUIForm m_form;


        public GUIView m_rootView = null;
        public GUIView m_focusedView = null;


        private GUIDelayAction actionBeforeUpdate;

        public GUILayer(GUIForm form,GUILayerType type)
        {
            LayerType = type;
            m_order = (int)type;


            m_form = form;
            m_bufferRect = form.GraphicsBind.CreateBuffer(256);
            m_bufferRectDynamic = form.GraphicsBind.CreateBuffer(256);

            m_bufferText = form.GraphicsBind.CreateBuffer(256);
            m_bufferTextDynamic = form.GraphicsBind.CreateBuffer(256);


            m_rootView = new GUIView();
            m_rootView.Rect = Vector4.zero;
            m_rootView.Layer = this;

            actionBeforeUpdate = new GUIDelayAction();
        }


        public void SetDirty()
        {
            actionBeforeUpdate.Call(()=> m_syncAll = true);
        }

   

        public void RemoveFocus(RigelGUIEvent e)
        {
            if(m_focusedView != null)
            {
                m_focusedView.RemoveFocused();
                m_focusedView = null;
                m_syncAll = true;
            }
            
        }

        public bool CheckFocused(RigelGUIEvent e)
        {
            return _CheckFocused(e);
        }

        public void Update(RigelGUIEvent e)
        {
            GUI.StartGUILayer(this);
            _Update(e);
            GUI.EndGUILayer(this);

        }


        public bool _CheckFocused(RigelGUIEvent e)
        {
            GUIView lastFocus = null;

            if (m_focusedView != null)
            {
                var curfocus = m_focusedView;
                var focused = m_focusedView.CheckFocused(e);
                if (focused)
                {
                    if (curfocus == m_focusedView) return true;
                }
                lastFocus = m_focusedView;
                m_focusedView = null;
                m_syncAll = true;
            }

            if (!m_rootView.CheckFocused(e))
            {
                if(lastFocus!= null)
                {
                    m_focusedView = lastFocus;
                }
                return false;
            }

            m_focusedView.SetOrderFocused();
            m_rootView.SyncOrder(0);

            m_syncAll = true;
            return true;
        }

        public void _Update(RigelGUIEvent e)
        {
            actionBeforeUpdate.Invoke();

            if (m_syncAll)
            {
                
                if(m_focusedView == null)
                {
                    //No focused view

                    GUI.SetDrawBuffer(m_bufferRect, m_bufferText);
                    m_bufferRect.Clear();
                    m_bufferText.Clear();

                    m_rootView.InternalUpdate(e);

                    m_bufferRect.IsBufferChanged = true;
                    m_bufferText.IsBufferChanged = true;

                }
                else
                {
                    //has focused view

                    //dynamic set buffer
                    GUI.SetDrawBuffer(m_bufferRectDynamic, m_bufferTextDynamic);
                    BufferRectDynamic.Clear();
                    BufferTextDynamic.Clear();
                    m_focusedView.InternalUpdate(e,null,true);
                    BufferRectDynamic.IsBufferChanged = true;
                    BufferTextDynamic.IsBufferChanged = true;

                    //normal set buffer
                    GUI.SetDrawBuffer(m_bufferRect, m_bufferText);
                    m_bufferRect.Clear();
                    m_bufferText.Clear();
                    m_rootView.InternalUpdate(e, m_focusedView);
                    m_bufferRect.IsBufferChanged = true;
                    m_bufferText.IsBufferChanged = true;
                }
                m_syncAll = false;

            }
            else
            {
                //Update Dynamic

                if(m_focusedView != null)
                {
                    GUI.SetDrawBuffer(m_bufferRectDynamic, m_bufferTextDynamic);
                    BufferRectDynamic.Clear();
                    BufferTextDynamic.Clear();
                    m_focusedView.InternalUpdate(e,null,true);
                    BufferRectDynamic.IsBufferChanged = true;
                    BufferTextDynamic.IsBufferChanged = true;
                }
            }
        }


        public void Destroy()
        {
            m_bufferRect.Dispose();
            m_bufferRectDynamic.Dispose();
            m_bufferText.Dispose();
            m_bufferTextDynamic.Dispose();
        }

        public bool AddView(GUIView view)
        {
            bool result = m_rootView.AddSubView(view);
            if (result)
            {
                m_syncAll = true;
            }
            return result;
        }

        public bool RemoveView(GUIView view)
        {
            return m_rootView.RemoveSubView(view);
        }


    }
}
