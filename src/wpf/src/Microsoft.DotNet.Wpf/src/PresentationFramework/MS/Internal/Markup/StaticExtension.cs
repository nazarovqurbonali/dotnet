// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

/***************************************************************************\
*
*
*  Class for Xaml markup extension for static field and property references.
*
*
\***************************************************************************/
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows;

using SR = System.Windows.SR;

namespace MS.Internal.Markup
{
    /// <summary>
    ///  WPF wrapper for StaticExtension.  Optimizes some common SystemResourceKeys & Commands
    /// </summary>
    internal class StaticExtension : System.Windows.Markup.StaticExtension
    {
        public StaticExtension() : base() { }
        public StaticExtension(String member) : base(member) { }

        /// <summary>
        ///  Return an object that should be set on the targetObject's targetProperty
        ///  for this markup extension.  For a StaticExtension this is a static field
        ///  or property value.
        /// </summary>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
        /// <returns>
        ///  The object to set on this property.
        /// </returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Member == null)
            {
                throw new InvalidOperationException(SR.MarkupExtensionStaticMember);
            }

            object value;
            if (MemberType != null)
            {
                value = SystemResourceKey.GetSystemResourceKey($"{MemberType.Name}.{Member}");
                if (value != null)
                {
                    return value;
                }
            }
            else
            {
                value = SystemResourceKey.GetSystemResourceKey(Member);
                if (value != null)
                {
                    return value;
                }

                // Validate the _member

                int dotIndex = Member.IndexOf('.');
                if (dotIndex < 0)
                {
                    throw new ArgumentException(SR.Format(SR.MarkupExtensionBadStatic, Member));
                }

                // Pull out the type substring (this will include any XML prefix, e.g. "av:Button")

                string typeString = Member.Substring(0, dotIndex);
                if (typeString == string.Empty)
                {
                    throw new ArgumentException(SR.Format(SR.MarkupExtensionBadStatic, Member));
                }

                // Get the IXamlTypeResolver from the service provider

                ArgumentNullException.ThrowIfNull(serviceProvider);

                IXamlTypeResolver xamlTypeResolver = serviceProvider.GetService(typeof(IXamlTypeResolver)) as IXamlTypeResolver;
                if (xamlTypeResolver == null)
                {
                    throw new ArgumentException(SR.Format(SR.MarkupExtensionNoContext, GetType().Name, "IXamlTypeResolver"));
                }

                // Use the type resolver to get a Type instance

                MemberType = xamlTypeResolver.Resolve(typeString);

                // Get the member name substring

                Member = Member.Substring(dotIndex + 1, Member.Length - dotIndex - 1);
            }

            value = CommandConverter.GetKnownControlCommand(MemberType, Member);
            if (value != null)
            {
                return value;
            }

            return base.ProvideValue(serviceProvider);
        }
    }
}

