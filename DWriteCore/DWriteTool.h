#pragma once

#include "Level_00.h"

class DWriteTool
{
public:
	void Init();

	void Release();

	FontMetrics GetFontMetrics(wchar_t* fontFamilyName, bool bold, bool italic);

private:
	IDWriteFactory* _factory = nullptr;
	IDWriteFontCollection* _fontCollection = nullptr;
};