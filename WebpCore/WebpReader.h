#pragma once

#include "Level_00.h"

class WebpReader
{
public:
	WebpReader();

public:
	int ImageWidth = 0;
	int ImageHeight = 0;
	vector<WebpFrame> FrameList;

public:
	/// <summary>
	/// 加载文件：加载文件数据并解码为帧列表
	/// </summary>
	int LoadFile(const wchar_t* filePath);

	/// <summary>
	/// 获取帧
	/// </summary>
	WebpFrame* GetFrame();

	/// <summary>
	/// 清理帧
	/// </summary>
	void ClearFrame();

private:
	/// <summary>
	/// 加载图片文件
	/// </summary>
	ImageFileData LoadImageFile(const wchar_t* filePath);

	/// <summary>
	/// 解码静态帧
	/// </summary>
	int DecodeStaticFrame(ImageFileData* fileData);

	/// <summary>
	/// 解码动画帧
	/// </summary>
	int DecodeAnimationFrame(ImageFileData* fileData);

private:
	vector<WebpFrame>::iterator _frameList_it;
};