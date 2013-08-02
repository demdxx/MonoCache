/**
 * @project MonoCache.Android
 * @year 2013
 * @author Dmitry Ponomarev <demdxx@gmail.com>
 */
using System;
using System.Threading.Tasks;
using System.Net;
using System.IO;

using Android.Widget;
using Android.Graphics;

namespace MonoCache.Android
{
  public static class ImageView_Ad
  {
    /**
     * Load or read image from cache
     * @param image this
     * @param url
     * @param cache
     * @param callback
     * @param format
     * @param placeholderImage
     * @param errorImage
     * @param ignoreSSL
     */
    public static async void SetCacheImageWithURL (this ImageView image, string url, ICache cache = null,
                                                   Action<HttpWebResponse, Bitmap, Exception> callback = null,
                                                   Bitmap.CompressFormat format = null,
                                                   Bitmap placeholderImage = null, Bitmap errorImage = null, bool ignoreSSL = false)
    {
      if (null == cache) {
        cache = AndroidCacheManager.GlobalCache;
      }
      Bitmap result = await Bitmap_MonoCache.FromCache (url, cache: cache);
      if (null != result) {
        image.SetImageBitmap (result);
        if (null != callback) {
          callback (null, result, null);
        }
      } else {
        SetImageWithURL (image, url, (response, bitmap, exception) => {
          if (null != bitmap) {
            bitmap.ToCache (url, cache: cache).Wait ();
          }
          if (null != callback) {
            callback (response, bitmap, exception);
          }
        }, placeholderImage, errorImage, ignoreSSL);
      }
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
                                            Bitmap placeholderImage = null, Bitmap errorImage = null, bool ignoreSSL = false)
    {
      if (null != placeholderImage) {
        image.SetImageBitmap (placeholderImage);
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
          Bitmap i = null;
          var response = request.EndGetResponse (result) as HttpWebResponse;
          if (HttpStatusCode.OK == response.StatusCode) {
            using (BinaryReader br = new BinaryReader (response.GetResponseStream ())) {
              byte[] arr = br.ReadBytes ((int) response.ContentLength);
              i = BitmapFactory.DecodeByteArray (arr, 0, arr.Length);
            }
          }

          if (null != i) {
            image.SetImageBitmap (i);
          } else if (null != errorImage) {
            image.SetImageBitmap (errorImage);
          }

          // Callback
          if (null != callback) {
            callback (response, i, null);
          }
        } catch (Exception e) {
          if (null != errorImage) {
            image.SetImageBitmap (errorImage);
          }
          callback (null, null, e);
        }
      }, null);

      return image;
    }
  }
}