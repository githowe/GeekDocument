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
	/// �����ļ��������ļ����ݲ�����Ϊ֡�б�
	/// </summary>
	int LoadFile(const wchar_t* filePath);

	/// <summary>
	/// ��ȡ֡
	/// </summary>
	WebpFrame* GetFrame();

	/// <summary>
	/// ����֡
	/// </summary>
	void ClearFrame();

private:
	/// <summary>
	/// ����ͼƬ�ļ�
	/// </summary>
	ImageFileData LoadImageFile(const wchar_t* filePath);

	/// <summary>
	/// ���뾲̬֡
	/// </summary>
	int DecodeStaticFrame(ImageFileData* fileData);

	/// <summary>
	/// ���붯��֡
	/// </summary>
	int DecodeAnimationFrame(ImageFileData* fileData);

private:
	vector<WebpFrame>::iterator _frameList_it;
};