using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rigel;

namespace Rigel.GUI
{
    public interface IGUIBuffer
    {
        void Clear();

        void AddVertices(Vector4 vert, Vector4 color, Vector2 uv);

        void RemoveRange(int startpos, int count);

        int Count { get; }
        int ItemByte { get; }
        int SizeInByte { get; }

        float VerticesZ(int index);

        Array GetData();

        void CopyTo(Array ary);
        void CopyTo(int index, Array ary, int arrayIndex, int count);

        bool IsBufferChanged { get; set; }
    }
}
