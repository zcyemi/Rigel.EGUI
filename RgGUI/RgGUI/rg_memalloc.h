#pragma once
#include "rg_include.h"

namespace rg
{
	struct RgMemAlloc
	{
		unsigned int AllocsCount = 0;

		void * Alloc(size_t sz);

		template<typename T>
		inline T * New() {
			T *t = (T*)Alloc(sizeof(T));
			new(t) T();
			return t;
		}
	};
}