#include "../RgGUI/RgGUIWindow.h"
#include "../RgGUI/Rgdx11.h"
#include "../RgGUI/RgGUIdx11.h"
#include "../RgGUI/rggui.h"

#include "../RgGUI/rglog.h"

#pragma comment(lib,"RgGUI.lib")

using namespace rg;


RgDX11 *dx11;

LRESULT CALLBACK WinProc(HWND hwnd, UINT msg, WPARAM wparam, LPARAM lparam)
{

	//imgui
	if (RgGUI_dx11_WndProc(hwnd, msg, wparam, lparam))
		return true;

	//dx11 proc
	if (dx11 != nullptr && dx11->WndProcHandler(hwnd, msg, wparam, lparam))
	{
		return true;
	}

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

void update();


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

	dx11 = new RgDX11();
	if (dx11->CreateDeiviceD3D(window.getWindow()) < 0)
	{
		dx11->ShutDown();
		delete dx11;
		window.ShutDown();
		return 1;
	}
	window.Show();

	bool done = false;
	done |= !RgGUI_dx11_Init(window.getWindow(), dx11->getD3D11Device(), dx11->getD3D11DeviceContext());

	MSG msg;
	ZeroMemory(&msg, sizeof(MSG));
	while (!done)
	{
		if (PeekMessage(&msg, NULL, 0, 0, PM_REMOVE))
		{
			TranslateMessage(&msg);
			DispatchMessage(&msg);
			continue;
		}
		if (msg.message == WM_QUIT)
		{
			done = true;
		}

		RgGUI_dx11_Frame();
		update();


		dx11->PreRender();
		//do draw

		//render rggui
		done |= !gui::Render();

		dx11->Present();

		Sleep(10);

	}
	RgGUI_dx11_Shutdown();

	dx11->ShutDown();
	delete dx11;
	window.ShutDown();

	getchar();

	return 0;
}

void update()
{
	//do draw

	gui::Begin("testwindow");

	auto win = gui::GetCurrentWindow();
	win->SetSize(gui::RgVec2(20, 30));
	gui::Text("test text");
	if (gui::Button("click me"))
	{
		std::cout << "you click the button" << std::endl;
	}
	gui::End();
}