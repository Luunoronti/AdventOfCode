// AMD SampleDX12 sample code
// 
// Copyright(c) 2020 Advanced Micro Devices, Inc.All rights reserved.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#include "Renderer.h"
#include "UI.h"
#include <Tracy.hpp>

#include <stdlib.h>

//--------------------------------------------------------------------------------------
//
// OnCreate
//
//--------------------------------------------------------------------------------------
void Renderer::OnCreate(Device* pDevice, SwapChain *pSwapChain, float FontSize)
{
    m_pDevice = pDevice;

    // Initialize helpers

    // Create all the heaps for the resources views
    const uint32_t cbvDescriptorCount = 4000;
    const uint32_t srvDescriptorCount = 8000;
    const uint32_t uavDescriptorCount = 10;
    const uint32_t dsvDescriptorCount = 10;
    const uint32_t rtvDescriptorCount = 60;
    const uint32_t samplerDescriptorCount = 20;
    m_ResourceViewHeaps.OnCreate(pDevice, cbvDescriptorCount, srvDescriptorCount, uavDescriptorCount, dsvDescriptorCount, rtvDescriptorCount, samplerDescriptorCount);

    // Create a commandlist ring for the Direct queue
    uint32_t commandListsPerBackBuffer = 8;
    m_CommandListRing.OnCreate(pDevice, backBufferCount, commandListsPerBackBuffer, pDevice->GetGraphicsQueue()->GetDesc());

    // Create a 'dynamic' constant buffer
    const uint32_t constantBuffersMemSize = 200 * 1024 * 1024;
    m_ConstantBufferRing.OnCreate(pDevice, backBufferCount, constantBuffersMemSize, &m_ResourceViewHeaps);

    // Create a 'static' pool for vertices, indices and constant buffers
    const uint32_t staticGeometryMemSize = (5 * 128) * 1024 * 1024;
    m_VidMemBufferPool.OnCreate(pDevice, staticGeometryMemSize, true, "StaticGeom");

    // initialize the GPU time stamps module
    m_GPUTimer.OnCreate(pDevice, backBufferCount);

    // Quick helper to upload resources, it has it's own commandList and uses suballocation.
    const uint32_t uploadHeapMemSize = 1000 * 1024 * 1024;
    m_UploadHeap.OnCreate(pDevice, uploadHeapMemSize);    // initialize an upload heap (uses suballocation for faster results)

    // Create GBuffer and render passes
    //
    {
        m_GBuffer.OnCreate(
            pDevice,
            &m_ResourceViewHeaps,
            {
                { GBUFFER_DEPTH, DXGI_FORMAT_D32_FLOAT},
                { GBUFFER_FORWARD, DXGI_FORMAT_R16G16B16A16_FLOAT},
                { GBUFFER_MOTION_VECTORS, DXGI_FORMAT_R16G16_FLOAT},
            },
            1
        );

        GBufferFlags fullGBuffer = GBUFFER_DEPTH | GBUFFER_FORWARD | GBUFFER_MOTION_VECTORS;
        m_RenderPassFullGBuffer.OnCreate(&m_GBuffer, fullGBuffer);
        m_RenderPassJustDepthAndHdr.OnCreate(&m_GBuffer, GBUFFER_DEPTH | GBUFFER_FORWARD);
    }

#if USE_SHADOWMASK    
    m_shadowResolve.OnCreate(m_pDevice, &m_ResourceViewHeaps, &m_ConstantBufferRing);

    // Create the shadow mask descriptors
    m_ResourceViewHeaps.AllocCBV_SRV_UAVDescriptor(1, &m_ShadowMaskUAV);
    m_ResourceViewHeaps.AllocCBV_SRV_UAVDescriptor(1, &m_ShadowMaskSRV);
#endif

    m_SkyDome.OnCreate(pDevice, &m_UploadHeap, &m_ResourceViewHeaps, &m_ConstantBufferRing, &m_VidMemBufferPool, "..\\AoC_DX12_V\\media\\cauldron-media\\envmaps\\papermill\\diffuse.dds", "..\\AoC_DX12_V\\media\\cauldron-media\\envmaps\\papermill\\specular.dds", DXGI_FORMAT_R16G16B16A16_FLOAT, 4);
    m_SkyDomeProc.OnCreate(pDevice, &m_ResourceViewHeaps, &m_ConstantBufferRing, &m_VidMemBufferPool, DXGI_FORMAT_R16G16B16A16_FLOAT, 1);
    m_Wireframe.OnCreate(pDevice, &m_ResourceViewHeaps, &m_ConstantBufferRing, &m_VidMemBufferPool, DXGI_FORMAT_R16G16B16A16_FLOAT, 1);
    m_WireframeBox.OnCreate(pDevice, &m_ResourceViewHeaps, &m_ConstantBufferRing, &m_VidMemBufferPool);
    m_DownSample.OnCreate(pDevice, &m_ResourceViewHeaps, &m_ConstantBufferRing, &m_VidMemBufferPool, DXGI_FORMAT_R16G16B16A16_FLOAT);
    m_Bloom.OnCreate(pDevice, &m_ResourceViewHeaps, &m_ConstantBufferRing, &m_VidMemBufferPool, DXGI_FORMAT_R16G16B16A16_FLOAT);
    m_TAA.OnCreate(pDevice, &m_ResourceViewHeaps, &m_VidMemBufferPool);
    m_MagnifierPS.OnCreate(pDevice, &m_ResourceViewHeaps, &m_ConstantBufferRing, &m_VidMemBufferPool, DXGI_FORMAT_R16G16B16A16_FLOAT);

    // Create tonemapping pass
    m_ToneMappingPS.OnCreate(pDevice, &m_ResourceViewHeaps, &m_ConstantBufferRing, &m_VidMemBufferPool, pSwapChain->GetFormat());
    m_ToneMappingCS.OnCreate(pDevice, &m_ResourceViewHeaps, &m_ConstantBufferRing);
    m_ColorConversionPS.OnCreate(pDevice, &m_ResourceViewHeaps, &m_ConstantBufferRing, &m_VidMemBufferPool, pSwapChain->GetFormat());

    // Initialize UI rendering resources
    m_ImGUI.OnCreate(pDevice, &m_UploadHeap, &m_ResourceViewHeaps, &m_ConstantBufferRing, pSwapChain->GetFormat(), FontSize);

    // Make sure upload heap has finished uploading before continuing
    m_VidMemBufferPool.UploadData(m_UploadHeap.GetCommandList());
    m_UploadHeap.FlushAndFinish();
}

