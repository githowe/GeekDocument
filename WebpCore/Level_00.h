#pragma once

#pragma region 标准库

#include <stdint.h>
#include <stdio.h>
#include <iostream>
#include <cassert>
#include <vector>

using namespace std;

#pragma endregion

#pragma region 系统库

#include <Windows.h>

#pragma endregion

#pragma region Webp库

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
/// 未解码的图片文件数据
/// </summary>
typedef struct ImageFileData
{
	uint8_t* data;
	size_t size;
} ImageFileData;

/// <summary>
/// 已解码的 Webp 帧
/// </summary>
typedef struct WebpFrame
{
	uint8_t* data;
	int timestamp;
} WebpFrame;