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

using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace MonoCache.IOS
{
  public static class UIImage_MonoCache
  {
    /**
     * Save Image to cache
     * @param image
     * @param key
     * @param rewrite
     * @param checkExpired
     * @param cache
     * @return surcess state
     */
    public static bool ToCache (this UIImage image, string key, bool rewrite = false, bool checkExpired = true, ICache cache = null)
    {
      return image.AsJPEG ().ToCache (key, rewrite, checkExpired, cache);
    }

    /**
     * Load image from MonoCache
     * @param key
     * @param checkExpired
     * @param cache
     * @return UIImage
     */
    public static UIImage FromCache (string key, bool checkExpired = true, ICache cache = null)
    {
      NSData data = NSData_MonoCache.FromCache (key, checkExpired, cache);
      return null != data ? UIImage.LoadFromData (data) : null;
    }
  }
}