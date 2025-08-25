#pragma once

#include "Level_01.h"

dll_export void* CreateWebpReader();

dll_export int LoadImageFile(WebpReader* reader, const wchar_t* filePath);

dll_export int GetImageWidth(WebpReader* reader);

dll_export int GetImageHeight(WebpReader* reader);

dll_export int GetFrameCount(WebpReader* reader);

dll_export void* GetFrame(WebpReader* reader);

dll_export void ClearFrame(WebpReader* reader);

dll_export void* GetFrameData(WebpFrame* frame);

dll_export int GetFrameTimestamp(WebpFrame* frame);