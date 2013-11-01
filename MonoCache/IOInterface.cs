/**
 * IOInterface.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonoCache
{
#if IO_INTERFACE
  public interface IOInterface
  {
    /* File path */
    string PathCombine(string path1, string path2);
    string PathCombineLocalApplicationData(string path);
    string PathGetExtension(string path);
    string PathGetFileNameWithoutExtension(string path);

    /* Directory */
    bool DirectoryExists(string path);
    bool DirectoryCreate(string path);
    string[] DirectoryGetFiles(string path, string pattern);

    /* File */
    bool FileExists(string path);
    void FileDelete(string path);
    DateTime FileGetLastWriteTimeUtc(string path);
    bool FileSerialize(string path, object value);
    object FileDeserialize(string path);
  }
#endif // END IO_INTERFACE
}
