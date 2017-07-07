#include "rggui.h"
using namespace std;

namespace rg
{
	namespace gui
	{
		RgVec2 operator+(RgVec2 & v1, RgVec2 & v2) {
			return RgVec2(v1.x + v2.x, v1.y + v2.y);
		}

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

			


		}

		void End()
		{
			RgGuiWindow* win = GetCurrentWindow();
			//clear win drawdata
			win->DrawList->ClearData();
			win->DrawSelf();

		}

		void Text(const char * t)
		{
		}

		bool Button(const char * t)
		{
			return false;
		}

		void RgGuiDrawList::AddRect(const RgVec2 & lt, const RgVec2 & rb)
		{
			RgU32 col = 0xffffffff;
			RgGuiDrawVert v1(lt, RgVec2(), col);
			RgGuiDrawVert v2(RgVec2(rb.x,lt.y), RgVec2(), col);
			RgGuiDrawVert v3(rb, RgVec2(), col);
			RgGuiDrawVert v4(RgVec2(lt.x,rb.y), RgVec2(), col);

			//RgGuiDrawVert v1(RgVec2(20.0f, 10.0f), RgVec2(), col);
			//RgGuiDrawVert v2(RgVec2(20.0f, 100.0f), RgVec2(), col);
			//RgGuiDrawVert v3(RgVec2(50.0f, 100.0f), RgVec2(), col);
			//RgGuiDrawVert v4(RgVec2(50.0f, 10.0f), RgVec2(), col);

			VertexBuffer.push_back(v1);
			VertexBuffer.push_back(v2);
			VertexBuffer.push_back(v3);
			VertexBuffer.push_back(v4);

			IndicesBuffer.push_back(IndicesIndex);
			IndicesBuffer.push_back(IndicesIndex + 1);
			IndicesBuffer.push_back(IndicesIndex + 2);
			IndicesBuffer.push_back(IndicesIndex);
			IndicesBuffer.push_back(IndicesIndex + 2);
			IndicesBuffer.push_back(IndicesIndex+ 3);

			IndicesIndex += 6;
			VertexCount += 4;
		}

		RgGuiDrawList::RgGuiDrawList()
		{
			RgLogD() << "create draw list";
		}

		void RgGuiDrawList::ClearData()
		{
			IndicesIndex = 0;
			VertexCount = 0;
			IndicesBuffer.clear();
			VertexBuffer.clear();
		}

		RgGuiWindow::RgGuiWindow(const char * name) :Name(name)
		{
			ID = RgHash(name, 0);
			this->DrawList = g_RgGuiMemAlloc.New<RgGuiDrawList>();
		}

		void RgGuiWindow::DrawSelf()
		{
			DrawList->AddRect(Pos, Pos + Size);
		}

		void RgGuiWindow::SetSize(RgVec2 & s)
		{
			Size = s;
		}

		void RgGuiWindow::SetPosition(RgVec2 & p)
		{
			Pos = p;
		}

		void RgGuiWindow::Move(RgVec2 & offset)
		{
			Pos.x += offset.x;
			Pos.y += offset.y;
		}

		void * RgMemAlloc::Alloc(size_t sz) {
			AllocsCount++;
			return malloc(sz);
		}

		RgGuiDrawVert::RgGuiDrawVert()
		{
		}

		RgGuiDrawVert::RgGuiDrawVert(RgVec2 pos_, RgVec2 uv_, RgU32 col_)
		{
			pos = pos_;
			uv = uv_;
			color = col_;
		}

		void RgGuiContext::SetScreenSize(RgU32 w, RgU32 h)
		{
			ScreenWidth = w;
			ScreenHeight = h;

			RgLogD() << "set screen size" << w << " " << h;
		}

}

}

