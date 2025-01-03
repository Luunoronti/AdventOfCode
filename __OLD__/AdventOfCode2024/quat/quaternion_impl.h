#pragma once

#include "quaternion.h"
#include "../math/math.h"

namespace mutil
{
    constexpr Quaternion operator+(const Quaternion &a, const Quaternion &b) { return Quaternion(a.w + b.w, a.x + b.x, a.y + b.y, a.z + b.z); }
	constexpr Quaternion operator-(const Quaternion &a, const Quaternion &b) { return Quaternion(a.w - b.w, a.x - b.x, a.y - b.y, a.z - b.z); }
	constexpr Quaternion operator*(const Quaternion &a, float s) { return Quaternion(a.w * s, a.x * s, a.y * s, a.z * s); }
	constexpr Quaternion operator*(float s, const Quaternion &a) { return a * s; }
	constexpr Quaternion operator/(const Quaternion &a, float s) { return Quaternion(a.w / s, a.x / s, a.y / s, a.z / s); }

	constexpr Quaternion operator-(const Quaternion &a) { return Quaternion(-a.w, -a.x, -a.y, -a.z); }

	constexpr Quaternion operator*(const Quaternion &a, const Quaternion &b)
	{
		return Quaternion(
			a.w * b.w - a.x * b.x - a.y * b.y - a.z * b.z,
			a.w * b.x + a.x * b.w + a.y * b.z - a.z * b.y,
			a.w * b.y + a.y * b.w + a.z * b.x - a.x * b.z,
			a.w * b.z + a.z * b.w + a.x * b.y - a.y * b.x);
	}

	constexpr Vector3 imag(const Quaternion &q) { return Vector3(q.x, q.y, q.z); }

	inline float length(const Quaternion &q)
	{
		return length(q.v4);
	}

	inline float lengthSq(const Quaternion &q)
	{
		return lengthSq(q.v4);
	}

	inline Quaternion normalize(const Quaternion &q)
	{
		float invsqrt = fastInverseSqrt(q.w * q.w + q.x * q.x + q.y * q.y + q.z * q.z);
		return q * invsqrt;
	}

	constexpr Quaternion conjugate(const Quaternion &q)
	{
		return Quaternion(q.w, -q.x, -q.y, -q.z);
	}

	constexpr Quaternion inverse(const Quaternion &q)
	{
		float lensq = q.w * q.w + q.x * q.x + q.y * q.y + q.z * q.z;
		return conjugate(q) / lensq;
	}

	constexpr Matrix4 torotation(const Quaternion &q)
	{
		const float x2 = q.x * q.x;
		const float y2 = q.y * q.y;
		const float z2 = q.z * q.z;

		const float xy = q.x * q.y;
		const float xz = q.x * q.z;
		const float xw = q.x * q.w;
		const float yz = q.y * q.z;
		const float yw = q.y * q.w;
		const float zw = q.z * q.w;

		return Matrix4(
			1 - 2 * y2 - 2 * z2, 2 * xy - 2 * zw, 2 * xz + 2 * yw, 0,
			2 * xy + 2 * zw, 1 - 2 * x2 - 2 * z2, 2 * yz - 2 * xw, 0,
			2 * xz - 2 * yw, 2 * yz + 2 * xw, 1 - 2 * x2 - 2 * y2, 0,
			0, 0, 0, 1);
	}

	constexpr Matrix3 torotation3(const Quaternion &q)
	{
		const float x2 = q.x * q.x;
		const float y2 = q.y * q.y;
		const float z2 = q.z * q.z;

		const float xy = q.x * q.y;
		const float xz = q.x * q.z;
		const float xw = q.x * q.w;
		const float yz = q.y * q.z;
		const float yw = q.y * q.w;
		const float zw = q.z * q.w;

		return Matrix3(
			1 - 2 * y2 - 2 * z2, 2 * xy - 2 * zw, 2 * xz + 2 * yw,
			2 * xy + 2 * zw, 1 - 2 * x2 - 2 * z2, 2 * yz - 2 * xw,
			2 * xz - 2 * yw, 2 * yz + 2 * xw, 1 - 2 * x2 - 2 * y2);
	}

	inline Quaternion rotateaxis(const Vector3 &axis, float angle)
	{
		return Quaternion(
			cosf(angle / 2),
			sinf(angle / 2) * axis);
	}

	inline Vector3 rotatevector(const Quaternion &q, const Vector3 &p)
	{
		// q * p * q'
		Quaternion r = q * Quaternion(0.0f, p) * conjugate(q);
		return imag(r);
	}

