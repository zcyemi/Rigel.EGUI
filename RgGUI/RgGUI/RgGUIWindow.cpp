#include "RgGUIWindow.h"

namespace rg
{

	RgGUIWindow::RgGUIWindow(const WindowDesc & desc)
	{
		WNDCLASSEX wc;

		wc.style = desc.style;
		wc.lpfnWndProc = desc.lpfnWndProc;
		wc.cbClsExtra = 0;
		wc.cbWndExtra = 0;
		wc.hInstance = desc.hInstance;
		wc.hIcon = NULL;
		wc.hIconSm = NULL;
		wc.hCursor = LoadCursor(NULL, IDC_ARROW);
		wc.hbrBackground = (HBRUSH)GetStockObject(BLACK_BRUSH);
		wc.lpszMenuName = NULL;
		wc.lpszClassName = desc.appName;
		wc.cbSize =sizeof(WNDCLASSEX);

		RegisterClassEx(&wc);

		mHwnd = CreateWindowEx(WS_EX_APPWINDOW, desc.appName, desc.appName, WS_CLIPSIBLINGS | WS_CLIPCHILDREN | WS_POPUP, desc.posx, desc.posy, desc.width, desc.height, NULL, NULL, desc.hInstance, NULL);

	}

	RgGUIWindow::~RgGUIWindow()
	{
	}

	void RgGUIWindow::Show()
	{
		ShowWindow(mHwnd, SW_SHOW);
		SetForegroundWindow(mHwnd);
		SetFocus(mHwnd);

		ShowCursor(true);
	}

}

