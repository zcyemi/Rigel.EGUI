#include "RgGUIdx11.h"
#include "rggui.h"
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
		return S_OK;
	}
}