//--------------------------------------------------------------------------------------
//
// OnDestroy
//
//--------------------------------------------------------------------------------------
void Renderer::OnDestroy()
{
    m_AsyncPool.Flush();

    m_ImGUI.OnDestroy();
    m_ColorConversionPS.OnDestroy();
    m_ToneMappingCS.OnDestroy();
    m_ToneMappingPS.OnDestroy();
    m_TAA.OnDestroy();
    m_Bloom.OnDestroy();
    m_DownSample.OnDestroy();
    m_MagnifierPS.OnDestroy();
    m_WireframeBox.OnDestroy();
    m_Wireframe.OnDestroy();
    m_SkyDomeProc.OnDestroy();
    m_SkyDome.OnDestroy();
#if USE_SHADOWMASK
    m_shadowResolve.OnDestroy();
#endif
    m_GBuffer.OnDestroy();

    m_UploadHeap.OnDestroy();
    m_GPUTimer.OnDestroy();
    m_VidMemBufferPool.OnDestroy();
    m_ConstantBufferRing.OnDestroy();
    m_ResourceViewHeaps.OnDestroy();
    m_CommandListRing.OnDestroy();
}

//--------------------------------------------------------------------------------------
//
// OnCreateWindowSizeDependentResources
//
//--------------------------------------------------------------------------------------
void Renderer::OnCreateWindowSizeDependentResources(SwapChain *pSwapChain, uint32_t Width, uint32_t Height)
{
    m_Width = Width;
    m_Height = Height;

    // Set the viewport & scissors rect
    m_Viewport = { 0.0f, 0.0f, static_cast<float>(Width), static_cast<float>(Height), 0.0f, 1.0f };
    m_RectScissor = { 0, 0, (LONG)Width, (LONG)Height };

#if USE_SHADOWMASK
    // Create shadow mask
    //
    m_ShadowMask.Init(m_pDevice, "shadowbuffer", &CD3DX12_RESOURCE_DESC::Tex2D(DXGI_FORMAT_R8G8B8A8_UNORM, Width, Height, 1, 1, 1, 0, D3D12_RESOURCE_FLAG_ALLOW_UNORDERED_ACCESS), D3D12_RESOURCE_STATE_PIXEL_SHADER_RESOURCE, NULL);
    m_ShadowMask.CreateUAV(0, &m_ShadowMaskUAV);
    m_ShadowMask.CreateSRV(0, &m_ShadowMaskSRV);
#endif

    // Create GBuffer
    //
    m_GBuffer.OnCreateWindowSizeDependentResources(pSwapChain, Width, Height);
    m_RenderPassFullGBuffer.OnCreateWindowSizeDependentResources(Width, Height);
    m_RenderPassJustDepthAndHdr.OnCreateWindowSizeDependentResources(Width, Height);
    
    m_TAA.OnCreateWindowSizeDependentResources(Width, Height, &m_GBuffer);

    // update bloom and downscaling effect
    //
    m_DownSample.OnCreateWindowSizeDependentResources(m_Width, m_Height, &m_GBuffer.m_HDR, 5); //downsample the HDR texture 5 times
    m_Bloom.OnCreateWindowSizeDependentResources(m_Width / 2, m_Height / 2, m_DownSample.GetTexture(), 5, &m_GBuffer.m_HDR);
    m_MagnifierPS.OnCreateWindowSizeDependentResources(&m_GBuffer.m_HDR);
}

//--------------------------------------------------------------------------------------
//
// OnDestroyWindowSizeDependentResources
//
//--------------------------------------------------------------------------------------
void Renderer::OnDestroyWindowSizeDependentResources()
{
    m_Bloom.OnDestroyWindowSizeDependentResources();
    m_DownSample.OnDestroyWindowSizeDependentResources();

    m_GBuffer.OnDestroyWindowSizeDependentResources();

    m_TAA.OnDestroyWindowSizeDependentResources();

    m_MagnifierPS.OnDestroyWindowSizeDependentResources();

#if USE_SHADOWMASK
    m_ShadowMask.OnDestroy();
#endif

}

