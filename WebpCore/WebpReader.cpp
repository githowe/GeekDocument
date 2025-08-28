#include "WebpReader.h"

WebpReader::WebpReader()
{
	_frameList_it = FrameList.begin();
}

int WebpReader::LoadFile(ImageFileData fileData)
{
	if (fileData.data == nullptr || fileData.size <= 0) return -1;
	// 读取图片信息
	WebPBitstreamFeatures webpInfo;
	VP8StatusCode status = WebPGetFeatures(fileData.data, fileData.size, &webpInfo);
	if (status != VP8_STATUS_OK) return -2;
	// 设置图片大小
	ImageWidth = webpInfo.width;
	ImageHeight = webpInfo.height;
	// 解码
	int decodeResult = 0;
	if (!webpInfo.has_animation)decodeResult = DecodeStaticFrame(&fileData);
	else decodeResult = DecodeAnimationFrame(&fileData);
	// 更新迭代器
	_frameList_it = FrameList.begin();

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