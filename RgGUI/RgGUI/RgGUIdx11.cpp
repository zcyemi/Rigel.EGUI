#include "RgGUIdx11.h"
#include "rggui.h"

using namespace rg::gui;
using namespace DirectX;

namespace rg
{
	static HWND g_hWnd = 0;
	static ID3D11Device * g_pd3dDevice = 0;
	static ID3D11DeviceContext * g_pd3dDeviceContext = 0;
	static ID3D11Buffer * g_pvertexBuffer = 0;
	static ID3D11Buffer * g_pindexBuffer = 0;

	static ID3DBlob * g_pVertexShaderBlob;
	static ID3D11VertexShader *g_pVertexShader;
	static ID3D11InputLayout *g_pInputLayout;

	static ID3D11Buffer *g_pConstBuffer;

	static ID3DBlob * g_pPixelShaderBlob;
	static ID3D11PixelShader *g_pPixelShader;

	static ID3D11BlendState * g_pBlendState;
	static ID3D11RasterizerState * g_pRasterizerState;
	static ID3D11DepthStencilState *g_pDepthStencilState;

	bool RgGUI_dx11_CreateDeviceObjects()
	{
		RgLogD() << "create device objects";

		if (!g_pd3dDevice) return false;

#pragma region shader
		static const char* vertexShader = "cbuffer vertexBuffer : register(b0) \
            {\
            float4x4 ProjectionMatrix; \
            };\
            struct VS_INPUT\
            {\
            float2 pos : POSITION;\
            float4 col : COLOR0;\
            float2 uv  : TEXCOORD0;\
            };\
            \
            struct PS_INPUT\
            {\
            float4 pos : SV_POSITION;\
            float4 col : COLOR0;\
            float2 uv  : TEXCOORD0;\
            };\
            \
            PS_INPUT main(VS_INPUT input)\
            {\
            PS_INPUT output;\
            output.pos = mul( ProjectionMatrix, float4(input.pos.xy, 0.f, 1.f));\
            output.col = input.col;\
            output.uv  = input.uv;\
            return output;\
            }";
		static const char* pixelShader =
			"struct PS_INPUT\
            {\
            float4 pos : SV_POSITION;\
            float4 col : COLOR0;\
            float2 uv  : TEXCOORD0;\
            };\
            sampler sampler0;\
            Texture2D texture0;\
            \
            float4 main(PS_INPUT input) : SV_Target\
            {\
            float4 out_col = input.col * texture0.Sample(sampler0, input.uv); \
            return out_col; \
            }";
#pragma endregion
		//compileshader
		D3DCompile(vertexShader, strlen(vertexShader), NULL, NULL, NULL, "main", "vs_4_0", 0, 0, &g_pVertexShaderBlob, NULL);
		if (g_pVertexShaderBlob == nullptr)
		{
			RgLogE() << "compile vertex shader error";
			return false;
		}
		if (g_pd3dDevice->CreateVertexShader(g_pVertexShaderBlob->GetBufferPointer(), g_pVertexShaderBlob->GetBufferSize(), NULL, &g_pVertexShader) != S_OK)
		{
			RgLogE() << "create dx11 vertex shader error";
			return false;
		}

		//input layout
		D3D11_INPUT_ELEMENT_DESC layout[] = {
			{"POSITION",0,DXGI_FORMAT_R32G32_FLOAT,0,(size_t)(&((RgGuiDrawVert*)0)->pos),D3D11_INPUT_PER_VERTEX_DATA,0},
			{ "TEXCOORD",0,DXGI_FORMAT_R32G32_FLOAT,0,(size_t)(&((RgGuiDrawVert*)0)->uv),D3D11_INPUT_PER_VERTEX_DATA,0 },
			{ "COLOR",0,DXGI_FORMAT_R8G8B8A8_UNORM,0,(size_t)(&((RgGuiDrawVert*)0)->color),D3D11_INPUT_PER_VERTEX_DATA,0 },
		};

		if (g_pd3dDevice->CreateInputLayout(layout, 3, g_pVertexShaderBlob->GetBufferPointer(), g_pVertexShaderBlob->GetBufferSize(), &g_pInputLayout) != S_OK)
		{
			RgLogE() << "create input layout error";
			return false;
		}

		//const buffer

		//pixel shader
		D3DCompile(pixelShader, strlen(pixelShader), NULL, NULL, NULL, "main", "ps_4_0", 0, 0, &g_pPixelShaderBlob, NULL);
		if (g_pPixelShaderBlob == nullptr)
		{
			RgLogE() << "compile pixel shader error!";
			return false;
		}

		if (g_pd3dDevice->CreatePixelShader(g_pPixelShaderBlob->GetBufferPointer(), g_pPixelShaderBlob->GetBufferSize(), nullptr, &g_pPixelShader) != S_OK)
		{
			RgLogE() << "create pixel shader error";
			return false;
		}

		//blending
		{
			D3D11_BLEND_DESC desc;
			ZeroMemory(&desc, sizeof(D3D11_BLEND_DESC));
			desc.AlphaToCoverageEnable = false;
			desc.RenderTarget[0].BlendEnable = true;
			desc.RenderTarget[0].SrcBlend = D3D11_BLEND_SRC_ALPHA;
			desc.RenderTarget[0].DestBlend = D3D11_BLEND_INV_SRC_ALPHA;
			desc.RenderTarget[0].BlendOp = D3D11_BLEND_OP_ADD;
			desc.RenderTarget[0].SrcBlendAlpha = D3D11_BLEND_INV_SRC_ALPHA;
			desc.RenderTarget[0].DestBlendAlpha = D3D11_BLEND_ZERO;
			desc.RenderTarget[0].BlendOpAlpha = D3D11_BLEND_OP_ADD;
			desc.RenderTarget[0].RenderTargetWriteMask = D3D11_COLOR_WRITE_ENABLE_ALL;
			g_pd3dDevice->CreateBlendState(&desc, &g_pBlendState);
		}

		//create the rasterizer state
		{
			D3D11_RASTERIZER_DESC desc;
			ZeroMemory(&desc, sizeof(desc));
			desc.FillMode = D3D11_FILL_SOLID;
			desc.CullMode = D3D11_CULL_NONE;
			desc.ScissorEnable = true;
			desc.DepthClipEnable = true;
			g_pd3dDevice->CreateRasterizerState(&desc, &g_pRasterizerState);
		}

		//depthe stencil state
		{
			D3D11_DEPTH_STENCIL_DESC desc;
			ZeroMemory(&desc, sizeof(desc));
			desc.DepthEnable = false;
			desc.DepthWriteMask = D3D11_DEPTH_WRITE_MASK_ALL;
			desc.DepthFunc = D3D11_COMPARISON_ALWAYS;
			desc.StencilEnable = false;
			desc.FrontFace.StencilFailOp = D3D11_STENCIL_OP_KEEP;
			desc.FrontFace.StencilDepthFailOp = D3D11_STENCIL_OP_KEEP;
			desc.FrontFace.StencilPassOp = D3D11_STENCIL_OP_KEEP;
			desc.FrontFace.StencilFunc = D3D11_COMPARISON_ALWAYS;
			desc.BackFace = desc.FrontFace;
			g_pd3dDevice->CreateDepthStencilState(&desc, &g_pDepthStencilState);
		}

		return true;
		
	}

