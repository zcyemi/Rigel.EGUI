#pragma once
#include "RgInclude.h"
#include "rggui_vec.h"
namespace rg {
	namespace gui
	{
		typedef unsigned short RgGuiDrawIdx;
		typedef unsigned int RgU32;

		struct RgGuiWindow;
		struct RgGuiContext;

		struct RgGuiDrawVert;
		struct RgGuiDrawList;
		struct RgGuiDrawData;

		struct RgMemAlloc;

		//utility
		int RgHash(const void* data, int data_size, unsigned int seed);



		RgGuiWindow * GetCurrentWindow();
		RgGuiContext& GetContext();

		RgGuiWindow *GetWindow(const char * name);
		RgGuiWindow *CreateGuiWindow(const char* name);

		void Init();


		void Render();
		void NewFrame();
		void ShutDown();

		void Begin(const char * name);
		void End();


		void Text(const char *t);
		bool Button(const char *t);




	}
}