	inline Vector3 toeuler(const Quaternion &q)
	{
		Vector3 euler;
		float a, b;

		a = 2.0f * (q.w * q.x + q.y * q.z);
		b = 1.0f - 2.0f * (q.x * q.x + q.y * q.y);
		euler.x = atan2f(a, b);

		float sinp = 2.0f * (q.w * q.y - q.z * q.x);
		if (fabsf(sinp) >= 1.0f)
			euler.y = sinp > 0 ? MUTIL_PI2 : -MUTIL_PI2;
		else
			euler.y = asinf(sinp);

		a = 2.0f * (q.w * q.z + q.z * q.y);
		b = 1.0f - 2.0f * (q.y * q.y + q.z * q.z);
		euler.z = atan2f(b, a);

		return euler;
	}

	inline Quaternion fromeuler(float x, float y, float z)
	{
		Quaternion qx = rotateaxis(Vector3(1.0f, 0.0f, 0.0f), x);
		Quaternion qy = rotateaxis(Vector3(0.0f, 1.0f, 0.0f), y);
		Quaternion qz = rotateaxis(Vector3(0.0f, 0.0f, 1.0f), z);

		return qx * qy * qz;
	}

	inline Quaternion fromeuler(const Vector3 &euler)
	{
		return fromeuler(euler.x, euler.y, euler.z);
	}

	inline float dot(const Quaternion &a, const Quaternion &b)
	{
		return dot(a.v4, b.v4);
	}

	constexpr Quaternion lerp(const Quaternion &a, const Quaternion &b, float t)
	{
		Vector4 l = lerp(a.v4, b.v4, t);
		return Quaternion(l.x, l.y, l.z, l.w);
	}

	inline Quaternion slerpNotShortest(const Quaternion &a, const Quaternion &b, float t)
	{
		const float theta = acosf(dot(a, b)) / 2; // half angle between a and b
		const float stheta = sinf(theta);
		const float l = sinf((1.0f - t) * theta);
		const float r = sinf(t * theta);

		return ((a * l) + (b * r)) / stheta;
	}

	inline Quaternion slerp(const Quaternion &a, const Quaternion &b, float t)
	{
		return dot(a, b) > 0
				   ? slerpNotShortest(a, b, t)
				   : slerpNotShortest(a, -b, t);
	}

	inline Quaternion nlerp(const Quaternion &a, const Quaternion &b, float t)
	{
		return normalize(lerp(a, b, t));
	}

	inline Quaternion sqrt(const Quaternion &a)
	{
		const float mag = length(a);
		const float coeff1 = mutil::sqrt((mag + a.a) / 2.0f);
		const float coeff2 = mutil::sqrt((mag - a.a) / 2.0f);
		return Quaternion(coeff1, normalize(imag(a)) * coeff2);
	}

	inline Quaternion exp(const Quaternion &a)
	{
		const float ea = expf(a.a);
		const float mag = length(a);

		const float cosmag = mutil::cos(mag);
		const float sinmag = mutil::sin(mag);

		return ea * Quaternion(cosmag, normalize(imag(a)) * sinmag);
	}

	inline Quaternion log(const Quaternion &a)
	{
		const float mag = length(a);
		const float lnmag = logf(mag);
		return Quaternion(lnmag, normalize(imag(a))) * acosf(a.a / lnmag);
	}

	inline float geodistance(const Quaternion &a, const Quaternion &b)
	{
		const float d = dot(a, b);
		const float d2 = d * d;
		return acosf(2.0f * d2 - 1.0f);
	}

	inline Quaternion angleAxis(const float& angle, mutil::Vector3 const& v)
	{
		float const a(angle);
		float const s = mutil::sin(a * 0.5f);
        return mutil::Quaternion(mutil::cos(a * 0.5f), v * s);
	}

	inline float Quaternion::length() const
	{
		return mutil::length(*this);
	}

	inline float Quaternion::lengthSq() const
	{
		return mutil::lengthSq(*this);
	}

	inline Quaternion Quaternion::normalized() const
	{
		return mutil::normalize(*this);
	}

	constexpr Quaternion Quaternion::conjugate() const
	{
		return mutil::conjugate(*this);
	}

	constexpr Quaternion Quaternion::inverse() const
	{
		return mutil::inverse(*this);
	}

	constexpr Matrix4 Quaternion::torotation() const
	{
		return mutil::torotation(*this);
	}

	constexpr Matrix3 Quaternion::torotation3() const
	{
		return mutil::torotation3(*this);
	}

	inline Vector3 Quaternion::toeuler() const
	{
		return mutil::toeuler(*this);
	}

	inline Quaternion cross(Quaternion const& q1, Quaternion const& q2)
	{
		return Quaternion(
			q1.w * q2.w - q1.x * q2.x - q1.y * q2.y - q1.z * q2.z,
			q1.w * q2.x + q1.x * q2.w + q1.y * q2.z - q1.z * q2.y,
			q1.w * q2.y + q1.y * q2.w + q1.z * q2.x - q1.x * q2.z,
			q1.w * q2.z + q1.z * q2.w + q1.x * q2.y - q1.y * q2.x);
	}

}
