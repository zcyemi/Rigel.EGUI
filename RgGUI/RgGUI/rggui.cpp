#include "rggui.h"
#include "rg_memalloc.h"
using namespace std;

namespace rg
{
	namespace gui
	{

		bool IsKeyDown(char key)
		{
			RgGuiIO& io = GetIO();
			return io.KeyDown[key];
		};


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

		RgGuiWindow * CreateGuiWindow(const char * name)
		{
			RgGuiContext& g = GetContext();

			RgGuiWindow * window = (RgGuiWindow*)g_RgGuiMemAlloc.Alloc(sizeof(RgGuiWindow));
			new(window) RgGuiWindow(name);

			g.Windows.push_back(window);

			return window;
		}

		void InitRgGUI()
		{
			RgLogD() << "init rggui";
			RgGuiContext& ctx = GetContext();
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

			//end release mem;
			RgLogW() << "RgGUI memAlloc cout:" << g_RgGuiMemAlloc.AllocsCount;
		}


		void Begin(const char * name)
		{
			static unsigned int id = 0;
			RgGuiContext& g = *GRgGui;
			RgGuiWindow* win = GetWindow(name);
			if (win == nullptr)
			{
				win = CreateGuiWindow(name);
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
		}

		bool Button(const char * t)
		{
			return false;
		}

		void DrawRect(RgVec4 & rect)
		{
			RgGuiWindow * win = GetCurrentWindow();
		}

}

}

