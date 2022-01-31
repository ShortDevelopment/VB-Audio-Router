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

static GUID CurrentDesktop = { 0x9a132c93,0x45e2,0x4cad,{0x98,0xec,0x7c,0xcd,0x6b,0xc3,0xaa,0x10} };
PINVOKE void SetCurrentDesktop(REFGUID desktopId) {
	CurrentDesktop = desktopId;
	ExitThread(0);
}

const CLSID CLSID_VirtualDesktopManager = { 0xaa509086, 0x5ca9, 0x4c25, { 0x8f, 0x95, 0x58, 0x9d, 0x3c, 0x07, 0xb4, 0x8a } };
const CLSID CLSID_ImmersiveShell = {
	0xC2F03A33, 0x21F5, 0x47FA, 0xB4, 0xBB, 0x15, 0x63, 0x62, 0xA2, 0xF2, 0x39 };

PINVOKE void MoveWindowToCurrentDesktop(HWND hWnd) {
	HRESULT hRes;

	IServiceProvider* pServiceProvider = NULL;
	HRESULT hr = ::CoCreateInstance(CLSID_ImmersiveShell, NULL, CLSCTX_LOCAL_SERVER, __uuidof(IServiceProvider), (PVOID*)&pServiceProvider);

	if (SUCCEEDED(hr))
	{
		IVirtualDesktopManager* pDesktopManager = NULL;
		hr = pServiceProvider->QueryService(__uuidof(IVirtualDesktopManager), &pDesktopManager);

		if (SUCCEEDED(hr))
		{
			hr = pDesktopManager->MoveWindowToDesktop(hWnd, CurrentDesktop);
			pDesktopManager->Release();
		}

		pServiceProvider->Release();
	}
	DisplayNumber(hr);
	ExitThread(hr);
}