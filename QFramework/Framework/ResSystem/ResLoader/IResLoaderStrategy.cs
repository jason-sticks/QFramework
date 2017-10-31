﻿/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 liangxie
 * 
 * http://liangxiegame.com
 * https://github.com/liangxiegame/QFramework
 * https://github.com/liangxiegame/QSingleton
 * https://github.com/liangxiegame/QChain
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
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

namespace QFramework
{
    //资源加载&释放策略
    public interface IResLoaderStrategy
    {
        void OnAllTaskFinish(IResLoader loader);
        void OnSyncLoadFinish(IResLoader loader, IRes res);
        void OnSyncLoadFinish(IResLoader loader, AssetBundleRes res);
        void OnSyncLoadFinish(IResLoader loader, AssetRes res);
        void OnSyncLoadFinish(IResLoader loader, InternalRes res);

        void OnAsyncLoadFinish(IResLoader loader, IRes res);
        void OnAsyncLoadFinish(IResLoader loader, AssetBundleRes res);
        void OnAsyncLoadFinish(IResLoader loader, AssetRes res);
        void OnAsyncLoadFinish(IResLoader loader, InternalRes res);
    }

    public class AbstractResLoaderStrategy : IResLoaderStrategy
    {
        public virtual void OnAllTaskFinish(IResLoader loader)
        {

        }

        public virtual void OnSyncLoadFinish(IResLoader loader, IRes res)
        {

        }

        public virtual void OnSyncLoadFinish(IResLoader loader, AssetBundleRes res)
        {

        }

        public virtual void OnSyncLoadFinish(IResLoader loader, AssetRes res)
        {

        }

        public virtual void OnSyncLoadFinish(IResLoader loader, InternalRes res)
        {

        }

        public virtual void OnAsyncLoadFinish(IResLoader loader, IRes res)
        {

        }

        public virtual void OnAsyncLoadFinish(IResLoader loader, AssetBundleRes res)
        {

        }

        public virtual void OnAsyncLoadFinish(IResLoader loader, AssetRes res)
        {

        }

        public virtual void OnAsyncLoadFinish(IResLoader loader, InternalRes res)
        {

        }
    }
}