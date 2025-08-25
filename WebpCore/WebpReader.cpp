#include "WebpReader.h"

WebpReader::WebpReader()
{
	_frameList_it = FrameList.begin();
}

int WebpReader::LoadFile(const wchar_t* filePath)
{
	// ����Դ����
	ImageFileData sourceData = LoadImageFile(filePath);
	if (sourceData.data == nullptr || sourceData.size <= 0) return -1;
	// ��ȡͼƬ��Ϣ
	WebPBitstreamFeatures webpInfo;
	VP8StatusCode status = WebPGetFeatures(sourceData.data, sourceData.size, &webpInfo);
	if (status != VP8_STATUS_OK) return -2;
	// ����ͼƬ��С
	ImageWidth = webpInfo.width;
	ImageHeight = webpInfo.height;
	// ����
	int decodeResult = 0;
	if (!webpInfo.has_animation)decodeResult = DecodeStaticFrame(&sourceData);
	else decodeResult = DecodeAnimationFrame(&sourceData);
	// ���µ�����
	_frameList_it = FrameList.begin();
	// ����Դ����
	free(sourceData.data);

	if (decodeResult == -1) return -3;
	return 0;
}

WebpFrame* WebpReader::GetFrame()
{
	if (_frameList_it != FrameList.end())
	{
		// ��ȡ������ָ���Ԫ��
		WebpFrame* result = &(*_frameList_it);
		// ���Ƶ�����
		_frameList_it++;
		// ����Ԫ��
		return result;
	}
	return nullptr;
}

void WebpReader::ClearFrame()
{
	// ���õ�����
	_frameList_it = FrameList.begin();
	// ����֡
	while (_frameList_it != FrameList.end())
	{
		// �ͷ�֡����
		WebpFrame* frame = &(*_frameList_it);
		free((void*)frame->data);
		// ���Ƶ�����
		_frameList_it++;
	}
	// ���Ԫ��
	FrameList.clear();
	_frameList_it = FrameList.begin();
}

ImageFileData WebpReader::LoadImageFile(const wchar_t* filePath)
{
	ImageFileData result = { nullptr, 0 };

	// ʹ�ö�����ģʽ���ļ�
	FILE* file;
	_wfopen_s(&file, filePath, L"rb");
	if (file == nullptr)
	{
		cout << "���ļ�ʧ��: " << filePath << endl;
		return result;
	}

	// �ƶ��ļ�ָ����ĩβ���Ի�ȡ�ļ���С
	fseek(file, 0, SEEK_END);
	size_t fileSize = ftell(file);
	// �����ļ�����
	uint8_t* fileData = (uint8_t*)malloc(fileSize + 1);
	if (fileData == nullptr)
	{
		cout << "�����ļ�����ʧ�ܣ������ڴ�ʧ��" << endl;
		fclose(file);
		return result;
	}

	// ���ļ�ָ���ƶ�����ͷ���Զ�ȡ�ļ�����
	fseek(file, 0, SEEK_SET);
	fread(fileData, fileSize, 1, file);
	// ��ȡ��ɺ�ر��ļ�
	fclose(file);

	// ����������ֹ��
	fileData[fileSize] = '\0';
	// ���ý��
	result.data = fileData;
	result.size = fileSize;

	return result;
}

int WebpReader::DecodeStaticFrame(ImageFileData* fileData)
{
	// ��������ʼ������������
	WebPDecoderConfig config;
	WebPInitDecoderConfig(&config);
	config.output.colorspace = MODE_RGBA;
	// ���룬�������洢�� config.output.u.RGBA.rgba ��
	VP8StatusCode code = WebPDecode(fileData->data, fileData->size, &config);
	if (code != VP8_STATUS_OK) return -1;

	// ����֡��֡����
	WebpFrame frame = { 0 };
	int frameSize = ImageWidth * ImageHeight * 4;
	frame.data = (uint8_t*)malloc(frameSize);
	if (frame.data == nullptr) return -2;
	// ����֡����
	memcpy(frame.data, config.output.u.RGBA.rgba, frameSize);
	// ���֡
	FrameList.push_back(frame);

	// �ͷŽ��������
	WebPFree(config.output.u.RGBA.rgba);

	return 0;
}

int WebpReader::DecodeAnimationFrame(ImageFileData* fileData)
{
	// ��������ʼ�� Webp ����
	WebPData webpData;
	WebPDataInit(&webpData);
	webpData.bytes = fileData->data;
	webpData.size = fileData->size;

	// ��������������
	WebPAnimDecoder* decoder = WebPAnimDecoderNew(&webpData, nullptr);
	// ��ȡ������Ϣ
	WebPAnimInfo animationInfo;
	WebPAnimDecoderGetInfo(decoder, &animationInfo);

	// ��ʼ����
	int frameSize = ImageWidth * ImageHeight * 4;
	uint8_t* tempData;
	while (WebPAnimDecoderHasMoreFrames(decoder))
	{
		// ����֡
		WebpFrame frame = { 0 };
		frame.data = (uint8_t*)malloc(frameSize);
		if (frame.data == nullptr) break;
		// ����
		WebPAnimDecoderGetNext(decoder, &tempData, &frame.timestamp);
		// ����֡����
		memcpy(frame.data, tempData, frameSize);
		// ���֡
		FrameList.push_back(frame);
	}
	// �ͷŶ���������
	WebPAnimDecoderDelete(decoder);

	// ����δ���
	if (FrameList.size() < animationInfo.frame_count) return -1;

	return 0;
}