#pragma once
#include "RgInclude.h"

#pragma comment(lib,"d3d11.lib")
#pragma comment(lib,"dxgi.lib")
#pragma comment(lib,"d3dcompiler.lib")
#pragma comment(lib,"dxguid.lib")
#include <d3d11.h>
#include <dxgi.h>
#include <DirectXMath.h>
#include <d3dcompiler.h>

using namespace DirectX;

namespace rg {


	class RgDX11
	{
	public:
		RgDX11();
		~RgDX11();

	public:
		void ShutDown();

		HRESULT CreateDeiviceD3D(HWND hwnd);
		void CreateRenderTarget();
		void CleanupRenderTarget();
		void CleanupDeviceD3D();

	public:
		LRESULT WndProcHandler(HWND hwnd, UINT msg, WPARAM wparam, LPARAM lparam);

	private:
		ID3D11Device * g_pD3D11Device = NULL;
		ID3D11DeviceContext * g_pD3D11Context = NULL;
		IDXGISwapChain * g_pSwapChain = NULL;
		ID3D11RenderTargetView * g_pMainRenderTargetView = NULL;

	};
}