#pragma once
#include "rg_include.h"
#include "rggui_defines.h"

namespace rg {
	namespace gui
	{
		bool IsKeyDown(char key);


		RgGuiWindow * GetCurrentWindow();
		RgGuiContext& GetContext();
		RgGuiIO& GetIO();

		RgGuiWindow *GetWindow(const char * name);
		RgGuiWindow *CreateGuiWindow(const char* name,RgGuiDrawWindowStyle style, RgGuiWindowSkin *skin);


		void InitRgGUI(RgGuiSkin& skin = RGGUI_SKIN_DEFAULT);


		bool Render();
		void NewFrame();
		void ShutDown();

		void Begin(const char * name, RgGuiDrawWindowStyle style = (int) RgGuiWindowStyle_Default, RgGuiWindowSkin *skin = NULL);

		void End();


		void Text(const char *t);
		bool Button(const char *t);

		//x,y,w,z
		void DrawRect(RgVec4& rect,RgU32 color);
		


		

}
}