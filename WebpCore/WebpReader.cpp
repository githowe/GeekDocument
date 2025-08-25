#include "WebpReader.h"

WebpReader::WebpReader()
{
	_frameList_it = FrameList.begin();
}

int WebpReader::LoadFile(const wchar_t* filePath)
{
	// 加载源数据
	ImageFileData sourceData = LoadImageFile(filePath);
	if (sourceData.data == nullptr || sourceData.size <= 0) return -1;
	// 读取图片信息
	WebPBitstreamFeatures webpInfo;
	VP8StatusCode status = WebPGetFeatures(sourceData.data, sourceData.size, &webpInfo);
	if (status != VP8_STATUS_OK) return -2;
	// 设置图片大小
	ImageWidth = webpInfo.width;
	ImageHeight = webpInfo.height;
	// 解码
	int decodeResult = 0;
	if (!webpInfo.has_animation)decodeResult = DecodeStaticFrame(&sourceData);
	else decodeResult = DecodeAnimationFrame(&sourceData);
	// 更新迭代器
	_frameList_it = FrameList.begin();
	// 清理源数据
	free(sourceData.data);

	if (decodeResult == -1) return -3;
	return 0;
}

WebpFrame* WebpReader::GetFrame()
{
	if (_frameList_it != FrameList.end())
	{
		// 获取迭代器指向的元素
		WebpFrame* result = &(*_frameList_it);
		// 后移迭代器
		_frameList_it++;
		// 返回元素
		return result;
	}
	return nullptr;
}

void WebpReader::ClearFrame()
{
	// 重置迭代器
	_frameList_it = FrameList.begin();
	// 遍历帧
	while (_frameList_it != FrameList.end())
	{
		// 释放帧数据
		WebpFrame* frame = &(*_frameList_it);
		free((void*)frame->data);
		// 后移迭代器
		_frameList_it++;
	}
	// 清空元素
	FrameList.clear();
	_frameList_it = FrameList.begin();
}

ImageFileData WebpReader::LoadImageFile(const wchar_t* filePath)
{
	ImageFileData result = { nullptr, 0 };

	// 使用二进制模式打开文件
	FILE* file;
	_wfopen_s(&file, filePath, L"rb");
	if (file == nullptr)
	{
		cout << "打开文件失败: " << filePath << endl;
		return result;
	}

	// 移动文件指针至末尾，以获取文件大小
	fseek(file, 0, SEEK_END);
	size_t fileSize = ftell(file);
	// 创建文件数据
	uint8_t* fileData = (uint8_t*)malloc(fileSize + 1);
	if (fileData == nullptr)
	{
		cout << "加载文件数据失败：分配内存失败" << endl;
		fclose(file);
		return result;
	}

	// 将文件指针移动至开头，以读取文件内容
	fseek(file, 0, SEEK_SET);
	fread(fileData, fileSize, 1, file);
	// 读取完成后关闭文件
	fclose(file);

	// 设置数据终止符
	fileData[fileSize] = '\0';
	// 设置结果
	result.data = fileData;
	result.size = fileSize;

	return result;
}

int WebpReader::DecodeStaticFrame(ImageFileData* fileData)
{
	// 创建并初始化解码器配置
	WebPDecoderConfig config;
	WebPInitDecoderConfig(&config);
	config.output.colorspace = MODE_RGBA;
	// 解码，解码结果存储在 config.output.u.RGBA.rgba 中
	VP8StatusCode code = WebPDecode(fileData->data, fileData->size, &config);
	if (code != VP8_STATUS_OK) return -1;

	// 创建帧与帧数据
	WebpFrame frame = { 0 };
	int frameSize = ImageWidth * ImageHeight * 4;
	frame.data = (uint8_t*)malloc(frameSize);
	if (frame.data == nullptr) return -2;
	// 复制帧数据
	memcpy(frame.data, config.output.u.RGBA.rgba, frameSize);
	// 添加帧
	FrameList.push_back(frame);

	// 释放解码器输出
	WebPFree(config.output.u.RGBA.rgba);

	return 0;
}

int WebpReader::DecodeAnimationFrame(ImageFileData* fileData)
{
	// 创建并初始化 Webp 数据
	WebPData webpData;
	WebPDataInit(&webpData);
	webpData.bytes = fileData->data;
	webpData.size = fileData->size;

	// 创建动画解码器
	WebPAnimDecoder* decoder = WebPAnimDecoderNew(&webpData, nullptr);
	// 读取动画信息
	WebPAnimInfo animationInfo;
	WebPAnimDecoderGetInfo(decoder, &animationInfo);

	// 开始解码
	int frameSize = ImageWidth * ImageHeight * 4;
	uint8_t* tempData;
	while (WebPAnimDecoderHasMoreFrames(decoder))
	{
		// 创建帧
		WebpFrame frame = { 0 };
		frame.data = (uint8_t*)malloc(frameSize);
		if (frame.data == nullptr) break;
		// 解码
		WebPAnimDecoderGetNext(decoder, &tempData, &frame.timestamp);
		// 复制帧数据
		memcpy(frame.data, tempData, frameSize);
		// 添加帧
		FrameList.push_back(frame);
	}
	// 释放动画解码器
	WebPAnimDecoderDelete(decoder);

	// 解码未完成
	if (FrameList.size() < animationInfo.frame_count) return -1;

	return 0;
}