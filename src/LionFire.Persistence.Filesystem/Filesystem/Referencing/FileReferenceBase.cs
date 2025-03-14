﻿using System;
using System.Collections.Generic;
using LionFire.Referencing;
using LionFire.Serialization;

namespace LionFire.Persistence.Filesystem;

[LionSerializable(SerializeMethod.ByValue)]
public abstract class FileReferenceBase<ConcreteType, TValue> : LocalReferenceBase<ConcreteType, TValue>
    where ConcreteType : FileReferenceBase<ConcreteType, TValue>
{
    public virtual string UriPrefixDefault => UriSchemeColon;
    public virtual string UriSchemeColon => Scheme + ":";
    //public abstract string UriScheme { get; }
    //public override string Scheme => UriScheme;

    public override IEnumerable<string> AllowedSchemes { get { yield return Scheme; } }


    public FileReferenceBase() { }
    public FileReferenceBase(string path)
    {
        this.Path = path;
    }

    #region Conversion and Implicit operators

    //public static implicit operator Handle(FileReference fsref)
    //{
    //    return fsref.ToHandle();
    //}

    #endregion

    public static string CoercePath(string path)
    {
        path = path.Replace('\\', '/');

        //if (value != null)
        //{
        //    if (value.Length >= 1)
        //    {
        //        if (value[0] == ':') throw new ArgumentException();
        //    }

        //    var colon = value.LastIndexOf(':');
        //    if (colon != -1 && colon != 1)
        //    {
        //        throw new ArgumentException();
        //    }
        //}

        return path;
    }

    protected override void InternalSetPath(string path)
    {
        this.path = CoercePath(path);
    }

    public override string Key
    {
        get => string.Concat(UriPrefixDefault, Path);
        protected set
        {
            if (value.StartsWith(UriSchemeColon))
            {
                if (value.StartsWith(UriPrefixDefault))
                {
                    value = value.Substring(UriPrefixDefault.Length);
                }
                else
                {
                    value = value.Substring(UriSchemeColon.Length);
                }
            }
            Path = value;
        }
    }
    public  string ToXamlString() => $"{{{this.GetType().Name} {Path}}}";
    public override string ToString() => Key; 

}