	void RgGUI_dx11_DestroyDeviceObjects()
	{
		RgLogD() << "destroy device objects";

		if (g_pvertexBuffer) g_pvertexBuffer->Release(); g_pvertexBuffer = nullptr;
		if (g_pindexBuffer) g_pindexBuffer->Release(); g_pindexBuffer = nullptr;
		if (g_pBlendState) g_pBlendState->Release(); g_pBlendState = nullptr;
		if (g_pDepthStencilState)g_pDepthStencilState->Release(); g_pDepthStencilState = nullptr;
		if (g_pRasterizerState) g_pRasterizerState->Release(); g_pRasterizerState = nullptr;
		if (g_pPixelShader) g_pPixelShader->Release(); g_pPixelShader = nullptr;
		if (g_pPixelShaderBlob) g_pPixelShaderBlob->Release(); g_pPixelShaderBlob = nullptr;
		if (g_pInputLayout) g_pInputLayout->Release(); g_pindexBuffer = nullptr;
		if (g_pVertexShader)g_pVertexShader->Release(); g_pVertexShader = nullptr;
		if (g_pVertexShaderBlob) g_pVertexShaderBlob->Release(); g_pVertexShaderBlob = nullptr;

	}


	bool RgGUI_dx11_Init(void * hwnd, ID3D11Device * device, ID3D11DeviceContext * context)
	{
		std::cout << "rggui init" << std::endl;

		g_hWnd = (HWND)hwnd;
		g_pd3dDevice = device;
		g_pd3dDeviceContext = context;

		//map key
		RgGuiIO& io = GetIO();
		io.KeyMap[RgGuiKey_Backspace] = VK_BACK;
		io.KeyMap[RgGuiKey_LeftArrow] = VK_LEFT;
		io.KeyMap[RgGuiKey_RightArrow] = VK_RIGHT;
		io.KeyMap[RgGuiKey_DownArrow] = VK_DOWN;
		io.KeyMap[RgGuiKey_UpArrow] = VK_UP;
		io.KeyMap[RgGuiKey_Enter] = VK_RETURN;
		io.KeyMap[RgGuiKey_Escape] = VK_ESCAPE;
		io.KeyMap[RgGuiKey_PageUp] = VK_PRIOR;
		io.KeyMap[RgGuiKey_PageDown] = VK_NEXT;

		RgGUI_dx11_CreateDeviceObjects();

		gui::Init();

		return true;
	}

	void RgGUI_dx11_Shutdown()
	{
		RgGUI_dx11_DestroyDeviceObjects();

		g_hWnd = 0;
		g_pd3dDevice = 0;
		g_pd3dDeviceContext = 0;
		g_pvertexBuffer = 0;
		g_pindexBuffer = 0;

		gui::ShutDown();

		std::cout << "rggui shutdown" << std::endl;
	}

	void RgGUI_dx11_Frame()
	{
		gui::NewFrame();
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
			}
				
			return true;
		case WM_KEYUP:
			if (wparam < 256)
				io.KeyDown[wparam] = false;
			return true;
		}
		return 0;
	}
}

