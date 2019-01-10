
#include <OpenGLES/ES2/gl.h>
#include <OpenGLES/ES2/glext.h>

#include <stdio.h>

#include "GlesHelper.h"
#include "UnityAppController.h"
#include "DisplayManager.h"

#include "EAGLContextHelper.h"
#include "CVTextureCache.h"

#include "iPhone_Profiler.h"


extern GLint gDefaultFBO;


extern "C" void InitEAGLLayer(void* eaglLayer, bool use32bitColor);

void InitGLES()
{
#if GL_EXT_discard_framebuffer
	_supportsDiscard = UnityHasRenderingAPIExtension("GL_EXT_discard_framebuffer");
#endif

#if GL_APPLE_framebuffer_multisample
	_supportsMSAA = UnityHasRenderingAPIExtension("GL_APPLE_framebuffer_multisample");
#endif

#if GL_OES_packed_depth_stencil
	_supportsPackedStencil = UnityHasRenderingAPIExtension("GL_OES_packed_depth_stencil");
#endif
}


void CreateSystemRenderingSurface(UnityRenderingSurface* surface)
{
	EAGLContextSetCurrentAutoRestore autorestore(surface->context);
	DestroySystemRenderingSurface(surface);

	const NSString* colorFormat = surface->use32bitColor ? kEAGLColorFormatRGBA8 : kEAGLColorFormatRGB565;

	surface->layer.opaque = YES;
	surface->layer.drawableProperties = [NSDictionary dictionaryWithObjectsAndKeys:
											[NSNumber numberWithBool:FALSE], kEAGLDrawablePropertyRetainedBacking,
											colorFormat, kEAGLDrawablePropertyColorFormat,
											nil
										];


	surface->colorFormat = surface->use32bitColor ? GL_RGBA8_OES : GL_RGB565;

	GLES_CHK(glGenRenderbuffers(1, &surface->systemColorRB));
	GLES_CHK(glBindRenderbuffer(GL_RENDERBUFFER, surface->systemColorRB));
	AllocateRenderBufferStorageFromEAGLLayer(surface->context, surface->layer);

	GLES_CHK(glGenFramebuffers(1, &surface->systemFB));
	GLES_CHK(glBindFramebuffer(GL_FRAMEBUFFER, surface->systemFB));
	GLES_CHK(glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_RENDERBUFFER, surface->systemColorRB));
}

void CreateRenderingSurface(UnityRenderingSurface* surface)
{
	EAGLContextSetCurrentAutoRestore autorestore(surface->context);
	DestroyRenderingSurface(surface);

	bool needRenderingSurface = surface->targetW != surface->systemW || surface->targetH != surface->systemH || surface->useCVTextureCache;
	if(needRenderingSurface)
	{
		if(surface->useCVTextureCache)
			surface->cvTextureCache = CreateCVTextureCache();

		if(surface->cvTextureCache)
		{
			surface->cvTextureCacheTexture = CreateReadableRTFromCVTextureCache(surface->cvTextureCache, surface->targetW, surface->targetH, &surface->cvPixelBuffer);
            surface->targetColorRT = GetGLTextureFromCVTextureCache(surface->cvTextureCacheTexture);
		}
		else
		{
			GLES_CHK(glGenTextures(1, &surface->targetColorRT));
		}

		GLES_CHK(glBindTexture(GL_TEXTURE_2D, surface->targetColorRT));
		GLES_CHK(glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GLES_UPSCALE_FILTER));
		GLES_CHK(glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GLES_UPSCALE_FILTER));
		GLES_CHK(glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE));
		GLES_CHK(glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE));

		if(!surface->cvTextureCache)
		{
			GLenum fmt  = surface->use32bitColor ? GL_RGBA : GL_RGB;
			GLenum type = surface->use32bitColor ? GL_UNSIGNED_BYTE : GL_UNSIGNED_SHORT_5_6_5;
			GLES_CHK(glTexImage2D(GL_TEXTURE_2D, 0, fmt, surface->targetW, surface->targetH, 0, fmt, type, 0));
		}

		GLES_CHK(glGenFramebuffers(1, &surface->targetFB));
		GLES_CHK(glBindFramebuffer(GL_FRAMEBUFFER, surface->targetFB));
		GLES_CHK(glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, surface->targetColorRT, 0));

		GLES_CHK(glBindTexture(GL_TEXTURE_2D, 0));
	}

#if GL_APPLE_framebuffer_multisample
	if(_supportsMSAA && surface->msaaSamples > 1)
	{
		GLES_CHK(glGenRenderbuffers(1, &surface->msaaColorRB));
		GLES_CHK(glBindRenderbuffer(GL_RENDERBUFFER, surface->msaaColorRB));

		GLES_CHK(glGenFramebuffers(1, &surface->msaaFB));
		GLES_CHK(glBindFramebuffer(GL_FRAMEBUFFER, surface->msaaFB));

		GLES_CHK(glRenderbufferStorageMultisampleAPPLE(GL_RENDERBUFFER, surface->msaaSamples, surface->colorFormat, surface->targetW, surface->targetH));
		GLES_CHK(glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_RENDERBUFFER, surface->msaaColorRB));
	}
