#include "WebpCore.h"

void* CreateWebpReader() { return new WebpReader(); }

int LoadImageFile(WebpReader* reader, const wchar_t* filePath) { return reader->LoadFile(filePath); }

int GetImageWidth(WebpReader* reader) { return reader->ImageWidth; }

int GetImageHeight(WebpReader* reader) { return reader->ImageHeight; }

int GetFrameCount(WebpReader* reader) { return reader->FrameList.size(); }

void* GetFrame(WebpReader* reader) { return reader->GetFrame(); }

void ClearFrame(WebpReader* reader) { reader->ClearFrame(); }

void* GetFrameData(WebpFrame* frame) { return frame->data; }

int GetFrameTimestamp(WebpFrame* frame) { return frame->timestamp; }