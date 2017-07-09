#include "rggui_dx11.h"
#include "rggui.h"
#include "rg_image.h"

#define DEFAULT_VERTEX_BUFFER_SIZE 256
#define RELEASE_DX(x) if(x){		\
		x->Release();				\
		x = nullptr;				\
	}

#define SHADER_VS L"vs.hlsl"
#define SHADER_PS L"ps.hlsl"

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

	static ID3D11DepthStencilState *m_depthStencilState;

	//texture
	static ID3D11Texture2D *m_textureIcon;
	static ID3D11Texture2D *m_textureFont;
	static ID3D11ShaderResourceView * m_textureIconSRV;
	static ID3D11ShaderResourceView * m_textureFontSRV;

	static ID3D11SamplerState * m_samplerState;

	struct DATA_CONST_BUFFER
	{
		float mvp[4][4];
	};

	static float m_mvp[4][4];


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
		RgLogW() << "release rggui dx";
		m_inited = false;
		
		RELEASE_DX(m_samplerState);
		RELEASE_DX(m_textureIconSRV);
		RELEASE_DX(m_textureIcon);
		RELEASE_DX(m_textureFontSRV);
		RELEASE_DX(m_textureFont);

		RELEASE_DX(m_depthStencilState);
		RELEASE_DX(m_vertexShader);
		RELEASE_DX(m_pixelShader);
		RELEASE_DX(m_inputlayout);
		RELEASE_DX(m_vertexShaderBlob);
		RELEASE_DX(m_pixleShaderBlob);
		RELEASE_DX(m_indexBuffer);
		RELEASE_DX(m_vertexBuffer);
		RELEASE_DX(m_constBuffer);

		delete m_vertexdata;
		m_vertexdata = nullptr;
		delete m_indexdata;
		m_indexdata = nullptr;
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

			result = dx_device->CreateBuffer(&vbufferdesc, nullptr, &m_vertexBuffer);
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
			ibufferdesc.ByteWidth = sizeof(RgGuiDrawIdx) * 256;
			ibufferdesc.BindFlags = D3D11_BIND_INDEX_BUFFER;
			ibufferdesc.MiscFlags = 0;
			ibufferdesc.StructureByteStride = 0;
			ibufferdesc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;

			//m_indexdata = new RgGuiDrawIdx[3]{ 0,2,1 };

			//D3D11_SUBRESOURCE_DATA indexdata;
			//indexdata.pSysMem = m_indexdata;
			//indexdata.SysMemPitch = 0;
			//indexdata.SysMemSlicePitch = 0;

			result = dx_device->CreateBuffer(&ibufferdesc, nullptr, &m_indexBuffer);
			if (result != S_OK)
			{
				RgLogE() << "create index buffer error";
				return false;
			}
		}

		//const buffer
		{
			D3D11_BUFFER_DESC cbufferdesc;
			cbufferdesc.Usage = D3D11_USAGE_DYNAMIC;
			cbufferdesc.ByteWidth = sizeof(DATA_CONST_BUFFER);
			cbufferdesc.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
			cbufferdesc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
			cbufferdesc.MiscFlags = 0;
			result = dx_device->CreateBuffer(&cbufferdesc, NULL, &m_constBuffer);

			if (result != S_OK)
			{
				RgLogE() << "create const buffer error";
				return false;
			}

			RgGuiContext& guictx= gui::GetContext();
			float R =(float) guictx.ScreenWidth;
			float B =(float) guictx.ScreenHeight;
			float L = 0.0f;
			float T = 0.0f;

			D3D11_MAPPED_SUBRESOURCE cbuffer_res;
			result = dx_deviceContext->Map(m_constBuffer, 0, D3D11_MAP_WRITE_DISCARD, 0, &cbuffer_res);
			if (result != S_OK)
			{
				RgLogE() << "map const buffer error";
				return false;
			}
			DATA_CONST_BUFFER *dataptr =(DATA_CONST_BUFFER*) cbuffer_res.pData;
			float mtx[4][4] =
			{
				{ 2.0f / (R - L),   0.0f,           0.0f,       0.0f },
				{ 0.0f,         2.0f / (T - B),     0.0f,       0.0f },
				{ 0.0f,         0.0f,           0.5f,       0.0f },
				{ (R + L) / (L - R),  (T + B) / (B - T),    0.5f,       1.0f },
			};

			memcpy(&dataptr->mvp, mtx, sizeof(mtx));
			dx_deviceContext->Unmap(m_constBuffer, 0);

			dx_deviceContext->VSSetConstantBuffers(0, 1, &m_constBuffer);
		}

		{
			//shader
			D3DCompileFromFile(GetDataPath(SHADER_VS).c_str(), nullptr, nullptr, "main", "vs_4_0", 0, 0, &m_vertexShaderBlob, nullptr);
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

			D3DCompileFromFile(GetDataPath(SHADER_PS).c_str(), nullptr, nullptr, "main", "ps_4_0", 0, 0, &m_pixleShaderBlob, NULL);
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

		{
			//depth stencil state
			D3D11_DEPTH_STENCIL_DESC depthStencilDesc;
			ZeroMemory(&depthStencilDesc, sizeof(depthStencilDesc));
			depthStencilDesc.DepthEnable = false;
			depthStencilDesc.DepthWriteMask = D3D11_DEPTH_WRITE_MASK_ALL;
			depthStencilDesc.DepthFunc = D3D11_COMPARISON_LESS;

			depthStencilDesc.StencilEnable = true;
			depthStencilDesc.StencilReadMask = 0xFF;
			depthStencilDesc.StencilWriteMask = 0xFF;

			depthStencilDesc.FrontFace.StencilFailOp = D3D11_STENCIL_OP_KEEP;
			depthStencilDesc.FrontFace.StencilDepthFailOp = D3D11_STENCIL_OP_INCR;
			depthStencilDesc.FrontFace.StencilPassOp = D3D11_STENCIL_OP_KEEP;
			depthStencilDesc.FrontFace.StencilFunc = D3D11_COMPARISON_ALWAYS;

			depthStencilDesc.BackFace.StencilFailOp = D3D11_STENCIL_OP_KEEP;
			depthStencilDesc.BackFace.StencilDepthFailOp = D3D11_STENCIL_OP_DECR;
			depthStencilDesc.BackFace.StencilPassOp = D3D11_STENCIL_OP_KEEP;
			depthStencilDesc.BackFace.StencilFunc = D3D11_COMPARISON_ALWAYS;

			result = dx_device->CreateDepthStencilState(&depthStencilDesc, &m_depthStencilState);
			if (result != S_OK)
			{
				RgLogE() << "create depthstencilstate error";
				return false;
			}
			dx_deviceContext->OMSetDepthStencilState(m_depthStencilState, 1);
		}

		{
			//texture icon;
			D3D11_TEXTURE2D_DESC textureDesc;
			RgImage * img;
			if (!RgImageLoad(GetDataPath(L"texture.tga"), &img, RgImageType_Targa))
			{
				RgLogE() << "load icon texture error";
				return false;
			}

			textureDesc.Height = img->Height();
			textureDesc.Width = img->Width();
			textureDesc.MipLevels = 0;
			textureDesc.ArraySize = 1;
			textureDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
			textureDesc.SampleDesc.Count = 1;
			textureDesc.SampleDesc.Quality = 0;
			textureDesc.Usage = D3D11_USAGE_DEFAULT;
			textureDesc.BindFlags = D3D11_BIND_SHADER_RESOURCE | D3D11_BIND_RENDER_TARGET;
			textureDesc.CPUAccessFlags = 0;
			textureDesc.MiscFlags = 0;

			result = dx_device->CreateTexture2D(&textureDesc, NULL, &m_textureIcon);
			if (result != S_OK)
			{
				RgLogE() << "create icon texture error";
				return false;
			}

			unsigned int rowPitch;
			rowPitch = img->Width() * 4 * sizeof(unsigned char);
			dx_deviceContext->UpdateSubresource(m_textureIcon, 0, NULL, img->Data(), rowPitch,0);

			D3D11_SHADER_RESOURCE_VIEW_DESC srvDesc;
			srvDesc.ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2D;
			srvDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
			srvDesc.Texture2D.MostDetailedMip = 0;
			srvDesc.Texture2D.MipLevels = -1;

			result = dx_device->CreateShaderResourceView(m_textureIcon, &srvDesc, &m_textureIconSRV);
			if (result != S_OK)
			{
				RgLogE() << "create srv error";
				return false;
			}

			//dx_deviceContext->GenerateMips(m_textureIconSRV);

			img->Release();
			delete img;
		}

		{
			//sampler state
			D3D11_SAMPLER_DESC samplerDesc;
			samplerDesc.Filter = D3D11_FILTER_MIN_MAG_MIP_POINT;
			samplerDesc.AddressU = D3D11_TEXTURE_ADDRESS_WRAP;
			samplerDesc.AddressV = D3D11_TEXTURE_ADDRESS_WRAP;
			samplerDesc.AddressW = D3D11_TEXTURE_ADDRESS_WRAP;
			samplerDesc.MipLODBias = 0.0F;
			samplerDesc.MaxAnisotropy = 1;
			samplerDesc.ComparisonFunc = D3D11_COMPARISON_ALWAYS;
			samplerDesc.BorderColor[0] = 0;
			samplerDesc.BorderColor[1] = 0;
			samplerDesc.BorderColor[2] = 0;
			samplerDesc.BorderColor[3] = 0;
			samplerDesc.MinLOD = 0;
			samplerDesc.MaxLOD = 0;

			result = dx_device->CreateSamplerState(&samplerDesc, &m_samplerState);
			if (result != S_OK)
			{
				RgLogE() << "create samplerState Error";
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
			memcpy(vertex_dataptr, data->VertexBuffer.Data, sizeof(RgGuiDrawVert) * data->VertexCount);
			dx_deviceContext->Unmap(m_vertexBuffer, 0);

			result = dx_deviceContext->Map(m_indexBuffer, 0, D3D11_MAP_WRITE_DISCARD, 0, &index_res);
			if (result != S_OK)
			{
				RgLogE() << "map index data error";
				return false;
			}
			RgGuiDrawIdx * index_dataptr = (RgGuiDrawIdx*)index_res.pData;
			memcpy(index_dataptr, data->IndicesBuffer.Data, sizeof(RgGuiDrawIdx)* data->IndicesBuffer.Size);
			dx_deviceContext->Unmap(m_indexBuffer, 0);
		}


		dx_deviceContext->IASetVertexBuffers(0, 1, &m_vertexBuffer, &stride, &offset);
		dx_deviceContext->IASetIndexBuffer(m_indexBuffer, DXGI_FORMAT_R32_UINT, 0);
		dx_deviceContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

		//dx_deviceContext->VSSetConstantBuffers(0, 1, &m_constbuffer);

		dx_deviceContext->IASetInputLayout(m_inputlayout);
		dx_deviceContext->VSSetShader(m_vertexShader, NULL, 0);
		dx_deviceContext->PSSetShader(m_pixelShader, NULL, 0);

		dx_deviceContext->PSSetShaderResources(0, 1, &(m_textureIconSRV));
		dx_deviceContext->PSSetSamplers(0, 1, &m_samplerState);
		dx_deviceContext->DrawIndexed(data->IndicesBuffer.Size, 0, 0);

		return true;
	}
}

