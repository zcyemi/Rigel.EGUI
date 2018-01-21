﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using Rigel.Graphics;
using Rigel.Graphics.Collections;

namespace Rigel.GUI
{


    [StructLayout(LayoutKind.Explicit)]
    public struct RigelEGUIVertex
    {
        [FieldOffset(0)]
        public Rigel.Vector4 Position;
        [FieldOffset(16)]
        public Rigel.Vector4 Color;
        [FieldOffset(32)]
        public Rigel.Vector2 UV;
    }

    public partial class GUIGraphicsBindDefault : IGUIGraphicsBind
    {
        public bool NeedRebuildCommandList { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


        private static readonly int VERT_SIZE = Utility.SizeOf<RigelEGUIVertex>();

        private IGraphicsAggregation m_graphics;

        //Graphics Objects
        private IInputLayout m_inputLayout;
        private IShader m_shader_vs_rect = null;
        private IShader m_shader_ps_rect = null;
        private IShader m_shader_vs_text = null;
        private IShader m_shader_ps_text = null;

        private GUIGraphicsIndicesBuffer m_indicesBuffer;

        private GraphicsScalingBuffer m_bufferRect;
        private GraphicsScalingBuffer m_bufferText;
        private IGraphicsBuffer m_constBuffer;

        private PipelineState m_pstateRect;
        private PipelineState m_pstateText;

        private Matrix4x4 m_guiMatrix;
        private int m_currentWidth;
        private int m_currentHeight;

        private Dictionary<GUILayer, GraphicsBuffer> m_layerBuffer_rect_dynamic;
        private Dictionary<GUILayer, IGraphicsBuffer> m_layerBuffer_rect;

        public GUIGraphicsBindDefault(IGraphicsAggregation aggregation)
        {
            m_graphics = aggregation;
            m_graphics.EventDrawInstant += DrawInstant;

            //Init Graphics Objects

            m_layerBuffer_rect = new Dictionary<GUILayer, IGraphicsBuffer>();
            m_layerBuffer_rect_dynamic = new Dictionary<GUILayer, GraphicsBuffer>();


            //Buffer
            {
                var vbufferDesc = new GraphicsBufferDesc
                {
                    CPUAccessFlags = GraphicsBufferCPUAccessFlag.None,
                    Usage = GraphicsResourceUsage.Default,
                    BindFlags = GraphicsBindFlag.VertexBuffer,
                    SizeInByte = Utility.SizeOf<RigelEGUIVertex>() * 256,
                    StructureByteStride = 0
                };

                m_bufferRect = new GraphicsScalingBuffer(m_graphics.Device, vbufferDesc, 2);
                m_bufferText = new GraphicsScalingBuffer(m_graphics.Device, vbufferDesc, 2);

                //const buffer

                var cbufferDesc = new GraphicsBufferDesc
                {
                    SizeInByte = Utility.SizeOf<Matrix4x4>(),
                    BindFlags = GraphicsBindFlag.ConstantBuffer,
                    StructureByteStride = 0,
                    Usage = GraphicsResourceUsage.Default
                };


                m_constBuffer = m_graphics.Device.CreateBuffer(cbufferDesc);

                m_guiMatrix = Matrix4x4.OrthoOffCenterLH(0, 800, 600, 0, 0, 10.0f);
                m_graphics.Context.UpdateSubReources(m_constBuffer, m_guiMatrix);

            }

            //Indices buffer
            {
                m_indicesBuffer = new GUIGraphicsIndicesBuffer(m_graphics.Device, 256);
            }



            //Shader and InputLayout
            {
                var shaderCompiler = m_graphics.Factory.GetShaderCompiler();

                var byteCodeVS_Rect = shaderCompiler.CompileFromText(ShaderSourceRect, "vs_4_0", "VS");
                var byteCodePS_Rect = shaderCompiler.CompileFromText(ShaderSourceRect, "ps_4_0", "PS");
                m_shader_vs_rect = m_graphics.Device.CreateShader(byteCodeVS_Rect, GraphicsShaderType.VertexShader);
                m_shader_ps_rect = m_graphics.Device.CreateShader(byteCodePS_Rect, GraphicsShaderType.PixelShader);

                var byteCodeVS_Text = shaderCompiler.CompileFromText(ShaderSourceText, "vs_4_0", "VS");
                var byteCodePS_Text = shaderCompiler.CompileFromText(ShaderSourceText, "ps_4_0", "PS");
                m_shader_vs_text = m_graphics.Device.CreateShader(byteCodeVS_Text, GraphicsShaderType.VertexShader);
                m_shader_ps_text = m_graphics.Device.CreateShader(byteCodePS_Text, GraphicsShaderType.PixelShader);

                m_inputLayout = m_graphics.Device.CreateInputLayout(byteCodeVS_Rect, new LayoutElement[] {
                    new LayoutElement("POSITION",0,GraphicsFormat.R32G32B32A32_Float,0,0),
                    new LayoutElement("COLOR",0,GraphicsFormat.R32G32B32A32_Float,0,16),
                    new LayoutElement("TEXCOORD",0,GraphicsFormat.R32G32_Float,0,32)
                });
            }

            {
                m_pstateRect = new PipelineState();
                m_pstateRect.InputAssembler.Layout = m_inputLayout;
                m_pstateRect.Rasterizer.Viewport = new RasterViewPort(0, 0, 800f, 600f, 0.0f, 1.0f);
                m_pstateRect.OutputMerger.SetTargets(m_graphics.DepthView, m_graphics.RenderView);
                m_pstateRect.Rasterizer.CullMode = GraphicsCullMode.Back;
                m_pstateRect.Rasterizer.EnableDepthClip = false;
                m_pstateRect.Rasterizer.FillMode = GraphicsFillMode.Solid;
                m_pstateRect.InputAssembler.PrimitiveTopology = GraphicsPrimitiveTopology.TriangleList;



                m_pstateText = m_pstateRect.Clone();


                m_pstateRect.InputAssembler.VertexBufferBind = new BufferView(m_bufferRect.Buffer, Utility.SizeOf<RigelEGUIVertex>(), 0);
                m_pstateRect.InputAssembler.IndexBufferBind = new BfferViewIndex(m_indicesBuffer.Buffer, GraphicsFormat.R32_UInt, 0);
                m_pstateRect.Pipeline.VertexShader = m_shader_vs_rect;
                m_pstateRect.Pipeline.PixelShader = m_shader_ps_rect;
                m_pstateRect.Pipeline.AddConstBuffer(GraphicsShaderType.VertexShader, 0, m_constBuffer);

                m_pstateText.InputAssembler.VertexBufferBind = new BufferView(m_bufferText.Buffer, Utility.SizeOf<RigelEGUIVertex>(), 0);
                m_pstateText.Pipeline.VertexShader = m_shader_vs_text;
                m_pstateText.Pipeline.PixelShader = m_shader_ps_text;
                m_pstateText.Pipeline.AddSampler(GraphicsShaderType.PixelShader, 0, m_graphics.Device.DefaultSampler);


            }

        }

        public void Destroy()
        {
            if(m_layerBuffer_rect != null)
            {
                foreach(var pair in m_layerBuffer_rect)
                {
                    if (pair.Value != null) pair.Value.Dispose();
                }
                m_layerBuffer_rect.Clear();
                m_layerBuffer_rect = null;
            }
            if(m_layerBuffer_rect_dynamic != null)
            {
                foreach(var pair in m_layerBuffer_rect_dynamic)
                {
                    if (pair.Value != null) pair.Value.Dispose();
                }
                m_layerBuffer_rect_dynamic.Clear();
                m_layerBuffer_rect_dynamic = null;
            }

            GraphicsUtility.Release(ref m_bufferRect);
            GraphicsUtility.Release(ref m_pstateText);

            GraphicsUtility.Release(ref m_indicesBuffer);

            GraphicsUtility.Release(ref m_constBuffer);

            GraphicsUtility.Release(ref m_shader_ps_rect);
            GraphicsUtility.Release(ref m_shader_ps_text);
            GraphicsUtility.Release(ref m_shader_vs_rect);
            GraphicsUtility.Release(ref m_shader_vs_text);
            GraphicsUtility.Release(ref m_inputLayout);


            m_graphics.EventDrawInstant -= DrawInstant;
            m_graphics = null;
        }

        public IGUIBuffer CreateBuffer()
        {
            return new GUIBufferVertices();
        }

        private static DrawAttribute drawAttr = new DrawAttribute(6, 0);
        private void DrawInstant(IDeviceContext context)
        {

            //context.ClearRenderTarget(m_graphics.RenderView, RigelColor.Random());
            context.SetPipelineState(m_pstateRect);
            //context.DrawIndexed(6, 0, 0);


            foreach (var pair in m_layerBuffer_rect_dynamic)
            {
                context.SetVertexBuffer(pair.Value.Buffer, VERT_SIZE, 0);
                context.DrawIndexed(pair.Key.BufferRectDynamic.Count / 4*6, 0, 0);
            }

            foreach (var pair in m_layerBuffer_rect)
            {
                context.SetVertexBuffer(pair.Value, VERT_SIZE, 0);
                context.DrawIndexed(pair.Key.BufferRect.Count /4*6, 0, 0);
            }


        }

        [TEMP]
        private static bool BufferUpdated = false;

        public void Update()
        {
            //if (!BufferUpdated)
            //{
            //    var vert1 = new RigelEGUIVertex()
            //    {
            //        Position = new Vector4(0, 0, 0.5f, 1.0f),
            //        Color = RigelColor.Red,
            //        UV = Vector2.zero,
            //    };

            //    var vert2 = vert1;
            //    var vert3 = vert2;
            //    var vert4 = vert3;

                
            //    vert2.Position = new Vector4(100, 0, 0.5f, 1.0f);
            //    vert3.Position = new Vector4(100, 25, 0.5f, 1.0f);
            //    vert4.Position = new Vector4(0, 25, 0.5f, 1.0f);

            //    vert3.Color = RigelColor.Blue;
            //    vert2.Color = RigelColor.Blue;

            //    m_bufferRect.UpdateData<RigelEGUIVertex>(m_graphics.Device, new RigelEGUIVertex[] {
            //        vert1,vert2,vert3,vert4
            //    });
            //    BufferUpdated = true;

            //    Console.WriteLine("Buffer updated");
            //}
        }

        public void SetDynamicBufferTexture(object vertexdata, int length)
        {
        }

        public void UpdateGUIParams(int width, int height)
        {
            if (m_currentWidth == width && m_currentHeight == height)
            {
                return;
            }
            m_currentWidth = width;
            m_currentHeight = height;

            m_guiMatrix = Matrix4x4.OrthoOffCenterLH(0, width, height, 0, 0, 10.0f);
        }

        public void SyncLayerBuffer(GUILayer layer)
        {

            var bufferdesc = new GraphicsBufferDesc();
            bufferdesc.BindFlags = GraphicsBindFlag.VertexBuffer;
            bufferdesc.CPUAccessFlags = GraphicsBufferCPUAccessFlag.None;
            bufferdesc.StructureByteStride = 0;
            bufferdesc.Usage = GraphicsResourceUsage.Default;

            //rect bufffer

            IGraphicsBuffer rectBuffer = null;

            if (!m_layerBuffer_rect.ContainsKey(layer))
            {
                int sizeinbyte = layer.BufferRect.SizeInByte;
                if(sizeinbyte != 0)
                {
                    bufferdesc.SizeInByte = sizeinbyte;

                    rectBuffer = m_graphics.Device.CreateBuffer(bufferdesc, layer.BufferRect.GetData());
                    m_layerBuffer_rect.Add(layer, rectBuffer);
                }
            }
            else
            {
                rectBuffer = m_layerBuffer_rect[layer];
            }
            if(rectBuffer != null && layer.BufferRect.IsBufferChanged)
            {
                //Sync buffer Data
                m_graphics.Context.UpdateSubReources(rectBuffer, layer.BufferRect.GetData());
                layer.BufferRect.IsBufferChanged = false;
            }


            //rect bufffer dynamic
            GraphicsBuffer rectBufferDynamic = null;
            if (!m_layerBuffer_rect_dynamic.ContainsKey(layer))
            {
                int sizeinbyte = layer.BufferRectDynamic.SizeInByte;

                bufferdesc.SizeInByte = sizeinbyte;
                if (sizeinbyte != 0)
                {
                    rectBufferDynamic = new GraphicsBuffer(m_graphics.Device, bufferdesc);
                    rectBufferDynamic.UpdateData(m_graphics.Device, layer.BufferRectDynamic.GetData(), sizeinbyte);
                    m_layerBuffer_rect_dynamic.Add(layer, rectBufferDynamic);
                }
            }
            else
            {
                rectBufferDynamic = m_layerBuffer_rect_dynamic[layer];
            }
            if(rectBufferDynamic != null && layer.BufferRectDynamic.IsBufferChanged)
            {

                rectBufferDynamic.UpdateData(m_graphics.Device, layer.BufferRectDynamic.GetData(), layer.BufferRectDynamic.SizeInByte);
                layer.BufferRectDynamic.IsBufferChanged = false;
            }

        }
    }

