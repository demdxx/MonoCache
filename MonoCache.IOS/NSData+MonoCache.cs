/**
 * NSData+MonoCache.cs
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

using MonoTouch.Foundation;

namespace MonoCache.IOS
{
  public static class NSData_MonoCache
  {
    /**
     * Save to MonoCache
     * @param data this
     * @param key
     * @param rewrite
     * @param checkExpired
     * @param cache
     * @return success state
     */
    public static bool ToCache (this NSData data, string key, bool rewrite = false, bool checkExpired = true, ICache cache = null)
    {
      if (null == cache) {
        cache = IOSCacheManager.GlobalCache;
      }
      return cache.Set (key, ToByteArray (data), rewrite, checkExpired);
    }

    /**
     * Load data from cache
     * @param key
     * @param checkExpired
     * @param cache
     * @return NSDate
     */
    public static NSData FromCache (string key, bool checkExpired = true, ICache cache = null)
    {
      if (null == cache) {
        cache = IOSCacheManager.GlobalCache;
      }
      byte[] bytes = (byte[])cache.Get (key, checkExpired);
      return null != bytes ? NSData.FromArray (bytes) : null;
    }

    #region Helpers

    public static byte[] ToByteArray (this NSData data)
    {
      var dataBytes = new byte [data.Length];
      System.Runtime.InteropServices.Marshal.Copy (data.Bytes, dataBytes, 0, Convert.ToInt32 (data.Length));
      return dataBytes;
    }

    #endregion // Helpers
  }
}