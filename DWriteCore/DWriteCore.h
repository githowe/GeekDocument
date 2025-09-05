#pragma once

#include "Level_01.h"

dll_export void* CreateDWriteTool();

dll_export FontMetrics GetFontMetrics(DWriteTool* tool, wchar_t* fontFamilyName, bool bold, bool italic);

dll_export void ReleaseDWriteTool(DWriteTool* tool);