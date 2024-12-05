#include "AoCBaseWithD3D.h"



void AoCBaseWithD3D::OnInitTests()
{
    EnableDebugLayer();
    RegisterWindowClass(::GetModuleHandle(NULL), TEXT("AoCWindowClass"));
    CreateWindow(TEXT("AoCWindowClass"), GetModuleHandle(NULL), TEXT("AoC DirectX12"), 1024, 768);
    ShowWindow(hWnd, SW_SHOW);
}

void AoCBaseWithD3D::OnCloseTests()
{
    ::CloseWindow(hWnd);
}

void AoCBaseWithD3D::EnableDebugLayer()
{
#if defined(_DEBUG)
    // Always enable the debug layer before doing anything DX12 related
    // so all possible errors generated while creating DX12 objects
    // are caught by the debug layer.
    ComPtr<ID3D12Debug> debugInterface;
    ThrowIfFailed(D3D12GetDebugInterface(IID_PPV_ARGS(&debugInterface)));
    debugInterface->EnableDebugLayer();
#endif
}

