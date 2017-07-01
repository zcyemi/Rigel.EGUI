#pragma once
#include "RgInclude.h"
#include "rggui_vec.h"
namespace rg {
	namespace gui
	{
		typedef unsigned short RgGuiDrawIdx;
		typedef unsigned int RgU32;
		
#define RgVector std::vector

		enum RgGuiKey
		{
			RgGuiKey_Space,
			RgGuiKey_Enter,
			RgGuiKey_Tab,
			RgGuiKey_LeftArrow,
			RgGuiKey_RightArrow,
			RgGuiKey_UpArrow,
			RgGuiKey_DownArrow,
			RgGuiKey_PageUp,
			RgGuiKey_PageDown,
			RgGuiKey_Backspace,
			RgGuiKey_Escape,
			RgGuiKey_COUNT
		};



#pragma region Struct Define
		struct RgGuiIO
		{
			int KeyMap[RgGuiKey_COUNT];
			bool MouseDown[5];
			float MouseWheel;

			RgVec2 MousePos;

			bool MouseDrawCursor;
			bool KeyCtrl;
			bool KeyShift;
			bool KeyAlt;
			bool KeySuper;

			bool KeyDown[512];
		};
		struct RgGuiWindow
		{
			const char * Name;
			unsigned int ID;
			RgVec2 Pos;
			RgVec2 Size;

			RgGuiWindow(const char * name);
		};

		struct RgGuiDrawVert
		{
			RgVec2 pos;
			RgVec2 uv;
			RgU32 color;
		};

		struct RgGuiDrawList
		{
			RgVector<RgGuiDrawIdx> IndicesBuffer;
			RgVector<RgGuiDrawVert> VertexBuffer;

			void AddRect(const RgVec2& lb, const RgVec2& rt);
			RgGuiDrawList();
		};

		struct RgGuiContext
		{
			RgGuiWindow* CurrentWindow;
			RgGuiDrawList DrawList;
			RgGuiIO IO;
			RgVector<RgGuiWindow *> Windows;
		};

		struct RgMemAlloc
		{
			unsigned int AllocsCount = 0;

			void * Alloc(size_t sz);
			template <class T>
			T* New();
		};


#pragma endregion

		

		//utility
		int RgHash(const void* data, int data_size, unsigned int seed);

		bool IsKeyDown(char key);


		RgGuiWindow * GetCurrentWindow();
		RgGuiContext& GetContext();
		RgGuiIO& GetIO();

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


		template<class T>
		inline T * RgMemAlloc::New() {
			T *t = (T*)Alloc(sizeof(T));
			new(t) T();
			return t;
		}

}
}