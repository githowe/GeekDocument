#include "DWriteTool.h"

void DWriteTool::Init()
{
	// 创建工厂
	DWriteCreateFactory(DWRITE_FACTORY_TYPE_SHARED, __uuidof(IDWriteFactory), reinterpret_cast<IUnknown**>(&_factory));
	// 获取系统字体集合
	_factory->GetSystemFontCollection(&_fontCollection);
}

void DWriteTool::Release()
{
	if (_fontCollection)
	{
		_fontCollection->Release();
		_fontCollection = nullptr;
	}
	if (_factory)
	{
		_factory->Release();
		_factory = nullptr;
	}
}

FontMetrics DWriteTool::GetFontMetrics(wchar_t* fontFamilyName, bool bold, bool italic)
{
	// 从字体集合中查找指定字体的索引
	UINT32 index = 0;
	BOOL 存在 = FALSE;
	_fontCollection->FindFamilyName(fontFamilyName, &index, &存在);
	// 获取字体族
	IDWriteFontFamily* fontFamily = nullptr;
	_fontCollection->GetFontFamily(index, &fontFamily);
	// 获取第一个匹配的字体
	IDWriteFont* font = nullptr;
	DWRITE_FONT_WEIGHT 字重 = DWRITE_FONT_WEIGHT_NORMAL;
	if (bold) 字重 = DWRITE_FONT_WEIGHT_BOLD;
	DWRITE_FONT_STRETCH 拉伸 = DWRITE_FONT_STRETCH_NORMAL;
	DWRITE_FONT_STYLE 样式 = DWRITE_FONT_STYLE_NORMAL;
	if (italic) 样式 = DWRITE_FONT_STYLE_ITALIC;
	fontFamily->GetFirstMatchingFont(字重, 拉伸, 样式, &font);
	// 获取字体面
	IDWriteFontFace* fontFace = nullptr;
	font->CreateFontFace(&fontFace);

	DWRITE_FONT_METRICS meterics;
	fontFace->GetMetrics(&meterics);

	const UINT32 标签 = DWRITE_MAKE_OPENTYPE_TAG('O', 'S', '/', '2');
	const void* 信息表 = nullptr;
	UINT32 表大小 = 0;
	void* 表上下文 = nullptr;
	BOOL 存在表 = FALSE;
	fontFace->TryGetFontTable(标签, &信息表, &表大小, &表上下文, &存在表);

	FontMetrics result = {};
	const BYTE* byteArray = reinterpret_cast<const BYTE*>(信息表);
	result.UnitsPerEm = meterics.designUnitsPerEm;
	result.TypoAscender = (byteArray[68] << 8) | byteArray[69];
	result.TypoDescender = (byteArray[70] << 8) | byteArray[71];

	fontFace->Release();
	font->Release();
	fontFamily->Release();

	return result;
}
