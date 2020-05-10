#region License
//  Copyright(c) Workshell Ltd
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
#endregion

using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: AssemblyTitle("Workshell.Tempus")]
[assembly: AssemblyDescription("Tempus - A .NET Job Scheduler")]

#if !SIGNED
[assembly: InternalsVisibleTo("Workshell.Tempus.AspNetCore")]
[assembly: InternalsVisibleTo("Workshell.Tempus.Tests")]
#else
[assembly: InternalsVisibleTo("Workshell.Tempus.AspNetCore, PublicKey=0024000004800000940000000602000000240000525341310004000001000100ed0db04e6ef7cb7ae6c0dbecb36b42bc629609ae4f059d5aacd1f467be55281c480336bfd79e4c28a5304ccb6448502b5c4f5184dcf76f264ea7d2f78f6e7ab134ca12d526e2257b2ee88b8429dc7ace03ad9c21b6b2710ca2b82e770e62683382924c50f7e554402e838dd4fd90bfcc2ec730ef2cb9c9c9b9992061d37ed789")]
[assembly: InternalsVisibleTo("Workshell.Tempus.Tests, PublicKey=0024000004800000940000000602000000240000525341310004000001000100ed0db04e6ef7cb7ae6c0dbecb36b42bc629609ae4f059d5aacd1f467be55281c480336bfd79e4c28a5304ccb6448502b5c4f5184dcf76f264ea7d2f78f6e7ab134ca12d526e2257b2ee88b8429dc7ace03ad9c21b6b2710ca2b82e770e62683382924c50f7e554402e838dd4fd90bfcc2ec730ef2cb9c9c9b9992061d37ed789")]
#endif