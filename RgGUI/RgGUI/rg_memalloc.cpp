#include "rg_memalloc.h"

namespace rg
{
	void * RgMemAlloc::Alloc(size_t sz)
	{
		AllocsCount++;
		return malloc(sz);
	}


	
}

