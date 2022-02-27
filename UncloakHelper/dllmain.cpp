// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include <servprov.h>
#include <dwmapi.h>
#pragma comment(lib, "Dwmapi.lib")

#include <shobjidl_core.h>

MIDL_INTERFACE("b706dded-208c-4795-b610-e7c002c31edc")
IUncloakWindowService : public IUnknown
{
public:
	virtual HRESULT STDMETHODCALLTYPE UncloakWindow(HWND hWnd) = 0;

};

HMODULE currentModule;

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	currentModule = hModule;
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

PINVOKE void UnCloakWindowShell(HWND hWnd) {
	HRESULT hres = 0;
	IServiceProvider* serviceProvider;
	CLSID CLSID_ImmersiveShell = { 0xc2f03a33,0x21f5,0x47fa,{0xb4,0xbb,0x15,0x63,0x62,0xa2,0xf2,0x39} };
	hres = CoCreateInstance(CLSID_ImmersiveShell, nullptr, CLSCTX_ALL, __uuidof(IServiceProvider), (void**)&serviceProvider);
	if (hres == 0) {
		IUnknown* immersiveApplication;
		IID IID_IImmersiveApplicationManager = { 0xbf63999f,0x7411,0x40da,{0x86,0x1c,0xdf,0x72,0xc0,0xff,0xee,0x84} };
		hres = serviceProvider->QueryService(IID_IImmersiveApplicationManager, IID_IImmersiveApplicationManager, (void**)&immersiveApplication);
		if (hres == 0) {
			IUncloakWindowService* uncloakService;
			hres = immersiveApplication->QueryInterface(&uncloakService);
			if (hres == 0) {
				hres = uncloakService->UncloakWindow(hWnd);
			}
		}
	}
	ExitThread(hres);
}

PINVOKE void UnloadLib() {
	FreeLibrary(currentModule);
}