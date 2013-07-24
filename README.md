MonoCache
=========

PCL library for caching data. The project is written in Mono environment and is compatible with the Net. Framework

Exxample
========

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
