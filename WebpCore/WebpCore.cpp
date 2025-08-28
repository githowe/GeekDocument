#include "WebpCore.h"

void* CreateWebpReader() { return new WebpReader(); }

int LoadImageFile(WebpReader* reader, uint8_t* sourceData, int size)
{
	ImageFileData fileData = { sourceData, size };
	return reader->LoadFile(fileData);
}

int GetImageWidth(WebpReader* reader) { return reader->ImageWidth; }

int GetImageHeight(WebpReader* reader) { return reader->ImageHeight; }

int GetFrameCount(WebpReader* reader) { return reader->FrameList.size(); }

void* GetFrame(WebpReader* reader) { return reader->GetFrame(); }

void ClearFrame(WebpReader* reader) { reader->ClearFrame(); }

void* GetFrameData(WebpFrame* frame) { return frame->data; }

int GetFrameTimestamp(WebpFrame* frame) { return frame->timestamp; }