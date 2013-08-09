/**
 * ICache.cs
 * 
 * @project MonoCache
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

namespace MonoCache
{
  public interface ICache
  {
    /**
     * Set value to cache
     * @param key
     * @param value
     * @param rewrite
     * @param checkExpired Check the end of the lifetime of
     * @return success state
     */
    bool Set(string key, object value, bool rewrite = false, bool checkExpired = true);

    /**
     * If object cached
     * @param key
     * @param checkExpired Check the end of the lifetime of
     * @return success status
     */
    bool Check(string key, bool checkExpired = true);

    /**
     * Get object by key
     * @param key
     * @param checkExpired Check the end of the lifetime of
     * @return object
     */
    object Get(string key, bool checkExpired = true);

    /**
     * Get address by key
     * @param key
     * @param checkExpired Check the end of the lifetime of
     * @return object
     */
    string GetAddress(string key, bool checkExpired = true);

    /**
     * Remove value by key
     * @param key
     */
    void Remove(string key);

    /**
     * Get object by string key
     * @param key
     * @return object
     */
    object this [string key] { get; }

    /**
     * Clear expired files
     */
    void ClearExpired();

    /**
     * Clear cache
     */
    void Clear();
  }
}