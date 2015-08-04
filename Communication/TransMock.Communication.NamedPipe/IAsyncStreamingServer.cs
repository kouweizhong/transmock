﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

/***************************************
//   Copyright 2014 - Svetoslav Vasilev

//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//     http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
*****************************************/

/// -----------------------------------------------------------------------------------------------------------
/// Module      :  IAsyncStreamingServer.cs
/// Description :  This interface defines the operations of an asyncronous streaming server.
/// -----------------------------------------------------------------------------------------------------------
/// 

namespace TransMock.Communication.NamedPipes
{
    /// <summary>
    /// Defines the operations of a streaming server
    /// </summary>
    public interface IAsyncStreamingServer : IDisposable
    {
        bool Start();

        void Stop();

        void Disconnect(int connectionId);       

        void WriteAllBytes(int connectionId, byte[] data);

        void WriteStream(int connectionId, Stream data);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        event EventHandler<ClientConnectedEventArgs> ClientConnected;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        event EventHandler<AsyncReadEventArgs> ReadCompleted;
    }    
}
