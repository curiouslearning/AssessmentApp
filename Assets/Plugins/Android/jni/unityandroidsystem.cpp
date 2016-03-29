#include <jni.h>
#include <stdint.h>
#include <cstring>
#include "com_fourthsky_unity_androidtools_UnityServiceConnection.h"
#include "com_fourthsky_unity_androidtools_UnityBroadcastReceiver.h"
#include "com_fourthsky_unity_androidtools_UnityActivityCallbacks.h"

#define DEBUG 1

#if DEBUG
	#include <android/log.h>
	#  define  D(x...)  __android_log_print(ANDROID_LOG_DEBUG, "UnityAndroidSystem", x)
	#else
	#  define  D(...)  do {} while (0)
#endif

#define DELETE_GLOBAL(env, x) env->DeleteGlobalRef(x); x = 0;

// Reference to Virtual Machine
static JavaVM* javaVm = 0;

// Class references
static jclass UnityBroadcastReceiver_class = 0;
static jclass UnityServiceConnection_class = 0;
static jclass UnityActivityCallbacks_class = 0;

static jfieldID UnityActivityCallbacks_activityResultCallbackPtr;
static jfieldID UnityActivityCallbacks_newIntentCallbackPtr;

static jmethodID UnityBroadcastReceiver_constructor = 0;
static jmethodID UnityServiceConnection_constructor = 0;


// Callback (delegates) references from C#
typedef void (*fnOnReceive)(jobject, jobject);
typedef void (*fnOnServiceConnected)(jobject, jobject);
typedef void (*fnOnServiceDisconnected)(jobject);
typedef bool (*fnOnActivityResult)(jint, jint, jobject);
typedef bool (*fnOnNewIntent)(jobject);

static jclass FindClass(JNIEnv* env, const char* className) {
	jclass cls = env->FindClass(className);
	if (cls != 0) {
		cls = reinterpret_cast<jclass>(env->NewGlobalRef(cls));
	}

	return cls;
}

