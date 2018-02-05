using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rigel;

namespace Rigel.GUI
{
    public class GUIForm
    {
        private List<GUILayer> m_layers;
        private GUILayer m_focusedLayer = null;
        public GUILayer FocusedLayer { get { return m_focusedLayer; } }

        public bool FastMode { get; set; } = false;
        private IGUIGraphicsBind m_graphicsBind;
        public IGUIGraphicsBind GraphicsBind
        {
            get
            {
                return m_graphicsBind;
            }
        }
        public GUIForm(IGUIGraphicsBind bind)
        {
            m_graphicsBind = bind;

            m_layers = new List<GUILayer>();
        }

        internal GUIFrame Frame = new GUIFrame();

        private Vector4 m_rect = Vector4.zero;
        public Vector4 Rect { get { return m_rect; } }

        private GUIDelayAction startFrameAction = new GUIDelayAction();
        private GUIDelayAction endFrameAction = new GUIDelayAction();


        protected virtual void Init()
        {

        }


        public void Update()
        {
            m_graphicsBind.Update();
            m_graphicsBind.UpdateGUIParams((int)m_rect.z, (int)m_rect.w);

            //Sync data
            foreach (var layer in m_layers)
            {
                m_graphicsBind.SyncLayerBuffer(layer);
            }
        }

        public void Destroy()
        {
            m_graphicsBind.Destroy();
            m_graphicsBind = null;

            foreach(var layer in m_layers)
            {
                layer.Destroy();
            }
            m_layers.Clear();
        }


        public void EmitGUIEvent(RigelGUIEvent e)
        {
            if (FastMode && e.EventType == RigelGUIEventType.MouseMove)
            {
                return;
            }

            m_rect.z = e.RenderWidth;
            m_rect.w = e.RenderHeight;

            //if (e.IsMouseActiveEvent())
            //{
            //    foreach (var layer in m_layers)
            //    {
            //        Console.WriteLine($"{layer.Value.BufferRect.Count}-{layer.Value.BufferRectDynamic.Count}");
            //    }
            //}

            CheckFocused(e);

            GUI.StartFrame(this,e);
            startFrameAction.Invoke();

            foreach (var layer in m_layers)
            {
                layer.Update(e);

            }

            endFrameAction.Invoke();
            GUI.EndFrame(this);

        }


        private void CheckFocused(RigelGUIEvent e)
        {
            if (!e.IsMouseActiveEvent()) return;

            //GUILayer lastFocusedLayer = null;
            //if(m_focusedLayer != null)
            //{
            //    if (!m_focusedLayer.CheckFocused(e))
            //    {
            //        lastFocusedLayer = m_focusedLayer;
            //        m_focusedLayer = null;
            //    }
            //}
            //if(m_focusedLayer == null)
            //{

            //}

            GUILayer lastFocusedLayer = m_focusedLayer;


            foreach (var layer in m_layers)
            {
                if (layer.CheckFocused(e))
                {
                    m_focusedLayer = layer;
                    break;
                }
                else
                {
                    if (lastFocusedLayer == layer)
                    {
                        lastFocusedLayer = null;
                    }
                }
            }

            if(lastFocusedLayer != null && lastFocusedLayer != m_focusedLayer)
            {
                lastFocusedLayer.RemoveFocus(e);
            }
        }

        public GUILayer GetLayer(GUILayerType type)
        {
            foreach(var layer in m_layers)
            {
                if (layer.LayerType == type) return layer;
            }
            return null;
        }


        public void AddRegion(IGUIView region,GUILayerType layertype)
        {
            startFrameAction.Call(() =>
            {
                var layer = GetLayer(layertype);
                if (layer == null)
                {
                    layer = new GUILayer(this, layertype);
                    m_layers.Add(layer);

                    m_layers.Sort((a, b) => { return a.Order.CompareTo(b.Order); });
                }
                layer.AddRegion(region);

            });
            
        }

        public void RemoveRegion(IGUIView region)
        {
            startFrameAction.Call(() =>
            {
                foreach (var layer in m_layers)
                {
                    if (layer.HasRegion(region))
                    {
                        layer.RemoveRegion(region);
                        return;
                    }
                }
            });
        }
    }
}
