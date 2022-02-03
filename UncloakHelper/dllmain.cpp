// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include <dwmapi.h>
#pragma comment(lib, "Dwmapi.lib")

#include <shobjidl_core.h>

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	CoInitialize(NULL);
	return TRUE;
}

#define PINVOKE extern "C" __declspec(dllexport)

PINVOKE HRESULT DisplayNumber(int num) {
	wchar_t buffer[256];
	wsprintfW(buffer, L"%d", num);
	MessageBoxW(nullptr, buffer, L"Information", MB_OK);

	return 0;
}

PINVOKE void CloakWindow(HWND hWnd) {
	BOOL value = true;
	HRESULT hRes = DwmSetWindowAttribute(hWnd, DWMWA_CLOAK, &value, sizeof(value));
	ExitThread(hRes);
}

PINVOKE void UnCloakWindow(HWND hWnd) {
	BOOL value = false;
	HRESULT hRes = DwmSetWindowAttribute(hWnd, DWMWA_CLOAK, &value, sizeof(value));
	ExitThread(hRes);
}

PINVOKE void Test() {
	PSRWLOCK lock;
	InitializeSRWLock(lock);
	AcquireSRWLockExclusive(lock);

	// ReleaseSRWLockExclusive(lock);
}