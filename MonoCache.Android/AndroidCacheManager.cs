/**
 * AndroidCacheManager.cs
 * 
 * @author Dmitry Ponomarev <demdxx@gmail.com>
 * @license MIT Copyright (c) 2013 demdxx. All rights reserved.
 * @project MonoCache.Android
 *
 *
 * Copyright (C) <2013> Dmitry Ponomarev <demdxx@gmail.com>
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using System;
using Android.Content;
using System.IO;

namespace MonoCache.Android
{
  public class AndroidCacheManager : CacheManager
  {
    private Context ApplicationContext;

    /**
     * Instnace property of manager
     */
    public static AndroidCacheManager Instance {
      get {
        return _Instance as AndroidCacheManager;
      }
    }

    /**
     * Constructor
     */
    public AndroidCacheManager (Context context) : base ()
    {
      ApplicationContext = context;
    }

    public static void Init (Context context)
    {
      if (null == _Instance) {
        _Instance = new AndroidCacheManager (context);
      }
    }

    /**
     * Initialize global cache
     * @param basePath Global cache base path
     * @param lifeTime
     */
    public override void InitGlobalCache (string basePath=null, long lifeTime = 1000 * 60 * 60 * 24 * 3)
    {
      if (string.IsNullOrEmpty (basePath)) {
        basePath = ApplicationContext.CacheDir.AbsolutePath;
      }
      base.InitGlobalCache (basePath, lifeTime);
    }

    /**
     * Get global cache interface
     * @return ICache
     */
    public static ICache GlobalCache
    {
      get {
        return Instance.GlobalCacheInterface;
      }
    }
  }
}