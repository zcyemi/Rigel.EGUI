#include "Rgdx11.h"


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
	HRESULT RgDX11::CreateDeiviceD3D(HWND hwnd)
	{
		DXGI_SWAP_CHAIN_DESC desc;
		{
			ZeroMemory(&desc, sizeof(desc));
			desc.BufferCount = 2;
			desc.BufferDesc.Width = 0;
			desc.BufferDesc.Height = 0;
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
		DXGI_SWAP_CHAIN_DESC sd;
		g_pSwapChain->GetDesc(&sd);

		ID3D11Texture2D *pBackbuffer;
		D3D11_RENDER_TARGET_VIEW_DESC rtvDesc;
		ZeroMemory(&rtvDesc, sizeof(rtvDesc));
		rtvDesc.Format = sd.BufferDesc.Format;
		rtvDesc.ViewDimension = D3D11_RTV_DIMENSION_TEXTURE2D;

		g_pSwapChain->GetBuffer(0, __uuidof(ID3D11Texture2D), (LPVOID*)&pBackbuffer);
		g_pD3D11Device->CreateRenderTargetView(pBackbuffer, &rtvDesc, &g_pMainRenderTargetView);
		g_pD3D11Context->OMSetRenderTargets(1, &g_pMainRenderTargetView, NULL);
		pBackbuffer->Release();
	}
	void RgDX11::CleanupRenderTarget()
	{
		if (g_pMainRenderTargetView)
		{
			g_pMainRenderTargetView->Release();
			g_pMainRenderTargetView= nullptr;
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
	}
	void RgDX11::Present()
	{
		g_pSwapChain->Present(0, 0);
	}
	LRESULT RgDX11::WndProcHandler(HWND hwnd, UINT msg, WPARAM wparam, LPARAM lparam)
	{
		if (mWndProcCust != nullptr)
		{
			if (mWndProcCust(hwnd, msg, wparam, lparam))
				return true;
		}
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