#ifdef __cplusplus
extern "C" {
#endif

	jobject CreateJavaBroadcastReceiver(fnOnReceive funcPtr) {
		// Check class objects
		if (UnityBroadcastReceiver_class == 0 || UnityBroadcastReceiver_constructor == 0) {
			D("JNI CreateJavaBroadcastReceiver: constructor description is null");
			return (jobject) 0;
		}

		// Check arguments
		if (funcPtr == 0) {
			D("JNI CreateJavaBroadcastReceiver: could not create a BroadcastReceiver with null action");
			return (jobject) 0;
		}

		// Check if virtual machine exists
		if (javaVm == 0) {
			D("JNI CreateJavaBroadcastReceiver: error JavaVM null");
			return (jobject) 0;
		}

		// Get JNI environment
		JNIEnv* env = 0;
		if (javaVm->GetEnv(reinterpret_cast<void**>(&env), JNI_VERSION_1_6) != JNI_OK) 	{
			D("JNI CreateJavaBroadcastReceiver: error obtaining JNI environment");
			return (jobject)0;
		}

		// Create our custom broadcast receiver
		jobject bcast = env->NewObject(UnityBroadcastReceiver_class, UnityBroadcastReceiver_constructor, reinterpret_cast<intptr_t>(funcPtr));

		// Check if any error occured and describe the error
		jthrowable exc = env->ExceptionOccurred();
		if (exc != 0) {
			// TODO describe the exception
			D("JNI CreateJavaBroadcastReceiver: error creating custom broadcast receiver");

			bcast = 0;
		}

		return bcast;
	}


	jobject CreateJavaServiceConnection(fnOnServiceConnected funcConnected, fnOnServiceDisconnected funcDisconnected) {
		// Check class objects
		if (UnityServiceConnection_class == 0 || UnityServiceConnection_constructor == 0) {
			D("JNI CreateJavaServiceConnection: constructor description is null");
			return (jobject) 0;
		}

		// If connected callback pointer are zero, don't create object, return NULL
		// (require at least on connected function)
		if (funcConnected == 0 ) {
			D("JNI CreateJavaServiceConnection: callback functions are NULL");
			return (jobject) 0;
		}

		// Check if virtual machine exists
		if (javaVm == 0) {
			D("JNI CreateJavaServiceConnection: error JavaVM null");
			return (jobject) 0;
		}

		// Get JNI environment
		JNIEnv* env = 0;
		if (javaVm->GetEnv(reinterpret_cast<void**>(&env), JNI_VERSION_1_6) != JNI_OK) {
			D("JNI CreateJavaServiceConnection: error obtaining JNI environment");
			return (jobject) 0;
		}

		// Create service connection
		jobject serviceConnection = env->NewObject(UnityServiceConnection_class,
												   UnityServiceConnection_constructor,
												   reinterpret_cast<intptr_t>(funcConnected),
												   reinterpret_cast<intptr_t>(funcDisconnected));

		// Check if any error occured and describe the error
		jthrowable exc = env->ExceptionOccurred();
		if (exc != 0) {
			// TODO describe the exception
			D("JNI CreateJavaServiceConnection: error creating service connection");

			serviceConnection = 0;
		}

		return serviceConnection;
	}


	bool RegisterOnActivityResultCallback(fnOnActivityResult funcPtr) {
		// Check if virtual machine exists
		if (javaVm == 0) {
			D("JNI RegisterOnActivityResultCallback: error JavaVM null");
			return false;
		}

		// Check if current activity is of extended type
		if (UnityActivityCallbacks_class == 0) {
			D("JNI RegisterOnActivityResultCallback: Callback class is not in the package");
			return false;
		}

		// Check arguments
		if (funcPtr == 0) {
			D("JNI RegisterOnActivityResultCallback: callback argument is null");
			return false;
		}

		// Check if field is found
		if (UnityActivityCallbacks_activityResultCallbackPtr == 0) {
			D("JNI RegisterOnActivityResultCallback: callback field on helper don't exists");
			return false;
		}

		// First, get JNI environment
		JNIEnv* env = 0;
		if (javaVm->GetEnv(reinterpret_cast<void**>(&env), JNI_VERSION_1_6) != JNI_OK) {
			D("JNI RegisterOnActivityResultCallback: error obtaining JNI environment");
			return false;
		}

		env->SetStaticIntField(UnityActivityCallbacks_class, UnityActivityCallbacks_activityResultCallbackPtr, reinterpret_cast<intptr_t>(funcPtr));
		return true;
	}


	bool RegisterOnNewIntentCallback(fnOnNewIntent funcPtr) {

		// Check if virtual machine exists
		if (javaVm == 0) {
			D("JNI RegisterOnNewIntentCallback: error JavaVM null");
			return false;
		}

		// Check if current activity is of extended type
		if (UnityActivityCallbacks_class == 0) {
			D("JNI RegisterOnNewIntentCallback: Callback class is not in the package");
			return false;
		}

		// Check arguments
		if (funcPtr == 0) {
			D("JNI RegisterOnNewIntentCallback: callback argument is null");
			return (jobject) 0;
		}

		// First, get JNI environment
		JNIEnv* env = 0;
		if (javaVm->GetEnv(reinterpret_cast<void**>(&env), JNI_VERSION_1_6) != JNI_OK) {
			D("JNI RegisterOnNewIntentCallback: error obtaining JNI environment");
			return false;
		}

		env->SetStaticIntField(UnityActivityCallbacks_class, UnityActivityCallbacks_newIntentCallbackPtr, reinterpret_cast<intptr_t>(funcPtr));
		return true;

	}


	jint JNI_OnLoad(JavaVM* vm, void* reserved)
	{
		// Record JavaVM
		javaVm = vm;

		// Getting JNI
		JNIEnv* env = 0;
		if (vm->GetEnv(reinterpret_cast<void**>(&env), JNI_VERSION_1_6) != JNI_OK) {
			return -1;
		}

		// Cache UnityBroadcastReceiver class and methods
		UnityBroadcastReceiver_class = FindClass(env, "com/fourthsky/unity/androidtools/UnityBroadcastReceiver");
		if (UnityBroadcastReceiver_class != 0) {
			UnityBroadcastReceiver_constructor = env->GetMethodID(UnityBroadcastReceiver_class, "<init>", "(I)V");
		}
		else {
			D("JNI JNIOnLoad: Could not find UnityBroadcastReceiver class");
			return -1;
		}

		// Cache UnityServiceConnection class and methods
		UnityServiceConnection_class = FindClass(env, "com/fourthsky/unity/androidtools/UnityServiceConnection");
		if (UnityServiceConnection_class != 0) {
			UnityServiceConnection_constructor = env->GetMethodID(UnityServiceConnection_class, "<init>", "(II)V");
		}
		else {
			D("JNI JNIOnLoad: Could not find UnityServiceConnection class");
			return -1;
		}

		// Cache Unity callback class
		UnityActivityCallbacks_class = FindClass(env, "com/fourthsky/unity/androidtools/UnityActivityCallbacks");
		if (UnityActivityCallbacks_class != 0) {
			UnityActivityCallbacks_activityResultCallbackPtr = env->GetStaticFieldID(UnityActivityCallbacks_class, "mActivityResultCallbackPtr", "I");
			UnityActivityCallbacks_newIntentCallbackPtr = env->GetStaticFieldID(UnityActivityCallbacks_class, "mNewIntentCallbackPtr", "I");
		}
		else {
			D("JNI JNIOnLoad: Could not find UnityActivityCallbacks class");
			return -1;
		}

		return JNI_VERSION_1_6;
	}

	void JNI_OnUnload(JavaVM *vm, void *reserved)
	{
		JNIEnv* env = 0;
		if (vm->GetEnv(reinterpret_cast<void**>(&env), JNI_VERSION_1_6) != JNI_OK) {
			return;
		}

		// Clear class references
		DELETE_GLOBAL(env, UnityBroadcastReceiver_class)
		DELETE_GLOBAL(env, UnityServiceConnection_class)
		DELETE_GLOBAL(env, UnityActivityCallbacks_class)

		// Clear JavaVM reference
		javaVm = 0;
	}

	JNIEXPORT void JNICALL Java_com_fourthsky_unity_androidtools_UnityBroadcastReceiver_nativeOnReceive
	  (JNIEnv *env, jobject thisObj, jint callbackPtr, jobject context, jobject intent)
	{
		// Only execute if pointer is not ZERO
		if (callbackPtr != 0) {
			fnOnReceive func = reinterpret_cast<fnOnReceive>(callbackPtr);
			(*func)(context, intent);
		}
	}

	JNIEXPORT void JNICALL Java_com_fourthsky_unity_androidtools_UnityServiceConnection_nativeOnServiceConnected
	  (JNIEnv *env, jobject thisObj, jint callbackPtr, jobject name, jobject binder)
	{
		// Only execute if pointer is not ZERO
		if (callbackPtr != 0) {
			fnOnServiceConnected func = reinterpret_cast<fnOnServiceConnected>(callbackPtr);
			(*func)(name, binder);
		}
	}

	/*
	 * Class:     com_fourthsky_unity_androidtools_UnityServiceConnection
	 * Method:    nativeOnServiceDisconnected
	 * Signature: (ILandroid/content/ComponentName;)V
	 */
	JNIEXPORT void JNICALL Java_com_fourthsky_unity_androidtools_UnityServiceConnection_nativeOnServiceDisconnected
	  (JNIEnv *env, jobject thisObj, jint callbackPtr, jobject name)
	{
		// Only execute if pointer is not ZERO
		if (callbackPtr != 0) {
			fnOnServiceDisconnected func = reinterpret_cast<fnOnServiceDisconnected>(callbackPtr);
			(*func)(name);
		}
	}

	/*
	 * Class:     com_fourthsky_unity_androidtools_UnityPlayerActivityEx
	 * Method:    nativeOnActivityResult
	 * Signature: (IIILandroid/content/Intent;)V
	 */
	JNIEXPORT void JNICALL Java_com_fourthsky_unity_androidtools_UnityActivityCallbacks_nativeOnActivityResult
	  (JNIEnv *env, jclass thisKlass, jint callbackPtr, jint requestCode, jint resultCode, jobject data)
	{
		// Only execute if pointer is not ZERO
		if (callbackPtr != 0) {
			fnOnActivityResult func = reinterpret_cast<fnOnActivityResult>(callbackPtr);
			(*func)(requestCode, resultCode, data);

		} else {
			D("Error calling OnActivityResult on native side");
		}
	}

	/*
	 * Class:     com_fourthsky_unity_androidtools_UnityPlayerActivityEx
	 * Method:    nativeOnNewIntent
	 * Signature: (ILandroid/content/Intent;)V
	 */
	JNIEXPORT void JNICALL Java_com_fourthsky_unity_androidtools_UnityActivityCallbacks_nativeOnNewIntent
	  (JNIEnv *env, jclass thisKlass, jint callbackPtr, jobject data)
	{
		// Only execute if pointer is not ZERO
		if (callbackPtr != 0) {
			fnOnNewIntent func = reinterpret_cast<fnOnNewIntent>(callbackPtr);
			(*func)(data);

		} else {
			D("Error calling OnNewIntent on native side");
		}
	}
	
#ifdef __cplusplus
}
#endif
