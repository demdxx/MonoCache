MonoCache
=========

PCL library for caching data. The project is written in Mono environment and is compatible with the Net. Framework

Exxample
========

### MonoCache

```CS

CacheManager.InitGlobalCache (dir, 1000 * 60 * 60 * 24 * 7);

// ...

if (CacheManager.GlobalCache.Set (url /* As Key */, obj)) {
  // Do something...
}

// ...

var obj = CacheManager.GlobalCache.Get (url);

// ...

CacheManager.GlobalCache.Remove (url);

```

### Image cache MonoCache.IOS & MonoCache.Android

```CS

UIImageView OR ImageView imageCtrl = ...

imageCtrl.SetCacheImageWithURL (url, (HttpWebResponse r, UIImage OR Bitmap i, Exception e) => {
  if (null != e) { System.Console.Write (e.Message); }
});

```
