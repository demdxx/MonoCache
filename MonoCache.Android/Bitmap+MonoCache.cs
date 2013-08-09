/**
 * Bitmap+MonoCache.cs
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
using System.IO;
using System.Threading.Tasks;

using Android.Graphics;

namespace MonoCache.Android
{
  public static class Bitmap_MonoCache
  {
    /**
     * Save image to cache
     * @param bmp this
     * @param key
     * @param rewrite
     * @param checkExpired
     * @param cache
     * @param format
     * @return async bool
     */
    public static bool ToCache (this Bitmap bmp, string key, bool rewrite = false,
                                            bool checkExpired = true, ICache cache = null,
                                            Bitmap.CompressFormat format = null)
    {
      if (null == format) {
        format = Bitmap.CompressFormat.Jpeg;
      }

      using (MemoryStream stream = new MemoryStream ()) {
        bmp.Compress (format, 100, stream);
        byte[] byteArray = stream.ToArray ();

        if (null == cache) {
          cache = AndroidCacheManager.GlobalCache;
        }
        return cache.Set (key, byteArray, rewrite, checkExpired);
      }
    }

    /**
     * Decode image file with width & height
     * @param file path
     * @param width in Pixels
     * @param height in Pixels
     * @param intelligent strict?
     * @return Bitmap or null
     */
    public static Bitmap DecodeImageFile (string filePath, int width, int height, bool intelligent = true)
    {
      if (null != filePath) {
        try {
          return DecodeImageFile (File.OpenRead (filePath), width, height, intelligent);
        } catch (System.IO.FileNotFoundException e) {
          System.Diagnostics.Debug.Write (
            String.Format ("MonoCache::DecodeFile exception: {0} {1}", e.Message, e.StackTrace));
        }
      }
      return null;
    }

    /**
     * Decode image file with width & height
     * @param file path
     * @param width in Pixels
     * @param height in Pixels
     * @param intelligent strict?
     * @return Bitmap or null
     */
    public static Bitmap DecodeImageFile (Stream stream, int width, int height, bool intelligent = true)
    {
      Bitmap result = null;
      try {
        if (null != stream) {
          BitmapFactory.Options o = null;
          using (var ms = new MemoryStream ()) {
            stream.CopyTo (ms);
            if (width > 0 && height > 0) {
              //decode image size
              o = new BitmapFactory.Options () { InJustDecodeBounds = true };
              BitmapFactory.DecodeStream (stream, null, o);

              //Find the correct scale value. It should be the power of 2.
              int width_tmp = o.OutWidth;
              int height_tmp = o.OutHeight;

              // Recalc image size
              if (intelligent) {
                if (width_tmp > width) {
                  height_tmp *= width / width_tmp;
                  width_tmp = width;
                }
                if (height_tmp > height) {
                  width_tmp *= height / height_tmp;
                  height_tmp = height;
                }
              }

              //decode with inSampleSize
              ms.Seek (0, SeekOrigin.Begin);
              o = new BitmapFactory.Options () { OutWidth = width_tmp, OutHeight = height_tmp };
            }
            result = BitmapFactory.DecodeStream (ms, null, o);
          }
        }
      } catch (Exception e) {
        System.Diagnostics.Debug.Write (
          String.Format ("MonoCache::DecodeFile exception: {0} {1}", e.Message, e.StackTrace));
      }
      return result;
    }

    /**
     * Bitmap from cache
     * @param key
     * @param checkExpired
     * @param cache
     * @return sync Bitmap
     */
    public static Bitmap FromCache (string key,
                                    int width = 0, int height = 0, bool intelligent = true,
                                    bool checkExpired = true, ICache cache = null)
    {
      if (null == cache) {
        cache = AndroidCacheManager.GlobalCache;
      }

      Bitmap result = null;
      if (cache is FileCache && width > 0 && height > 0) {
        result = DecodeImageFile (cache.GetAddress (key, checkExpired), width, height, intelligent);
      } else {
        byte[] img = (byte[])cache.Get (key, checkExpired);
        if (null != img && img.Length > 0) {
          result = BitmapFactory.DecodeByteArray (img, 0, img.Length);
        }
      }
      return result;
    }
  }
}