    public class GUIBufferVertices : GUIBufferList<RigelEGUIVertex>, IGUIBuffer
    {
        public int Count
        {
            get { return m_data.Count; }
        }

        private static readonly int s_itemByte = Utility.SizeOf<RigelEGUIVertex>();
        public int ItemByte { get { return s_itemByte; } }

        public int SizeInByte { get { return Count * s_itemByte; } }

        public bool IsBufferChanged
        {
            get; set;
        }
        public void AddVertices(Vector4 vert, Vector4 color, Vector2 uv)
        {
            m_data.Add(new RigelEGUIVertex()
            {
                Position = vert,
                Color = color,
                UV = uv
            });


        }

        public void Clear()
        {
            m_data.Clear();
        }

        public void CopyTo(Array ary)
        {
            if (ary.GetType() != typeof(RigelEGUIVertex).MakeArrayType()) return;
            m_data.CopyTo((RigelEGUIVertex[])ary);
        }

        public void CopyTo(int index, Array ary, int arrayIndex, int count)
        {
            if (ary.GetType() != typeof(RigelEGUIVertex).MakeArrayType()) return;
            m_data.CopyTo(index, (RigelEGUIVertex[])ary, arrayIndex, count);
        }

        public void RemoveRange(int startpos, int count)
        {
            m_data.RemoveRange(startpos, count);
        }

