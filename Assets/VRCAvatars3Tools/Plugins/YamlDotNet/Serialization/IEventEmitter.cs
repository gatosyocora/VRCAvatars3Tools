﻿// This file is part of YamlDotNet - A .NET library for YAML.
// Copyright (c) Antoine Aubry and contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
// of the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using VRCAvatars3Tools.Plugins.YamlDotNet.Core;

namespace VRCAvatars3Tools.Plugins.YamlDotNet.Serialization
{
    public interface IEventEmitter
    {
        void Emit(AliasEventInfo eventInfo, IEmitter emitter);
        void Emit(ScalarEventInfo eventInfo, IEmitter emitter);
        void Emit(MappingStartEventInfo eventInfo, IEmitter emitter);
        void Emit(MappingEndEventInfo eventInfo, IEmitter emitter);
        void Emit(SequenceStartEventInfo eventInfo, IEmitter emitter);
        void Emit(SequenceEndEventInfo eventInfo, IEmitter emitter);
    }
}