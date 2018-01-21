using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rigel;
using Rigel.Graphics;

namespace Rigel.GUI
{
    public interface IGUIGraphicsBind
    {
        void SetDynamicBufferTexture(object vertexdata, int length);

        IGUIBuffer CreateBuffer();

        bool NeedRebuildCommandList { get; set; }

        //void SyncDrawTarget(GUIDrawStage stage, GUIDrawTarget drawtarget);
        void UpdateGUIParams(int width, int height);

        void SyncLayerBuffer(GUILayer layer);

        void Update();

        void Destroy();
    }
}
