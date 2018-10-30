using System.Collections.Generic;
using System.Text;

namespace LionFire.Serialization
{
    /// <summary>
    /// - REVIEW: If this is going to be a parameter object, make it a struct?
    /// </summary>
    public class SerializationContext
    {
        #region FUTURE ?

        //public bool RequireExactType { get; set; }

        #endregion

        

        public object SerializerOptions { get; set; }

        public SerializationFlags Flags { get; set; }

        #region DeserializationStrategies

        public virtual IEnumerable<DeserializationStrategy> DefaultDeserializationStrategies
        {
            get
            {
                yield return DeserializationStrategy.Detect;
            }
        }
        public IEnumerable<DeserializationStrategy> DeserializationStrategies
        {
            get => deserializationStrategies ?? DefaultDeserializationStrategies; set => deserializationStrategies = value;
        }

        #region Encoding

        public Encoding Encoding { get; set; }

        #endregion

        private IEnumerable<DeserializationStrategy> deserializationStrategies;

        #endregion

        public virtual void OnSerialized(ISerializationStrategy s) { }
    }
}
