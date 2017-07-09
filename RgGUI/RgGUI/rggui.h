#pragma once
#include "rg_include.h"
#include "rggui_defines.h"

namespace rg {
	namespace gui
	{
		bool IsKeyDown(char key);

		std::wstring GetDataPath(const WCHAR* filename);

		RgGuiWindow * GetCurrentWindow();
		RgGuiContext& GetContext();
		RgGuiIO& GetIO();

		RgGuiWindow *GetWindow(const char * name);
		RgGuiWindow *CreateGuiWindow(const char*name, RgGuiWindowDesc * desc);


		void InitRgGUI(RgGuiSkin& skin = RGGUI_SKIN_DEFAULT);


		bool Render();
		void NewFrame();
		void ShutDown();

		void Begin(const char* name, RgGuiWindowDesc * desc = nullptr);

		void End();


		void Text(const char *t);
		bool Button(const char *t);

		//x,y,w,z
		void DrawRectangle(RgVec4& rect,RgU32 color);
		void Rectangle(const RgVec2 size);
		void Rectangle();


		

}
}