#endif
}

void CreateSharedDepthbuffer(UnityRenderingSurface* surface)
{
	EAGLContextSetCurrentAutoRestore autorestore(surface->context);
	DestroySharedDepthbuffer(surface);

	surface->depthFormat = surface->use24bitDepth ? GL_DEPTH_COMPONENT24_OES : GL_DEPTH_COMPONENT16;
#if GL_OES_packed_depth_stencil
	if(_supportsPackedStencil && surface->use24bitDepth)
		surface->depthFormat = GL_DEPTH24_STENCIL8_OES;
#endif

	GLES_CHK(glGenRenderbuffers(1, &surface->depthRB));
	GLES_CHK(glBindRenderbuffer(GL_RENDERBUFFER, surface->depthRB));

	bool needMSAA = GL_APPLE_framebuffer_multisample && (surface->msaaSamples > 1);

#if GL_APPLE_framebuffer_multisample
	if(needMSAA)
		GLES_CHK(glRenderbufferStorageMultisampleAPPLE(GL_RENDERBUFFER, surface->msaaSamples, surface->depthFormat, surface->targetW, surface->targetH));
#endif

	if(!needMSAA)
		GLES_CHK(glRenderbufferStorage(GL_RENDERBUFFER, surface->depthFormat, surface->targetW, surface->targetH));

	SetupUnityDefaultFBO(surface);
	GLES_CHK(glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_DEPTH_ATTACHMENT, GL_RENDERBUFFER, surface->depthRB));

	if(_supportsPackedStencil && surface->use24bitDepth)
		GLES_CHK(glFramebufferRenderbuffer(GL_FRAMEBUFFER, GL_STENCIL_ATTACHMENT, GL_RENDERBUFFER, surface->depthRB));
}

void CreateUnityRenderBuffers(UnityRenderingSurface* surface)
{
	int w 	= surface->targetW;
	int h 	= surface->targetH;
	int api = surface->context.API;

	unsigned texid = 0, rbid = 0;

	if(surface->msaaFB)			rbid  = surface->msaaColorRB;
	else if(surface->targetFB)	texid = surface->targetColorRT;
	else						rbid  = surface->systemColorRB;

	surface->unityColorBuffer = UnityCreateUpdateExternalColorSurface(api, surface->unityColorBuffer, texid, rbid, w, h, surface->use32bitColor);
	surface->unityDepthBuffer = UnityCreateUpdateExternalDepthSurface(api, surface->unityDepthBuffer, 0, surface->depthRB, w, h, surface->use24bitDepth);
}

void DestroySystemRenderingSurface(UnityRenderingSurface* surface)
{
	EAGLContextSetCurrentAutoRestore autorestore(surface->context);

	GLES_CHK(glBindRenderbuffer(GL_RENDERBUFFER, 0));
	GLES_CHK(glBindFramebuffer(GL_FRAMEBUFFER, 0));

	if(surface->systemColorRB)
	{
		GLES_CHK(glBindRenderbuffer(GL_RENDERBUFFER, surface->systemColorRB));
		DeallocateRenderBufferStorageFromEAGLLayer(surface->context);

		GLES_CHK(glBindRenderbuffer(GL_RENDERBUFFER, 0));
		GLES_CHK(glDeleteRenderbuffers(1, &surface->systemColorRB));
		surface->systemColorRB = 0;
	}

	if(surface->depthRB && surface->targetFB == 0 && surface->msaaFB == 0)
	{
		GLES_CHK(glDeleteRenderbuffers(1, &surface->depthRB));
		surface->depthRB = 0;
	}

	if(surface->systemFB)
	{
		GLES_CHK(glDeleteFramebuffers(1, &surface->systemFB));
		surface->systemFB = 0;
	}
}

void DestroyRenderingSurface(UnityRenderingSurface* surface)
{
	EAGLContextSetCurrentAutoRestore autorestore(surface->context);

	if(surface->targetColorRT && !surface->cvTextureCache)
	{
		GLES_CHK(glDeleteTextures(1, &surface->targetColorRT));
		surface->targetColorRT = 0;
	}

	if(surface->cvTextureCacheTexture)	CFRelease(surface->cvTextureCacheTexture);
	if(surface->cvPixelBuffer)			CFRelease(surface->cvPixelBuffer);
	if(surface->cvTextureCache)			CFRelease(surface->cvTextureCache);
	surface->cvTextureCache = 0;


	if(surface->targetFB)
	{
		GLES_CHK(glDeleteFramebuffers(1, &surface->targetFB));
		surface->targetFB = 0;
	}

	if(surface->msaaColorRB)
	{
		GLES_CHK(glDeleteRenderbuffers(1, &surface->msaaColorRB));
		surface->msaaColorRB = 0;
	}

	if(surface->msaaFB)
	{
		GLES_CHK(glDeleteFramebuffers(1, &surface->msaaFB));
		surface->msaaFB = 0;
	}
}

