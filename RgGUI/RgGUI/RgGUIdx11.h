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

namespace rg
{
	bool RgGUI_dx11_Init(void* hwnd, ID3D11Device *device, ID3D11DeviceContext * context);
	void RgGUI_dx11_Shutdown();
	void RgGUI_dx11_Frame();
}