        public float VerticesZ(int index)
        {
            return m_data[index].Position.z;
        }

        public Array GetData()
        {
            return m_data.ToArray();
        }
    }

    public partial class GUIGraphicsBindDefault
    {
        #region ShaderSource
        private static readonly string ShaderSourceRect = @"
        struct VS_IN
            {
	            float4 pos : POSITION;
	            float4 col : COLOR;
	            float4 uv : TEXCOORD;
            };

            struct PS_IN
            {
	            float4 pos : SV_POSITION;
	            float4 col : COLOR;
	            float2 uv : TEXCOORD0;
            };

            float4x4 worldViewProj;

            //Texture2D texfont;
            //SamplerState MeshTextureSampler;

            PS_IN VS( VS_IN input )
            {
	            PS_IN output = (PS_IN)0;

                float4x4 mtx = float4x4(
                    2.0/800.0,0,0,0,
                    0,-2.0/600,0,0,
                    0,0,1,0,
                    -1,1,0,1
                );
	
	            float4 pos = input.pos;
	            output.pos = mul(pos, mtx);
	            output.col = input.col;
	            output.uv = input.uv;
	            return output;
            }

            float4 PS( PS_IN input ) : SV_Target
            {
	            return input.col;
            }
            ";

        private static readonly string ShaderSourceText = @"
            struct VS_IN
            {
	            float4 pos : POSITION;
	            float4 col : COLOR;
	            float4 uv : TEXCOORD;
            };

