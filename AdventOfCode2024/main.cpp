
#include "AoC_2024_01.h"
#include "AoC_2024_02.h"
#include "AoC_2024_03.h"

int main()
{
    AoCBase::ExecuteSteps<AoC_2024_01>();
    AoCBase::ExecuteSteps<AoC_2024_02>();
    AoCBase::ExecuteSteps<AoC_2024_03>();

    return 0;
}






/** OTHER CODE HERE, FOR LATER */


/*
#pragma once
#define _WIN32_WINNT 0x600


#include <iostream>
#include <fstream>
#include <vector>
#include <sstream>

#include <d3d12.h>
#include <dxgi1_4.h>
#include <D3Dcompiler.h>
#include <DirectXMath.h>
#include <windows.h>
#include <wrl.h>


#include <stdio.h>

#include <d3d11.h>
#include <d3dcompiler.h>

#pragma comment(lib,"d3d11.lib")
#pragma comment(lib,"d3dcompiler.lib")

#pragma comment(lib, "d3d12.lib")
#pragma comment(lib, "dxgi.lib")

using namespace std;
using namespace Microsoft::WRL;



*/


//#define RET_IF_FAILED(hr)    if(hr != S_OK) { cout << std::endl << "Error: " << std::hex << hr << std::endl ; return -1; }
//
//
//#if defined(_DEBUG)
//#include <dxgidebug.h>
//void EnableDebugLayer() {
//    ComPtr<ID3D12Debug> debugController;
//    if(SUCCEEDED(D3D12GetDebugInterface(IID_PPV_ARGS(&debugController))))
//    {
//        debugController->EnableDebugLayer();
//    }
//}
//#endif
//
//
//HRESULT CompileComputeShader(_In_ LPCWSTR srcFile, _In_ LPCSTR entryPoint, _In_ ID3D11Device* device, _Outptr_ ID3DBlob** blob)
//{
//    if(!srcFile || !entryPoint || !device || !blob)
//        return E_INVALIDARG;
//
//    *blob = nullptr;
//    UINT flags = D3DCOMPILE_ENABLE_STRICTNESS;
//#if defined( DEBUG ) || defined( _DEBUG )
//    flags |= D3DCOMPILE_DEBUG;
//#endif
//
//    // We generally prefer to use the higher CS shader profile when possible as CS 5.0 is better performance on 11-class hardware
//    LPCSTR profile = (device->GetFeatureLevel() >= D3D_FEATURE_LEVEL_11_0) ? "cs_5_0" : "cs_4_0";
//    const D3D_SHADER_MACRO defines[] =
//    {
//        "EXAMPLE_DEFINE", "1",
//        NULL, NULL
//    };
//
//    ID3DBlob* shaderBlob = nullptr;
//    ID3DBlob* errorBlob = nullptr;
//    HRESULT hr = D3DCompileFromFile(srcFile, defines, D3D_COMPILE_STANDARD_FILE_INCLUDE, entryPoint, profile, flags, 0, &shaderBlob, &errorBlob);
//    if(FAILED(hr))
//    {
//        if(errorBlob)
//        {
//            OutputDebugStringA((char*)errorBlob->GetBufferPointer());
//            errorBlob->Release();
//        }
//
//        if(shaderBlob)
//            shaderBlob->Release();
//        return hr;
//    }
//    *blob = shaderBlob;
//    return hr;
//}
//
//
//
//
//
//
//
//
//
//
//int TestD3D12()
//{
//#if defined(_DEBUG) 
//    EnableDebugLayer();
//#endif
//
//    // Create Device
//    const D3D_FEATURE_LEVEL lvl[] = { D3D_FEATURE_LEVEL_11_1, D3D_FEATURE_LEVEL_11_0, D3D_FEATURE_LEVEL_10_1, D3D_FEATURE_LEVEL_10_0 };
//
//    UINT createDeviceFlags = 0;
//#ifdef _DEBUG
//    createDeviceFlags |= D3D11_CREATE_DEVICE_DEBUG;
//#endif
//
//    ID3D11Device* device = nullptr;
//    HRESULT hr = D3D11CreateDevice(nullptr, D3D_DRIVER_TYPE_HARDWARE, nullptr, createDeviceFlags, lvl, _countof(lvl), D3D11_SDK_VERSION, &device, nullptr, nullptr);
//    if(hr == E_INVALIDARG)
//    {
//        // DirectX 11.0 Runtime doesn't recognize D3D_FEATURE_LEVEL_11_1 as a valid value
//        hr = D3D11CreateDevice(nullptr, D3D_DRIVER_TYPE_HARDWARE, nullptr, 0, &lvl[1], _countof(lvl) - 1, D3D11_SDK_VERSION, &device, nullptr, nullptr);
//    }
//
//    if(FAILED(hr))
//    {
//        cout << "Failed creating Direct3D 11 device: " << hex << hr << endl;
//        return -1;
//    }
//
//    // Verify compute shader is supported
//    if(device->GetFeatureLevel() < D3D_FEATURE_LEVEL_11_0)
//    {
//        D3D11_FEATURE_DATA_D3D10_X_HARDWARE_OPTIONS hwopts = { 0 };
//        (void)device->CheckFeatureSupport(D3D11_FEATURE_D3D10_X_HARDWARE_OPTIONS, &hwopts, sizeof(hwopts));
//        if(!hwopts.ComputeShaders_Plus_RawAndStructuredBuffers_Via_Shader_4_x)
//        {
//            device->Release();
//            cout << "DirectCompute is not supported by this device" << endl;
//            return -1;
//        }
//    }
//
//    // Compile shader
//    ID3DBlob* csBlob = nullptr;
//    hr = CompileComputeShader(L"ComputeShader.hlsl", "CSMain", device, &csBlob);
//    if(FAILED(hr))
//    {
//        device->Release();
//        cout << "Failed compiling shader: " << hex << hr << endl;
//        return -1;
//    }
//
//    // Create shader
//    ID3D11ComputeShader* computeShader = nullptr;
//    hr = device->CreateComputeShader(csBlob->GetBufferPointer(), csBlob->GetBufferSize(), nullptr, &computeShader);
//
//    csBlob->Release();
//    if(FAILED(hr))
//    {
//        device->Release();
//    }
//
//    cout << "Success" << endl;
//
//    // Clean up
//    computeShader->Release();
//
//    device->Release();
//
//    return 0;
//
//
//}
//
//
//int TestD3D12_2()
//{
//#if defined(_DEBUG) 
//    EnableDebugLayer();
//#endif
//
//    // Create D3D12 device
//    ComPtr<ID3D12Device> device;
//    D3D12CreateDevice(nullptr, D3D_FEATURE_LEVEL_11_0, IID_PPV_ARGS(&device));
//
//    // Create command queue
//    D3D12_COMMAND_QUEUE_DESC queueDesc = {};
//    queueDesc.Type = D3D12_COMMAND_LIST_TYPE_DIRECT;
//    queueDesc.Flags = D3D12_COMMAND_QUEUE_FLAG_NONE;
//    ComPtr<ID3D12CommandQueue> commandQueue;
//    device->CreateCommandQueue(&queueDesc, IID_PPV_ARGS(&commandQueue));
//
//    // Load and compile the compute shader
//    ComPtr<ID3DBlob> computeShader;
//    D3DCompileFromFile(L"ComputeShader.hlsl", nullptr, nullptr, "CSMain", "cs_5_0", 0, 0, &computeShader, nullptr);
//
//    // Create root signature
//    D3D12_ROOT_SIGNATURE_DESC rootSignatureDesc = {};
//    rootSignatureDesc.Flags = D3D12_ROOT_SIGNATURE_FLAG_ALLOW_INPUT_ASSEMBLER_INPUT_LAYOUT;
//    ComPtr<ID3DBlob> signatureBlob;
//    ComPtr<ID3DBlob> errorBlob;
//    D3D12SerializeRootSignature(&rootSignatureDesc, D3D_ROOT_SIGNATURE_VERSION_1, &signatureBlob, &errorBlob);
//    ComPtr<ID3D12RootSignature> rootSignature;
//    device->CreateRootSignature(0, signatureBlob->GetBufferPointer(), signatureBlob->GetBufferSize(), IID_PPV_ARGS(&rootSignature));
//
//    // Create pipeline state
//    D3D12_COMPUTE_PIPELINE_STATE_DESC pipelineStateDesc = {};
//    pipelineStateDesc.pRootSignature = rootSignature.Get();
//    pipelineStateDesc.CS = { computeShader->GetBufferPointer(), computeShader->GetBufferSize() };
//    ComPtr<ID3D12PipelineState> pipelineState;
//    device->CreateComputePipelineState(&pipelineStateDesc, IID_PPV_ARGS(&pipelineState));
//
//    // Create command allocator and command list
//    ComPtr<ID3D12CommandAllocator> commandAllocator;
//    device->CreateCommandAllocator(D3D12_COMMAND_LIST_TYPE_DIRECT, IID_PPV_ARGS(&commandAllocator));
//    ComPtr<ID3D12GraphicsCommandList> commandList;
//    device->CreateCommandList(0, D3D12_COMMAND_LIST_TYPE_DIRECT, commandAllocator.Get(), pipelineState.Get(), IID_PPV_ARGS(&commandList));
//
//    // Set the compute shader pipeline state
//    commandList->SetPipelineState(pipelineState.Get());
//    commandList->SetComputeRootSignature(rootSignature.Get());
//
//    // Dispatch the compute shader
//    commandList->Dispatch(1, 1, 1);
//    commandList->Close();
//
//    // Execute the command list
//    ID3D12CommandQueue* commandQueuePtr = commandQueue.Get();
//    ID3D12CommandList* commandLists[] = { commandList.Get() };
//    commandQueuePtr->ExecuteCommandLists(_countof(commandLists), commandLists);
//
//    // Wait for GPU to finish execution
//    ComPtr<ID3D12Fence> fence;
//    device->CreateFence(0, D3D12_FENCE_FLAG_NONE, IID_PPV_ARGS(&fence));
//    HANDLE eventHandle = CreateEventEx(nullptr, nullptr, 0, EVENT_ALL_ACCESS);
//    commandQueuePtr->Signal(fence.Get(), 1);
//    fence->SetEventOnCompletion(1, eventHandle);
//    WaitForSingleObject(eventHandle, INFINITE);
//    CloseHandle(eventHandle);
//
//    return 0;
//
//}
//
//
//
