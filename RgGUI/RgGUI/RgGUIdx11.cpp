#include "RgGUIdx11.h"
#include "rggui.h"

#define DEFAULT_VERTEX_BUFFER_SIZE 256

using namespace rg::gui;
using namespace DirectX;

namespace rg
{
	static HWND mhwnd;
	static ID3D11Device* dx_device;
	static ID3D11DeviceContext * dx_deviceContext;

	static ID3D11Buffer * m_constBuffer;
	static ID3D11Buffer * m_vertexBuffer;
	static ID3D11Buffer * m_indexBuffer;

	static RgGuiDrawVert * m_vertexdata;
	static unsigned int *m_indexdata;
	
	static ID3DBlob * m_vertexShaderBlob;
	static ID3DBlob * m_pixleShaderBlob;
	static ID3D11VertexShader * m_vertexShader;
	static ID3D11PixelShader * m_pixelShader;
	static ID3D11InputLayout * m_inputlayout;

	static bool m_inited = false;

	bool RgGUI_dx11_createObjects();

	bool RgGUI_dx11_Draw(RgGuiDrawList *data);

	bool RgGUI_dx11_Init(void * hwnd, ID3D11Device * device, ID3D11DeviceContext * context)
	{
		m_inited = true;

		mhwnd =(HWND)hwnd;
		dx_device = device;
		dx_deviceContext = context;

		if (!RgGUI_dx11_createObjects())
		{
			m_inited = false;

			RgLogE() << "dx11 create objects error ";
		}

		RgGuiContext& ctx = GetContext();
		ctx.RenderDrawListFunction = RgGUI_dx11_Draw;

		if (!m_inited) RgGUI_dx11_Shutdown();

		return m_inited;
	}
	void RgGUI_dx11_Shutdown()
	{
	}
	void RgGUI_dx11_Frame()
	{
		if (!m_inited) return;
	}
	LRESULT RgGUI_dx11_WndProc(HWND hwnd, UINT msg, WPARAM wparam, LPARAM lparam)
	{

		RgGuiIO& io = GetIO();
		switch (msg)
		{
		case WM_LBUTTONDOWN:
			io.MouseDown[0] = true;
			return true;
		case WM_LBUTTONUP:
			io.MouseDown[0] = false;
			return true;
		case WM_RBUTTONDOWN:
			io.MouseDown[1] = true;
			return true;
		case WM_RBUTTONUP:
			io.MouseDown[1] = false;
			return true;
		case WM_MBUTTONDOWN:
			io.MouseDown[2] = true;
			return true;
		case WM_MBUTTONUP:
			io.MouseDown[2] = false;
			return true;
		case WM_MOUSEWHEEL:
			io.MouseWheel += GET_WHEEL_DELTA_WPARAM(wparam) > 0 ? +1.0f : -1.0f;
			return true;
		case WM_MOUSEMOVE:
			io.MousePos.x = (signed short)(lparam);
			io.MousePos.y = (signed short)(lparam >> 16);
			return true;
		case WM_KEYDOWN:
			if (wparam < 256)
			{
				io.KeyDown[wparam] = true;
				if (wparam == 'Q')
				{
					PostQuitMessage(0);
					
				}
			}
			return true;
		case WM_KEYUP:
			if (wparam < 256)
				io.KeyDown[wparam] = false;
			return true;
		}
		return 0;
	}
	bool RgGUI_dx11_createObjects()
	{
		HRESULT result;

		auto ctx = dx_deviceContext;

		//vertex
		{
			D3D11_BUFFER_DESC vbufferdesc;
			vbufferdesc.ByteWidth = sizeof(RgGuiDrawVert) * DEFAULT_VERTEX_BUFFER_SIZE;
			vbufferdesc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
			vbufferdesc.Usage = D3D11_USAGE_DYNAMIC;
			vbufferdesc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
			vbufferdesc.MiscFlags = 0;
			vbufferdesc.StructureByteStride = 0;

			m_vertexdata = new RgGuiDrawVert[3];

			m_vertexdata[0].pos = RgVec2(-1.0f, -1.0f);
			m_vertexdata[0].uv = RgVec2(0, 0);
			m_vertexdata[0].color = (((255<<8) | 0) << 8)<<8 | 255;

			m_vertexdata[1].pos = RgVec2(0.0f, 1.0f);
			m_vertexdata[1].uv = RgVec2(0, 0);
			m_vertexdata[1].color = 0;

			m_vertexdata[2].pos = RgVec2(1.0f, -1.0f);
			m_vertexdata[2].uv = RgVec2(0, 0);
			m_vertexdata[2].color = 0;

			D3D11_SUBRESOURCE_DATA vertexdata;
			vertexdata.pSysMem = m_vertexdata;
			vertexdata.SysMemPitch = 0;
			vertexdata.SysMemSlicePitch = 0;

			result = dx_device->CreateBuffer(&vbufferdesc, &vertexdata, &m_vertexBuffer);
			if (result != S_OK)
			{
				RgLogE() << "Create vertex buffer error";
				return false;
			}
		}

		//index buffer
		{
			D3D11_BUFFER_DESC ibufferdesc;
			ibufferdesc.Usage = D3D11_USAGE_DYNAMIC;
			ibufferdesc.ByteWidth = sizeof(RgGuiDrawIdx) * 3;
			ibufferdesc.BindFlags = D3D11_BIND_INDEX_BUFFER;
			ibufferdesc.MiscFlags = 0;
			ibufferdesc.StructureByteStride = 0;
			ibufferdesc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;

			m_indexdata = new RgGuiDrawIdx[3]{ 0,2,1 };

			D3D11_SUBRESOURCE_DATA indexdata;
			indexdata.pSysMem = m_indexdata;
			indexdata.SysMemPitch = 0;
			indexdata.SysMemSlicePitch = 0;

			result = dx_device->CreateBuffer(&ibufferdesc, &indexdata, &m_indexBuffer);
			if (result != S_OK)
			{
				RgLogE() << "create index buffer error";
				return false;
			}
		}

		{
			//shader
			D3DCompileFromFile(L"../RgGUI/vs.hlsl", nullptr, nullptr, "main", "vs_4_0", 0, 0, &m_vertexShaderBlob, nullptr);
			if (m_vertexShaderBlob == nullptr)
			{
				RgLogE() << "compile vertex shader error";
				return false;
			}
			if (dx_device->CreateVertexShader(m_vertexShaderBlob->GetBufferPointer(), m_vertexShaderBlob->GetBufferSize(), NULL, &m_vertexShader) != S_OK)
			{
				RgLogE() << "create vertex shader error";
				return false;
			}

			D3DCompileFromFile(L"../RgGUI/ps.hlsl", nullptr, nullptr, "main", "ps_4_0", 0, 0, &m_pixleShaderBlob, NULL);
			if (m_pixleShaderBlob == nullptr)
			{
				RgLogE() << "compile pixel shader error";
				return false;
			}

			if (dx_device->CreatePixelShader(m_pixleShaderBlob->GetBufferPointer(), m_pixleShaderBlob->GetBufferSize(), nullptr, &m_pixelShader) != S_OK)
			{
				RgLogE() << "create pixel shader error";
				return false;
			}
		}

		//input layout
		{
			D3D11_INPUT_ELEMENT_DESC layout[3];
			layout[0].SemanticName = "POSITION";
			layout[0].SemanticIndex = 0;
			layout[0].Format = DXGI_FORMAT_R32G32_FLOAT;
			layout[0].InputSlot = 0;
			layout[0].AlignedByteOffset = 0;
			layout[0].InputSlotClass = D3D11_INPUT_PER_VERTEX_DATA;
			layout[0].InstanceDataStepRate = 0;

			layout[1].SemanticName = "TEXCOORD";
			layout[1].SemanticIndex = 0;
			layout[1].Format = DXGI_FORMAT_R32G32_FLOAT;
			layout[1].InputSlot = 0;
			layout[1].AlignedByteOffset = D3D11_APPEND_ALIGNED_ELEMENT;
			layout[1].InputSlotClass = D3D11_INPUT_PER_VERTEX_DATA;
			layout[1].InstanceDataStepRate = 0;

			layout[2].SemanticName = "COLOR";
			layout[2].SemanticIndex = 0;
			layout[2].Format = DXGI_FORMAT_R8G8B8A8_UNORM;
			layout[2].InputSlot = 0;
			layout[2].AlignedByteOffset = D3D11_APPEND_ALIGNED_ELEMENT;
			layout[2].InputSlotClass = D3D11_INPUT_PER_VERTEX_DATA;
			layout[2].InstanceDataStepRate = 0;

			result = dx_device->CreateInputLayout(layout, 3, m_vertexShaderBlob->GetBufferPointer(), m_vertexShaderBlob->GetBufferSize(), &m_inputlayout);

			if (result != S_OK)
			{
				RgLogE() << "create input layout error";
				return false;
			}
				
		}

		return true;
	}
	bool RgGUI_dx11_Draw(RgGuiDrawList * data)
	{
		if (!m_inited) return false;

		HRESULT result;

		unsigned int stride = sizeof(RgGuiDrawVert);
		unsigned int offset = 0;

		D3D11_MAPPED_SUBRESOURCE vertex_res, index_res;
		//mapdata
		{
			result = dx_deviceContext->Map(m_vertexBuffer, 0, D3D11_MAP_WRITE_DISCARD, 0, &vertex_res);
			if (result != S_OK)
			{
				RgLogE() << "map vertex data error";
				return false;
			}
			RgGuiDrawVert * vertex_dataptr = (RgGuiDrawVert*)vertex_res.pData;
			memcpy(vertex_dataptr, data->VertexBuffer.data(), sizeof(RgGuiDrawVert) * data->VertexCount);
			dx_deviceContext->Unmap(m_vertexBuffer, 0);

			result = dx_deviceContext->Map(m_indexBuffer, 0, D3D11_MAP_WRITE_DISCARD, 0, &index_res);
			if (result != S_OK)
			{
				RgLogE() << "map index data error";
				return false;
			}
			RgGuiDrawIdx * index_dataptr = (RgGuiDrawIdx*)index_res.pData;
			memcpy(index_dataptr, data->IndicesBuffer.data(), sizeof(RgGuiDrawIdx)* data->IndicesIndex);
			dx_deviceContext->Unmap(m_indexBuffer, 0);
		}


		dx_deviceContext->IASetVertexBuffers(0, 1, &m_vertexBuffer, &stride, &offset);
		dx_deviceContext->IASetIndexBuffer(m_indexBuffer, DXGI_FORMAT_R32_UINT, 0);
		dx_deviceContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

		//dx_deviceContext->VSSetConstantBuffers(0, 1, &m_constbuffer);

		dx_deviceContext->IASetInputLayout(m_inputlayout);
		dx_deviceContext->VSSetShader(m_vertexShader, NULL, 0);
		dx_deviceContext->PSSetShader(m_pixelShader, NULL, 0);

		dx_deviceContext->DrawIndexed(3, 0, 0);

		return true;
	}
}

