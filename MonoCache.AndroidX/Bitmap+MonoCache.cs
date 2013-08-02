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
    public static async Task<bool> ToCache (this Bitmap bmp, string key, bool rewrite = false,
                                            bool checkExpired = true, ICache cache = null,
                                            Bitmap.CompressFormat format = null)
    {
      if (null == format) {
        format = Bitmap.CompressFormat.Jpeg;
      }

      bool result = await Task<bool>.Factory.StartNew (() => {
        MemoryStream stream = new MemoryStream ();
        bmp.Compress (format, 100, stream);
        byte[] byteArray = stream.ToArray ();

        if (null == cache) {
          cache = AndroidCacheManager.GlobalCache;
        }
        return cache.Set (key, byteArray, rewrite, checkExpired);
      });
      return result;
    }

    /**
     * Bitmap from cache
     * @param key
     * @param checkExpired
     * @param cache
     * @return sync Bitmap
     */
    public static async Task<Bitmap> FromCache (string key, bool checkExpired = true, ICache cache = null)
    {
      if (null == cache) {
        cache = AndroidCacheManager.GlobalCache;
      }
      byte[] img = (byte[]) cache.Get (key, checkExpired);

      Bitmap result = null;
      if (null != img && img.Length > 0) {
        result = await BitmapFactory.DecodeByteArrayAsync (img, 0, img.Length);
      }
      return result;
    }
  }
}

