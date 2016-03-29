LOCAL_PATH := $(call my-dir)

include $(CLEAR_VARS)

LOCAL_LDLIBS := -llog

LOCAL_MODULE    := unityandroidsystem
LOCAL_SRC_FILES := unityandroidsystem.cpp
LOCAL_CPPFLAGS += -fexceptions

include $(BUILD_SHARED_LIBRARY)
