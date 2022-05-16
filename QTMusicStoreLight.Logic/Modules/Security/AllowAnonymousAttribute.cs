﻿//@CodeCopy
//MdStart
#if ACCOUNT_ON
using System;

namespace QTMusicStoreLight.Logic.Modules.Security
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    internal partial class AllowAnonymousAttribute : AuthorizeAttribute
    {
        public AllowAnonymousAttribute()
            : base(false)
        {

        }
    }
}
#endif
//MdEnd
