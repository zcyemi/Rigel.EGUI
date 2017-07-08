#include "rg_vec.h"

namespace rg
{
	RgVec2 operator+(const RgVec2 & v1, const RgVec2 & v2)
	{
		return RgVec2(v1.x + v2.x, v1.y + v2.y);
	}
}


