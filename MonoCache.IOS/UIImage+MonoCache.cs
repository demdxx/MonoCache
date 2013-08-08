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
using System.Collections.Generic;

using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.ImageIO;
using MonoTouch.CoreGraphics;

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
    public static bool ToCache (this UIImage image, string key, bool rewrite = false,
                                bool checkExpired = true, ICache cache = null, bool asPNG = false)
    {
      if (asPNG) {
        return image.AsPNG ().ToCache (key, rewrite, checkExpired, cache);
      }
      return image.AsJPEG ().ToCache (key, rewrite, checkExpired, cache);
    }

    /**
     * Get GIF from NSData
     * @param data
     * @return Animated Image
     */
    public static UIImage GifFromData(NSData data)
    {
      CGImageSource source = CGImageSource.FromData (data);

      var properties = source.CopyProperties (new CGImageOptions());
      var gifProperties = properties.ObjectForKey (MonoTouch.ImageIO.CGImageProperties.GIFDictionary);

      var count = source.ImageCount;
      var images = new List<UIImage> ();

      for (var i = 0; i < count; i++) {
        var image = source.CreateImage (i, null);
        images.Add (UIImage.FromImage (image));
      }

      double duration = Convert.ToDouble (gifProperties.ValueForKey (MonoTouch.ImageIO.CGImageProperties.GIFDelayTime));
      if (duration <= 0.0f) {
        duration = (1.0f / 10.0f) * count;
      }

      return UIImage.CreateAnimatedImage (images.ToArray (), duration);
    }

    /**
     * Get UIImage from byte buffer
     * @param buff
     * @return UIImage instance
     */
    public static UIImage FromBytes (byte[] buff)
    {
      if (0x47 == buff [0]) {
        // Is GIF image
        return GifFromData (NSData.FromArray (buff));
      }
      return UIImage.LoadFromData (NSData.FromArray (buff));
    }

    /**
     * Get UIImage from NSData buffer
     * @param buff
     * @return UIImage instance
     */
    public static UIImage FromData (NSData data)
    {
      if (0x47 == data.GetByte (0)) {
        // Is GIF image
        return GifFromData (data);
      }
      return UIImage.LoadFromData (data);
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
      return null != data ? FromData (data) : null;
    }
  }
}