            struct PS_IN
            {
	            float4 pos : SV_POSITION;
	            float4 col : COLOR;
	            float2 uv : TEXCOORD0;
            };

            float4x4 worldViewProj;

            Texture2D texfont;
            SamplerState MeshTextureSampler;

            PS_IN VS( VS_IN input )
            {
	            PS_IN output = (PS_IN)0;
	
	            float4 pos = input.pos;
	            output.pos = mul(pos, worldViewProj);
	            output.col = input.col;
	            output.uv = input.uv;
	            return output;
            }

            float4 PS( PS_IN input ) : SV_Target
            {
	            float4 v = input.col;
	            v.a = texfont.Sample(MeshTextureSampler,input.uv).r;
	            return v;
            }";

        #endregion


        private class GUIGraphicsIndicesBuffer : IGraphicsObj
        {
            private int m_bufferSize = 768;
            private int m_vertCount = 512;
            private const int m_scaleTime = 2;

            private IGraphicsBuffer m_buffer;

            private GraphicsBufferDesc m_desc;

            public IGraphicsBuffer Buffer
            {
                get { return m_buffer; }
            }

            public GUIGraphicsIndicesBuffer(IDevice device, int vsize = 512)
            {

                m_vertCount = vsize;
                m_bufferSize = GetIndicesCount(vsize);

                m_desc = new GraphicsBufferDesc();
                m_desc.BindFlags = GraphicsBindFlag.IndicesBuffer;
                m_desc.CPUAccessFlags = GraphicsBufferCPUAccessFlag.None;
                m_desc.SizeInByte = m_bufferSize * sizeof(uint);
                m_desc.StructureByteStride = 0;
                m_desc.Usage = GraphicsResourceUsage.Default;
                m_buffer = device.CreateBuffer(m_desc, CreateDataArray(vsize));

            }


