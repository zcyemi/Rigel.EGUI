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

        private Dictionary<GUILayerType, GUILayer> m_layers;

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

            m_layers = new Dictionary<GUILayerType, GUILayer>();
        }


        protected virtual void Init()
        {

        }


        public void Update()
        {
            m_graphicsBind.Update();

            //Sync data
            foreach(var pair in m_layers)
            {
                
            }
            
        }

        public void Destroy()
        {
            m_graphicsBind.Destroy();
            m_graphicsBind = null;
        }


        public void EmitGUIEvent(RigelGUIEvent e)
        {
            if (e.IsMouseActiveEvent())
            {
                CheckFocused(e);
            }

            foreach(var layer in m_layers)
            {
                layer.Value.Update(e);
            }

        }


        private void CheckFocused(RigelGUIEvent e)
        {

        }

        public void AddRegion(GUIRegion region,GUILayerType layertype)
        {
            if (!m_layers.ContainsKey(layertype)) m_layers.Add(layertype, new GUILayer(layertype));

            var layer = m_layers[layertype];
            layer.AddRegion(region);
        }
    }
}
