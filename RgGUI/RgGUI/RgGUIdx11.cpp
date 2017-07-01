#include "RgGUIdx11.h"
#include "rggui.h"

using namespace rg::gui;

namespace rg
{
	static HWND g_hWnd = 0;
	static ID3D11Device * g_pd3dDevice = 0;
	static ID3D11DeviceContext * g_pd3dDeviceContext = 0;
	static ID3D11Buffer * g_pvertexBuffer = 0;
	static ID3D11Buffer * g_pindexBuffer = 0;


	bool RgGUI_dx11_Init(void * hwnd, ID3D11Device * device, ID3D11DeviceContext * context)
	{
		std::cout << "rggui init" << std::endl;

		g_hWnd = (HWND)hwnd;
		g_pd3dDevice = device;
		g_pd3dDeviceContext = context;

		//map key
		RgGuiIO& io = GetIO();
		io.KeyMap[RgGuiKey_Backspace] = VK_BACK;

		gui::Init();

		return true;
	}

	void RgGUI_dx11_Shutdown()
	{
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

