using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LionFire.Serialization
{
    public class SerializationStrategyPreference
    {
        #region Construction

        public SerializationStrategyPreference()
        {

        }
        public SerializationStrategyPreference(ISerializationService service)
        {
            this.Service = service;
        }
        public SerializationStrategyPreference(ISerializationStrategy strategy)
        {
            this.Strategy = strategy;
        }
        public SerializationStrategyPreference(SerializationStrategyPreference preference, ISerializationStrategy strategy)
        {
            this.Strategy = strategy;

            AssignFrom(preference);
        }

        public void AssignFrom(SerializationStrategyPreference preference)
        {
            this.Preference = preference.Preference;
            this.SerializePreference = preference.SerializePreference;
            this.DeserializePreference = preference.DeserializePreference;
        }

        #endregion

        #region Service

        public ISerializationService Service
        {
            get {
                return service;
            }
            set {
                if (strategy != null) throw new ArgumentException("Only one of these can be set: Service, Strategy");
                service = value;
            }
        }
        
        private ISerializationService service;

        #endregion

        public IEnumerable<SerializationStrategyPreference> ResolveToStrategies()
        {
            if (individualStrategies == null)
            {
                if (Strategy != null)
                {
                    individualStrategies = new SerializationStrategyPreference[] { this };
                }
                individualStrategies = this.Service.AllStrategies.Select(strategy => new SerializationStrategyPreference(this, strategy));
            }
            return individualStrategies;
        }
        private IEnumerable<SerializationStrategyPreference> individualStrategies;

        #region Strategy

        public ISerializationStrategy Strategy
        {
            get { return strategy; }
            set {
                if (service != null) throw new ArgumentException("Only one of these can be set: Service, Strategy");
                strategy = value; }
        }
        private ISerializationStrategy strategy;

        #endregion

        /// <summary>
        /// Preference for both serialization and deserialization
        /// Recommended range: -100 to 100
        /// </summary>
        public float Preference { get; set; }

        /// <summary>
        /// Recommended range: -100 to 100, or use min value to disable
        /// </summary>
        public float SerializePreference { get; set; }

        /// <summary>
        /// Recommended range: -100 to 100, or use min value to disable
        /// </summary>
        public float DeserializePreference { get; set; }

    }
}
