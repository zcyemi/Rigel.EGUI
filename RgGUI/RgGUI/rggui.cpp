#include "rggui.h"
#include "rg_memalloc.h"
#include "rggui_textrender.h"
using namespace std;

namespace rg
{
	namespace gui
	{

		bool IsKeyDown(char key)
		{
			RgGuiIO& io = GetIO();
			return io.KeyDown[key];
		}
		std::wstring GetDataPath(const WCHAR * filename)
		{
			std::wstring path = util::GetWorkDirectory();
			path = path.append(L"\\Data\\").append(filename);

			return path;
		}


		RgGuiWindow * GetCurrentWindow()
		{
			RgGuiContext& c = *GRgGui;
			return c.CurrentWindow;
		}

		RgGuiContext & GetContext()
		{
			return *GRgGui;
		}

		RgGuiIO & GetIO()
		{
			return GRgGui->IO;
		}

		RgGuiWindow* GetWindow(const char * name)
		{
			RgGuiContext& g = GetContext();
			unsigned int id = RgHash(name, 0);
			for (RgGuiWindow* w : g.Windows)
			{
				if (w ->ID == id) return w;
			}
			return nullptr;
		}

		RgGuiWindow * CreateGuiWindow(const char * name, RgGuiWindowDesc * desc)
		{
			RgGuiContext& g = GetContext();

			RgGuiWindow * window = (RgGuiWindow*)g_RgGuiMemAlloc.Alloc(sizeof(RgGuiWindow));
			new(window) RgGuiWindow(name,desc);

			g.Windows.push_back(window);

			return window;
		}


		void InitRgGUI(RgGuiSkin & skin)
		{
			RgLogD() << "init rggui ";
			RgGuiContext& ctx = GetContext();
			ctx.SetSkin(skin);

			ctx.Init();
		}


		bool Render()
		{
			RgGuiContext& ctx = GetContext();
			if (ctx.RenderDrawListFunction != nullptr)
			{
				return ctx.RenderDrawListFunction(ctx.CurrentWindow->DrawList);
			}

			return false;
		}

		void NewFrame()
		{
		}

		void ShutDown()
		{
			//before release mem
			RgGuiContext& ctx = GetContext();
			ctx.Release();

			//end release mem;
			RgLogW() << "RgGUI memAlloc cout:" << g_RgGuiMemAlloc.AllocsCount;
		}

		void Begin(const char * name, RgGuiWindowDesc * desc)
		{
			static unsigned int id = 0;
			RgGuiContext& g = *GRgGui;
			RgGuiWindow* win = GetWindow(name);
			if (win == nullptr)
			{
				win = CreateGuiWindow(name, desc);
			}

			g.CurrentWindow = win;

			win->Begin();
		}

		void End()
		{
			RgGuiWindow* win = GetCurrentWindow();
			//clear win drawdata
			win->End();

		}

		void Text(const char * t)
		{
			RgGuiContext & ctx = GetContext();
			RgGuiWindow * win = GetCurrentWindow();
			ctx.TextRender->DrawTextWithRect(t, win, RgVec2(10, 10), RgVec2(100, 20));
		}

		bool Button(const char * t, PARAM_ICON_F)
		{
			return false;
		}

		void DrawRectangle(RgVec4 & rect, RgU32 color)
		{
			RgGuiWindow * win = GetCurrentWindow();
			win->DrawList->SetColor(color);
			win->DrawList->AddRect(RgVec2(rect.x,rect.y),RgVec2(rect.x + rect.z,rect.y+rect.w));
		}

		void Rectangle(const RgVec2 size)
		{
			RgGuiWindow * win = GetCurrentWindow();
			win->DrawList->AddRect(win->temp_layoutOffset, win->temp_layoutOffset + size);
			win->temp_layoutOffset.y += size.y;
			win->temp_layoutOffset.y += win->Skin.LINE_OFFSET;
		}

		void Rectangle()
		{
			Rectangle(SKIN_BUTTON_SIZE);
		}

		void Icon(PARAM_ICON_F)
		{
			RgGuiWindow * win = GetCurrentWindow();
			RgVec2 off = win->temp_layoutOffset + SKIN_ICON_OFFSET;

			win->DrawList->SetColor(0xffffffff);
			win->DrawList->AddRect(off, off+ SKIN_ICON_SIZE);
			RgVec2 icon_lt(0);
			RgGui_IconGetUV(icon,&icon_lt);
			RgVec2 icon_rb = icon_lt + RgGui_IconGetUVsize();

			win->DrawList->SetLastRectUV(icon_lt, icon_rb);

			win->temp_layoutOffset.y += SKIN_ICON_FULLSIZE.y;
			win->temp_layoutOffset.y += win->Skin.LINE_OFFSET;

		}

}

}

