﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SampleUnirx
{
    public interface ICancelable : IDisposable
    {
        bool IsDisposed { get; }
    }
}