            public void Dispose()
            {
                GraphicsUtility.Release(ref m_buffer);
            }

            public void ExtenSize(IDevice device, int vsize)
            {
                if (!CheckBufferExten(vsize)) return;

                int newvsize = m_vertCount * m_scaleTime;
                while (newvsize < vsize)
                {
                    newvsize *= m_scaleTime;
                }


                m_buffer.Dispose();

                m_vertCount = newvsize;
                m_bufferSize = GetIndicesCount(m_vertCount);

                m_desc.SizeInByte = m_bufferSize * sizeof(uint);
                m_buffer = device.CreateBuffer(m_desc, CreateDataArray(m_vertCount));

                Console.WriteLine($"exten buffer to:{m_vertCount} - {m_bufferSize}");

            }


            public bool CheckBufferExten(int vsize)
            {
                if (vsize > m_vertCount) return true;
                return false;
            }

            public static int GetIndicesCount(int vertCount)
            {
                return vertCount / 4 * 6;
            }

            public static uint[] CreateDataArray(int vertCount)
            {
                int indCount = GetIndicesCount(vertCount);
                uint[] data = new uint[indCount];

                uint c = 0;
                uint iv = 0;

                int imax = vertCount / 4;
                for (uint i = 0; i < imax; i++)
                {

                    data[c] = iv;
                    data[c + 1] = iv + 1;
                    data[c + 2] = iv + 2;
                    data[c + 3] = iv;
                    data[c + 4] = iv + 2;
                    data[c + 5] = iv + 3;

                    iv += 4;

                    c += 6;
                }

                return data;
            }
        }
    }

}
