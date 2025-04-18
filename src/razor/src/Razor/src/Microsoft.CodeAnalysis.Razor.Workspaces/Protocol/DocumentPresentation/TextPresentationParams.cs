﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT license. See License.txt in the project root for license information.

namespace Microsoft.CodeAnalysis.Razor.Protocol.DocumentPresentation;

/// <summary>
/// Class representing the parameters sent for a textDocument/_vs_textPresentation request.
/// </summary>
internal class TextPresentationParams : VSInternalTextPresentationParams, IPresentationParams
{
}
