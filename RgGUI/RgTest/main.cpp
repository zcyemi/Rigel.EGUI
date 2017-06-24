#include "../RgGUI/RgGUIWindow.h"
#pragma comment(lib,"RgGUI.lib")

using namespace rg;

LRESULT CALLBACK WinProc(HWND hwnd, UINT msg, WPARAM wparam, LPARAM lparam)
{
	switch (msg)
	{
	case WM_DESTROY:
		PostQuitMessage(0);
		return 0;
	case WM_CLOSE:
		PostQuitMessage(0);
		return 0;
	default:
		return DefWindowProc(hwnd, msg, wparam, lparam);
	}
}


int main()
{
	WindowDesc desc;
	desc.appName = L"App Test";
	desc.height = 600;
	desc.width = 800;
	desc.hInstance = GetModuleHandle(NULL);
	desc.lpfnWndProc = WinProc;
	desc.posx = 200;
	desc.posy = 100;
	desc.style = CS_HREDRAW | CS_VREDRAW | CS_OWNDC;

	RgGUIWindow window(desc);
	window.Show();

	MSG msg;
	ZeroMemory(&msg, sizeof(MSG));
	bool done = false;
	while (!done)
	{
		if (PeekMessage(&msg, NULL, 0, 0, PM_REMOVE))
		{
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
		if (msg.message == WM_QUIT)
		{
			done = true;
		}
	}

	return 0;
}