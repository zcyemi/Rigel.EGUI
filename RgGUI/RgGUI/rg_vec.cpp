#include "rg_vec.h"

namespace rg
{
	RgVec2 operator+(RgVec2 & v1, RgVec2 & v2)
	{
		return RgVec2(v1.x + v2.x, v1.y + v2.y);
	}
}


