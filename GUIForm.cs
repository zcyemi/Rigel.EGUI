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


        protected virtual void Init()
        {

        }


        public void Update()
        {
            m_graphicsBind.Update();

            //Sync data
            foreach(var layer in m_layers)
            {
                m_graphicsBind.SyncLayerBuffer(layer);
            }
            
        }

        public void Destroy()
        {
            m_graphicsBind.Destroy();
            m_graphicsBind = null;
        }


        public void EmitGUIEvent(RigelGUIEvent e)
        {

            //if (e.IsMouseActiveEvent())
            //{
            //    foreach (var layer in m_layers)
            //    {
            //        Console.WriteLine($"{layer.Value.BufferRect.Count}-{layer.Value.BufferRectDynamic.Count}");
            //    }
            //}

            CheckFocused(e);

            foreach (var layer in m_layers)
            {
                layer.Update(e);
            }

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


        public void AddRegion(GUIRegion region,GUILayerType layertype)
        {
            var layer = GetLayer(layertype);

            if (layer == null)
            {
                layer = new GUILayer(this, layertype);
                m_layers.Add(layer);

                m_layers.Sort((a, b) => { return a.Order.CompareTo(b.Order); });
            }

            layer.AddRegion(region);
        }
    }
}
