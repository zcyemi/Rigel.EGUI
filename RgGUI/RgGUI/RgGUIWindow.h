#pragma once
#include "RgInclude.h"

namespace rg
{
	struct WindowDesc
	{
		LPCWSTR appName;
		HINSTANCE hInstance;
		UINT style;
		WNDPROC lpfnWndProc;

		int posx;
		int posy;
		int width;
		int height;

	};

	class RgGUIWindow
	{
	public:
		RgGUIWindow(const WindowDesc & desc);
		~RgGUIWindow();

		void Show();
		void GetWindowSize(unsigned int* w, unsigned int* h);

		void ShutDown();

		const HWND& getWindow();

	private:
		HWND mHwnd;
		WNDCLASSEX mWc;

		int mWidth, mHeight;
	};


}
