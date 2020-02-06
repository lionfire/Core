using System;
using System.Text;
using LionFire.Referencing;

namespace LionFire.Vos
{
    // TODO - finish and test and refactor.  This may be half-baked
    public abstract class HostVosReferenceBase<TConcrete> : VosReferenceBase<TConcrete>, IHostReference
        where TConcrete : HostVosReferenceBase<TConcrete>
    {
        #region Key

        // DUPLICATE logic with VosReferenceBase
        public override string Key
        {
            get
            {
                var sb = new StringBuilder();
                if (Host != null)
                {
                    sb.Append(LionPath.HostDelimiter);
                    sb.Append(Host);
                }
                if (Port != null)
                {
                    sb.Append(":");
                    sb.Append(Port);
                }
                if (Path != null)
                {
                    sb.Append(Path);
                }

                this.AppendFilterKey(VosFilters.Layer.ToString(), "|", sb);
                this.AppendFilterKey(VosFilters.Location.ToString(), "^", sb);

                return sb.ToString();
            }
            protected set
            {
                if (Path != null || Host != null || Port != null) throw new AlreadySetException();
                //if (string.IsNullOrWhiteSpace(value))
                //{
                //    Reset();
                //    return;
                //}
                int index = 0;
                if (value.StartsWith(LionPath.HostDelimiter))
                {
                    index += LionPath.HostDelimiter.Length;
                    Host = value.Substring(index, index + value.IndexOfAny(LionPath.Delimiters, index));
                }
                if (value[index] == LionPath.PortDelimiter)
                {
                    index++;
                    Port = value.Substring(index, index + value.IndexOfAny(LionPath.Delimiters, index));
                }
                if (value[index] == LionPath.PathDelimiter)
                {
                    //index += PathDelimiter.Length; -- Keep the initial /
                    //Path = value.Substring(index);
                    Path = value.Substring(index, index + value.IndexOfAny(LionPath.Delimiters, index));
                }
                if (value[index] == VosPath.LayerDelimiter)
                {
                    throw new NotImplementedException("TODO: Reimplement for Layer delimiter");
                    //index++;
                    //Package = value.Substring(index, index + value.IndexOfAny(VosPath.Delimiters, index));
                }
                if (value[index] == VosPath.LocationDelimiter)
                {
                    throw new NotImplementedException("TODO: Reimplement for LocationDelimiter");
                    //index++;
                    //Package = value.Substring(index, index + value.IndexOfAny(VosPath.Delimiters, index));
                }
            }
        }

        #endregion

        #region IHostReference

        public string Host { get; private set; }
        public string Port { get; private set; }

        #endregion
    }

}
