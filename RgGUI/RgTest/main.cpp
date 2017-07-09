#include <rggui_window.h>
#include <rg_dx11.h>
#include <rggui_dx11.h>
#include <rggui.h>
#include <rg_log.h>

#include <rg_font.h>
#include <rg_image.h>

#pragma comment(lib,"RgGUI.lib")

using namespace rg;
using namespace rg::gui;

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
	case WM_QUIT:
		PostQuitMessage(0);
		return 0;
	default:
		return DefWindowProc(hwnd, msg, wparam, lparam);
	}
}

void update();


int main()
{
	font::RgFontFreeType * font;
	font::RgFontFreeType::LoadFont("arial.ttf", font);
	font->SetPixelSize(0, 16);
	//font->LoadChar(0x0077);

	RgU32 index= font->GetCharIndex(0x0077L);
	RgLogD() << "index" << index;
	font->LoadGlyph(index);
	font->RenderGlyph(FT_RENDER_MODE_NORMAL);

	FT_GlyphSlotRec_ r = *font->Glyph;
	RgLogD()<< font->Glyph->advance.x;
	RgLogD() << font->Glyph->bitmap.width;
	RgLogD() << font->Glyph->bitmap.rows;


	delete font;
	font = 0;


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
	if (dx11->CreateDeiviceD3D(window.getWindow(),800,600) < 0)
	{
		dx11->ShutDown();
		delete dx11;
		window.ShutDown();
		return 1;
	}
	window.Show();

	bool done = false;
	gui::InitRgGUI();

	unsigned int w, h;
	window.GetWindowSize(&w,&h);
	gui::GetContext().SetScreenSize(w,h);
	RgGUI_dx11_Init(window.getWindow(), dx11->getD3D11Device(), dx11->getD3D11DeviceContext());

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
		gui::Render();

		dx11->Present();

		Sleep(10);

	}
	RgGUI_dx11_Shutdown();

	dx11->ShutDown();
	delete dx11;

	RgLogD() << "shutdown window";
	window.ShutDown();

	getchar();

	return 0;
}

void update()
{
	//do draw


	static RgGuiWindowDesc desc = RgGuiWindowDesc(RgVec2(0),RgVec2(200,300),RgGuiWindowStyle_Header);

	Begin("testwindow", &desc);

	auto win = GetCurrentWindow();

	gui::Rectangle();
	gui::Rectangle();
	gui::Rectangle(RgVec2(100,100));

	Text("test text");
	if (Button("click me"))
	{
		std::cout << "you click the button" << std::endl;
	}
	End();

}