#pragma once
#include "AoCBaseWithWindow.h"

// Windows Runtime Library. Needed for Microsoft::WRL::ComPtr<> template class.
#include <wrl.h>
using namespace Microsoft::WRL;

// D3D12 extension library.
#include "directx/d3dx12.h"


// DirectX 12 specific headers.
#include <d3d12.h>
#include <dxgi1_6.h>
#include <d3dcompiler.h>
#include <DirectXMath.h>

#include <algorithm>
#include <chrono>

#pragma comment(lib, "d3d12.lib")
#pragma comment(lib, "dxgi.lib")
#pragma comment(lib,"d3dcompiler.lib")

inline void ThrowIfFailed(HRESULT hr)
{
    if(FAILED(hr))
    {
        throw std::exception();
    }
}

class AoCBaseWithD3D : public AoCBaseWithWindow
{
public:

protected:

    virtual void OnInitTests() override;
    virtual void OnCloseTests() override;

    void EnableDebugLayer();

    friend class AoCBase;
    friend class AoCBaseWithWindow;
    
protected:
    // The number of swap chain back buffers.
    static const uint8_t g_NumFrames = 3;
    // Use WARP adapter
    bool g_UseWarp = false;
    uint32_t g_ClientWidth = 1280;
    uint32_t g_ClientHeight = 720;
    // Set to true once the DX12 objects have been initialized.
    bool g_IsInitialized = false;



    // DirectX 12 Objects
    ComPtr<ID3D12Device2> g_Device;
    ComPtr<ID3D12CommandQueue> g_CommandQueue;
    ComPtr<IDXGISwapChain4> g_SwapChain;
    ComPtr<ID3D12Resource> g_BackBuffers[g_NumFrames];
    ComPtr<ID3D12GraphicsCommandList> g_CommandList;
    ComPtr<ID3D12CommandAllocator> g_CommandAllocators[g_NumFrames];
    ComPtr<ID3D12DescriptorHeap> g_RTVDescriptorHeap;
    UINT g_RTVDescriptorSize;
    UINT g_CurrentBackBufferIndex;

    // Synchronization objects
    ComPtr<ID3D12Fence> g_Fence;
    uint64_t g_FenceValue = 0;
    uint64_t g_FrameFenceValues[g_NumFrames] = {};
    HANDLE g_FenceEvent;

    // By default, enable V-Sync.
    // Can be toggled with the V key.
    bool g_VSync = true;
    bool g_TearingSupported = false;
    // By default, use windowed mode.
    // Can be toggled with the Alt+Enter or F11
    bool g_Fullscreen = false;


};