void Renderer::OnUpdateDisplayDependentResources(SwapChain* pSwapChain)
{
    // Update pipelines in case the format of the RTs changed (this happens when going HDR)
    m_ColorConversionPS.UpdatePipelines(pSwapChain->GetFormat(), pSwapChain->GetDisplayMode());
    m_ToneMappingPS.UpdatePipelines(pSwapChain->GetFormat());
    m_ImGUI.UpdatePipeline((pSwapChain->GetDisplayMode() == DISPLAYMODE_SDR) ? pSwapChain->GetFormat() : m_GBuffer.m_HDR.GetFormat());
}

//--------------------------------------------------------------------------------------
//
// LoadScene
//
//--------------------------------------------------------------------------------------
int Renderer::LoadScene(GLTFCommon *pGLTFCommon, int Stage)
{
    // show loading progress
    //
    ImGui::OpenPopup("Loading");
    if (ImGui::BeginPopupModal("Loading", NULL, ImGuiWindowFlags_AlwaysAutoResize))
    {
        float progress = (float)Stage / 13.0f;
        ImGui::ProgressBar(progress, ImVec2(0.f, 0.f), NULL);
        ImGui::EndPopup();
    }

    // use multi threading
    AsyncPool *pAsyncPool = &m_AsyncPool;

    // Loading stages
    //
    if (Stage == 0)
    {
    }
    else if (Stage == 5)
    {
        Profile p("m_pGltfLoader->Load");

        m_pGLTFTexturesAndBuffers = new GLTFTexturesAndBuffers();
        m_pGLTFTexturesAndBuffers->OnCreate(m_pDevice, pGLTFCommon, &m_UploadHeap, &m_VidMemBufferPool, &m_ConstantBufferRing);
    }
    else if (Stage == 6)
    {
        Profile p("LoadTextures");

        // here we are loading onto the GPU all the textures and the inverse matrices
        // this data will be used to create the PBR and Depth passes       
        m_pGLTFTexturesAndBuffers->LoadTextures(pAsyncPool);
    }
    else if (Stage == 7)
    {
        Profile p("m_GLTFDepth->OnCreate");

        //create the glTF's textures, VBs, IBs, shaders and descriptors for this particular pass
        m_GLTFDepth = new GltfDepthPass();
        m_GLTFDepth->OnCreate(
            m_pDevice,
            &m_UploadHeap,
            &m_ResourceViewHeaps,
            &m_ConstantBufferRing,
            &m_VidMemBufferPool,
            m_pGLTFTexturesAndBuffers,
            pAsyncPool
        );
    }
    else if (Stage == 9)
    {
        Profile p("m_GLTFPBR->OnCreate");

        // same thing as above but for the PBR pass
        m_GLTFPBR = new GltfPbrPass();
        m_GLTFPBR->OnCreate(
            m_pDevice,
            &m_UploadHeap,
            &m_ResourceViewHeaps,
            &m_ConstantBufferRing,
            m_pGLTFTexturesAndBuffers,
            &m_SkyDome,
            false,                  // use a SSAO mask
            USE_SHADOWMASK,
            &m_RenderPassFullGBuffer,
            pAsyncPool
        );

    }
    else if (Stage == 10)
    {
        Profile p("m_GLTFBBox->OnCreate");

        // just a bounding box pass that will draw boundingboxes instead of the geometry itself
        m_GLTFBBox = new GltfBBoxPass();
        m_GLTFBBox->OnCreate(
            m_pDevice,
            &m_UploadHeap,
            &m_ResourceViewHeaps,
            &m_ConstantBufferRing,
            &m_VidMemBufferPool,
            m_pGLTFTexturesAndBuffers,
            &m_Wireframe
        );

        // we are borrowing the upload heap command list for uploading to the GPU the IBs and VBs
        m_VidMemBufferPool.UploadData(m_UploadHeap.GetCommandList());

    }
    else if (Stage == 11)
    {
        Profile p("Flush");

        m_UploadHeap.FlushAndFinish();

        //once everything is uploaded we dont need he upload heaps anymore
        m_VidMemBufferPool.FreeUploadHeap();

        // tell caller that we are done loading the map
        return 0;
    }

    Stage++;
    return Stage;
}

//--------------------------------------------------------------------------------------
//
// UnloadScene
//
//--------------------------------------------------------------------------------------
void Renderer::UnloadScene()
{
    // wait for all the async loading operations to finish
    m_AsyncPool.Flush();

    m_pDevice->GPUFlush();

    if (m_GLTFPBR)
    {
        m_GLTFPBR->OnDestroy();
        delete m_GLTFPBR;
        m_GLTFPBR = NULL;
    }

    if (m_GLTFDepth)
    {
        m_GLTFDepth->OnDestroy();
        delete m_GLTFDepth;
        m_GLTFDepth = NULL;
    }

    if (m_GLTFBBox)
    {
        m_GLTFBBox->OnDestroy();
        delete m_GLTFBBox;
        m_GLTFBBox = NULL;
    }

    if (m_pGLTFTexturesAndBuffers)
    {
        m_pGLTFTexturesAndBuffers->OnDestroy();
        delete m_pGLTFTexturesAndBuffers;
        m_pGLTFTexturesAndBuffers = NULL;
    }

    while (!m_shadowMapPool.empty())
    {
        m_shadowMapPool.back().ShadowMap.OnDestroy();
        m_shadowMapPool.pop_back();
    }
}

