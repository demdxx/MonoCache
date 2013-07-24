/**
 * Manager.cs
 * 
 * @author Dmitry Ponomarev <demdxx@gmail.com>
 * @license MIT Copyright (c) 2013 demdxx. All rights reserved.
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
using System.IO;

namespace MonoCache.IOS
{
  public class IOSCacheManager : CacheManager
  {
    /**
     * Instnace property of manager
     */
    public static IOSCacheManager Instance {
      get {
        if (null == _Instance) {
          _Instance = new IOSCacheManager ();
        }
        return _Instance as IOSCacheManager;
      }
    }

    /**
     * Constructor
     */
    public IOSCacheManager () : base ()
    {
      // ...
    }

    /**
     * Initialize global cache
     * @param basePath Global cache base path
     * @param lifeTime
     */
    public override void InitGlobalCache (string basePath=null, long lifeTime = 1000 * 60 * 60 * 24 * 3)
    {
      if (string.IsNullOrEmpty (basePath)) {
        var documents = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
        basePath = Path.Combine (documents, "..", "Library", "Caches/g.cache");
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