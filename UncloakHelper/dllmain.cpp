// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include <dwmapi.h>
#pragma comment(lib, "Dwmapi.lib");

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	return TRUE;
}

void DisplayNumber(int num) {
	wchar_t buffer[256];
	wsprintfW(buffer, L"%d", num);
	MessageBoxW(nullptr, buffer, L"Information", MB_OK);
}

#define PINVOKE extern "C" __declspec(dllexport)

PINVOKE HRESULT UncloakWindow(HWND hWnd) {
	MessageBoxA(nullptr, "Called uncload", "caption", 0);

	BOOL value = false;
	HRESULT hRes = DwmSetWindowAttribute(hWnd, DWMWA_CLOAK, &value, sizeof(value));
	DisplayNumber(hRes);
	return hRes;
}