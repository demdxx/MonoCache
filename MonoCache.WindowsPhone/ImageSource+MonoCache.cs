using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MonoCache.WindowsPhone
{
  public static class ImageSource_MonoCache
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
    public static bool ToCache(this ImageSource image, string key, bool rewrite = false,
                                bool checkExpired = true, ICache cache = null, bool asPNG = false)
    {
      var bmp = new WriteableBitmap((BitmapSource)image);
      using (var stream = new MemoryStream())
      {
        if (asPNG)
        {
          // TODO: Png encode
          return false;
        }
        else
        {
          bmp.SaveJpeg(stream, bmp.PixelWidth, bmp.PixelHeight, 0, 100);
        }
        if (null == cache) {
          cache = WindowPhoneCacheManage.GlobalCache;
        }
        return cache.Set(key, stream.ToArray(), rewrite, checkExpired);
      }
    }

    /**
     * Get GIF from NSData
     * @param data
     * @return Animated Image
     */
    public static ImageSource GifFromBytes(byte[] data)
    {
      return null;
    }

    /**
     * Get UIImage from byte buffer
     * @param buff
     * @return UIImage instance
     */
    public static ImageSource FromBytes(byte[] buff)
    {
      if (0x47 == buff[0])
      {
        // Is GIF image
        return GifFromBytes(buff);
      }
      using (var stream = new MemoryStream(buff))
      {
        BitmapImage bitmapImage = new BitmapImage();
        bitmapImage.SetSource(stream);
        return bitmapImage;
      }
    }

    /**
     * Load image from MonoCache
     * @param key
     * @param checkExpired
     * @param cache
     * @return UIImage
     */
    public static ImageSource FromCache(string key, bool checkExpired = true, ICache cache = null)
    {
      if (null == cache)
      {
        cache = WindowPhoneCacheManage.GlobalCache;
      }
      var data = cache.Get(key, checkExpired);
      return null != data ? FromBytes((byte[])data) : null;
    }
  }
}
