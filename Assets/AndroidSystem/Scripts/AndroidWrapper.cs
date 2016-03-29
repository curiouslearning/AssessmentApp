using UnityEngine;
using System;


public class AndroidWrapper : IDisposable
{

#if UNITY_ANDROID
    protected AndroidJavaObject mJavaObject;
#endif

    protected bool disposed;

    protected AndroidWrapper()
    {

    }

    ~AndroidWrapper()
    {
        Dispose(false);
    }

    public AndroidJavaObject JavaObject
    {
        get
        {
#if UNITY_ANDROID
            return mJavaObject;
#else
            return null;
#endif
        }
        set
        {
#if UNITY_ANDROID
            mJavaObject = value;
#endif
        }
    }


    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
#if UNITY_ANDROID
        if (!this.disposed)
        {
            if (disposing)
            {
                if (mJavaObject != null)
                {
                    mJavaObject.Dispose();
                    mJavaObject = null;
                }
            }
        }
#endif
        this.disposed = true;
    }

}