void DestroySharedDepthbuffer(UnityRenderingSurface* surface)
{
	EAGLContextSetCurrentAutoRestore autorestore(surface->context);

	if(surface->depthRB)
	{
		GLES_CHK(glDeleteRenderbuffers(1, &surface->depthRB));
		surface->depthRB = 0;
	}
}

void DestroyUnityRenderBuffers(UnityRenderingSurface* surface)
{
	EAGLContextSetCurrentAutoRestore autorestore(surface->context);

	if(surface->unityColorBuffer)
	{
		UnityDestroyExternalColorSurface(surface->context.API, surface->unityColorBuffer);
		surface->unityColorBuffer = 0;
	}

	if(surface->unityDepthBuffer)
	{
		UnityDestroyExternalDepthSurface(surface->context.API, surface->unityDepthBuffer);
		surface->unityDepthBuffer = 0;
	}
}

void PreparePresentRenderingSurface(UnityRenderingSurface* surface, EAGLContext* mainContext)
{
	{
		EAGLContextSetCurrentAutoRestore autorestore(surface->context);

	#if GL_APPLE_framebuffer_multisample
		if(surface->msaaSamples > 1 && _supportsMSAA)
		{
			Profiler_StartMSAAResolve();

			GLuint targetFB = surface->targetFB ? surface->targetFB : surface->systemFB;

			GLES_CHK(glBindFramebuffer(GL_READ_FRAMEBUFFER_APPLE, surface->msaaFB));
			GLES_CHK(glBindFramebuffer(GL_DRAW_FRAMEBUFFER_APPLE, targetFB));
			GLES_CHK(glResolveMultisampleFramebufferAPPLE());

			Profiler_EndMSAAResolve();
		}
	#endif

		if(surface->allowScreenshot && UnityIsCaptureScreenshotRequested())
		{
			GLint targetFB = surface->targetFB ? surface->targetFB : surface->systemFB;
			GLES_CHK(glBindFramebuffer(GL_FRAMEBUFFER, targetFB));
			UnityCaptureScreenshot();
		}
	}

	AppController_RenderPluginMethod(@selector(onFrameResolved));

	if(surface->targetColorRT)
	{
		// shaders are bound to context
		EAGLContextSetCurrentAutoRestore autorestore(mainContext);

		gDefaultFBO = surface->systemFB;
		GLES_CHK(glBindFramebuffer(GL_FRAMEBUFFER, gDefaultFBO));
		UnityBlitToSystemFB(surface->targetColorRT, surface->targetW, surface->targetH, surface->systemW, surface->systemH);
	}

#if GL_EXT_discard_framebuffer
	if(_supportsDiscard)
	{
		EAGLContextSetCurrentAutoRestore autorestore(surface->context);

		GLenum	discardAttach[] = {GL_COLOR_ATTACHMENT0, GL_DEPTH_ATTACHMENT, GL_STENCIL_ATTACHMENT};

		if(surface->msaaFB)
			GLES_CHK(glDiscardFramebufferEXT(GL_READ_FRAMEBUFFER_APPLE, 3, discardAttach));

		if(surface->targetFB)
		{
			GLES_CHK(glBindFramebuffer(GL_FRAMEBUFFER, surface->targetFB));
			GLES_CHK(glDiscardFramebufferEXT(GL_FRAMEBUFFER, 3, discardAttach));
		}

		GLES_CHK(glBindFramebuffer(GL_FRAMEBUFFER, surface->systemFB));
		GLES_CHK(glDiscardFramebufferEXT(GL_FRAMEBUFFER, 2, &discardAttach[1]));
	}
#endif
}

void SetupUnityDefaultFBO(UnityRenderingSurface* surface)
{
	extern GLint gDefaultFBO;
	if(surface->msaaFB)			gDefaultFBO = surface->msaaFB;
	else if(surface->targetFB)	gDefaultFBO = surface->targetFB;
	else						gDefaultFBO = surface->systemFB;

	GLES_CHK(glBindFramebuffer(GL_FRAMEBUFFER, gDefaultFBO));
}


@implementation GLView
+ (Class) layerClass
{
	return [CAEAGLLayer class];
}
@end


extern "C" bool UnityDefaultFBOHasMSAA()
{
	const UnityRenderingSurface& targetSurface = *GetMainRenderingSurface();
	return GL_APPLE_framebuffer_multisample && targetSurface.msaaSamples > 1 && _supportsMSAA;
}

extern "C" void* UnityDefaultFBOColorBuffer()
{
	return GetMainRenderingSurface()->unityColorBuffer;
}


void CheckGLESError(const char* file, int line)
{
	GLenum e = glGetError();
	if( e )
		printf_console ("OpenGLES error 0x%04X in %s:%i\n", e, file, line);
}

