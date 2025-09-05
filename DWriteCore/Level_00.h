#pragma once

#pragma region 标准库

#include <iostream>

using namespace std;

#pragma endregion

#pragma region 系统库

#include <Windows.h>
#include <dwrite.h>

#pragma comment(lib, "dwrite.lib")

#pragma endregion

#define dll_export extern "C" __declspec(dllexport)

typedef struct FontMetrics
{
	int UnitsPerEm;
	// OpenType 升部
	SHORT TypoAscender;
	// OpenType 降部
	SHORT TypoDescender;
} FontMetrics;