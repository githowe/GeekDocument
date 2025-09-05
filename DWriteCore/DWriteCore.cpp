#include "DWriteCore.h"

void* CreateDWriteTool()
{
	DWriteTool* tool = new DWriteTool();
	tool->Init();
	return tool;
}

FontMetrics GetFontMetrics(DWriteTool* tool, wchar_t* fontFamilyName, bool bold, bool italic)
{
	return tool->GetFontMetrics(fontFamilyName, bold, italic);
}

void ReleaseDWriteTool(DWriteTool* tool)
{
	tool->Release();
}
