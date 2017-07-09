#pragma once

namespace rg
{
	struct RgVec2
	{
		float x, y;
		RgVec2() { x = y = 0.0f; }
		RgVec2(float _x, float _y) { x = _x; y = _y; }
		RgVec2(float v) { x = y = v; }

		inline RgVec2(const RgVec2& r)
		{
			x = r.x;
			y = r.y;
		}

		inline RgVec2& operator +=(const RgVec2& v)
		{
			x += v.x;
			y += v.y;
			return *this;
		}

		inline RgVec2& operator *(float v)
		{
			x *= v;
			y *= v;
			return *this;
		}
	};

	struct RgVec4
	{
		float x, y, z, w;
		RgVec4() { x = y = z = w = 0.0f; }
		RgVec4(float _x, float _y, float _z, float _w) :x(_x), y(_y), z(_z), w(_w) {}
	};

	RgVec2 operator +(const RgVec2& v1, const RgVec2& v2);

}

