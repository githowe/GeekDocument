#pragma once

#pragma region ��׼��

#include <stdint.h>
#include <stdio.h>
#include <iostream>
#include <cassert>
#include <vector>

using namespace std;

#pragma endregion

#pragma region ϵͳ��

#include <Windows.h>

#pragma endregion

#pragma region Webp��

extern "C"
{
#include <decode.h>
#include <types.h>
#include <mux_types.h>
#include <demux.h>
#include <mux.h>
}

#pragma comment(lib, "libsharpyuv_dll.lib")
#pragma comment(lib, "libwebp_dll.lib")
#pragma comment(lib, "libwebpdecoder_dll.lib")
#pragma comment(lib, "libwebpdemux_dll.lib")

#pragma endregion

#define dll_export extern "C" __declspec(dllexport)

/// <summary>
/// δ�����ͼƬ�ļ�����
/// </summary>
typedef struct ImageFileData
{
	uint8_t* data;
	size_t size;
} ImageFileData;

/// <summary>
/// �ѽ���� Webp ֡
/// </summary>
typedef struct WebpFrame
{
	uint8_t* data;
	int timestamp;
} WebpFrame;