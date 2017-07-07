#include "rg_dx11.h"


namespace rg
{
	RgDX11::RgDX11()
	{
	}
	RgDX11::~RgDX11()
	{
	}
	void RgDX11::ShutDown()
	{
		CleanupDeviceD3D();
	}
	HRESULT RgDX11::CreateDeiviceD3D(HWND hwnd, int width, int height)
	{
		mWidth = width;
		mHeight = height;

		DXGI_SWAP_CHAIN_DESC desc;
		{
			ZeroMemory(&desc, sizeof(desc));
			desc.BufferCount = 2;
			desc.BufferDesc.Width = width;
			desc.BufferDesc.Height = height;
			desc.BufferDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
			desc.BufferDesc.RefreshRate.Numerator = 60;
			desc.BufferDesc.RefreshRate.Denominator = 1;
			desc.Flags = DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH;
			desc.BufferUsage = DXGI_USAGE_RENDER_TARGET_OUTPUT;
			desc.OutputWindow = hwnd;
			desc.SampleDesc.Count = 1;
			desc.SampleDesc.Quality = 0;
			desc.Windowed = true;
			desc.SwapEffect = DXGI_SWAP_EFFECT_DISCARD;
		}

		UINT createDeviceFlags = 0;
		D3D_FEATURE_LEVEL featureLevel;
		const D3D_FEATURE_LEVEL freatureLevelArray[1] = { D3D_FEATURE_LEVEL_11_0 };
		if (D3D11CreateDeviceAndSwapChain(NULL, D3D_DRIVER_TYPE_HARDWARE, NULL, createDeviceFlags, freatureLevelArray, 1, D3D11_SDK_VERSION, &desc, &g_pSwapChain, &g_pD3D11Device, &featureLevel, &g_pD3D11Context) != S_OK)
			return E_FAIL;

		CreateRenderTarget();

		return S_OK;

	}
	void RgDX11::CreateRenderTarget()
	{
		HRESULT result;


		{
			D3D11_TEXTURE2D_DESC depthBufferDesc;
			ZeroMemory(&depthBufferDesc, sizeof(depthBufferDesc));
			depthBufferDesc.Width = mWidth;
			depthBufferDesc.Height = mHeight;
			depthBufferDesc.MipLevels = 1;
			depthBufferDesc.ArraySize = 1;
			depthBufferDesc.Format = DXGI_FORMAT_D24_UNORM_S8_UINT;
			depthBufferDesc.SampleDesc.Count = 1;
			depthBufferDesc.SampleDesc.Quality = 0;
			depthBufferDesc.Usage = D3D11_USAGE_DEFAULT;
			depthBufferDesc.BindFlags = D3D11_BIND_DEPTH_STENCIL;
			depthBufferDesc.CPUAccessFlags = 0;
			depthBufferDesc.MiscFlags = 0;

			result = g_pD3D11Device->CreateTexture2D(&depthBufferDesc, NULL, &g_pDepthStencilBuffer);
		}
		{
			D3D11_DEPTH_STENCIL_DESC depthStencilDesc;
			ZeroMemory(&depthStencilDesc, sizeof(depthStencilDesc));
			depthStencilDesc.DepthEnable = true;
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

			result = g_pD3D11Device->CreateDepthStencilState(&depthStencilDesc, &g_pDepthStencilState);

			g_pD3D11Context->OMSetDepthStencilState(g_pDepthStencilState, 1);
		}
		{
			D3D11_DEPTH_STENCIL_VIEW_DESC depthStencilViewDesc;
			ZeroMemory(&depthStencilViewDesc, sizeof(depthStencilViewDesc));
			depthStencilViewDesc.Format = DXGI_FORMAT_D24_UNORM_S8_UINT;
			depthStencilViewDesc.ViewDimension = D3D11_DSV_DIMENSION_TEXTURE2D;
			depthStencilViewDesc.Texture2D.MipSlice = 0;

			result = g_pD3D11Device->CreateDepthStencilView(g_pDepthStencilBuffer, &depthStencilViewDesc, &g_pDepthStencilView);
		}

		{
			DXGI_SWAP_CHAIN_DESC sd;
			g_pSwapChain->GetDesc(&sd);

			ID3D11Texture2D *pBackbuffer;
			D3D11_RENDER_TARGET_VIEW_DESC rtvDesc;
			ZeroMemory(&rtvDesc, sizeof(rtvDesc));
			rtvDesc.Format = sd.BufferDesc.Format;
			rtvDesc.ViewDimension = D3D11_RTV_DIMENSION_TEXTURE2D;

			g_pSwapChain->GetBuffer(0, __uuidof(ID3D11Texture2D), (LPVOID*)&pBackbuffer);
			result = g_pD3D11Device->CreateRenderTargetView(pBackbuffer, &rtvDesc, &g_pMainRenderTargetView);


			g_pD3D11Context->OMSetRenderTargets(1, &g_pMainRenderTargetView, g_pDepthStencilView);
			pBackbuffer->Release();
		}

		{
			D3D11_RASTERIZER_DESC rasterDesc;
			rasterDesc.AntialiasedLineEnable = false;
			rasterDesc.CullMode = D3D11_CULL_NONE;
			rasterDesc.DepthBias = 0;
			rasterDesc.DepthBiasClamp = 0.0f;
			rasterDesc.DepthClipEnable = true;
			rasterDesc.FillMode = D3D11_FILL_SOLID;;
			rasterDesc.FrontCounterClockwise = false;
			rasterDesc.MultisampleEnable = false;
			rasterDesc.ScissorEnable = false;
			rasterDesc.SlopeScaledDepthBias = 0.0f;

			result = g_pD3D11Device->CreateRasterizerState(&rasterDesc, &g_pRasterState);

			g_pD3D11Context->RSSetState(g_pRasterState);
		}

		//viewport
		{
			D3D11_VIEWPORT viewport;
			viewport.Width = (float)mWidth;
			viewport.Height = (float)mHeight;
			viewport.MaxDepth = 1.0f;
			viewport.MinDepth = 0.0f;
			viewport.TopLeftX = 0.0f;
			viewport.TopLeftY = 0.0f;
			g_pD3D11Context->RSSetViewports(1, &viewport);
		}
	}
	void RgDX11::CleanupRenderTarget()
	{
		if (g_pMainRenderTargetView)
		{
			g_pMainRenderTargetView->Release();
			g_pMainRenderTargetView= nullptr;
		}

		if(g_pDepthStencilBuffer)
		{
			g_pDepthStencilBuffer->Release();
			g_pDepthStencilBuffer = nullptr;
		}
		if (g_pDepthStencilState)
		{
			g_pDepthStencilState->Release();
			g_pDepthStencilState = nullptr;
		}
		if (g_pDepthStencilView)
		{
			g_pDepthStencilView->Release();
			g_pDepthStencilView = nullptr;
		}

		if (g_pRasterState)
		{
			g_pRasterState->Release();
			g_pRasterState = nullptr;
		}
		
	}
	void RgDX11::CleanupDeviceD3D()
	{
		CleanupRenderTarget();
		if (g_pSwapChain) g_pSwapChain->Release(); g_pSwapChain = nullptr;
		if (g_pD3D11Context)g_pD3D11Context->Release(); g_pD3D11Context = nullptr;
		if (g_pD3D11Device) g_pD3D11Device->Release(); g_pD3D11Device = nullptr;
	}
	ID3D11Device * RgDX11::getD3D11Device()
	{
		return g_pD3D11Device;
	}
	ID3D11DeviceContext * RgDX11::getD3D11DeviceContext()
	{
		return g_pD3D11Context;
	}
	void RgDX11::PreRender()
	{
		g_pD3D11Context->ClearRenderTargetView(g_pMainRenderTargetView, (float*)&mClearColor);
		g_pD3D11Context->ClearDepthStencilView(g_pDepthStencilView, D3D11_CLEAR_DEPTH, 1.0, 0);
	}
	void RgDX11::Present()
	{
		g_pSwapChain->Present(0, 0);
	}
	LRESULT RgDX11::WndProcHandler(HWND hwnd, UINT msg, WPARAM wparam, LPARAM lparam)
	{
		//if (mWndProcCust != nullptr)
		//{
		//	if (mWndProcCust(hwnd, msg, wparam, lparam))
		//		return true;
		//}
		switch (msg)
		{
		case WM_SIZE:
			if (g_pD3D11Device != NULL && wparam != SIZE_MINIMIZED)
			{
				CleanupRenderTarget();
				g_pSwapChain->ResizeBuffers(0, (UINT)LOWORD(lparam), (UINT)HIWORD(lparam), DXGI_FORMAT_UNKNOWN, 0);
				CreateRenderTarget();
			}
			return 0;
		}
		return DefWindowProc(hwnd, msg, wparam, lparam);
	}
}


