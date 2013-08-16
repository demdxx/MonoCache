/**
 * ImageView+MonoCache.cs
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
using System.Threading.Tasks;
using System.Net;
using System.IO;

using Android.Widget;
using Android.Graphics;
using Android.App;

namespace MonoCache.Android
{
  public static class ImageView_MonoCache
  {
    /**
     * Load or read image from cache
     * @param image this
     * @param url
     * @param callback
     * @param format
     * @param placeholderImage
     * @param errorImage
     * @param ignoreSSL
     * @param cache
     */
    public static void SetCacheImageWithURL (this ImageView image, string url,
                                             Action<HttpWebResponse, Bitmap, Exception> callback = null,
                                             int width = 0, int height = 0, bool intelligent = true,
                                             Bitmap.CompressFormat format = null,
                                             Bitmap placeholderImage = null, Bitmap errorImage = null,
                                             bool ignoreSSL = false, ICache cache = null)
    {
      if (null == cache) {
        cache = AndroidCacheManager.GlobalCache;
      }
      try {
        Bitmap result = Bitmap_MonoCache.FromCache (url, cache: cache);
        if (null != result) {
          if (null != image.Context && image.Context is Activity) {
            ((Activity) image.Context).RunOnUiThread(delegate {
              image.SetImageBitmap (result);
            });
          } else {
            image.SetImageBitmap (result);
          }
          if (null != callback) {
            callback (null, result, null);
          }
        } else {
          SetImageWithURL (image, url, (response, bitmap, exception) => {
            if (null != bitmap) {
              bitmap.ToCache (url, cache: cache);
            }
            if (null != callback) {
              callback (response, bitmap, exception);
            }
          }, width, height, intelligent, placeholderImage, errorImage, ignoreSSL);
        }
      } catch (Exception e) {
        System.Diagnostics.Debug.Write (String.Format ("Message: {0} Stack trace: {1}", e.Message, e.StackTrace));
        if (null != callback) {
          callback (null, null, e);
        }
      }
    }

    /**
     * Get file with url
     * @param url
     * @param callback
     * @param ignoreSSL
     */
    public static void GetFileWithURL(string url, Action<HttpWebResponse, Exception> callback, bool ignoreSSL = false)
    {
      HttpWebRequest request = HttpWebRequest.Create (url) as HttpWebRequest;
      request.Method = "GET";
      request.Accept = "image/*";

      // Trust for all servers
      if (ignoreSSL) {
        ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, ssl) =>  true;
      } else {
        ServicePointManager.ServerCertificateValidationCallback = null;
      }

      // Start request
      request.BeginGetResponse ((IAsyncResult result) => {
        try {
          // Callback
          if (null != callback) {
            var response = request.EndGetResponse (result) as HttpWebResponse;
            callback (response, null);
          }
        } catch (Exception e) {
          if (null != callback) {
            callback (null, e);
          }
        }
      }, null);
    }

    /**
     * Load image from url without caching
     * @param image this
     * @param url
     * @param callback
     * @param placeholderImage
     * @param errorImage
     * @param ignoreSSL
     * @return self
     */
    public static ImageView SetImageWithURL(this ImageView image, string url, Action<HttpWebResponse, Bitmap, Exception> callback,
                                            int width = 0, int height = 0, bool intelligent = true,
                                            Bitmap placeholderImage = null, Bitmap errorImage = null, bool ignoreSSL = false)
    {
      if (null != placeholderImage) {
        image.SetImageBitmap (placeholderImage);
      }

      GetFileWithURL(url, (HttpWebResponse response, Exception e) => {
        if (null != e) {
          if (null != callback) {
            callback (response, null, e);
          }
        } else {
          Bitmap i = null;
          try {
            if (HttpStatusCode.OK == response.StatusCode) {
              using (var rs = response.GetResponseStream ()) {
                i = Bitmap_MonoCache.DecodeImageFile (rs, width, height, intelligent);
              }
            }

            if (null != image.Context && image.Context is Activity) {
              ((Activity) image.Context).RunOnUiThread(delegate {
                if (null != i) {
                  image.SetImageBitmap (i);
                } else if (null != errorImage) {
                  image.SetImageBitmap (errorImage);
                }
              });
            } else if (null != i) {
              image.SetImageBitmap (i);
            } else if (null != errorImage) {
              image.SetImageBitmap (errorImage);
            }
          } catch (Exception ex) {
            System.Diagnostics.Debug.Write (String.Format ("SetImageWithURL: {0} {1}", ex.Message, ex.StackTrace));
            e = ex;
          }

          // Callback
          if (null != callback) {
            callback (response, i, e);
          }
        }
      }, ignoreSSL);

      return image;
    }
  }
}