#pragma once
#include "RgInclude.h"
#include "rggui_vec.h"
namespace rg {
	namespace gui
	{
		typedef unsigned short RgGuiDrawIdx;
		typedef unsigned int RgU32;

		struct RgGuiDrawList;
		struct RgVec2;
		struct RgVec4;
		
#define RgVector std::vector

		struct RgVec2
		{
			float x, y;
			RgVec2() { x = y = 0.0f; }
			RgVec2(float _x, float _y) { x = _x; y = _y; }
		};

		struct RgVec4
		{
			float x, y, z, w;
			RgVec4() { x = y = z = w = 0.0f; }
			RgVec4(float _x, float _y, float _z, float _w) { x = _x; y = _y; z = _z; w = _w; }
		};

		RgVec2 operator +(RgVec2& v1, RgVec2& v2);

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

			RgGuiDrawList * DrawList;

			RgGuiWindow(const char * name);

			void DrawSelf();
			void SetSize(RgVec2& s);
		};

		struct RgGuiDrawVert
		{
			RgVec2 pos;
			RgVec2 uv;
			RgU32 color;

			RgGuiDrawVert(RgVec2 pos_, RgVec2 uv_, RgU32 col_);
		};

		struct RgGuiDrawList
		{
			RgVector<RgGuiDrawIdx> IndicesBuffer;
			RgVector<RgGuiDrawVert> VertexBuffer;

			RgU32 IndicesIndex = 0;

			void AddRect(const RgVec2& lb, const RgVec2& rt);
			RgGuiDrawList();

			void ClearData();
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