void Renderer::AllocateShadowMaps(GLTFCommon* pGLTFCommon)
{
    // Go through the lights and allocate shadow information
    uint32_t NumShadows = 0;
    for (int i = 0; i < pGLTFCommon->m_lightInstances.size(); ++i)
    {
        const tfLight& lightData = pGLTFCommon->m_lights[pGLTFCommon->m_lightInstances[i].m_lightId];
        if (lightData.m_shadowResolution)
        {
            SceneShadowInfo ShadowInfo;
            ShadowInfo.ShadowResolution = lightData.m_shadowResolution;
            ShadowInfo.ShadowIndex = NumShadows++;
            ShadowInfo.LightIndex = i;
            m_shadowMapPool.push_back(ShadowInfo);
        }
    }

    if (NumShadows > MaxShadowInstances)
    {
        Trace("Number of shadows has exceeded maximum supported. Please grow value in gltfCommon.h/perFrameStruct.h");
        throw;
    }

    // If we had shadow information, allocate all required maps and bindings
    if (!m_shadowMapPool.empty())
    {
        m_ResourceViewHeaps.AllocDSVDescriptor((uint32_t)m_shadowMapPool.size(), &m_ShadowMapPoolDSV);
        m_ResourceViewHeaps.AllocCBV_SRV_UAVDescriptor((uint32_t)m_shadowMapPool.size(), &m_ShadowMapPoolSRV);

        std::vector<SceneShadowInfo>::iterator CurrentShadow = m_shadowMapPool.begin();
        for( uint32_t i = 0; CurrentShadow < m_shadowMapPool.end(); ++i, ++CurrentShadow)
        {
            auto tex = CD3DX12_RESOURCE_DESC::Tex2D(DXGI_FORMAT_D32_FLOAT, CurrentShadow->ShadowResolution, CurrentShadow->ShadowResolution, 1, 1, 1, 0, D3D12_RESOURCE_FLAG_ALLOW_DEPTH_STENCIL);
            CurrentShadow->ShadowMap.InitDepthStencil(m_pDevice, "m_pShadowMap", &tex);
            CurrentShadow->ShadowMap.CreateDSV(CurrentShadow->ShadowIndex, &m_ShadowMapPoolDSV);
            CurrentShadow->ShadowMap.CreateSRV(CurrentShadow->ShadowIndex, &m_ShadowMapPoolSRV);
        }
    }
}

