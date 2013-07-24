/**
 * UIImageView+MonoCache.cs
 *
 * @project MonoTouch.UIImageView+CacheLoader
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
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;

using MonoTouch.UIKit;
using MonoTouch.Foundation;

using MonoCache;
using MonoCache.IOS;

namespace MonoCache.IOS
{
  public static class UIImageView_MonoCacheLoader
  {
    public static UIImageView SetCacheImageWithURL(this UIImageView img, string url, Action<HttpWebResponse, UIImage, Exception> callback,
                                                   UIImage placeholderImage = null, UIImage errorImage = null, bool ignoreSSL = false, ICache cache = null)
    {
      // Get image from cache
      UIImage image = UIImage_MonoCache.FromCache (url);

      if (null != image) {
        // Set image from cache
        img.Image = image;

        // Callback
        if (null != callback) {
          callback (null, image, null);
        }
        return img;
      }
      return SetImageWithURL (img, url, (HttpWebResponse response, UIImage i, Exception e) => {
        if (null != i) {
          i.ToCache (url, true);
        }
        if (null != callback) {
          callback (response, i, e);
        }
      }, placeholderImage, errorImage, ignoreSSL);
    }


    public static UIImageView SetImageWithURL(this UIImageView img, string url, Action<HttpWebResponse, UIImage, Exception> callback,
                                              UIImage placeholderImage = null, UIImage errorImage = null, bool ignoreSSL = false)
    {
      if (null != placeholderImage) {
        img.Image = placeholderImage;
      }

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
          UIImage i = null;
          var response = request.EndGetResponse (result) as HttpWebResponse;
          if (HttpStatusCode.OK == response.StatusCode) {
            using (BinaryReader br = new BinaryReader (response.GetResponseStream ())) {
              byte[] arr = br.ReadBytes ((int) response.ContentLength);
              i = UIImage.LoadFromData (NSData.FromArray (arr));
            }
          }

          NSThread.MainThread.BeginInvokeOnMainThread (() => {
            if (null != i) {
              img.Image = i;
            } else if (null != errorImage) {
              img.Image = errorImage;
            }

            // Callback
            if (null != callback) {
              callback (response, i, null);
            }
          });
        } catch (Exception e) {
          NSThread.MainThread.BeginInvokeOnMainThread (() => {
            if (null != errorImage) {
              img.Image = errorImage;
            }
            callback (null, null, e);
          });
        }
      }, null);

      return img;
    }
  }
}