#include "rggui_window.h"

namespace rg
{

	RgGUIWindow::RgGUIWindow(const WindowDesc & desc)
	{

		mWc.style = desc.style;
		mWc.lpfnWndProc = desc.lpfnWndProc;
		mWc.cbClsExtra = 0;
		mWc.cbWndExtra = 0;
		mWc.hInstance = desc.hInstance;
		mWc.hIcon = NULL;
		mWc.hIconSm = NULL;
		mWc.hCursor = LoadCursor(NULL, IDC_ARROW);
		mWc.hbrBackground = (HBRUSH)GetStockObject(BLACK_BRUSH);
		mWc.lpszMenuName = NULL;
		mWc.lpszClassName = desc.appName;
		mWc.cbSize =sizeof(WNDCLASSEX);

		RegisterClassEx(&mWc);
		mHwnd = CreateWindowEx(WS_EX_APPWINDOW, desc.appName, desc.appName, WS_CLIPSIBLINGS | WS_CLIPCHILDREN | WS_CAPTION | WS_SYSMENU, desc.posx, desc.posy, desc.width, desc.height, NULL, NULL, desc.hInstance, NULL);


		mWidth = desc.width;
		mHeight = desc.height;
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

	void RgGUIWindow::ShutDown()
	{
		UnregisterClass(mWc.lpszClassName, mWc.hInstance);
		CloseWindow(mHwnd);
	}

	void RgGUIWindow::GetWindowSize(unsigned int* w,unsigned int* h)
	{
		*w = mWidth;
		*h = mHeight;
	}

	const HWND & RgGUIWindow::getWindow()
	{
		return mHwnd;
	}

}

