using System;

namespace Dycen;

[System.AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = true)]
public sealed class NotifyOnCreateAttribute : Attribute
{
}
