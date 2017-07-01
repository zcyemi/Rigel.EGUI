#include "rggui.h"
using namespace std;

namespace rg
{
	namespace gui
	{
		int RgHash(const void * data, int data_size, unsigned int seed = 0)
		{
			static unsigned int crc32_lut[256] = { 0 };
			if (!crc32_lut[1])
			{
				const unsigned int polynomial = 0xEDB88320;
				for (unsigned int i = 0; i < 256; i++)
				{
					unsigned int crc = i;
					for (unsigned int j = 0; j < 8; j++)
						crc = (crc >> 1) ^ (unsigned int(-int(crc & 1)) & polynomial);
					crc32_lut[i] = crc;
				}
			}

			seed = seed;
			unsigned int crc = seed;
			const unsigned char* current = (const unsigned char*)data;

			if (data_size > 0) {
				while (data_size--)
					crc = (crc >> 8) ^ crc32_lut[(crc & 0xFF) ^ *current++];
			}
			else
			{
				while (unsigned char c = *current++)
				{
					crc = (crc >> 8) ^ crc32_lut[(crc & 0xFF) ^ c];
				}
			}
			return ~crc;
		}
		bool IsKeyDown(char key)
		{
			RgGuiIO& io = GetIO();
			return io.KeyDown[key];
		}
		;





		static RgMemAlloc g_RgGuiMemAlloc;
		static RgGuiContext g_RgGuiContext;
		RgGuiContext *GRgGui = &g_RgGuiContext;



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

		void Init()
		{
			RgGuiContext& ctx = GetContext();
		}

		void Render()
		{
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
		}

		void End()
		{
		}

		void Text(const char * t)
		{
		}

		bool Button(const char * t)
		{
			return false;
		}

		void RgGuiDrawList::AddRect(const RgVec2 & lb, const RgVec2 & rt)
		{
		}

		RgGuiDrawList::RgGuiDrawList()
		{
			RgLogD() << "create draw list";
		}

		RgGuiWindow::RgGuiWindow(const char * name) :Name(name)
		{
			ID = RgHash(name, 0);
		}

		void * RgMemAlloc::Alloc(size_t sz) {
			AllocsCount++;
			return malloc(sz);
		}

}

}