//--------------------------------------------------------------------------------------
//
// OnRender
//
//--------------------------------------------------------------------------------------
void Renderer::OnRender(const UIState* pState, const Camera& Cam, SwapChain* pSwapChain)
{
    ZoneScopedC(0xb2b1e6);

    // Timing values
    UINT64 gpuTicksPerSecond;
    m_pDevice->GetGraphicsQueue()->GetTimestampFrequency(&gpuTicksPerSecond);

    // Let our resource managers do some house keeping
    m_CommandListRing.OnBeginFrame();
    m_ConstantBufferRing.OnBeginFrame();
    m_GPUTimer.OnBeginFrame(gpuTicksPerSecond, &m_TimeStamps);

    // Sets the perFrame data 
    per_frame *pPerFrame = NULL;
    if (m_pGLTFTexturesAndBuffers)
    {
        // fill as much as possible using the GLTF (camera, lights, ...)
        pPerFrame = m_pGLTFTexturesAndBuffers->m_pGLTFCommon->SetPerFrameData(Cam);

        // Set some lighting factors
        pPerFrame->iblFactor = pState->IBLFactor;
        pPerFrame->emmisiveFactor = pState->EmissiveFactor;
        pPerFrame->invScreenResolution[0] = 1.0f / ((float)m_Width);
        pPerFrame->invScreenResolution[1] = 1.0f / ((float)m_Height);

		pPerFrame->wireframeOptions.setX(pState->WireframeColor[0]);
        pPerFrame->wireframeOptions.setY(pState->WireframeColor[1]);
        pPerFrame->wireframeOptions.setZ(pState->WireframeColor[2]);
        pPerFrame->wireframeOptions.setW(pState->WireframeMode == UIState::WireframeMode::WIREFRAME_MODE_SOLID_COLOR ? 1.0f : 0.0f);
        pPerFrame->lodBias = 0.0f;
        m_pGLTFTexturesAndBuffers->SetPerFrameConstants();
        m_pGLTFTexturesAndBuffers->SetSkinningMatricesForSkeletons();
    }

    // command buffer calls
    ID3D12GraphicsCommandList* pCmdLst1 = m_CommandListRing.GetNewCommandList();

    m_GPUTimer.GetTimeStamp(pCmdLst1, "Begin Frame");

    auto tr = CD3DX12_RESOURCE_BARRIER::Transition(pSwapChain->GetCurrentBackBufferResource(), D3D12_RESOURCE_STATE_PRESENT, D3D12_RESOURCE_STATE_RENDER_TARGET);
    pCmdLst1->ResourceBarrier(1, &tr);

    // Render shadow maps 
    std::vector<CD3DX12_RESOURCE_BARRIER> ShadowReadBarriers;
    std::vector<CD3DX12_RESOURCE_BARRIER> ShadowWriteBarriers;
    if (m_GLTFDepth && pPerFrame != NULL)
    {
        std::vector<SceneShadowInfo>::iterator ShadowMap = m_shadowMapPool.begin();
        while (ShadowMap < m_shadowMapPool.end())
        {
            pCmdLst1->ClearDepthStencilView(m_ShadowMapPoolDSV.GetCPU(ShadowMap->ShadowIndex), D3D12_CLEAR_FLAG_DEPTH, 1.0f, 0, 0, nullptr);
            ++ShadowMap;
        }
        m_GPUTimer.GetTimeStamp(pCmdLst1, "Clear shadow maps");

        // Render all shadows
        ShadowMap = m_shadowMapPool.begin();
        while (ShadowMap < m_shadowMapPool.end())
        {
            SetViewportAndScissor(pCmdLst1, 0, 0, ShadowMap->ShadowResolution, ShadowMap->ShadowResolution);
            auto handle = m_ShadowMapPoolDSV.GetCPU(ShadowMap->ShadowIndex);
            pCmdLst1->OMSetRenderTargets(0, NULL, false, &handle);
            
            per_frame* cbDepthPerFrame = m_GLTFDepth->SetPerFrameConstants();
            cbDepthPerFrame->mCameraCurrViewProj = pPerFrame->lights[ShadowMap->LightIndex].mLightViewProj;
            cbDepthPerFrame->lodBias = 0.0f;

            m_GLTFDepth->Draw(pCmdLst1);

            // Push a barrier
            ShadowReadBarriers.push_back(CD3DX12_RESOURCE_BARRIER::Transition(ShadowMap->ShadowMap.GetResource(), D3D12_RESOURCE_STATE_DEPTH_WRITE, D3D12_RESOURCE_STATE_PIXEL_SHADER_RESOURCE));
            ShadowWriteBarriers.push_back(CD3DX12_RESOURCE_BARRIER::Transition(ShadowMap->ShadowMap.GetResource(), D3D12_RESOURCE_STATE_PIXEL_SHADER_RESOURCE, D3D12_RESOURCE_STATE_DEPTH_WRITE));

            m_GPUTimer.GetTimeStamp(pCmdLst1, "Shadow map");
            ++ShadowMap;
        }
        
        // Transition all shadow map barriers
        pCmdLst1->ResourceBarrier((UINT)ShadowReadBarriers.size(), ShadowReadBarriers.data());
    }

    // Shadow resolve ---------------------------------------------------------------------------
#if USE_SHADOWMASK
    if (pPerFrame != NULL)
    {
        const D3D12_RESOURCE_BARRIER preShadowResolve[] =
        {
            CD3DX12_RESOURCE_BARRIER::Transition(m_ShadowMask.GetResource(), D3D12_RESOURCE_STATE_PIXEL_SHADER_RESOURCE, D3D12_RESOURCE_STATE_UNORDERED_ACCESS),
            CD3DX12_RESOURCE_BARRIER::Transition(m_MotionVectorsDepthMap.GetResource(), D3D12_RESOURCE_STATE_DEPTH_WRITE, D3D12_RESOURCE_STATE_PIXEL_SHADER_RESOURCE)
        };
        pCmdLst1->ResourceBarrier(ARRAYSIZE(preShadowResolve), preShadowResolve);

        ShadowResolveFrame shadowResolveFrame;
        shadowResolveFrame.m_Width = m_Width;
        shadowResolveFrame.m_Height = m_Height;
        shadowResolveFrame.m_ShadowMapSRV = m_ShadowMapSRV;
        shadowResolveFrame.m_DepthBufferSRV = m_MotionVectorsDepthMapSRV;
        shadowResolveFrame.m_ShadowBufferUAV = m_ShadowMaskUAV;

        m_shadowResolve.Draw(pCmdLst1, m_pGLTFTexturesAndBuffers, &shadowResolveFrame);

        const D3D12_RESOURCE_BARRIER postShadowResolve[] =
        {
            CD3DX12_RESOURCE_BARRIER::Transition(m_ShadowMask.GetResource(), D3D12_RESOURCE_STATE_UNORDERED_ACCESS, D3D12_RESOURCE_STATE_PIXEL_SHADER_RESOURCE),
            CD3DX12_RESOURCE_BARRIER::Transition(m_MotionVectorsDepthMap.GetResource(), D3D12_RESOURCE_STATE_PIXEL_SHADER_RESOURCE, D3D12_RESOURCE_STATE_DEPTH_WRITE)
        };
        pCmdLst1->ResourceBarrier(ARRAYSIZE(postShadowResolve), postShadowResolve);
    }
    m_GPUTimer.GetTimeStamp(pCmdLst1, "Shadow resolve");
#endif

    // Render Scene to the GBuffer ------------------------------------------------
    if (pPerFrame != NULL)
    {
        ZoneScopedNC("GBuffer", 0xb2b1e6);

        pCmdLst1->RSSetViewports(1, &m_Viewport);
        pCmdLst1->RSSetScissorRects(1, &m_RectScissor);

        if (m_GLTFPBR)
        {
            const bool bWireframe = pState->WireframeMode != UIState::WireframeMode::WIREFRAME_MODE_OFF;

            std::vector<GltfPbrPass::BatchList> opaque, transparent;
            m_GLTFPBR->BuildBatchLists(&opaque, &transparent, bWireframe);

            // Render opaque geometry
            {
                m_RenderPassFullGBuffer.BeginPass(pCmdLst1, true);
#if USE_SHADOWMASK
                m_GLTFPBR->DrawBatchList(pCmdLst1, &m_ShadowMaskSRV, &solid, bWireframe);
#else
                m_GLTFPBR->DrawBatchList(pCmdLst1, &m_ShadowMapPoolSRV, &opaque, bWireframe);
#endif
                m_GPUTimer.GetTimeStamp(pCmdLst1, "PBR Opaque");
                m_RenderPassFullGBuffer.EndPass();
            }

            // draw skydome
            {
                m_RenderPassJustDepthAndHdr.BeginPass(pCmdLst1, false);

                // Render skydome
                if (pState->SelectedSkydomeTypeIndex == 1)
                {
                    math::Matrix4 clipToView = math::inverse(pPerFrame->mCameraCurrViewProj);
                    m_SkyDome.Draw(pCmdLst1, clipToView);
                    m_GPUTimer.GetTimeStamp(pCmdLst1, "Skydome cube");
                }
                else if (pState->SelectedSkydomeTypeIndex == 0)
                {
                    SkyDomeProc::Constants skyDomeConstants;
                    skyDomeConstants.invViewProj = math::inverse(pPerFrame->mCameraCurrViewProj);
                    skyDomeConstants.vSunDirection = math::Vector4(1.0f, 0.05f, 0.0f, 0.0f);
                    skyDomeConstants.turbidity = 10.0f;
                    skyDomeConstants.rayleigh = 2.0f;
                    skyDomeConstants.mieCoefficient = 0.005f;
                    skyDomeConstants.mieDirectionalG = 0.8f;
                    skyDomeConstants.luminance = 1.0f;
                    m_SkyDomeProc.Draw(pCmdLst1, skyDomeConstants);

                    m_GPUTimer.GetTimeStamp(pCmdLst1, "Skydome proc");
                }

                m_RenderPassJustDepthAndHdr.EndPass();
            }

            // draw transparent geometry
            {
                m_RenderPassFullGBuffer.BeginPass(pCmdLst1, false);

                std::sort(transparent.begin(), transparent.end());
                m_GLTFPBR->DrawBatchList(pCmdLst1, &m_ShadowMapPoolSRV, &transparent, bWireframe);
                m_GPUTimer.GetTimeStamp(pCmdLst1, "PBR Transparent");

                m_RenderPassFullGBuffer.EndPass();
            }
        }

        // draw object's bounding boxes
        if (m_GLTFBBox && pPerFrame != NULL)
        {
            if (pState->bDrawBoundingBoxes)
            {
                m_GLTFBBox->Draw(pCmdLst1, pPerFrame->mCameraCurrViewProj);

                m_GPUTimer.GetTimeStamp(pCmdLst1, "Bounding Box");
            }
        }

        // draw light's frustums
        if (pState->bDrawLightFrustum && pPerFrame != NULL)
        {
            UserMarker marker(pCmdLst1, "light frustrums");

            math::Vector4 vCenter = math::Vector4(0.0f, 0.0f, 0.5f, 0.0f);
            math::Vector4 vRadius = math::Vector4(1.0f, 1.0f, 0.5f, 0.0f);
            math::Vector4 vColor = math::Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            for (uint32_t i = 0; i < pPerFrame->lightCount; i++)
            {
                math::Matrix4 spotlightMatrix = math::inverse(pPerFrame->lights[i].mLightViewProj); // XMMatrixInverse(NULL, pPerFrame->lights[i].mLightViewProj);
                math::Matrix4 worldMatrix = pPerFrame->mCameraCurrViewProj * spotlightMatrix; //spotlightMatrix * pPerFrame->mCameraCurrViewProj;
                m_WireframeBox.Draw(pCmdLst1, &m_Wireframe, worldMatrix, vCenter, vRadius, vColor);
            }

            m_GPUTimer.GetTimeStamp(pCmdLst1, "Light's frustum");
        }
    }

    if (ShadowWriteBarriers.size())
        pCmdLst1->ResourceBarrier((UINT)ShadowWriteBarriers.size(), ShadowWriteBarriers.data());

    D3D12_RESOURCE_BARRIER preResolve[1] = {
        CD3DX12_RESOURCE_BARRIER::Transition(m_GBuffer.m_HDR.GetResource(), D3D12_RESOURCE_STATE_RENDER_TARGET, D3D12_RESOURCE_STATE_PIXEL_SHADER_RESOURCE),
    };
    pCmdLst1->ResourceBarrier(1, preResolve);

    // Post proc---------------------------------------------------------------------------

    // Bloom, takes HDR as input and applies bloom to it.
    {
        ZoneScopedNC("Bloom", 0xb2b1e6);

        D3D12_CPU_DESCRIPTOR_HANDLE renderTargets[] = { m_GBuffer.m_HDRRTV.GetCPU() };
        pCmdLst1->OMSetRenderTargets(ARRAYSIZE(renderTargets), renderTargets, false, NULL);

        m_DownSample.Draw(pCmdLst1);
        m_GPUTimer.GetTimeStamp(pCmdLst1, "Downsample");

        m_Bloom.Draw(pCmdLst1, &m_GBuffer.m_HDR);
        m_GPUTimer.GetTimeStamp(pCmdLst1, "Bloom");
    }

    // Apply TAA & Sharpen to m_HDR
    if (pState->bUseTAA)
    {
        ZoneScopedNC("TAA", 0xb2b1e6);

        m_TAA.Draw(pCmdLst1, D3D12_RESOURCE_STATE_PIXEL_SHADER_RESOURCE, D3D12_RESOURCE_STATE_PIXEL_SHADER_RESOURCE);
        m_GPUTimer.GetTimeStamp(pCmdLst1, "TAA");
    }

    // Magnifier Pass: m_HDR as input, pass' own output
    if (pState->bUseMagnifier)
    {
        ZoneScopedNC("Magnifier", 0xb2b1e6);

        // Note: assumes m_GBuffer.HDR is in D3D12_RESOURCE_STATE_PIXEL_SHADER_RESOURCE
        m_MagnifierPS.Draw(pCmdLst1, pState->MagnifierParams, m_GBuffer.m_HDRSRV);
        m_GPUTimer.GetTimeStamp(pCmdLst1, "Magnifier");

        // Transition magnifier state to PIXEL_SHADER_RESOURCE, as it is going to be pRscCurrentInput replacing m_GBuffer.m_HDR which is in that state.
        auto tr = CD3DX12_RESOURCE_BARRIER::Transition(m_MagnifierPS.GetPassOutputResource(), D3D12_RESOURCE_STATE_RENDER_TARGET, D3D12_RESOURCE_STATE_PIXEL_SHADER_RESOURCE);
        pCmdLst1->ResourceBarrier(1, &tr);
    }

    // Start tracking input/output resources at this point to handle HDR and SDR render paths 
    ID3D12Resource*              pRscCurrentInput = pState->bUseMagnifier ? m_MagnifierPS.GetPassOutputResource()     : m_GBuffer.m_HDR.GetResource();
    CBV_SRV_UAV                  SRVCurrentInput  = pState->bUseMagnifier ? m_MagnifierPS.GetPassOutputSRV()          : m_GBuffer.m_HDRSRV;
    D3D12_CPU_DESCRIPTOR_HANDLE  RTVCurrentOutput = pState->bUseMagnifier ? m_MagnifierPS.GetPassOutputRTV().GetCPU() : m_GBuffer.m_HDRRTV.GetCPU();
    CBV_SRV_UAV                  UAVCurrentOutput = pState->bUseMagnifier ? m_MagnifierPS.GetPassOutputUAV()          : m_GBuffer.m_HDRUAV;
    

    // If using FreeSync HDR we need to do the tonemapping in-place and then apply the GUI, later we'll apply the color conversion into the swapchain
    const bool bHDR = pSwapChain->GetDisplayMode() != DISPLAYMODE_SDR;
    if (bHDR)
    {
        // In place Tonemapping ------------------------------------------------------------------------
        {
            ZoneScopedNC("HDR: Tonemapping", 0xb2b1e6);
            D3D12_RESOURCE_BARRIER inputRscToUAV = CD3DX12_RESOURCE_BARRIER::Transition(pRscCurrentInput, D3D12_RESOURCE_STATE_PIXEL_SHADER_RESOURCE, D3D12_RESOURCE_STATE_UNORDERED_ACCESS);
            pCmdLst1->ResourceBarrier(1, &inputRscToUAV);

            m_ToneMappingCS.Draw(pCmdLst1, &UAVCurrentOutput, pState->Exposure, pState->SelectedTonemapperIndex, m_Width, m_Height);

            D3D12_RESOURCE_BARRIER inputRscToRTV = CD3DX12_RESOURCE_BARRIER::Transition(pRscCurrentInput, D3D12_RESOURCE_STATE_UNORDERED_ACCESS, D3D12_RESOURCE_STATE_RENDER_TARGET);
            pCmdLst1->ResourceBarrier(1, &inputRscToRTV);
        }

        // Render HUD  ------------------------------------------------------------------------
        {
            ZoneScopedNC("HDR: HUD", 0xb2b1e6);
            pCmdLst1->RSSetViewports(1, &m_Viewport);
            pCmdLst1->RSSetScissorRects(1, &m_RectScissor);
            pCmdLst1->OMSetRenderTargets(1, &RTVCurrentOutput, true, NULL);

            m_ImGUI.Draw(pCmdLst1);

            D3D12_RESOURCE_BARRIER hdrToSRV = CD3DX12_RESOURCE_BARRIER::Transition(pRscCurrentInput, D3D12_RESOURCE_STATE_RENDER_TARGET, D3D12_RESOURCE_STATE_PIXEL_SHADER_RESOURCE);
            pCmdLst1->ResourceBarrier(1, &hdrToSRV);

            m_GPUTimer.GetTimeStamp(pCmdLst1, "ImGUI Rendering");
        }
    }

    // submit command buffer #1
    ThrowIfFailed(pCmdLst1->Close());
    ID3D12CommandList* CmdListList1[] = { pCmdLst1 };
    m_pDevice->GetGraphicsQueue()->ExecuteCommandLists(1, CmdListList1);

    // Wait for swapchain (we are going to render to it) -----------------------------------
    {
        ZoneScopedNC("Waiting for swap chain", 0xb2b1e6);
        pSwapChain->WaitForSwapChain();
    }
    // Keep tracking input/output resource views 
    pRscCurrentInput = pState->bUseMagnifier ? m_MagnifierPS.GetPassOutputResource() : m_GBuffer.m_HDR.GetResource(); // these haven't changed, re-assign as sanity check
    SRVCurrentInput  = pState->bUseMagnifier ? m_MagnifierPS.GetPassOutputSRV()      : m_GBuffer.m_HDRSRV;            // these haven't changed, re-assign as sanity check
    RTVCurrentOutput = *pSwapChain->GetCurrentBackBufferRTV();
    UAVCurrentOutput = {}; // no BackBufferUAV.


    ID3D12GraphicsCommandList* pCmdLst2 = m_CommandListRing.GetNewCommandList();

    pCmdLst2->RSSetViewports(1, &m_Viewport);
    pCmdLst2->RSSetScissorRects(1, &m_RectScissor);
    pCmdLst2->OMSetRenderTargets(1, pSwapChain->GetCurrentBackBufferRTV(), true, NULL);

    if (bHDR)
    {
        // FS HDR mode! Apply color conversion now.
        m_ColorConversionPS.Draw(pCmdLst2, &SRVCurrentInput);
        m_GPUTimer.GetTimeStamp(pCmdLst2, "Color conversion");

        auto tr = CD3DX12_RESOURCE_BARRIER::Transition(pRscCurrentInput, D3D12_RESOURCE_STATE_PIXEL_SHADER_RESOURCE, D3D12_RESOURCE_STATE_RENDER_TARGET);
        pCmdLst2->ResourceBarrier(1, &tr);
    }
    else
    {
        // non FreeSync HDR mode, that is SDR, here we apply the tonemapping from the HDR into the swapchain and then we render the GUI

        // Tonemapping ------------------------------------------------------------------------
        {
            ZoneScopedNC("SDR: Tonemapping", 0xb2b1e6);
            m_ToneMappingPS.Draw(pCmdLst2, &SRVCurrentInput, pState->Exposure, pState->SelectedTonemapperIndex);
            m_GPUTimer.GetTimeStamp(pCmdLst2, "Tone mapping");

            auto tr = CD3DX12_RESOURCE_BARRIER::Transition(pRscCurrentInput, D3D12_RESOURCE_STATE_PIXEL_SHADER_RESOURCE, D3D12_RESOURCE_STATE_RENDER_TARGET);
            pCmdLst2->ResourceBarrier(1, &tr);
        }

        // Render HUD  ------------------------------------------------------------------------
        {
            ZoneScopedNC("SDR: HUD", 0xb2b1e6);
            m_ImGUI.Draw(pCmdLst2);
            m_GPUTimer.GetTimeStamp(pCmdLst2, "ImGUI Rendering");
        }
    }

    // If magnifier is used, make sure m_GBuffer.m_HDR which is not pRscCurrentInput gets reverted back to RT state.
    if(pState->bUseMagnifier)
        auto tr = CD3DX12_RESOURCE_BARRIER::Transition(m_GBuffer.m_HDR.GetResource(), D3D12_RESOURCE_STATE_PIXEL_SHADER_RESOURCE, D3D12_RESOURCE_STATE_RENDER_TARGET);
        pCmdLst2->ResourceBarrier(1, &tr);

    if (!m_pScreenShotName.empty())
    {
        m_SaveTexture.CopyRenderTargetIntoStagingTexture(m_pDevice->GetDevice(), pCmdLst2, pSwapChain->GetCurrentBackBufferResource(), D3D12_RESOURCE_STATE_RENDER_TARGET);
    }

    // Transition swapchain into present mode

    auto tr2 = CD3DX12_RESOURCE_BARRIER::Transition(pSwapChain->GetCurrentBackBufferResource(), D3D12_RESOURCE_STATE_RENDER_TARGET, D3D12_RESOURCE_STATE_PRESENT);
    pCmdLst2->ResourceBarrier(1, &tr2);

    m_GPUTimer.OnEndFrame();

    m_GPUTimer.CollectTimings(pCmdLst2);

    // Close & Submit the command list #2 -------------------------------------------------
    ThrowIfFailed(pCmdLst2->Close());

    ID3D12CommandList* CmdListList2[] = { pCmdLst2 };
    m_pDevice->GetGraphicsQueue()->ExecuteCommandLists(1, CmdListList2);

    // Handle screenshot request
    if (!m_pScreenShotName.empty())
    {
        m_SaveTexture.SaveStagingTextureAsJpeg(m_pDevice->GetDevice(), m_pDevice->GetGraphicsQueue(), m_pScreenShotName.c_str());
        m_pScreenShotName.clear();
    }
}
