#pragma once
#include "rg_include.h"
#include "rggui_defines.h"

namespace rg {
	namespace gui
	{

#define PARAM_ICON ICON::RgGuiIcon icon = ICON::ICON_NONE
#define PARAM_ICON_F ICON::RgGuiIcon icon

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
		bool Button(const char *t, PARAM_ICON);

		//x,y,w,z
		void DrawRectangle(RgVec4& rect,RgU32 color);
		void Rectangle(const RgVec2 size);
		void Rectangle();


		void Icon(PARAM_ICON);

		

}
}